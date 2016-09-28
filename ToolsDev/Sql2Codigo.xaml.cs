using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ToolsDev
{
	/// <summary>
	/// Interaction logic for Sql2Codigo.xaml
	/// </summary>
	public partial class Sql2Codigo : Window
	{
		public Sql2Codigo()
		{
			InitializeComponent();
		}


		private void clickConverteCodigo(object sender, RoutedEventArgs e)
		{
			txtSql.Text = textoCodigo2Sql(txtCodigo.Text);
			if ((bool)chClipBoard.IsChecked)
			{
				System.Windows.Clipboard.SetText(txtSql.Text);
			}

		}



		private void clickConverteSql(object sender, RoutedEventArgs e)
		{
			string pInicio = "";
			string inicio = "";
			string termino = "";
			if ((bool)vbLegado.IsChecked)
			{
				pInicio = string.Format("{0} = \"\" ", txtVariavel.Text);
				inicio = string.Format("{0} = {0} + \" ", txtVariavel.Text);
				termino = "\"";
			}
			if ((bool)vbNet.IsChecked)
			{
				pInicio = string.Format("{0} = \"\" ", txtVariavel.Text);
				inicio = string.Format("{0} += \" ", txtVariavel.Text);
				termino = "\"";
			}
			if ((bool)vbStringBuilder.IsChecked)
			{
				pInicio = string.Format("{0}.Clear()", txtVariavel.Text);
				inicio = string.Format("{0}.Append(\" ", txtVariavel.Text);
				termino = " \")";
			}
			if ((bool)csStringBuilder.IsChecked)
			{
				pInicio = string.Format("{0}.Clear();", txtVariavel.Text);
				inicio = string.Format("{0}.Append(\" ", txtVariavel.Text);
				termino = " \");";
			}

			txtCodigo.Text = textoSql2Codigo(txtSql.Text, pInicio, inicio, termino);
			if ((bool)chClipBoard.IsChecked)
			{
				System.Windows.Clipboard.SetText(txtCodigo.Text);
			}
		}

		private string textoCodigo2Sql(string codigo)
		{
			StringBuilder retorno = new StringBuilder();
			string[] linhas = codigo.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
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
					if ((bool)csStringBuilder.IsChecked)
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

			}

			return retorno.ToString();
		}

		private string textoSql2Codigo(string sql, string strpInicio, string strInicio, string strFim)
		{
			StringBuilder retorno = new StringBuilder();
			retorno.AppendLine(strpInicio);


			string[] linhas = sql.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string linha in linhas)
			{
				string iLinha = linha.Trim();

				retorno.AppendFormat("{0}{1}{2} \n", strInicio, iLinha, strFim);
			}
			return retorno.ToString();
		}

		private void dblclick_txtSql(object sender, MouseButtonEventArgs e)
		{
			txtSql.Text = System.Windows.Clipboard.GetText();
			clickConverteSql(null, null);
		}

		private void txtCodigo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			txtCodigo.Text = System.Windows.Clipboard.GetText();
			clickConverteCodigo(null, null);
		}

	}
}
