using System.Windows.Controls;

namespace Tools3.View.Banco
{
	/// <summary>
	/// Interaction logic for Sql2Codigo.xaml
	/// </summary>
	public partial class Sql2Codigo : Page
	{
		public Sql2Codigo()
		{
			InitializeComponent();
			DataContext = new ViewModel.Banco.Sql2CodigoViewModel();
		}
	}
}
