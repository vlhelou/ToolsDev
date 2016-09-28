using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;

namespace ToolsDev
{
	/// <summary>
	/// Interaction logic for CriaClasse.xaml
	/// </summary>
	public partial class CriaClasse : Window
	{
		clConfiguracao configuracao = new clConfiguracao();
		SqlConnection cn = new SqlConnection();

		private string objetoNome;
		private string schemaNome;

		public CriaClasse()
		{
			InitializeComponent();
			cbServidor.ItemsSource = configuracao.ServidoresBanco;


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
		private void Connecta_Click(object sender, RoutedEventArgs e)
		{

			cbBases.IsEnabled = false;
			btnCria.IsEnabled = false;
			txtObjeto.IsEnabled = false;
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
				cbBases.IsEnabled = true;
				btnCria.IsEnabled = true;
				txtObjeto.IsEnabled = true;
				SqlCommand cmd = cn.CreateCommand();
				cmd.CommandText = "select name from master.sys.databases order by name";
				DataTable dt = new DataTable();
				dt.Load(cmd.ExecuteReader());
				cbBases.ItemsSource = dt.DefaultView;

			}
		}

		private void cbBases_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox cb = (ComboBox)sender;
			cn.ChangeDatabase(cb.SelectedValue.ToString());
		}

		private void btnCria_Click(object sender, RoutedEventArgs e)
		{
			SqlCommand cmd = cn.CreateCommand();

			objetoNome = "";
			schemaNome = "";

			cmd.Parameters.AddWithValue("@objeto", txtObjeto.Text);
			cmd.CommandText = @"select name, SCHEMA_NAME(schema_id) [schema]
				from sys.objects 
				where 
					object_id=OBJECT_ID(@objeto)";
			DataTable dtobj = new DataTable();
			dtobj.Load(cmd.ExecuteReader());
			if (dtobj.Rows.Count == 1)
			{
				objetoNome = dtobj.Rows[0]["name"].ToString();
				schemaNome = dtobj.Rows[0]["schema"].ToString();
			}



			cmd.CommandText = @"select 
					cl.name NomeColuna
					, tp.name NomeTipo
					, cl.max_length
					, cl.is_nullable
					, isnull((SELECT top 1 value FROM fn_listextendedproperty('prog', 'schema', @sc, 'table',@ob, 'Column', cl.name)), cl.name ) as NomeObjeto
				from sys.columns cl
					inner join sys.systypes tp on 
						cl.system_type_id = tp.xtype
				where object_id=object_id(@objeto)  and tp.name not like 'DOM%'
				order by column_id";
			cmd.Parameters.AddWithValue("@ob", objetoNome);
			cmd.Parameters.AddWithValue("@sc", schemaNome);
			DataTable dt = new DataTable();
			dt.Load(cmd.ExecuteReader());




			txtEntidade.Text = CriaEntidade(dt);
			txtRepositorio.Text = CriaRepositorio(dt);


		}

		private string CriaEntidade(DataTable colunas)
		{
			StringBuilder retorno = new StringBuilder();
			foreach (DataRow ln in colunas.Rows)
			{
				string tipo = TipoSql2CS(ln["NomeTipo"].ToString());
				if (((bool)ln["is_nullable"]) & (tipo != "string"))
					tipo += "?";
				retorno.AppendFormat("public {0} {1}  {{ get;set; }} \n", tipo, ln["NomeObjeto"]);
			}
			return retorno.ToString();
		}
		private string CriaRepositorio(DataTable colunas)
		{
			string nomeObjeto = ObtemAtribuioPROG();
			if (nomeObjeto == "")
				nomeObjeto = objetoNome;
			StringBuilder retorno = new StringBuilder();
			List<string> listaColunas = new List<string>();
			List<string> listaColunasAspas = new List<string>();
			foreach (DataRow ln in colunas.Rows)
			{
				listaColunas.Add(ln["NomeColuna"].ToString());
				listaColunasAspas.Add("\"" + ln["NomeColuna"].ToString() + "\"");
			}

			retorno.AppendLine(RepositorioConexao(listaColunasAspas));

			retorno.AppendLine(RepositorioAtribui(nomeObjeto, colunas));

			retorno.AppendLine(RepositorioBusca(nomeObjeto, listaColunas));
			retorno.AppendLine(RepositorioGrava(nomeObjeto, listaColunas, colunas));
			retorno.AppendLine(RepositorioExclui(nomeObjeto, listaColunas));
			retorno.AppendLine(RepositorioDispose());

			return retorno.ToString();
		}


		private string TipoSql2CS(string tipo)
		{
			string retorno = "";
			switch (tipo)
			{

				case "image":
				case "varbinary":
				case "binary":
					retorno = "byte[]";
					break;

				case "text":
				case "ntext":
				case "varchar":
				case "char":
				case "nvarchar":
				case "nchar":
				case "xml":
					retorno = "string";
					break;

				case "date":
				
				case "datetime2":
				case "datetimeoffset":
				case "smalldatetime":
				case "datetime":
				case "timestamp":
					retorno = "DateTime";
					break;
				case "time":
					retorno = "TimeSpan";
					break;
				case "real":
				case "money":
				case "float":
				case "decimal":
				case "numeric":
				case "smallmoney":
					retorno = "decimal";
					break;


				case "tinyint":
					retorno = "byte";
					break;
				case "smallint":
					retorno = "short";
					break;
				case "int":
					retorno = "int";
					break;
				case "bigint":
					retorno = "Int64";
					break;
				case "bit":
					retorno = "bool";
					break;
				case "uniqueidentifier":
					retorno = "Guid";
					break;
			}
			return retorno;

		}

