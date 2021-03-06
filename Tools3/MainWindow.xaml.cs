﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tools3
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}


		private void btnNavega_Click(object sender, RoutedEventArgs e)
		{
			//FMApoio.Navigate(new View.PublicacaoPg(viewmodel));
			//FMPrincipal.Navigate(typeof(View.Banco.Sql2Codigo));
			Button botao = (Button)sender;
			Titulo.Text = botao.Content.ToString();
			switch (botao.CommandParameter.ToString())
			{
				case "sql2codigo":
					FMPrincipal.Navigate(new View.Banco.Sql2Codigo());
					break;
				case "buscabanco":
					FMPrincipal.Navigate(new View.Banco.BuscaBanco());
					break;
				case "dicionario":
					FMPrincipal.Navigate(new View.Banco.Dicionario());
					break;
				case "montain":
					FMPrincipal.Navigate(new View.Banco.MontaIn());
					break;
				case "gerainsert":
					FMPrincipal.Navigate(new View.Banco.GeraInsert());
					break;
				case "criaclasse":
					FMPrincipal.Navigate(new View.Programacao.CriaClasse());
					break;
				case "lelinhas":
					FMPrincipal.Navigate(new View.LeLinhas());
					break;
			}


		}


	}
}
