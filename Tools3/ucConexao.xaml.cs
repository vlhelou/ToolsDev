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
using System.Data.SqlClient;

namespace Tools3
{
	/// <summary>
	/// Interaction logic for ucConexao.xaml
	/// </summary>
	public partial class ucConexao : UserControl
	{

		SqlConnection ConexaoDB;
		ViewModel.ucConexaoViewModel viewmodel = new ViewModel.ucConexaoViewModel();
		public ucConexao()
		{
			InitializeComponent();
			DataContext = viewmodel;
		}

		private void Connecta_Click(object sender, RoutedEventArgs e)
		{
			ConexaoDB = viewmodel.Connecta();
		}

	}
}
