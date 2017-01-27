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
	/// Interaction logic for GeraInsert.xaml
	/// </summary>
	public partial class GeraInsert : Page
	{
		Tools3.ViewModel.Banco.GeraInsertViewModel viewmodel = new Tools3.ViewModel.Banco.GeraInsertViewModel();
		public GeraInsert()
		{
			InitializeComponent();
			DataContext = viewmodel;
			Conexao.ConexaoAberta += Connectou;
		}

		public void Connectou(object sender, EventArgs e)
		{
			viewmodel.CN = Conexao.ConexaoDB;
		}

	}
}
