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
using System.Data;
using System.Data.SqlClient;

namespace ToolsDev
{
	/// <summary>
	/// Interaction logic for GeraInsert.xaml
	/// </summary>
	public partial class GeraInsert : Window
	{
		private DataTable dtRegistros;
		public GeraInsert()
		{
			InitializeComponent();
		}

		private void cbBase_SelectionChanged(object sender, EventArgs args)
		{
			//MessageBox.Show(cbBase.SelectedValue.ToString());
			Conexao.cn.ChangeDatabase(cbBase.SelectedValue.ToString());
		}

		private void ConexaoAberta(object sender, EventArgs args)
		{
			cbBase.IsEnabled = true;
			cbBase.ItemsSource = Conexao.ListaBases();
		}

		private void ExecutaSQL(object sender, EventArgs args)
		{
			SqlCommand cmd = Conexao.cn.CreateCommand();
			dtRegistros = new DataTable();
			cmd.CommandText = txtSql.Text;
			try
			{
				dtRegistros.Load(cmd.ExecuteReader());
				grResultado.ItemsSource = dtRegistros.DefaultView;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void PressF5(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.F5)
				ExecutaSQL(null, null);
		}

		private void GeraScript(object sender, RoutedEventArgs e)
		{
			if ((dtRegistros != null) && (dtRegistros.Rows.Count > 0))
			{

				List<string> Colunas = new List<string>();
				foreach (DataColumn coluna in dtRegistros.Columns)
					Colunas.Add(coluna.ColumnName);
				StringBuilder sqlInsert = new StringBuilder();
				sqlInsert.AppendFormat("insert into {0} ({1}) \n values ", txtTabela.Text, string.Join(",", Colunas));
				sqlInsert.AppendLine(MontaLinhas());
				txtSql.Text = sqlInsert.ToString();
			}
		}

		private string MontaLinhas()
		{
			List<string> registros = new List<string>();
			foreach (DataRow ln in dtRegistros.Rows)
			{
				registros.Add(string.Format("({0})", MontaColuna(ln)));
			}
			return string.Format("{0}", string.Join("\n,", registros));
		}

		private string MontaColuna(DataRow ln)
		{
			List<String> vlrColuna = new List<string>();
			foreach (DataColumn coluna in dtRegistros.Columns)
			{
				if (ln.IsNull(coluna))
					vlrColuna.Add("NULL");
				else
				{
					switch (coluna.DataType.ToString().ToLower())
					{
						case "system.decimal":
							vlrColuna.Add(ln[coluna].ToString().Replace(",", "."));
							break;
						case "system.string":
							vlrColuna.Add('\'' + ln[coluna].ToString().Replace("'", "''") + '\'');
							break;
						case "system.int":
						case "system.int16":
						case "system.int32":
						case "system.int64":
							vlrColuna.Add(ln[coluna].ToString());
							break;
						case "system.datetime":
							vlrColuna.Add('\'' + Convert.ToDateTime(ln[coluna]).ToString("yyyy-MM-dd HH:mm:ss") + '\'');
							break;
						case "system.boolean":
							vlrColuna.Add((bool)ln[coluna] ? "1" : "0");
							break;
						default:
							vlrColuna.Add(ln[coluna].ToString());
							break;

					}
				}
			}
			return string.Join("", string.Join(",", vlrColuna));
		}



	}
}
