using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
namespace ToolsDev
{
	public class clSrvBanco
	{
		System.Data.SqlClient.SqlConnectionStringBuilder sqlcn = new System.Data.SqlClient.SqlConnectionStringBuilder();
		clSrvBanco()
		{

		}
		public string Nome { get; set; }
		public string ConnectionString { get; set; }

		public static IList<clSrvBanco> LeXML(XmlDocument doc)
		{
			IList<clSrvBanco> retorno = new List<clSrvBanco>();
			foreach (XmlNode item in doc.GetElementsByTagName("SrvBancos"))
			{ 
				retorno.Add(new clSrvBanco(){
					Nome=item.Attributes["Nome"].Value,
					ConnectionString = item.Attributes["ConnectionString"].Value
				});
			
			}

			return retorno;

		}
	}
}
