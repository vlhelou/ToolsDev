using System;
using System.Windows;
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;


namespace ToolsDev
{
	/// <summary>
	/// Interaction logic for BuscaBanco.xaml
	/// </summary>
	public partial class BuscaBanco : Window
	{
		ObservableCollection<ObjetoBanco> OC { get; set; }
		ObservableCollection<ObjetoBanco> ChildOC { get; set; }
		public BuscaBanco()
		{
			InitializeComponent();
		}

		private void ConexaoAberta(object sender, EventArgs args)
		{
			cbBase.IsEnabled = true;
			chMarcado.IsEnabled = true;
			cbBase.ItemsSource = Conexao.ListaBases();
		}


		private void cbBase_SelectionChanged(object sender, EventArgs args)
		{
			//MessageBox.Show(cbBase.SelectedValue.ToString());
			Conexao.cn.ChangeDatabase(cbBase.SelectedValue.ToString());
			PopulaTree(null, null);
		}


		private void PopulaTree(object sender, EventArgs args)
		{

			OC = new ObservableCollection<ObjetoBanco>();
			SqlCommand cmd = Conexao.cn.CreateCommand();
			ObjetoBanco itbase = new ObjetoBanco() { Selecionado = (bool)chMarcado.IsChecked, Nome = cbBase.SelectedValue.ToString(), id=1 };
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
			foreach (DataRow lnSC in dtSC.Rows)
			{
				ObjetoBanco schema = new ObjetoBanco() { Nome = lnSC["name"].ToString(), Selecionado = (bool)chMarcado.IsChecked };
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
				cmd.Parameters.AddWithValue("@sc", lnSC["name"].ToString());
				DataTable dtTabela = new DataTable();
				dtTabela.Load(cmd.ExecuteReader());
				cmd.Parameters.Clear();
				foreach (DataRow lnTabela in dtTabela.Rows)
				{
					ObjetoBanco tabela = new ObjetoBanco() { Nome = lnTabela["name"].ToString(), Selecionado = (bool)chMarcado.IsChecked, id = (int)lnTabela["Object_id"] };
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
					cmd.Parameters.AddWithValue("@id", (int)lnTabela["Object_id"]);
					DataTable dtColuna = new DataTable();
					dtColuna.Load(cmd.ExecuteReader());
					cmd.Parameters.Clear();
					foreach (DataRow lnColuna in dtColuna.Rows)
					{
						ObjetoBanco coluna = new ObjetoBanco() {
							Nome = lnColuna["Coluna"].ToString(), 
							Selecionado = (bool)chMarcado.IsChecked };
						tabela.Filhos.Add(coluna);
					}
					schema.Filhos.Add(tabela);
				}
				itbase.Filhos.Add(schema);
			}
			OC.Add(itbase);
			this.DataContext = OC;
			
		}

		private void Pesquisa_Click(object sender, RoutedEventArgs e)
		{
			
			IList<ObjetoBanco> listaAcao = new List<ObjetoBanco>();
			foreach (ObjetoBanco schema in OC[0].Filhos)
			{
				if (schema.Selecionado)
				{
					foreach (ObjetoBanco tabela in schema.Filhos)
					{
						System.Text.StringBuilder Sql = new System.Text.StringBuilder();
						Sql.AppendFormat("select count(1) from [{0}].[{1}] where 1=2 ",schema.Nome,tabela.Nome);
						bool temColuna = false;
						if (tabela.Selecionado)
						{
							foreach (ObjetoBanco coluna in tabela.Filhos)
							{
								if (coluna.Selecionado)
								{
									temColuna = true;
									Sql.AppendFormat(" or [{0}] like @chave ", coluna.Nome);
								}
							}
							if (temColuna)
							{
								tabela.Sql = Sql.ToString();
								listaAcao.Add(tabela);
							}
								

						}
					}
				}
			}

			if (listaAcao.Count > 0)
			{
				pg1.Minimum = 0;
				pg1.Maximum = listaAcao.Count - 1;
				SqlCommand cmd = Conexao.cn.CreateCommand();
				cmd.Parameters.AddWithValue("@chave", txtPesquisa.Text);
				for(int i=0;i<listaAcao.Count;i++)
				{
					cmd.CommandText = listaAcao[i].Sql;
					listaAcao[i].qtRegistros = (int)cmd.ExecuteScalar();
					pg1.Value = i;
				}
			}
			//var resultado = sele
			var consulta = from lista in listaAcao
						   where lista.qtRegistros > 0
						   select new { id=lista.id, Nome=lista.Nome, Qt = lista.qtRegistros, Select = lista.Sql };
			GridResultado.ItemsSource = consulta.ToList();
			//MessageBox.Show(listaAcao.Count.ToString());
		}
	}

	public class ObjetoBanco
	{
		public ObjetoBanco() {
			Filhos = new ObservableCollection<ObjetoBanco>();
		}
		public int? id { get; set; }
		public string Nome { get; set; }
		public bool Selecionado { get; set; }
		public string Sql { get; set; }
		public int qtRegistros { get; set; }
		public ObservableCollection<ObjetoBanco> Filhos { get; set; }
	}
}
