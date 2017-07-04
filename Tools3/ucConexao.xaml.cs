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


		public SqlConnection ConexaoDB { get; set; }
		ViewModel.ucConexaoViewModel viewmodel = new ViewModel.ucConexaoViewModel();
		public ucConexao()
		{
			InitializeComponent();
			DataContext = viewmodel;
		}

		public event EventHandler ConexaoAberta;
		public event EventHandler ConexaoFechada;

		protected void Conecta(object sender, EventArgs e)
		{
			ConexaoDB = viewmodel.Connecta();
			ConexaoAberta?.Invoke(this, e);
		}

		protected void DesConecta(object sender, EventArgs e)
		{
			viewmodel.Desconecta();
			ConexaoFechada?.Invoke(this, e);
		}

		protected void DigitaSenha (object sender, EventArgs e)
		{
			PasswordBox campoSenha = (PasswordBox)sender;
			viewmodel.Conexao.Password = campoSenha.Password;
		}

	}
}
