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
using System.Data;

namespace ToolsDev
{
	/// <summary>
	/// Interaction logic for ucConexao.xaml
	/// </summary>
	public partial class ucConexao : UserControl
	{
		public SqlConnection cn;
		clConfiguracao configuracao = new clConfiguracao();

		public bool Conectado { get; set; }

		public ucConexao()
		{
			InitializeComponent();
			cbServidor.ItemsSource = configuracao.ServidoresBanco;
			this.Conectado = false;

		}

		public event EventHandler ConexaoAberta;

		private void Connecta_Click(object sender, RoutedEventArgs e)
		{
			this.Conectado = false;
			this.cn = new SqlConnection();
			SqlConnectionStringBuilder sqlcnb = new SqlConnectionStringBuilder();
			sqlcnb.DataSource = txtServidor.Text;
			sqlcnb.IntegratedSecurity = !(bool)chTrust.IsChecked;
			if (!sqlcnb.IntegratedSecurity)
			{
				sqlcnb.UserID = txtUsuario.Text;
				sqlcnb.Password = txtSenha.Password;
			}
			cn = new SqlConnection(sqlcnb.ConnectionString);
			cn.Open();
			if (cn.State == ConnectionState.Open)
			{
				this.Conectado = true;
				ConexaoAberta(this, EventArgs.Empty);
				SqlCommand cmd = cn.CreateCommand();
				cmd.CommandText = "select name from master.sys.databases order by name";
				DataTable dt = new DataTable();
				dt.Load(cmd.ExecuteReader());
			}
		}

		private void cbServidor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox cb = (ComboBox)sender;

			try
			{
				SqlConnectionStringBuilder sqlcb = new SqlConnectionStringBuilder(cb.SelectedValue.ToString());
				txtServidor.Text = sqlcb.DataSource;
				chTrust.IsChecked = !sqlcb.IntegratedSecurity;
				txtUsuario.Text = sqlcb.UserID;
				txtSenha.Password = sqlcb.Password;

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public IList<string> ListaBases() {
			IList<string> retorno = new List<string>();
			SqlCommand cmd = this.cn.CreateCommand();
			cmd.CommandText = "select name from sys.databases order by name";
			DataTable dt = new DataTable();
			dt.Load(cmd.ExecuteReader());
			foreach (DataRow ln in dt.Rows)
				retorno.Add(ln[0].ToString());


			return retorno;
		}
	}
}
