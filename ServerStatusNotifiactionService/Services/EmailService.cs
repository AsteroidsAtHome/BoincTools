using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using serverStatusNotifier.Models;
using Microsoft.Extensions.Configuration;
using Serilog;

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

		public void SendEmail(string to, string subject, string body)
		{

			var sender = _config.GetValue<string>("SenderEmail") ?? "[sender]";
			MailMessage message = new MailMessage(sender, to);
			var subjectTemplate = _config.GetValue<string>("EmailSubjectTemplate");
			message.Subject = string.Concat(subjectTemplate, " - ", subject);
			message.Body = body;

			var server = _config.GetValue<string>("SNMPAddress");
			var port = _config.GetValue<int>("SNMPPort");
			SmtpClient client = new SmtpClient(server, port);

			var snmpUsername = _config.GetValue<string>("SNMPUsername");
			var snmpPassword = _config.GetValue<string>("SNMPPassword");
			var snmpUseSSL = _config.GetValue<bool>("SNMPUseSSL");

			client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.UseDefaultCredentials = false;
			client.Credentials = new NetworkCredential(snmpUsername, snmpPassword);
			client.EnableSsl = snmpUseSSL;

			try
			{
				System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
				client.Send(message);
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
