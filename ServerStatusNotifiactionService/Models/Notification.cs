using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using serverStatusNotifier.Helpers;
using System.Xml.Linq;

namespace serverStatusNotifier.Models
{
	public class Notification
	{
		public string Name { get; set; }

		public NotificationEnums NotificationType
		{
			get
			{
				var result = EnumHelper<NotificationEnums>.GetValueFromName(Name);
				return result;
			}
		}

        public int WarningLevel { get; set; }

		public string Comparer { get; set; }

		public bool Notify { get; set; } = false;

		public bool Sent { get; set; } = false;
    }
}
