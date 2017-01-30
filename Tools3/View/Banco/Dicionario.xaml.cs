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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tools3.View.Banco
{
	/// <summary>
	/// Interaction logic for Dicionario.xaml
	/// </summary>
	public partial class Dicionario : Page
	{
		Tools3.ViewModel.Banco.DicionarioViewModel viewmodel = new ViewModel.Banco.DicionarioViewModel();
		public Dicionario()
		{
			InitializeComponent();
			DataContext = viewmodel;
			Conexao.ConexaoAberta += Connectou;
			Conexao.ConexaoFechada += DesConectou;
		}


		public void Connectou(object sender, EventArgs e)
		{
			viewmodel.CN = Conexao.ConexaoDB;
		}
		public void DesConectou(object sender, EventArgs e)
		{
			viewmodel.CN = Conexao.ConexaoDB;
		}

		private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			viewmodel.Selecionado = (Tools3.Model.Banco.BuscaBanco.ObjetoBancoModel) treeView.SelectedItem;
		}
	}
}
