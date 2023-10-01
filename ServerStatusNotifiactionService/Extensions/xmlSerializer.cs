using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace serverStatusNotifier.Extensions
{
	public class xmlSerializer
	{
		public T? Deserialize<T>(string xmlString)
		{
			try
			{
				var xmlSerializer = new XmlSerializer(typeof(T));
				var stringreader = new StringReader(xmlString);

				var instance = (T?)xmlSerializer.Deserialize(stringreader);
				return instance;
			}
			catch (Exception ex)
			{
				// _logger.Information(ex.Message);
			}

			return default;
		}
	}
}
