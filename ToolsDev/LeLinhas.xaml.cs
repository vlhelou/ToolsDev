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
using System.IO;

namespace ToolsDev
{
	/// <summary>
	/// Interaction logic for LeLinhas.xaml
	/// </summary>
	public partial class LeLinhas : Window
	{
		public LeLinhas()
		{
			InitializeComponent();
			//txtArquivo.Text = @"D:\Temporario\TSE\Prestacao\prestacao_contas_2008\2008\Candidato\Despesa\\DespesaCandidato.uft8";
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			//txtArquivo.Text
			if (File.Exists(txtArquivo.Text))
			{
				int qtln = 20;
				int lnini = 0;
				int.TryParse(qtLinhas.Text, out qtln);
				int.TryParse(linhaInicial.Text, out lnini);
				StreamReader sr = new StreamReader(txtArquivo.Text);
				for (int i = 0; i < lnini; i++)
				{
					sr.ReadLine();
				}
				System.Text.StringBuilder sbLinhas = new StringBuilder();
				for (int i = 0; i < qtln; i++)
				{
					sbLinhas.AppendLine(sr.ReadLine());
				}
				sr.Close();
				linhas.Text = sbLinhas.ToString();
			}
		}
	}
}
