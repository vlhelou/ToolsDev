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
	/// Interaction logic for Dicionario.xaml
	/// </summary>
	public partial class Dicionario : Window
	{
		IList<DicObjetoBanco> objetos = new List<DicObjetoBanco>();
		public string Origem { get; set; }
		public Dicionario()
		{
			
			InitializeComponent();
		}

		private void ConexaoAberta(object sender, EventArgs args)
		{
			cbBase.IsEnabled = true;
			cbBase.ItemsSource = Conexao.ListaBases();

		}

		private void cbBase_SelectionChanged(object sender, EventArgs args)
		{
			objetos = new List<DicObjetoBanco>();
			Conexao.cn.ChangeDatabase(cbBase.SelectedValue.ToString());
			SqlCommand cmd = Conexao.cn.CreateCommand();
			cmd.CommandText = @"select 
						OBJECT_SCHEMA_NAME(id) [schema]
					, name
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
				order by [schema] , tipo, name";
			DataTable dt = new DataTable();
			dt.Load(cmd.ExecuteReader());
			foreach(DataRow ln in dt.Rows)
			{
				objetos.Add(new DicObjetoBanco()
				{
					schema = ln["schema"].ToString(),
					Nome = ln["name"].ToString(),
					tipo = ln["Tipo"].ToString(),
					id = (int)ln["id"]
				});
			}
			grObjetos.ItemsSource = objetos;
		}

		private void AplicaFiltro(object sender, EventArgs args)
		{ 
			TextBox obj = (TextBox)sender;
			if (string.IsNullOrEmpty(obj.Text))
				grObjetos.ItemsSource = objetos;
			else
				grObjetos.ItemsSource = objetos.Where(p=>p.Nome.ToUpper().StartsWith(obj.Text.ToUpper()));
		}

		private void grObjetosSelect(object sender, EventArgs args)
		{
			this.Origem = "Objeto";
			DataGrid obj = grObjetos;
			List<dicColuna> colunas = new List<dicColuna>();
			if (obj.SelectedItem!=null)
			{
				DicObjetoBanco linha = (DicObjetoBanco)obj.SelectedItem;
				listaDicionario(linha);
				//MessageBox.Show(linha.id.ToString());
				SqlCommand cmd = Conexao.cn.CreateCommand();
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
				cmd.Parameters.AddWithValue("@id", linha.id);
				DataTable dt = new DataTable();
				dt.Load(cmd.ExecuteReader());
				cmd.Parameters.Clear();
				foreach (DataRow ln in dt.Rows)
					colunas.Add(new dicColuna() {
						Nome=ln["Coluna"].ToString(),
						Tipo=ln["Tipo"].ToString(),
						Tamanho = (Int16) ln["Tamanho"],
						id = (Int16)ln["colid"]

					});
				grColunas.ItemsSource = colunas;
			}
		}

		private void grColunaSelect(object sender, EventArgs args)
		{
			this.Origem = "Coluna";
			DataGrid obj = grColunas;
			if (obj.SelectedItem != null)
				listaDicionario((dicColuna)obj.SelectedItem);
		}
	
		private void listaDicionario(DicObjetoBanco objBanco)
		{
			SqlCommand cmd = Conexao.cn.CreateCommand();
			cmd.CommandText = "SELECT objtype, objname, name, value FROM fn_listextendedproperty (NULL, 'schema', @schema, @tipo, @objeto, null, null);";
			cmd.Parameters.AddWithValue("@schema", objBanco.schema);
			cmd.Parameters.AddWithValue("@tipo", objBanco.tipo);
			cmd.Parameters.AddWithValue("@objeto", objBanco.Nome);
			DataTable dt = new DataTable();
			dt.Load(cmd.ExecuteReader());
			cmd.Parameters.Clear();
			dt.Columns.Add("Script");
			foreach (DataRow ln in dt.Rows)
				ln["script"] = Script(objBanco, ln);
			grDicionario.ItemsSource = dt.DefaultView;
		}

		private void listaDicionario(dicColuna coluna)
		{
			DicObjetoBanco linha = (DicObjetoBanco)grObjetos.SelectedItem;


			SqlCommand cmd = Conexao.cn.CreateCommand();
			cmd.CommandText = "SELECT objtype, objname, name, value FROM fn_listextendedproperty (NULL, 'schema', @schema, 'table', @objeto, 'Column', @coluna);";
			cmd.Parameters.AddWithValue("@schema", linha.schema);
			cmd.Parameters.AddWithValue("@objeto", linha.Nome);
			cmd.Parameters.AddWithValue("@coluna", coluna.Nome);
			DataTable dt = new DataTable();
			dt.Load(cmd.ExecuteReader());
			cmd.Parameters.Clear();
			dt.Columns.Add("script");
			foreach (DataRow ln in dt.Rows)
				ln["script"] = Script(linha,coluna,ln);
			grDicionario.ItemsSource = dt.DefaultView;


		}

		private void GravaProprietade(object sender, EventArgs args)
		{
			try
			{
				SqlCommand cmd = Conexao.cn.CreateCommand();
				DicObjetoBanco obj = (DicObjetoBanco)grObjetos.SelectedItem;
				cmd.Parameters.AddWithValue("@nome", this.xpNome.Text);
				cmd.Parameters.AddWithValue("@valor", this.xpValor.Text);
				cmd.Parameters.AddWithValue("@schema", obj.schema);
				cmd.Parameters.AddWithValue("@tipo", obj.tipo);
				cmd.Parameters.AddWithValue("@nomeObjeto", obj.Nome);
				if (this.Origem == "Objeto")
				{
					cmd.CommandText = @"
					EXEC sp_addextendedproperty 
					@name = @nome, 
					@value = @valor,
					@level0type = N'Schema', 
					@level0name = @schema,
					@level1type = @tipo,  
					@level1name = @nomeObjeto";
				}
				else
				{
					dicColuna coluna = (dicColuna)grColunas.SelectedItem;
					cmd.Parameters.AddWithValue("@coluna", coluna.Nome);
					cmd.CommandText = @"
					EXEC sp_addextendedproperty 
					@name = @nome, 
					@value = @valor,
					@level0type = N'Schema', 
					@level0name = @schema,
					@level1type = @tipo,  
					@level1name = @nomeObjeto,
					@level2type = N'Column',
					@level2name = @coluna";
				}
				cmd.ExecuteNonQuery();
				if (this.Origem == "Objeto")
					grObjetosSelect(null, null);
				else
					grColunaSelect(null, null);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void ApagaPropriedade(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{

				
				try
				{
					SqlCommand cmd = Conexao.cn.CreateCommand();
					DicObjetoBanco obj = (DicObjetoBanco)grObjetos.SelectedItem;
					DataRowView prop = (DataRowView)grDicionario.SelectedItem;
					cmd.Parameters.AddWithValue("@nome", prop.Row["name"].ToString());
					cmd.Parameters.AddWithValue("@schema", obj.schema);
					cmd.Parameters.AddWithValue("@tipo", obj.tipo);
					cmd.Parameters.AddWithValue("@nomeObjeto", obj.Nome);
					if (this.Origem == "Objeto")
					{
						cmd.CommandText = @"
					EXEC sp_dropextendedproperty 
					@name = @nome, 
					@level0type = N'Schema', 
					@level0name = @schema,
					@level1type = @tipo,  
					@level1name = @nomeObjeto";
					}
					else
					{
						dicColuna coluna = (dicColuna)grColunas.SelectedItem;
						cmd.Parameters.AddWithValue("@coluna", coluna.Nome);
						cmd.CommandText = @"
					EXEC sp_dropextendedproperty 
					@name = @nome, 
					@level0type = N'Schema', 
					@level0name = @schema,
					@level1type = @tipo,  
					@level1name = @nomeObjeto,
					@level2type = N'Column',
					@level2name = @coluna";
					}
					cmd.ExecuteNonQuery();
					if (this.Origem == "Objeto")
						grObjetosSelect(null, null);
					else
						grColunaSelect(null, null);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			
			
			}
		}

		private string Script(DicObjetoBanco objBanco, DataRow ln )
		{
			string retorno=@"EXEC sp_addextendedproperty @name = '{0}', @value = '{1}',	@level0type = N'Schema', @level0name = '{2}', @level1type = '{3}',  @level1name = '{4}'";
			return string.Format(retorno, ln["name"].ToString(), ln["value"].ToString(), objBanco.schema,objBanco.tipo, objBanco.Nome);
		}

		private string Script(DicObjetoBanco objBanco, dicColuna coluna, DataRow ln)
		{
			string retorno = @"EXEC sp_addextendedproperty @name = '{0}', @value = '{1}',	@level0type = N'Schema', @level0name = '{2}', @level1type = '{3}',  @level1name = '{4}', @level2type = N'Column', @level2name = '{5}'";
			return string.Format(retorno, ln["name"].ToString(), ln["value"].ToString(), objBanco.schema, objBanco.tipo, objBanco.Nome, coluna.Nome);
		}

	}

	


	public class DicObjetoBanco
	{
		public string schema { get; set; }
		public string Nome { get; set; }
		public string tipo { get; set; }
		public int id { get; set; }
	}

	public class dicColuna
	{
		public string Nome { get; set; }
		public string Tipo { get; set; }
		public Int16 Tamanho { get; set; }
		public Int16 id { get; set; }
	}
}