		private string ObtemAtribuioPROG()
		{
			object retorno;
			SqlCommand cmd = cn.CreateCommand();
			cmd.CommandText = "SELECT value FROM fn_listextendedproperty ('prog', 'schema', @schema, 'table',@objeto, null, null);";
			cmd.Parameters.AddWithValue("@schema", this.schemaNome);
			cmd.Parameters.AddWithValue("@objeto", this.objetoNome);
			retorno = cmd.ExecuteScalar();
			if (retorno != null)
				return retorno.ToString();
			return "";
		}


		private string  RepositorioConexao(List<string> listaColunasAspas)
		{
			StringBuilder retorno = new StringBuilder();
			//cria conexao
			retorno.AppendLine("private SqlCommand cmd;");
			retorno.AppendLine("private SqlDataAdapter da;");
			retorno.AppendLine("private bool disposed = false;");
			//cria lista de colunas
			retorno.AppendFormat("public static List<string> _lstColunas = new List<string>() {{ {0} }}; \r\n", string.Join(",", listaColunasAspas));

			//retorno.AppendFormat("private const string _Colunas = \"{0}\"; \n", string.Join(",", listaColunas));
			retorno.AppendLine("public static string _Colunas = Util.MontaColunas(_lstColunas, \"\", \"\");");
			//cria construtor
			retorno.AppendFormat("public {0}Rpt(SqlCommand Cmd) \n", objetoNome);
			retorno.AppendLine("{ ");
			retorno.AppendLine("cmd = Cmd; ");
			retorno.AppendLine("da = new SqlDataAdapter(cmd); ");
			retorno.AppendLine("} ");


			return retorno.ToString();
		}
		
		private string RepositorioAtribui( string nomeObjeto, DataTable colunas)
		{
			StringBuilder retorno = new StringBuilder();

			// Atribui
			retorno.AppendFormat("public static {0}Etd Atribui(DataRow ln, string prefixo=\"\") \n", nomeObjeto);
			retorno.AppendLine("{ ");
			retorno.AppendLine("if (ln != null) ");
			retorno.AppendLine("{ ");
			retorno.AppendFormat("{0}Etd retorno = new {0}Etd(); \n", nomeObjeto);
			foreach (DataRow ln in colunas.Rows)
			{
				string coluna = ln["NomeColuna"].ToString();
				string atributo = ln["NomeObjeto"].ToString();
				string tipo = TipoSql2CS(ln["NomeTipo"].ToString());
				bool nulo = (bool)ln["is_nullable"];
				if (tipo == "string")
				{
					retorno.AppendFormat("retorno.{0} = ln[prefixo+\"{1}\"].ToString(); \n", atributo, coluna);
				}
				else
				{
					if (nulo)
					{
						retorno.AppendFormat("retorno.{0} = ln.IsNull(prefixo+\"{1}\") ? null : ({2}?)ln[prefixo+\"{1}\"]; \n", atributo, coluna, tipo);
					}
					else
						retorno.AppendFormat("retorno.{0} = ({2})ln[prefixo+\"{1}\"]; \n", atributo, coluna, tipo);

				}
			}
			retorno.AppendLine("return retorno; ");
			retorno.AppendLine("} ");
			retorno.AppendLine("return null; ");
			retorno.AppendLine("} ");


			return retorno.ToString();
		}

		private string RepositorioBusca( string nomeObjeto, List<string> listaColunas)
		{
			StringBuilder retorno = new StringBuilder();
			//Busca

			retorno.AppendFormat("public {0}Etd Busca(int Codigo) \n", nomeObjeto);
			retorno.AppendLine("{ ");
			retorno.AppendFormat("cmd.CommandText = string.Format(\"select {{0}} from {0} where {1} = @id\", _Colunas); \n", txtObjeto.Text, listaColunas[0].ToString());
			retorno.AppendLine("cmd.Parameters.AddWithValue(\"@id\", Codigo); ");
			retorno.AppendLine("DataTable dt = new DataTable(); ");
			retorno.AppendLine("da.Fill(dt); ");
			retorno.AppendLine("cmd.Parameters.Clear(); ");
			retorno.AppendLine("if (dt.Rows.Count == 1) ");
			retorno.AppendLine("{ ");
			retorno.AppendLine("return Atribui(dt.Rows[0]); ");
			retorno.AppendLine("} ");
			retorno.AppendLine("return null; ");
			retorno.AppendLine("} ");



			return retorno.ToString();
		}

