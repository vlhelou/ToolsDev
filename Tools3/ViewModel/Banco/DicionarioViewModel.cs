using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Tools3.Model.Banco.BuscaBanco;
using Tools3.Model.Banco.Dicionario;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Tools3.ViewModel.Banco
{
	public class DicionarioViewModel : INotifyPropertyChanged
	{

		private SqlConnection _CN;
		private SqlCommand cmd;
		private ObjetoBancoModel _Selecionado = new ObjetoBancoModel();
		private ObjetoBancoModel _Base = new ObjetoBancoModel();
		private DicionarioModel _DicionarioSelecionado;

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

		public ObjetoBancoModel Base
		{
			get { return _Base; }
			set { _Base = value; }
		}

		public ObjetoBancoModel Selecionado
		{
			get { return _Selecionado; }
			set
			{
				_Selecionado = value;
				OnPropertyChanged(nameof(Selecionado));
				PopulaDicionario();
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
					xtype in ('FN','FS', 'FT','P','PC','TF','U','V')
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
					cmd.CommandText = @"select 
						cl.name Coluna
						, tp.name Tipo
						, cl.length Tamanho
						, cl.colid
					from sys.syscolumns cl
						inner join sys.types tp on 
							cl.xtype = tp.user_type_id
					where cl.name<>'' and id=@id
					order by colid";
					cmd.Parameters.AddWithValue("@id", Tabela.Id);
					DataTable dtColuna = new DataTable();
					dtColuna.Load(cmd.ExecuteReader());
					cmd.Parameters.Clear();
					foreach (DataRow lnColuna in dtColuna.Rows)
					{
						ObjetoBancoModel Coluna = new ObjetoBancoModel()
						{
							Pai = Tabela,
							Tipo = "column",
							Id = (short)lnColuna["colid"],
							Nome = lnColuna["Coluna"].ToString()
						};
						Tabela.Filhos.Add(Coluna);
					}

					Schema.Filhos.Add(Tabela);
				}
				_Base.Filhos.Add(Schema);
			}
		}

		private void PopulaDicionario()
		{
			Dicionario.Clear();
			bool executa = false;
			switch (_Selecionado.Tipo.ToLower())
			{
				case "function":
				case "procedure":
				case "table":
				case "view":
					cmd.CommandText = "select name, value from sys.extended_properties where major_id=@id and minor_id=0 order by name";
					cmd.Parameters.AddWithValue("@id", _Selecionado.Id);
					executa = true;
					break;
				case "schema":
					break;
				case "column":
					cmd.CommandText = "select name, value from sys.extended_properties where major_id=@idPai and minor_id=@id order by name";
					cmd.Parameters.AddWithValue("@id", _Selecionado.Id);
					cmd.Parameters.AddWithValue("@idPai", _Selecionado.Pai.Id);
					executa = true;
					break;
			}
			if (executa)
			{
				DataTable dtDicionario = new DataTable();
				dtDicionario.Load(cmd.ExecuteReader());
				cmd.Parameters.Clear();
				foreach (DataRow ln in dtDicionario.Rows)
				{
					Dicionario.Add(new DicionarioModel()
					{
						Campo = ln["name"].ToString(),
						Valor = ln["value"].ToString()
					}
						);
				}
				OnPropertyChanged(nameof(Dicionario));

			}

		}

		public string Campo { get; set; }
		public string Valor { get; set; }

		public DicionarioModel DicionarioSelecionado {
			get { return _DicionarioSelecionado; }
			set
			{
				_DicionarioSelecionado = value;
				Campo = _DicionarioSelecionado.Campo;
				Valor = _DicionarioSelecionado.Valor;
				OnPropertyChanged(nameof(Campo));
				OnPropertyChanged(nameof(Valor));
			}
		}
		public ObservableCollection<DicionarioModel> Dicionario { get; set; } = new ObservableCollection<DicionarioModel>();

		public ICommand ApagaDicionarioClick
		{
			get { return new CommandHandler(() => ApagaDicionario()); }
		}

		public void ApagaDicionario()
		{
			cmd.Parameters.AddWithValue("@name", DicionarioSelecionado.Campo);
			switch (_Selecionado.Tipo.ToLower())
			{
				case "function":
				case "procedure":
				case "table":
					cmd.Parameters.AddWithValue("@level0type", "SCHEMA");
					cmd.Parameters.AddWithValue("@level0name", Selecionado.Pai.Nome);
					cmd.Parameters.AddWithValue("@level1type", Selecionado.Tipo.ToUpper());
					cmd.Parameters.AddWithValue("@level1name", Selecionado.Nome);

					break;
				case "column":
					cmd.Parameters.AddWithValue("@level0type", "SCHEMA");
					cmd.Parameters.AddWithValue("@level0name", Selecionado.Pai.Pai.Nome);
					cmd.Parameters.AddWithValue("@level1type", Selecionado.Pai.Tipo.ToUpper());
					cmd.Parameters.AddWithValue("@level1name", Selecionado.Pai.Nome);
					cmd.Parameters.AddWithValue("@level2type", "COLUMN");
					cmd.Parameters.AddWithValue("@level2name", Selecionado.Nome);

					break;
			}
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "sys.sp_dropextendedproperty";
			cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			cmd.CommandType = CommandType.Text;
			Campo = string.Empty;
			Valor = string.Empty;
			OnPropertyChanged(nameof(Campo));
			OnPropertyChanged(nameof(Valor));
			PopulaDicionario();

		}

		public ICommand GravaDicionarioClick
		{
			get { return new CommandHandler(() => GravaDicionario()); }
		}

		public void GravaDicionario()
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@name", Campo);
			cmd.Parameters.AddWithValue("@value", Valor);
			if (Dicionario.Where(p => p.Campo == Campo).Count() > 0)
			{
				cmd.CommandText = "sys.sp_updateextendedproperty";
			}
			else
			{
				cmd.CommandText = "sys.sp_addextendedproperty";
			}


			switch (_Selecionado.Tipo.ToLower())
			{
				case "function":
				case "procedure":
				case "table":
					cmd.Parameters.AddWithValue("@level0type", "SCHEMA");
					cmd.Parameters.AddWithValue("@level0name", Selecionado.Pai.Nome);
					cmd.Parameters.AddWithValue("@level1type", Selecionado.Tipo.ToUpper());
					cmd.Parameters.AddWithValue("@level1name", Selecionado.Nome);

					break;
				case "column":
					cmd.Parameters.AddWithValue("@level0type", "SCHEMA");
					cmd.Parameters.AddWithValue("@level0name", Selecionado.Pai.Pai.Nome);
					cmd.Parameters.AddWithValue("@level1type", Selecionado.Pai.Tipo.ToUpper());
					cmd.Parameters.AddWithValue("@level1name", Selecionado.Pai.Nome);
					cmd.Parameters.AddWithValue("@level2type", "COLUMN");
					cmd.Parameters.AddWithValue("@level2name", Selecionado.Nome);

					break;
			}


			cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();

			cmd.CommandType = CommandType.Text;
			Valor = string.Empty;
			PopulaDicionario();
			OnPropertyChanged(nameof(Valor));
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
