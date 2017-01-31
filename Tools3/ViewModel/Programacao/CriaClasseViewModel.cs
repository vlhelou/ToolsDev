using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Tools3.Model.Banco.BuscaBanco;
using System.Windows.Input;

namespace Tools3.ViewModel.Programacao
{
	public class CriaClasseViewModel : INotifyPropertyChanged
	{
		private SqlConnection _CN;
		private SqlCommand cmd;
		private ObjetoBancoModel _Base = new ObjetoBancoModel();
		public ObjetoBancoModel Base
		{
			get { return _Base; }
			set { _Base = value; }
		}


		public SqlConnection CN
		{
			get { return _CN; }
			set
			{
				_CN = value;
				if (_CN.State == ConnectionState.Open)
				{
					cmd = _CN.CreateCommand();
					PopulaBase();
				}
				else
				{
					_Base = new ObjetoBancoModel();
				}

				OnPropertyChanged(nameof(Base));
			}
		}

		private ObjetoBancoModel _Selecionado = new ObjetoBancoModel();

		public ObjetoBancoModel Selecionado
		{
			get { return _Selecionado; }
			set
			{
				_Selecionado = value;
				OnPropertyChanged(nameof(Selecionado));
			}
		}

		private void PopulaBase()
		{
			cmd.CommandText = "select DB_NAME();";
			string nome = cmd.ExecuteScalar().ToString();
			_Base = new ObjetoBancoModel()
			{
				Nivel = 1,
				Tipo = "Base",
				Nome = nome,
				Pai = null

			};
			cmd.CommandText = @"select name, schema_id from sys.schemas where principal_id=1";
			DataTable dtSC = new DataTable();
			dtSC.Load(cmd.ExecuteReader());
			foreach (DataRow LnSchema in dtSC.Rows)
			{
				ObjetoBancoModel Schema = new ObjetoBancoModel()
				{
					Pai = _Base,
					Tipo = "Schema",
					Nome = LnSchema["Name"].ToString(),
					Id = (int)LnSchema["schema_id"]
				};
				cmd.CommandText = @"select 
					name
					, xtype
					, case xtype 
						when 'FN' then 'function'
						when 'FS' then 'function'
						when 'FT' then 'function'
						when 'P' then 'procedure'
						when 'PC' then 'procedure'
						when 'TF' then 'function'
						when 'U' then 'table'
						when 'V' then 'view'
						end as Tipo
					, id
				from sys.sysobjects 
				where 
					xtype in ('U')
					and OBJECTproperty(id,'IsMSShipped')=0
					and uid=@sc
				order by tipo, name";
				cmd.Parameters.AddWithValue("@sc", Schema.Id);
				DataTable dtTabela = new DataTable();
				dtTabela.Load(cmd.ExecuteReader());
				cmd.Parameters.Clear();
				foreach (DataRow lnTabela in dtTabela.Rows)
				{
					ObjetoBancoModel Tabela = new ObjetoBancoModel()
					{
						Pai = Schema,
						Tipo = lnTabela["Tipo"].ToString(),
						Nome = lnTabela["Name"].ToString(),
						Id = (int)lnTabela["id"]
					};

					Schema.Filhos.Add(Tabela);
				}
				_Base.Filhos.Add(Schema);
			}
		}

		public string Entidade { get; set; }
		public string DAO { get; set; }
		public string TypeScript { get; set; }

		public ICommand GeraClick
		{
			get { return new CommandHandler(() => Gera()); }
		}

		public void Gera()
		{
			if (Selecionado.Tipo != "table")
				return;

			cmd.CommandText = "select top 0 * from " + Selecionado.NomeSql;
			DataTable tabela = new DataTable();
			tabela.Load(cmd.ExecuteReader());
			GeraEntidade(tabela);
			GeraDao(tabela);
		}


		private void GeraEntidade(DataTable tabela)
		{
			StringBuilder resultado = new StringBuilder();

			foreach (DataColumn coluna in tabela.Columns)
			{
				string tipo = NomeTipo(coluna);
				
				if (coluna.AllowDBNull & tipo!="string")
					tipo += "?";
				resultado.AppendFormat("public {0} {1}  {{ get;set; }} \n", tipo, coluna.ColumnName);
			}
			Entidade = resultado.ToString();
			OnPropertyChanged(nameof(Entidade));
		}

		private void GeraDao(DataTable tabela)
		{
			StringBuilder resultado = new StringBuilder();
			DAO = resultado.ToString();
			string nomeObjeto = Selecionado.Nome;
			List<string> listaColunasAspas = new List<string>();
			foreach (DataColumn coluna in tabela.Columns)
				listaColunasAspas.Add("\"" + coluna.ColumnName + "\"");

			resultado.Append(GeraDAOConexao(nomeObjeto,listaColunasAspas));
			resultado.Append(GeraDAOAtribui(nomeObjeto, tabela));
			resultado.Append(GeraDAOBusca(nomeObjeto, tabela));
			resultado.Append(GeraDAOGrava(nomeObjeto, tabela));
			resultado.Append(GeraDAOExclui(nomeObjeto, tabela.Columns[0].ColumnName));
			resultado.Append(GeraDAODispose());
			DAO = resultado.ToString();
			OnPropertyChanged(nameof(DAO));
		}
		private string NomeTipo(DataColumn coluna)
		{
			string[] tipos = coluna.DataType.ToString().Split('.');
			string tipo = tipos[tipos.Length - 1];

			switch (tipo)
			{
				case "Int32":
					tipo = "int";
					break;
				case "SqlHierarchyId":
				case "String":
					tipo = "string";
					break;

			}

			return tipo;

		}

