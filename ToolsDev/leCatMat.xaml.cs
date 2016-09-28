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
	/// Interaction logic for leCatMat.xaml
	/// </summary>
	public partial class leCatMat : Window
	{
		System.Windows.Forms.WebBrowser wb = new System.Windows.Forms.WebBrowser();
		public leCatMat()
		{
			InitializeComponent();
			wb.DocumentCompleted += wb_DocumentCompleted;
			wb.ScriptErrorsSuppressed = true;

			wb.Navigate(new Uri("file:///C:\\Temporario\\catmat\\catmat_A_1.html"));
			//this.Close();
		}

		private void wb_DocumentCompleted(object sender, EventArgs e)
		{
			//MessageBox.Show(wb.DocumentText.Length.ToString());
			//MessageBox.Show("ponto");
			
			System.Windows.Forms.HtmlElement tabela = wb.Document.GetElementsByTagName("table")[3];
			System.Windows.Forms.HtmlElementCollection listatr = tabela.GetElementsByTagName("tr");
			StringBuilder isaida = new StringBuilder();
			for (int i = 0; i < listatr.Count; i++)
			{
				isaida.AppendLine(listatr[i].InnerText);
			}
			saida.Text = isaida.ToString();
			//MessageBox.Show(tabela.InnerText.Substring(0,200));
		}
	}
}
