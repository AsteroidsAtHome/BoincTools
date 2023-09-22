using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace serverStatusNotifier
{
	public enum NotificationEnums
	{
		[JsonProperty("ResultsReadyToSendLowerLimit")]
		[Display(Name = "ResultsReadyToSendLowerLimit")]
		ResultsReadyToSend = 1,

		[JsonProperty("ResultsInProgressLowerLimit")]
		[Display(Name = "ResultsInProgressLowerLimit")]
		ResultsInProgressLower =2,

		[JsonProperty("WorkunitsWaitingForValidationUpperLimit")]
		[Display(Name = "WorkunitsWaitingForValidationUpperLimit")]
		WorkunitsWaitingForValidation = 3,

		[JsonProperty("WorkunitsWaitingForAssimilationUpperLimit")]
		[Display(Name = "WorkunitsWaitingForAssimilationUpperLimit")]
		WorkunitsWaitingForAssimilation = 4,

		[JsonProperty("WorkunitsWaitingForDeletionUpperLimit")]
		[Display(Name = "WorkunitsWaitingForDeletionUpperLimit")]
		WorkunitsWaitingForDeletion = 5,

		[JsonProperty("ResultsWaitingForDeletionUpperLimit")]
		[Display(Name = "ResultsWaitingForDeletionUpperLimit")]
		ResultsWaitingForDeletion = 6,

		[JsonProperty("HostsWithResentCreditLowerLimit")]
		[Display(Name = "HostsWithResentCreditLowerLimit")]
		HostsWithResentCredit = 7
	}
}
