using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.Mail;
//using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using serverStatusNotifier.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.ComponentModel;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace serverStatusNotifier.Services
{
	public class EmailService : IEmailService
	{
		private readonly ILogger _logger = Log.ForContext<EmailService>();
		private readonly IConfiguration _config;
		private readonly string stateFileName = "email_service_state";

		public EmailService(IConfiguration config)
		{
			_config = config;
		}

		public EmailServiceState GetEmailServiceState()
		{
			FileStream fs;
			var emailServiceStateTemplate = new EmailServiceState
			{
				EmailSentOn = "1900-01-01 00:00:00",
				NotificationsSent = new List<string>()
			};

			var result = emailServiceStateTemplate;

			if (!File.Exists(stateFileName))
			{
				var xmlSerializer = new XmlSerializer(typeof(EmailServiceState));
				var emailServiceState = emailServiceStateTemplate;

				fs = new FileStream(stateFileName, FileMode.Create);
				TextWriter writer = new StreamWriter(fs);
				xmlSerializer.Serialize(writer, emailServiceState);
				writer.Close();
				fs.Close();
			}
			else
			{
				fs = new FileStream(stateFileName, FileMode.Open);
				TextReader reader = new StreamReader(fs);
				var _xmlSerializer = new XmlSerializer(typeof(EmailServiceState));
				var emailServiceState = _xmlSerializer.Deserialize(reader) as EmailServiceState;
				reader.Close();
				fs.Close();

				if (emailServiceState != null)
				{
					result = emailServiceState;
				}
			}

			return result;
		}

		public void SendMessage(List<User> users, string subject, string messageBody)
		{
			var sender = _config.GetValue<string>("SenderEmail") ?? "[sender]";
			var senderName = _config.GetValue<string>("SenderName") ?? "[sender]";
			var messages = new List<MimeMessage>();

			foreach (var user in users)
			{
				var messageText = @$"Hi {user.Firstname},

{messageBody}

Have a nice day!
Notification service";

				var message = new MimeMessage();
				message.From.Add(new MailboxAddress(senderName, sender));
				message.To.Add(new MailboxAddress(user.Fullname, user.Email));

				var subjectTemplate = _config.GetValue<string>("EmailSubjectTemplate");
				message.Subject = string.Concat(subjectTemplate, " - ", subject); ;
				message.Body = new TextPart("plain")
				{
					Text = messageText
				};

				messages.Add(message);
			}

			try
			{
				var server = _config.GetValue<string>("SMTPAddress");
				var port = _config.GetValue<int>("SMTPPort");
				var smtpUsername = _config.GetValue<string>("SMTPUsername");
				var smtpPassword = _config.GetValue<string>("SMTPPassword");
				var timeout = _config.GetValue<int>("SMTPTimeoutInMilliseconds");
				var smtpUseSSL = _config.GetValue<bool>("SMTPUseSSL");
				var smtpUseTls = _config.GetValue<bool>("SMTPUseTls");
				var secureOptions = (smtpUseSSL, smtpUseTls) switch
				{
					(false, false) => SecureSocketOptions.Auto,
					(true, false) => SecureSocketOptions.SslOnConnect,
					(false, true) => SecureSocketOptions.StartTls,
					(true, true) => SecureSocketOptions.StartTlsWhenAvailable
				};

				using (var client = new SmtpClient())
				{
					client.Connect(server, port, secureOptions);
					client.Authenticate(smtpUsername, smtpPassword);
					client.Timeout = timeout;
					foreach(var message in messages)
					{
						_logger.Information($"Sending email to {message.To.ToString()}");
						client.Send(message);
					}

					client.Disconnect(true);
				}
			}
			catch (Exception ex)
			{
				_logger.Error("Exception caught in Runner.SendEmail(): {0}", ex.ToString());
			}
		}

		public void SaveStateToFile(NotificationsModel notifications)
		{
			var notificationsSent = notifications.Notifications
				.Where(_ => _.Sent == true)
				.Select(n => n.NotificationType.ToString())
				.ToList();

			var emailServiceState = new EmailServiceState
			{
				EmailSentOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
				NotificationsSent = notificationsSent
			};

			var fs = new FileStream(stateFileName, FileMode.Create);
			var writer = new StreamWriter(fs);


			// Serialize the object, and close the TextWriter.
			var xmlSerializer = new XmlSerializer(typeof(EmailServiceState));
			xmlSerializer.Serialize(writer, emailServiceState);
			writer.Close();
			fs.Close();
		}
	}
}
