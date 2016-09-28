using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ToolsDev
{
	public class clConfiguracao
	{

		public clConfiguracao()
		{
			XmlDocument docParametro=new XmlDocument();
			this.ServidoresBanco = new List<clSrvBanco>();
			if (System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "parametros.xml"))
			{
				docParametro.Load(System.AppDomain.CurrentDomain.BaseDirectory + "parametros.xml");
				this.ServidoresBanco = clSrvBanco.LeXML(docParametro);
			}

		}
		public IList<clSrvBanco> ServidoresBanco { get; set; }

		//IList<clSrvBanco> ServidoresBanco = new List<clSrvBanco>();
	}
}
