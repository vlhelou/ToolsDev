using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools3.Model.Banco.BuscaBanco
{
	public class ObjetoBancoModel : INotifyPropertyChanged
	{
		private bool _Selecionado = true;
		public string Nome { get; set; }
		public int Id { get; set; }
		public string Tipo { get; set; }
		public bool Selecionado {
			get { return _Selecionado; }
			set {
				_Selecionado = value;
				foreach(ObjetoBancoModel item in Filhos)
				{
					item.Selecionado = _Selecionado;
				}
				OnPropertyChanged(nameof(Selecionado));
				OnPropertyChanged(nameof(Filhos));
			}
		} 

		public string NomeSql
		{
			get
			{
				if (Pai != null)
				{
					return $"[{Pai.Nome}].[{Nome}]";
				}
				return "";
			}
		}
		public byte Nivel { get; set; }

		public ObjetoBancoModel Pai { get; set; }
		public List<ObjetoBancoModel> Filhos { get; set; } = new List<ObjetoBancoModel>();


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
