using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tools3.ViewModel.Banco
{
	public class Sql2CodigoViewModel : INotifyPropertyChanged
	{

		private string Saida { get; set; } = "stringbuilder";
		public string Variavel { get; set; } = "sql";
		public string SQL { get; set; }
		public string Codigo { get; set; }


		public bool SaidaIgual
		{
			get
			{
				return Saida == "+=";
			}
			set
			{
				if (value)
					Saida = "+=";
				else
					Saida = "stringbuilder";
				OnPropertyChanged(nameof(SaidaIgual));
				OnPropertyChanged(nameof(SaidaStringBuilder));
			}
		}
		public bool SaidaStringBuilder
		{
			get
			{
				return Saida == "stringbuilder";
			}
			set
			{
				if (value)
					Saida = "stringbuilder";
				else
					Saida = "+=";
				OnPropertyChanged(nameof(SaidaIgual));
				OnPropertyChanged(nameof(SaidaStringBuilder));
			}

		}


		public ICommand SqltoCodigoClick
		{
			get
			{
				return new CommandHandler(() => SqltoCodigoDo());
			}
		}

		public void SqltoCodigoDo()
		{
			StringBuilder retorno = new StringBuilder();
			string strFim = "";
			string strInicio = "";
			if (Saida == "+=")
			{
				retorno.AppendFormat("{0}=\"\" \n", Variavel);
				strInicio = $"{Variavel}+=";
				strFim = ";";
			}
			else
			{
				strInicio = $"{Variavel}.append(";
				strFim = ");";

			}

			string[] linhas = SQL.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string linha in linhas)
			{
				string iLinha = linha.Trim();

				retorno.AppendFormat("{0}\"{1}\"{2} \n", strInicio, iLinha, strFim);
			}
			Codigo = retorno.ToString();
			System.Windows.Clipboard.SetText(Codigo);
			OnPropertyChanged(nameof(Codigo));
		}

		public ICommand CodigotoSqlClick
		{
			get
			{
				return new CommandHandler(() => CodigotoSqlDo());
			}
		}

		public void CodigotoSqlDo()
		{
			StringBuilder retorno = new StringBuilder();
			string[] linhas = Codigo.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			int qtTabs = 0;
			foreach (string linha in linhas)
			{
				string iLinha = linha.Trim();
				int pontoi = iLinha.IndexOf('"');
				int pontof = iLinha.LastIndexOf('"');

				if ((pontoi < pontof) & (pontof * pontoi > 0))
				{
					string resultado = iLinha.Substring(pontoi + 1, pontof - pontoi - 1).Replace("''", "'").Trim();
					//verifica se o código esta comentado
					if (SaidaStringBuilder)
					{
						if (iLinha.StartsWith("//"))
							continue;
					}
					else
					{
						if (iLinha.StartsWith("'"))
							continue;
					}


					if (resultado.ToUpper().StartsWith("SELECT"))
						qtTabs = 0;
					if (resultado.ToUpper().StartsWith("FROM"))
						qtTabs = 0;
					if (resultado.ToUpper().StartsWith("WHERE"))
						qtTabs = 0;
					if (resultado.ToUpper().StartsWith("GROUP BY"))
						qtTabs = 0;
					if (resultado.ToUpper().StartsWith("ORDER BY"))
						qtTabs = 0;
					if (resultado.ToUpper().StartsWith("INNER"))
						qtTabs = 1;

					retorno.AppendLine(string.Join("", Enumerable.Repeat("\t", qtTabs)) + resultado);

					if (resultado.ToUpper().StartsWith("SELECT"))
						qtTabs = 1;
					if (resultado.ToUpper().StartsWith("FROM"))
						qtTabs = 1;
					if (resultado.ToUpper().StartsWith("WHERE"))
						qtTabs = 1;
					if (resultado.ToUpper().StartsWith("GROUP BY"))
						qtTabs = 1;
					if (resultado.ToUpper().StartsWith("ORDER BY"))
						qtTabs = 1;

					if (resultado.ToUpper().StartsWith("INNER"))
						qtTabs = 2;

				}
				SQL = retorno.ToString();
				OnPropertyChanged(nameof(SQL));

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
