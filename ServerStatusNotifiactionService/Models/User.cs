﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverStatusNotifier.Models
{
	public class User
	{
		public string Firstname { get; set; }

		public string Lastname { get; set; }

		public string Fullname => string.Concat(Firstname, " ", Lastname);

		public string Email { get; set; }
	}

	public class NotifyEmails
	{
		public List<User> Users { get; set; } = new List<User>();
	}
}
