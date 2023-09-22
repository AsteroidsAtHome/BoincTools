using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace serverStatusNotifier.Models
{

	[XmlRoot(ElementName = "server_status")]
	public class ServerStatus
	{
		[XmlElement(ElementName = "database_file_states")]
		public DatabaseFileStates DatabaseFileStates { get; set; }
	}

	public class DatabaseFileStates
	{
		[XmlElement(ElementName = "results_ready_to_send")]
		public int ResultsReadyToSend { get; set; }

		[XmlElement(ElementName = "results_in_progress")]
		public int ResultsInProgress { get; set; }

		[XmlElement(ElementName = "workunits_waiting_for_validation")]
		public int WorkunitsWaitingForValidation { get; set; }

		[XmlElement(ElementName = "workunits_waiting_for_assimilation")]
		public int WorkunitsWaitingForAssimilation { get; set; }

		[XmlElement(ElementName = "workunits_waiting_for_deletion")]
		public int WorkunitsWaitingForDeletion { get; set; }

		[XmlElement(ElementName = "results_waiting_for_deletion")]
		public int ResultsWaitingForDeletion { get; set; }

		[XmlElement(ElementName = "transitioner_backlog_hours")]
		public double TransitionerBacklogHours { get; set; }

		[XmlElement(ElementName = "users_with_resent_credit")]
		public int UsersWithRecentCredit { get; set; }

		[XmlElement(ElementName = "users_with_credit")]
		public int UsersWithCredit { get; set; }

		[XmlElement(ElementName = "users_registered_in_past_24_hours")]
		public int UsersRegisteredInPast24Hours { get; set; }

		[XmlElement(ElementName = "hosts_with_recent_credit")]
		public int HostsWithResentCredit { get; set; }

		[XmlElement(ElementName = "hosts_with_credit")]
		public int HostsWithCredit { get; set; }

		[XmlElement(ElementName = "hosts_registered_in_past_24_hours")]
		public int HostsRegisteredInPast24Hours { get; set; }

		[XmlElement(ElementName = "current_floating_point_speed")]
		public double CurrentFloatingPointSpeed { get; set; }
	}
}
