using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tools3.ViewModel
{
	public class LeLinhasViewModel : INotifyPropertyChanged
	{
		public string ArquivoPath { get; set; }
		public int Inicio { get; set; } = 1;
		public int Termino { get; set; } = 3;
		private string Tipo { get; set; } = "linha";

		public bool rdLinha
		{
			get { return Tipo == "linha"; }
			set
			{
				if (value)
					Tipo = "linha";
			}
		}
		public bool rdByte
		{
			get { return Tipo == "byte"; }
			set
			{
				if (value)
					Tipo = "byte";

			}
		}

		public string Lido { get; set; }

		public List<EncodingInfo> Encodes { get; set; } = Encoding.GetEncodings().OrderBy(p=>p.DisplayName).ToList();

		public EncodingInfo EncodeSelecionado { get; set; }
		public ICommand ExecutaClick
		{
			get { return new CommandHandler(() => Executa()); }
		}

		private void Executa()
		{
			//EncodingInfo x;
			//x.DisplayName
			StringBuilder resultado = new StringBuilder();
			if (System.IO.File.Exists(ArquivoPath) & EncodeSelecionado!=null)
			{

				System.IO.FileInfo fi = new System.IO.FileInfo(ArquivoPath);

				//System.IO.StreamReader sr = new System.IO.StreamReader(fi.FullName,EncodeSelecionado.GetEncoding());
				using (System.IO.StreamReader sr = new System.IO.StreamReader(fi.FullName, EncodeSelecionado.GetEncoding()))
				{
					if (Tipo == "linha")
					{
						for (int i = 1; i <= Termino; i++)
						{
							string linha = sr.ReadLine();
							if (i >= Inicio)
							{
								resultado.AppendLine(linha);
							}
						}
					}
					else
					{
						int total = (Termino - Inicio);
						char[] retorno = new char[total];
						for (int i = 0; i <= Inicio; i++)
							sr.Read();
						sr.Read(retorno, 0, retorno.Length);
						resultado.Append(new string(retorno));
					}
				}
				Lido = resultado.ToString();
				OnPropertyChanged(nameof(Lido));
			}
		}
		protected void OnPropertyChanged(string propertyname)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;

	}
}
