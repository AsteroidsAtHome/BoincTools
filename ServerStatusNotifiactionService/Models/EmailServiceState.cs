using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverStatusNotifier.Models
{
	public class EmailServiceState
	{
		public string EmailSentOn { get; set; }

		public List<string> NotificationsSent { get; set; } = new List<string>();
    }
}