		private string GeraDAOConexao(string objetoNome, List<string> listaColunasAspas)
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

		private string GeraDAOAtribui(string objetoNome, DataTable tabela)
		{
			StringBuilder retorno = new StringBuilder();

			// Atribui
			retorno.AppendFormat("public static {0}Etd Atribui(DataRow ln, string prefixo=\"\") \n", objetoNome);
			retorno.AppendLine("{ ");
			retorno.AppendLine("if (ln != null) ");
			retorno.AppendLine("{ ");
			retorno.AppendFormat("{0}Etd retorno = new {0}Etd(); \n", objetoNome);
			foreach (DataColumn col in tabela.Columns)
			{
				string coluna = col.ColumnName;
				//string atributo = ln["NomeObjeto"].ToString();
				string tipo = NomeTipo(col);
				bool nulo = col.AllowDBNull;
				if (tipo == "string")
				{
					retorno.AppendFormat("retorno.{0} = ln[prefixo+\"{0}\"].ToString(); \n", coluna);
				}
				else
				{
					if (nulo)
					{
						retorno.AppendFormat("retorno.{0} = ln.IsNull(prefixo+\"{0}\") ? null : ({1}?)ln[prefixo+\"{0}\"]; \n",  coluna, tipo);
					}
					else
						retorno.AppendFormat("retorno.{0} = ({1})ln[prefixo+\"{0}\"]; \n", coluna, tipo);

				}
			}
			retorno.AppendLine("return retorno; ");
			retorno.AppendLine("} ");
			retorno.AppendLine("return null; ");
			retorno.AppendLine("} ");


			return retorno.ToString();

		}

		private string GeraDAOBusca(string objetoNome, DataTable tabela)
		{
			StringBuilder retorno = new StringBuilder();
			//Busca

			retorno.AppendFormat("public {0}Etd Busca(int id) \n", objetoNome);
			retorno.AppendLine("{ ");
			retorno.AppendFormat("cmd.CommandText = string.Format(\"select {{0}} from {0} where {1} = @id\", _Colunas); \n", objetoNome, tabela.Columns[0].ColumnName.ToString());
			retorno.AppendLine("cmd.Parameters.AddWithValue(\"@id\", id); ");
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

		private string GeraDAOGrava(string objetoNome, DataTable tabela)
		{
			StringBuilder retorno = new StringBuilder();
			string coluna1 = tabela.Columns[0].ColumnName;
			//Grava
			retorno.AppendFormat("public {0}Etd Grava({0}Etd Item) ", objetoNome);
			retorno.AppendLine("{ ");
			retorno.AppendLine("bool novo = false;");
			retorno.AppendFormat("cmd.CommandText = string.Format(\"select {{0}} from {0} where {1} = @id\", _Colunas); \n", tabela.TableName, coluna1);
			retorno.AppendFormat("cmd.Parameters.AddWithValue(\"@id\", Item.{0}); \n", coluna1);
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

			foreach (DataColumn col in tabela.Columns)
			{
				string coluna = col.ColumnName;
				string tipo = NomeTipo(col);
				bool nulo = col.AllowDBNull;
				if (nulo)
				{
					if (tipo == "string")
					{
						retorno.AppendFormat("if (string.IsNullOrEmpty(Item.{0}))   \n", coluna);
						retorno.AppendFormat("ln[\"{0}\"] = DBNull.Value; \n", coluna);
						retorno.AppendLine("else");
						retorno.AppendFormat("ln[\"{0}\"] = Item.{0}.Trim(); \n", coluna);
					}
					else
					{
						retorno.AppendFormat("if (Item.{0} == null)   \n", coluna);
						retorno.AppendFormat("ln[\"{0}\"] = DBNull.Value; \n", coluna);
						retorno.AppendLine("else");
						retorno.AppendFormat("ln[\"{0}\"] = Item.{0}; \n", coluna);
					}

				}
				else
					retorno.AppendFormat("ln[\"{0}\"] = Item.{0}; \n", coluna);
			}

			retorno.AppendLine("if (novo)");
			retorno.AppendLine("dtup.Rows.Add(ln); ");
			retorno.AppendLine("daup.Update(dtup); ");
			retorno.AppendLine("cmd.Parameters.Clear();");
			retorno.AppendLine("if (novo)");
			retorno.AppendLine("{");
			retorno.AppendLine("cmd.CommandText = \"select @@identity\";");
			retorno.AppendFormat("Item.{0} = Convert.ToInt32(cmd.ExecuteScalar());\n", coluna1);
			retorno.AppendLine("}");

			retorno.AppendFormat("return Busca(Item.{0}); ", coluna1);
			retorno.AppendLine("} ");


			return retorno.ToString();
		}

		private string GeraDAOExclui(string objetoNome, string Coluna1)
		{
			StringBuilder retorno = new StringBuilder();
			retorno.AppendLine("public void Exclui(int id)");
			retorno.AppendLine("{");
			retorno.AppendLine($"cmd.CommandText = \"delete {objetoNome} where {Coluna1} = @id\";");
			retorno.AppendLine("cmd.Parameters.AddWithValue(\"@id\", id);");
			retorno.AppendLine("cmd.ExecuteNonQuery();");
			retorno.AppendLine("cmd.Parameters.Clear();");
			retorno.AppendLine("}");
			return retorno.ToString();
		}


		private string GeraDAODispose()
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

		protected void OnPropertyChanged(string propertyname)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;

	}
}
