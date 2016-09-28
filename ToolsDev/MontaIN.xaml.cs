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
	/// Interaction logic for MontaIN.xaml
	/// </summary>
	public partial class MontaIN : Window
	{
		public MontaIN()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			string[] linhas = txtOrigem.Text.Split(Environment.NewLine.ToCharArray());
			List<string> conversao = new List<string>();
			if ((bool)opData.IsChecked)
			{
				
				DateTime iData;
				foreach(string linha in linhas)
				{
					if (DateTime.TryParse(linha, out iData))
						conversao.Add(string.Format("'{0}'",iData.ToString("yyyy-MM-dd HH:mm:ss")));
				}
			}
			if ((bool)opString.IsChecked)
			{ 
				foreach(string linha in linhas)
				{
					if (!string.IsNullOrEmpty(linha))
						conversao.Add(string.Format("'{0}'", linha.Replace("'", "''")));
				}
					
			}

			if ((bool)opSemAlteracao.IsChecked)
			{
				foreach (string linha in linhas)
				{
					if (!string.IsNullOrEmpty(linha))
						conversao.Add(linha);
				}

			}
			txtDestino.Text = "";
			if (conversao.Count > 0)
			{
				txtDestino.Text = "(" + string.Join(",", conversao) + ")";
				if ((bool)chClipBoard.IsChecked)
				{
					System.Windows.Clipboard.SetText(txtDestino.Text);
				}

			}
				
		}

		private void txtOrigem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			txtOrigem.Text = System.Windows.Clipboard.GetText();
			Button_Click(null, null);
		}

	}
}
