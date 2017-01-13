using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace Tools3.Model
{
	public class ucConexaoModel 
	{
		public string Server { get; set; }
		public string Database { get; set; }
		public bool Trust { get; set; }
		public bool NTrust => !Trust;

		public string User { get; set; }
		public string Password { get; set; }

		public string Nome => $"{Server} ({Database})";


	}
}
