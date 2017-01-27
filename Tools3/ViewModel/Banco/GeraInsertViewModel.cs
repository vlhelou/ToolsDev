using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Input;

namespace Tools3.ViewModel.Banco
{
	public class GeraInsertViewModel : INotifyPropertyChanged
	{
		private SqlConnection _CN;
		private SqlCommand cmd;
		public SqlConnection CN
		{
			get { return _CN; }
			set
			{
				_CN = value;
				if (_CN.State == ConnectionState.Open)
					cmd = _CN.CreateCommand();
			}
		}
		public ICommand ExecutaClick { get { return new CommandHandler(() => Executa()); } }
		private void Executa()
		{
			StringBuilder saida = new StringBuilder();
			cmd.CommandText = Sql;
			Resultado = new DataTable();
			Resultado.Load(cmd.ExecuteReader());
			OnPropertyChanged(nameof(Resultado));
			if (!string.IsNullOrEmpty(TabelaSaida))
			{
				List<string> Colunas = new List<string>();
				foreach (DataColumn coluna in Resultado.Columns)
					Colunas.Add(coluna.ColumnName);
				foreach(DataRow ln in Resultado.Rows)
				{
					saida.AppendFormat("insert into {0} ({1}) values ({2}); \n", 
						TabelaSaida, 
						string.Join(",",Colunas),
						MontaValues(Colunas,ln));
				}
				System.Windows.Clipboard.SetText(saida.ToString());
			}
		}

		private string MontaValues(List<string> Colunas, DataRow Linha)
		{
			List<String> vlrColuna = new List<string>();
			foreach (string ColunaNome in Colunas)
			{
				DataColumn coluna = Linha.Table.Columns[ColunaNome];
				if (Linha.IsNull(ColunaNome))
					vlrColuna.Add("NULL");
				else
				{
					switch (coluna.DataType.ToString().ToLower())
					{
						case "system.decimal":
							vlrColuna.Add(Linha[coluna].ToString().Replace(",", "."));
							break;
						case "system.string":
							vlrColuna.Add('\'' + Linha[coluna].ToString().Replace("'", "''") + '\'');
							break;
						case "system.int":
						case "system.int16":
						case "system.int32":
						case "system.int64":
							vlrColuna.Add(Linha[coluna].ToString());
							break;
						case "system.datetime":
							vlrColuna.Add('\'' + Convert.ToDateTime(Linha[coluna]).ToString("yyyy-MM-dd HH:mm:ss") + '\'');
							break;
						case "system.boolean":
							vlrColuna.Add((bool)Linha[coluna] ? "1" : "0");
							break;
						default:
							vlrColuna.Add(Linha[coluna].ToString());
							break;

					}
				}
			}
			return string.Join("", string.Join(",", vlrColuna));
		}
		public string Sql { get; set; }
		public DataTable Resultado { get; set; }
		public string TabelaSaida { get; set; }


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
