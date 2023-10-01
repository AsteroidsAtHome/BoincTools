using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverStatusNotifier.Models
{
	public static class Messages
	{
		public static readonly string ResultsReadyToSend = "Server is running low on \"results_ready_to_send\" (only {0} tasks left). You may consider adding new tasks any time soon.";

		public static readonly string ResultsInProgress = "There are too few \"results_in_progress\" (only {0} tasks). You may want to check the server for any issues.";

		public static readonly string WorkunitsWaitingForValidation = "There are too many \"workunits_waiting_for_validation\" ({0}). You may want to check the server for any issues.";

		public static readonly string WorkunitsWaitingForAssimilation = "There are too many \"workunits_waiting_for_assimilation\" ({0}). You may want to check the server for any issues.";

		public static readonly string WorkunitsWaitingForDeletion = "There are too many \"workunits_waiting_for_deletion\" ({0}). You may want to check the server for any issues.";

		public static readonly string ResultsWaitingForDeletion = "There are too many \"workunits_waiting_for_deletion\" ({0}). You may want to check the server for any issues.";

		public static readonly string HostsWithResentCredit = "There are too few \"hosts_with_recent_credit\" (just {0} users). You might want to think about how to promote your project.";
	}
}
