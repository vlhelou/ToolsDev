using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	/// Interaction logic for BuscaBanco.xaml
	/// </summary>
	public partial class BuscaBanco : Page
	{
		private ViewModel.Banco.BuscaBancoViewModel viewmodel = new ViewModel.Banco.BuscaBancoViewModel();
		public BuscaBanco()
		{
			InitializeComponent();
			DataContext = viewmodel;
			viewmodel.BarraExecucao = pgExecucao;
			Conexao.ConexaoAberta += Connectou;
		}

		public void Connectou (object sender, EventArgs e)
		{
			//Debug.WriteLine("connectou");
			//Debug.WriteLine(Conexao.ConexaoDB.State);
			viewmodel.CN = Conexao.ConexaoDB;
		}

	}
}
