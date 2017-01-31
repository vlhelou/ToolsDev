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

namespace Tools3.View.Programacao
{
	/// <summary>
	/// Interaction logic for CriaClasse.xaml
	/// </summary>
	public partial class CriaClasse : Page
	{
		Tools3.ViewModel.Programacao.CriaClasseViewModel viewmodel = new ViewModel.Programacao.CriaClasseViewModel();
		public CriaClasse()
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
			viewmodel.Selecionado = (Tools3.Model.Banco.BuscaBanco.ObjetoBancoModel)treeView.SelectedItem;
		}

	}
}
