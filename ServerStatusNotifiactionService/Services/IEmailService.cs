using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using serverStatusNotifier.Models;

namespace serverStatusNotifier.Services
{
	public interface IEmailService
	{
		EmailServiceState GetEmailServiceState();

		void SendMessage(List<User> users, string subject, string body);

		void SaveStateToFile(NotificationsModel notifications);
	}
}
