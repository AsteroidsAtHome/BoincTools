using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverStatusNotifier.Extensions
{
	public static class Extensions
	{
		public static bool CompareWith(this int value, int level, string comparer)
		{
			var result = false;
			switch (comparer)
			{
				case ">":
					result = level > value;
					break;
				case "<":
					result = level < value;
					break;
				case ">=":
					result = level >= value;
					break;
				case "<=":
					result = level <= value;
					break;
				case "==":
					result = level == value;
					break;
				case "!=":
					result = level != value;
					break;
				default:
					result = false;
					break;
			}

			return result;
		}
	}
}
