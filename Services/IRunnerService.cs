using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using serverStatusNotifier.Models;

namespace serverStatusNotifier.Services
{
	public interface IRunnerService
	{
		void CheckServerStatus(ServerStatus serverStatus);
	}
}
