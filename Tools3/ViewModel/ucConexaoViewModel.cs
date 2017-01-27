using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Data;
using System.Windows;

namespace Tools3.ViewModel
{
	public class ucConexaoViewModel : INotifyPropertyChanged
	{
		private System.Data.SqlClient.SqlConnection _cn= new System.Data.SqlClient.SqlConnection();
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
			_cn.StateChange += MudaEstadoConexao();
		}

		private StateChangeEventHandler MudaEstadoConexao()
		{
			if (_cn.State == ConnectionState.Open)
			{
				Conectado = true;
				DesConectado = false;
				ShowConecta = Visibility.Collapsed;
				ShowDesConecta = Visibility.Visible;
			} else
			{
				Conectado = false;
				DesConectado = true;
				ShowConecta = Visibility.Visible;
				ShowDesConecta = Visibility.Collapsed;

			}
			OnPropertyChanged(nameof(ShowConecta));
			OnPropertyChanged(nameof(ShowDesConecta));
			OnPropertyChanged(nameof(Conectado));
			OnPropertyChanged(nameof(DesConectado));
			return null;
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
			_cn.ConnectionString = str.ConnectionString;
			try
			{
				
				_cn.Open();
				_cn.StateChange += MudaEstadoConexao();
				return _cn;
			} catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return null;
			}
		}

		public void Desconecta()
		{
			_cn.Close();
			_cn.StateChange += MudaEstadoConexao();

		}

		public bool Conectado { get; set; }
		public bool DesConectado { get; set; }
		public Visibility ShowConecta { get; set; } = Visibility.Visible;
		public Visibility ShowDesConecta { get; set; } = Visibility.Collapsed;

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
