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
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Tools3.ViewModel.Banco
{
	public class BuscaBancoViewModel : INotifyPropertyChanged
	{
		private SqlConnection _cn;
		private SqlCommand cmd;

		public SqlConnection CN
		{
			get { return _cn; }
			set
			{
				_cn = value;
				if (_cn.State == ConnectionState.Open)
				{
					cmd = _cn.CreateCommand();
					PopulaBase();
				}
				else
				{
					_Base = new ObjetoBancoModel();
				}

				OnPropertyChanged(nameof(Base));
			}
		}

		public string Alvo { get; set; }

		public int TabelasTotal { get; set; } = 0;
		public int TabelasFeitas { get; set; } = 0;

		private ObjetoBancoModel _Base = new ObjetoBancoModel();
		public ObjetoBancoModel Base
		{
			get { return _Base; }
			set { _Base = value; }
		}

		public ObservableCollection<ItensLocalizadosModel> Localizados { get; set; } = new ObservableCollection<ItensLocalizadosModel>();
		
		public ProgressBar BarraExecucao { get; set; }

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
			cmd.CommandText = @"select 
					distinct sc.name
				from sys.tables tb
					inner join sys.columns cl on 
						tb.object_id = cl.object_id
					inner join sys.types tp on 
						cl.system_type_id = tp.system_type_id
					inner join sys.schemas sc on 
						tb.schema_id = sc.schema_id
				where tp.name in  ('text', 'ntext', 'varchar', 'char', 'nvarchar', 'nchar')
				order by 
					sc.name";
			DataTable dtSC = new DataTable();
			dtSC.Load(cmd.ExecuteReader());
			foreach (DataRow LnSchema in dtSC.Rows)
			{
				ObjetoBancoModel Schema = new ObjetoBancoModel()
				{
					Pai = _Base,
					Tipo = "Schema",
					Nome = LnSchema["Name"].ToString()
				};
				cmd.CommandText = @"select 
						distinct tb.name, tb.Object_id
					from sys.tables tb
						inner join sys.columns cl on 
							tb.object_id = cl.object_id
						inner join sys.types tp on 
							cl.system_type_id = tp.system_type_id
						inner join sys.schemas sc on 
							tb.schema_id = sc.schema_id
					where 
						tp.name in  ('text', 'ntext', 'varchar', 'char', 'nvarchar', 'nchar')
						and sc.name=@sc
					order by 
						tb.name";
				cmd.Parameters.AddWithValue("@sc", Schema.Nome);
				DataTable dtTabela = new DataTable();
				dtTabela.Load(cmd.ExecuteReader());
				cmd.Parameters.Clear();
				foreach (DataRow lnTabela in dtTabela.Rows)
				{
					ObjetoBancoModel Tabela = new ObjetoBancoModel()
					{
						Pai = Schema,
						Tipo = "Tabela",
						Nome = lnTabela["Name"].ToString(),
						Id = (int)lnTabela["Object_id"]
					};
					cmd.CommandText = @"select 
							cl.name Coluna, tp.name Tipo, cl.max_length Tamanho
						from sys.columns cl
							inner join sys.types tp on 
								cl.system_type_id = tp.system_type_id
						where 
							tp.name in  ('text', 'ntext', 'varchar', 'char', 'nvarchar', 'nchar')
							and cl.object_id=@id
						order by 
							cl.column_id";
					cmd.Parameters.AddWithValue("@id", Tabela.Id);
					DataTable dtColuna = new DataTable();
					dtColuna.Load(cmd.ExecuteReader());
					cmd.Parameters.Clear();
					foreach (DataRow lnColuna in dtColuna.Rows)
					{
						ObjetoBancoModel Coluna = new ObjetoBancoModel()
						{
							Pai = Tabela,
							Tipo = "Coluna",
							Nome = lnColuna["Coluna"].ToString()
						};
						Tabela.Filhos.Add(Coluna);
					}

					Schema.Filhos.Add(Tabela);
				}
				_Base.Filhos.Add(Schema);
			}
		}

		public ICommand PesquisaClick
		{
			get { return new CommandHandler(() => Pesquisa()); }
		}

		private void Pesquisa()
		{
			TabelasFeitas = 0;
			TabelasTotal = 0;
			Localizados.Clear();
			foreach (var sch in _Base.Filhos.Where(p => p.Selecionado))
			{
				TabelasTotal += sch.Filhos.Where(p => p.Selecionado).Count();
			}
			OnPropertyChanged(nameof(TabelasFeitas));
			OnPropertyChanged(nameof(TabelasTotal));

			cmd.Parameters.AddWithValue("@chave", Alvo);

			foreach (var sch in _Base.Filhos.Where(p => p.Selecionado))
			{
				foreach (var tbl in sch.Filhos.Where(p => p.Selecionado))
				{
					if (tbl.Filhos.Count() > 0)
					{
						BuscaNaTabela(tbl);
					}
					
					TabelasFeitas++;
					BarraExecucao.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
					OnPropertyChanged(nameof(TabelasFeitas));
				}
			}
			cmd.Parameters.Clear();
			OnPropertyChanged(nameof(Localizados));

		}

		private void BuscaNaTabela(ObjetoBancoModel tabela)
		{
			StringBuilder sql = new StringBuilder();
			sql.AppendFormat("select count(1) from {0} where 1=2 ", tabela.NomeSql);
			foreach(var coluna in tabela.Filhos)
			{
				sql.AppendFormat(" or [{0}] like @chave ", coluna.Nome);
			}
			cmd.CommandText = sql.ToString();
			int quantidade = (int)cmd.ExecuteScalar();
			if (quantidade > 0)
			{
				ItensLocalizadosModel achado = new ItensLocalizadosModel()
				{
					Objeto = tabela,
					Registros = quantidade,
					Select=sql.ToString()
				};
				Localizados.Add(achado);
			}
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