		private string RepositorioGrava(string nomeObjeto, List<string> listaColunas, DataTable colunas)
		{
			StringBuilder retorno = new StringBuilder();

			//Grava
			retorno.AppendFormat("public {0}Etd Grava({0}Etd Item) ", nomeObjeto);
			retorno.AppendLine("{ ");
			retorno.AppendLine("bool novo = false;");
			retorno.AppendFormat("cmd.CommandText = string.Format(\"select {{0}} from {0} where {1} = @id\", _Colunas); \n", txtObjeto.Text, listaColunas[0].ToString());
			retorno.AppendFormat("cmd.Parameters.AddWithValue(\"@id\", Item.{0}); \n", listaColunas[0].ToString());
			retorno.AppendLine("SqlDataAdapter daup = new SqlDataAdapter(cmd); ");
			retorno.AppendLine("SqlCommandBuilder cb = new SqlCommandBuilder(daup); ");
			retorno.AppendLine("DataTable dtup = new DataTable(); ");
			retorno.AppendLine("daup.Fill(dtup); ");

			retorno.AppendLine("DataRow ln; ");
			retorno.AppendLine("if (dtup.Rows.Count==0) ");
			retorno.AppendLine("{ ");
			retorno.AppendLine("novo = true; ");
			retorno.AppendLine("ln = dtup.NewRow(); ");
			retorno.AppendLine("} ");
			retorno.AppendLine("else ");
			retorno.AppendLine("{ ");
			retorno.AppendLine("ln = dtup.Rows[0]; ");
			retorno.AppendLine("} ");

			foreach (DataRow ln in colunas.Rows)
			{
				string coluna = ln["NomeColuna"].ToString();
				string atributo = ln["NomeObjeto"].ToString();
				string tipo = TipoSql2CS(ln["NomeTipo"].ToString());
				bool nulo = (bool)ln["is_nullable"];
				if (nulo)
				{
					if (tipo == "string")
					{
						retorno.AppendFormat("if (string.IsNullOrEmpty(Item.{0}))   \n", atributo);
						retorno.AppendFormat("ln[\"{0}\"] = DBNull.Value; \n", coluna);
						retorno.AppendLine("else");
						retorno.AppendFormat("ln[\"{0}\"] = Item.{1}.Trim(); \n", coluna, atributo);
					}
					else
					{
						retorno.AppendFormat("if (Item.{0} == null)   \n", atributo);
						retorno.AppendFormat("ln[\"{0}\"] = DBNull.Value; \n", coluna);
						retorno.AppendLine("else");
						retorno.AppendFormat("ln[\"{0}\"] = Item.{1}; \n", coluna, atributo);
					}

				}
				else
					retorno.AppendFormat("ln[\"{0}\"] = Item.{1}; \n", coluna, atributo);
			}

			retorno.AppendLine("if (novo)");
			retorno.AppendLine("dtup.Rows.Add(ln); ");
			retorno.AppendLine("daup.Update(dtup); ");
			retorno.AppendLine("cmd.Parameters.Clear();");
			retorno.AppendLine("if (novo)");
			retorno.AppendLine("{");
			retorno.AppendLine("cmd.CommandText = \"select @@identity\";");
			retorno.AppendFormat("Item.{0} = Convert.ToInt32(cmd.ExecuteScalar());\n", listaColunas[0].ToString());
			retorno.AppendLine("}");

			retorno.AppendFormat("return Busca(Item.{0}); ", listaColunas[0].ToString());
			retorno.AppendLine("} ");


			return retorno.ToString();
		}


		private string RepositorioExclui(string nomeObjeto, List<string> listaColunas)
		{
			StringBuilder retorno = new StringBuilder();
			retorno.AppendLine("public void Exclui(int id)");
			retorno.AppendLine("{");
			retorno.AppendLine($"cmd.CommandText = \"delete {txtObjeto.Text} where {listaColunas[0]} = @id\";");
			retorno.AppendLine("cmd.Parameters.AddWithValue(\"@id\", id);");
			retorno.AppendLine("cmd.ExecuteNonQuery();");
			retorno.AppendLine("cmd.Parameters.Clear();");
			retorno.AppendLine("}");
			return retorno.ToString();
		}

		private string RepositorioDispose()
		{
			StringBuilder retorno = new StringBuilder();
			
			retorno.AppendLine(" public void Dispose() ");
			retorno.AppendLine(" { ");
			retorno.AppendLine(" Dispose(true); ");
			retorno.AppendLine(" GC.SuppressFinalize(this); ");
			retorno.AppendLine(" } ");
			retorno.AppendLine(" protected virtual void Dispose(bool disposing) ");
			retorno.AppendLine(" { ");
			retorno.AppendLine(" // Check to see if Dispose has already been called. ");
			retorno.AppendLine(" if (!this.disposed) ");
			retorno.AppendLine(" { ");
			retorno.AppendLine(" if (disposing) ");
			retorno.AppendLine(" { ");
			retorno.AppendLine(" // Dispose managed resources. ");
			retorno.AppendLine(" da.Dispose(); ");
			retorno.AppendLine(" } ");
			retorno.AppendLine(" disposed = true; ");
			retorno.AppendLine(" } ");
			retorno.AppendLine(" } ");
			return retorno.ToString();
		}

	}


}
