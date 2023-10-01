using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using serverStatusNotifier.Models;

namespace serverStatusNotifier.Services
{
    public class RunnerService : IRunnerService
    {
        private readonly ILogger _logger = Log.ForContext<RunnerService>();
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public RunnerService(
            IConfiguration config,
            IEmailService emailService)
        {
            _config = config;
            _emailService = emailService;
        }

        public void CheckServerStatus(ServerStatus serverStatus)
        {
			var notificationsList = _config.GetSection("Notifications")?.Get<List<Notification>>() ?? new List<Notification>();
			var notifications = new NotificationsModel(notificationsList);

			if (notifications == null)
            {
                _logger.Warning("There are no notifications predefined. Check \"appsettings,json\" file");
                return;
            }

            var hasNoAnyNotifications = !notifications.Notifications.Any()
                || !notifications.Notifications.Select(_ => _.Notify == true).Any();

            if (hasNoAnyNotifications) return;

            var emailServiceState = _emailService.GetEmailServiceState();

            var messageBody = PrepareNotifications(serverStatus, notifications);

            var minutes = _config.GetValue<int>("ResendNotificationEmailsInMinutes");
            var point = DateTime.Parse(emailServiceState.EmailSentOn).AddMinutes(minutes);
			var diffs = notifications.Notifications
				.Where(_ => _.Sent == true)
				.Select(n => n.NotificationType.ToString())
				.Except(emailServiceState.NotificationsSent)
				.ToList();

			var noNewNotifications = !diffs.Any();
			if (point > DateTime.Now && noNewNotifications)
            {
                _logger.Information("Notifications: Nothing to do");
                return;
            }

			var noNeedToNotify = !notifications.Notifications
				.Where(_ => _.Sent == true)
				.Select(n => n.NotificationType)
				.Any();

			if (noNeedToNotify)
			{
				_emailService.SaveStateToFile(notifications);
				_logger.Information("All notifications have been cleared");
				return;
			}

            // 2) send emails
            var users = _config.GetSection("NotifyEmails").Get<List<User>>()?.ToList();
            if (users == null) return;

            foreach (var user in users)
            {
                _logger.Information($"Sending email to {user.Firstname} {user.Lastname} ({user.Email})");

                var message = @$"Hi {user.Firstname},

{messageBody}

Have a nice day!
Notification service";

                _emailService.SendEmail(user.Email, "Server warning notification", message);
            }

            // 3) TODO: save state to file
            _emailService.SaveStateToFile(notifications);
        }

        private static string PrepareNotifications(ServerStatus serverStatus, NotificationsModel notifications)
        {
            var messageBody = new StringBuilder();
            var addNotification = false;

            foreach (var notification in notifications.Notifications.Where(notification => notification.Notify))
            {
                switch (notification.NotificationType)
                {
                    //case "ResultsReadyToSendLowerLimit":
                    case NotificationEnums.ResultsReadyToSend:
                        var tasksReadyToSend = serverStatus?.DatabaseFileStates.ResultsReadyToSend ?? 0;
                        addNotification = notification.WarningLevel > tasksReadyToSend;
                        if (addNotification)
                        {
                            var msg = string.Format(Messages.ResultsReadyToSend, tasksReadyToSend);
                            messageBody.AppendLine(msg);
                            Log.Logger.Warning(msg);
							notification.Sent = true;
                        }
                        break;
                    //case "ResultsInProgressLowerLimit":
                    case NotificationEnums.ResultsInProgressLower:
                        var tasksInProgress = serverStatus?.DatabaseFileStates.ResultsInProgress ?? 0;
                        addNotification = notification.WarningLevel > tasksInProgress;
                        if (addNotification)
                        {
                            var msg = string.Format(Messages.ResultsInProgress, tasksInProgress);
                            messageBody.AppendLine(msg);
                            Log.Logger.Warning(msg);
							notification.Sent = true;
						}
                        break;
                    //case "WorkunitsWaitingForValidationUpperLimit":
                    case NotificationEnums.WorkunitsWaitingForValidation:
                        var workunitsWaitingForValidation = serverStatus?.DatabaseFileStates.WorkunitsWaitingForValidation ?? 0;
                        addNotification = workunitsWaitingForValidation > notification.WarningLevel;
                        if (addNotification)
                        {
                            var msg = string.Format(Messages.WorkunitsWaitingForValidation, workunitsWaitingForValidation);
                            messageBody.AppendLine(msg);
                            Log.Logger.Warning(msg);
							notification.Sent = true;
						}
                        break;
                    //WorkunitsWaitingForAssimilationUpperLimit
                    case NotificationEnums.WorkunitsWaitingForAssimilation:
                        var workunitsWaitingForAssimilation = serverStatus?.DatabaseFileStates.WorkunitsWaitingForAssimilation ?? 0;
                        addNotification = workunitsWaitingForAssimilation > notification.WarningLevel;
                        if (addNotification)
                        {
                            var msg = string.Format(Messages.WorkunitsWaitingForAssimilation, workunitsWaitingForAssimilation);
                            messageBody.AppendLine(msg);
                            Log.Logger.Warning(msg);
							notification.Sent = true;
						}
                        break;
                    //case "WorkunitsWaitnigForDeletionUpperLimit":
                    case NotificationEnums.WorkunitsWaitingForDeletion:
                        var workunitsWaitingForDeletion = serverStatus?.DatabaseFileStates.WorkunitsWaitingForDeletion ?? 0;
                        addNotification = workunitsWaitingForDeletion > notification.WarningLevel;
                        if (addNotification)
                        {
                            var msg = string.Format(Messages.WorkunitsWaitingForDeletion, workunitsWaitingForDeletion);
                            messageBody.AppendLine(msg);
                            Log.Logger.Warning(msg);
							notification.Sent = true;
						}
                        break;
                    //case "ResultsWaitingForDeletionUpperLimit":
                    case NotificationEnums.ResultsWaitingForDeletion:
                        var resultsWaitingForDeletion = serverStatus?.DatabaseFileStates.ResultsWaitingForDeletion ?? 0;
                        addNotification = resultsWaitingForDeletion > notification.WarningLevel;
                        if (addNotification)
                        {
                            var msg = string.Format(Messages.ResultsWaitingForDeletion, resultsWaitingForDeletion);
                            messageBody.AppendLine(msg);
                            Log.Logger.Warning(msg);
							notification.Sent = true;
						}
                        break;
                    //case "HostsWithResentCreditLowerLimit":
                    case NotificationEnums.HostsWithResentCredit:
                        var hostsWithResentCredit = serverStatus?.DatabaseFileStates.HostsWithResentCredit ?? 0;
                        addNotification = notification.WarningLevel > hostsWithResentCredit;
                        if (addNotification)
                        {
                            var msg = string.Format(Messages.HostsWithResentCredit, hostsWithResentCredit);
                            messageBody.AppendLine(msg);
                            Log.Logger.Warning(msg);
							notification.Sent = true;
						}
                        break;
                }
            }

            return messageBody.ToString();
        }
    }
}
