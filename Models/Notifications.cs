using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace serverStatusNotifier.Models
{
	public class NotificationsModel
	{
        public NotificationsModel()
        {
        }

        public NotificationsModel(List<Notification> notifications)
        {
			Notifications = new List<Notification>();
			Notifications.AddRange(notifications);
        }

        [XmlElement(ElementName = "Notifications")]
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
