using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Tools3.ViewModel
{
	public class ucConexaoViewModel : INotifyPropertyChanged
	{
		public ucConexaoViewModel()
		{
			string origem = System.AppDomain.CurrentDomain.BaseDirectory + "conexoes.json";
			if (System.IO.File.Exists(origem))
			{
				JObject origemConexao = JObject.Parse(System.IO.File.ReadAllText(origem));
				List<Model.ucConexaoModel> prelista = new List<Model.ucConexaoModel>();
				foreach (var cn in origemConexao["Conexoes"])
				{
					prelista.Add(cn.ToObject<Model.ucConexaoModel>());
				}
				Conexoes.Clear();
				Conexoes = new ObservableCollection<Model.ucConexaoModel>(prelista);
				OnPropertyChanged(nameof(Conexoes));

			}
		}

		public ObservableCollection<Model.ucConexaoModel> Conexoes { get; set; } = new ObservableCollection<Model.ucConexaoModel>();

		public bool IsTrust
		{
			get
			{
				if (_Conexao != null)
					return _Conexao.Trust;
				return false;
			}
			set
			{
				if (_Conexao.Trust == value)
					return;
				_Conexao.Trust = value;
				OnPropertyChanged(nameof(IsTrust));
				OnPropertyChanged(nameof(Conexao));
			}
		}

		private Model.ucConexaoModel _Conexao=new Model.ucConexaoModel();
		public Model.ucConexaoModel Conexao
		{
			get { return _Conexao; }
			set
			{
				_Conexao = value;
				OnPropertyChanged(nameof(Conexao));
				OnPropertyChanged(nameof(IsTrust));
			}
		}

		public System.Data.SqlClient.SqlConnection Connecta()
		{
			System.Data.SqlClient.SqlConnectionStringBuilder str = new System.Data.SqlClient.SqlConnectionStringBuilder();
			str.InitialCatalog = _Conexao.Database;
			str.DataSource = _Conexao.Server;
			str.IntegratedSecurity = _Conexao.Trust;
			str.UserID = _Conexao.User;
			str.Password = _Conexao.Password;
			System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(str.ConnectionString);
			try
			{
				cn.Open();
				return cn;
			} catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return null;
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
