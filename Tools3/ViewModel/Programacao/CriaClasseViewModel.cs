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
		private List<stcColunas> Colunas = new List<stcColunas>();

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
			MontaColunas();

			GeraEntidade();
			GeraDao();
			GeraTypeScript();

		}


		private void GeraEntidade()
		{
			StringBuilder resultado = new StringBuilder();

			foreach (var coluna in Colunas)
			{
				string tipo = TipoSql2C(coluna.Tipo);
				if (coluna.Nulo && tipo != "string")
					tipo += "?";
				resultado.AppendFormat("public {0} {1}  {{ get;set; }} \n", tipo, coluna.Nome);
			}

			Entidade = resultado.ToString();
			OnPropertyChanged(nameof(Entidade));
		}

		private void GeraDao()
		{
			StringBuilder resultado = new StringBuilder();
			DAO = resultado.ToString();
			string nomeObjeto = Selecionado.Nome;
			List<string> listaColunasAspas = new List<string>();
			foreach (var coluna in Colunas)
				listaColunasAspas.Add("\"" + coluna.Nome + "\"");

			//        foreach (DataColumn coluna in tabela.Columns)
			//listaColunasAspas.Add("\"" + coluna.ColumnName + "\"");

			resultado.Append(GeraDAOConexao(nomeObjeto, listaColunasAspas));
			resultado.Append(GeraDAOAtribui(nomeObjeto));
			resultado.Append(GeraDAOBusca(nomeObjeto));
			resultado.Append(GeraDAOGrava(nomeObjeto));
			resultado.Append(GeraDAOExclui(nomeObjeto, Colunas[0].Nome));
			resultado.Append(GeraDAODaCommand());
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

		private string TipoSql2C(string tipo)
		{
			string retorno = "";
			switch (tipo.ToLower().Trim())
			{
				case "varchar":
				case "char":
				case "nvarchar":
				case "nchar":
				case "text":
				case "ntext":
				case "hierarchyid":
					retorno = "string";
					break;
				case "bit":
					retorno = "bool";
					break;
				case "int":
					retorno = "int";
					break;
				case "smallint":
					retorno = "short";
					break;
				case "tinyint":
					retorno = "byte";
					break;
				case "date":
				case "datetime":
					retorno = "DateTime";
					break;
			}

			return retorno;
		}

		private string TipoSql2TypeScript(string tipo)
		{

			Dictionary<string, string> TipoTypeScript = new Dictionary<string, string>
			 {
				{"varchar", "string" },
				{"char", "string" },
				{"nvarchar", "string" },
				{"nchar", "string" },
				{"text", "string" },
				{"ntext", "string" },
				{"hierarchyid", "string" },
				{"bit", "bool" },
				{"int", "number" },
				{"smallint", "number" },
				{"tinyint", "number" },
				{"date", "Date" },
				{"datetime", "Date" },
				{"uniqueidentifier","string" }
			};
			tipo = tipo.ToLower().Trim();
			if (!TipoTypeScript.ContainsKey(tipo.ToLower().Trim()))
				throw new Exception("tipo ainda não cadastrado");
			return TipoTypeScript[tipo];

		}

		private string TipoSqlType(string tipo)
		{
			Dictionary<string, string> TipoTypeScript = new Dictionary<string, string>
			 {
				{"varchar", "VarChar" },
				{"char", "Char" },
				{"nvarchar", "NVarChar" },
				{"nchar", "NChar" },
				{"text", "Text" },
				{"ntext", "NText" },
				{"hierarchyid", "string" },
				{"bit", "Bit" },
				{"int", "Int" },
				{"smallint", "SmallInt" },
				{"tinyint", "TinyInt" },
				{"date", "Date" },
				{"datetime", "DateTime" },
				{ "uniqueidentifier","UniqueIdentifier"}
			};
			//System.Data.SqlDbType.UniqueIdentifier
			tipo = tipo.ToLower().Trim();
			if (!TipoTypeScript.ContainsKey(tipo.ToLower().Trim()))
				throw new Exception("tipo ainda não cadastrado");
			return TipoTypeScript[tipo];

			//SqlDbType.DateTime
			//return "";
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

		private string GeraDAOAtribui(string objetoNome)
		{
			StringBuilder retorno = new StringBuilder();

			// Atribui

			retorno.AppendFormat("public static {0}Etd Atribui(object ln, string prefixo=\"\") \n", objetoNome);
			retorno.AppendLine("{ ");
			retorno.AppendLine("if (ln != null && ln is DataRow) ");
			retorno.AppendLine("{ ");
			retorno.AppendFormat("{0}Etd retorno = new {0}Etd(); \n", objetoNome);
			foreach (var coluna in Colunas)
			{
				string tipo = TipoSql2C(coluna.Tipo);
				if (tipo == "string")
				{
					retorno.AppendFormat("retorno.{0} = ln[prefixo+\"{0}\"].ToString(); \n", coluna.Nome);
				}
				else
				{
					if (coluna.Nulo)
						retorno.AppendFormat("retorno.{0} = ln.IsNull(prefixo+\"{0}\") ? null : ({1}?)ln[prefixo+\"{0}\"]; \n", coluna.Nome, tipo);
					else
						retorno.AppendFormat("retorno.{0} = ({1})ln[prefixo+\"{0}\"]; \n", coluna.Nome, tipo);
				}
			}
			/*
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
			}*/
			retorno.AppendLine("return retorno; ");
			retorno.AppendLine("} ");
			retorno.AppendLine("return null; ");
			retorno.AppendLine("} ");


			return retorno.ToString();

		}

		private string GeraDAOBusca(string objetoNome)
		{
			StringBuilder retorno = new StringBuilder();
			//Busca

			retorno.AppendFormat("public {0}Etd Busca(int id) \n", objetoNome);
			retorno.AppendLine("{ ");
			retorno.AppendFormat("cmd.CommandText = string.Format(\"select {{0}} from {0} where {1} = @id\", _Colunas); \n", objetoNome, Colunas[0].Nome);
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

		private string GeraDAOGrava(string objetoNome)
		{
			StringBuilder retorno = new StringBuilder();
			string coluna1 = Colunas[0].Nome;
			//Grava
			retorno.AppendFormat("public {0}Etd Grava({0}Etd Item) ", objetoNome);
			retorno.AppendLine("{ ");
			retorno.AppendLine("bool novo = false;");
			retorno.AppendFormat("cmd.CommandText = string.Format(\"select {{0}} from {0} where {1} = @id\", _Colunas); \n", Selecionado.NomeSql, coluna1);
			retorno.AppendFormat("cmd.Parameters.AddWithValue(\"@id\", Item.{0}); \n", coluna1);
			retorno.AppendLine("SqlDataAdapter daup = new SqlDataAdapter(cmd); ");
			retorno.AppendLine("ComandBuilder(daup); ");
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

			foreach (var coluna in Colunas)
			{
				if (!coluna.Calculada & !coluna.Identity)
				{
					string tipo = TipoSql2C(coluna.Tipo);
					if (coluna.Nulo)
					{
						if (tipo == "string")
						{
							retorno.AppendFormat("if (string.IsNullOrEmpty(Item.{0}))   \n", coluna.Nome);
							retorno.AppendFormat("ln[\"{0}\"] = DBNull.Value; \n", coluna.Nome);
							retorno.AppendLine("else");
							retorno.AppendFormat("ln[\"{0}\"] = Item.{0}.Trim(); \n", coluna.Nome);
						}
						else
						{
							retorno.AppendFormat("if (Item.{0} == null)   \n", coluna.Nome);
							retorno.AppendFormat("ln[\"{0}\"] = DBNull.Value; \n", coluna.Nome);
							retorno.AppendLine("else");
							retorno.AppendFormat("ln[\"{0}\"] = Item.{0}; \n", coluna.Nome);
						}

					}
					else
						retorno.AppendFormat("ln[\"{0}\"] = Item.{0}; \n", coluna.Nome);
				}
			}

			retorno.AppendLine("if (novo)");
			retorno.AppendLine("dtup.Rows.Add(ln); ");
			retorno.AppendLine("daup.Update(dtup); ");
			retorno.AppendLine("cmd.Parameters.Clear();");

			if (Colunas.Where(p => p.Identity == true).Count() > 0)
			{
				retorno.AppendLine("if (novo)");
				retorno.AppendLine("{");
				retorno.AppendLine("cmd.CommandText = \"select @@identity\";");
				retorno.AppendFormat("Item.{0} = Convert.ToInt32(cmd.ExecuteScalar());\n", coluna1);
				retorno.AppendLine("}");
			}


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

		private string GeraDAODaCommand()
		{
			StringBuilder retorno = new StringBuilder();
			retorno.AppendLine("public void ComandBuilder(SqlDataAdapter dap)");
			retorno.AppendLine("{");
			//SqlDataAdapter da = new SqlDataAdapter();


			retorno.AppendLine("SqlCommand cmdi = cmd.Connection.CreateCommand();");
			retorno.AppendLine("SqlCommand cmdu = cmd.Connection.CreateCommand();");
			string[] colunas = Colunas.Where(p => !p.Calculada & !p.Identity & !p.Default).Select(p => p.Nome).ToArray();
			string[] parametros = Colunas.Where(p => !p.Calculada & !p.Identity & !p.Default).Select(p => "@" + p.Nome).ToArray();
			retorno.AppendFormat("  cmdi.CommandText = \"insert into {0}({1}) values({2})\";\n", Selecionado.NomeSql, string.Join(",", colunas), string.Join(",", parametros));
			//SqlDataAdapter da = new SqlDataAdapter(cmd);
			//da.UpdateCommand
			foreach (var col in Colunas.Where(p => !p.Calculada & !p.Identity & !p.Default))
			{
				retorno.AppendFormat("cmdi.Parameters.Add(\"@{0}\", SqlDbType.{1},{2}, \"{0}\"); \n", col.Nome, TipoSqlType(col.Tipo), col.Comprimento);
			}

			retorno.AppendFormat("cmdu.CommandText = \"update {0} set ", Selecionado.NomeSql);
			List<string> Colunasupdate = new List<string>();
			foreach (var col in Colunas.Where(p => !p.Calculada & !p.Identity))
			{
				Colunasupdate.Add($"{col.Nome}=@{col.Nome}");
			}

			retorno.Append(string.Join(",", Colunasupdate));
			retorno.AppendFormat(" where {0}=@{0}\";\n", Colunas.Where(p => p.Identity).FirstOrDefault().Nome);


			foreach (var col in Colunas.Where(p => !p.Calculada))
			{
				retorno.AppendFormat("cmdu.Parameters.Add(\"@{0}\", SqlDbType.{1},{2}, \"{0}\"); \n", col.Nome, TipoSqlType(col.Tipo), col.Comprimento);
			}


			retorno.AppendLine("dap.InsertCommand=cmdi;");
			retorno.AppendLine("dap.UpdateCommand=cmdu;");
			retorno.AppendLine();
			retorno.AppendLine();
			retorno.AppendLine();






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

		private void GeraTypeScript()
		{
			StringBuilder retorno = new StringBuilder();
			retorno.AppendFormat("export class {0} {{ \n", Selecionado.NomeSql.Replace("[", "").Replace("]", "").Replace(".", ""));
			foreach (var coluna in Colunas)
			{
				retorno.AppendFormat("{0}:{1};\n", coluna.Nome, TipoSql2TypeScript(coluna.Tipo));
			}

			retorno.Append("} \n");
			TypeScript = retorno.ToString();
			OnPropertyChanged(nameof(TypeScript));
		}

		protected void OnPropertyChanged(string propertyname)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void MontaColunas()
		{
			cmd.CommandText = $@"SELECT 
	            c.name
	            , types.name Tipo
	            , column_id
	            , c.precision
	            , c.scale
	            , c.is_nullable
	            , c.is_identity
	            , is_computed 
				, c.default_object_id
            FROM sys.columns c
	            inner JOIN sys.objects o ON o.object_id = c.object_id
	            inner join sys.types on 
		            c.user_type_id = types.user_type_id
            WHERE o.object_id = {Selecionado.Id}
            ORDER BY c.column_id";
			DataTable dt = new DataTable();
			dt.Load(cmd.ExecuteReader());
			Colunas = new List<stcColunas>();
			foreach (DataRow ln in dt.Rows)
			{
				stcColunas coluna = new stcColunas()
				{
					Ordem = (int)ln["Column_id"],
					Nome = ln["Name"].ToString(),
					Tipo = ln["Tipo"].ToString(),
					Comprimento = (byte)ln["precision"],
					Precisao = (byte)ln["scale"],
					Nulo = (bool)ln["is_nullable"],
					Identity = (bool)ln["is_identity"],
					Calculada = (bool)ln["is_computed"],
					Default = Convert.ToInt32(ln["default_object_id"])>0
				};
				Colunas.Add(coluna);
			}


		}

		private struct stcColunas
		{

			public int Ordem;
			public string Nome;
			public string Tipo;
			public byte Comprimento;
			public byte Precisao;
			public bool Nulo;
			public bool Identity;
			public bool Calculada;
			public bool Default;

		}
	}
}
