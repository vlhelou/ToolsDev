using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tools3.ViewModel.Banco
{
	public class MontaInViewModel : INotifyPropertyChanged
	{


		public string TipoConversao { get; set; }
		public string Origem { get; set; }
		public string Destino { get; set; }

		private string Saida = "semformatacao";
		public bool rdData
		{
			get { return Saida == "data"; }
			set
			{
				if (value)
				{
					Saida = "data";
				}
				OnPropertyChanged(nameof(rdData));
				OnPropertyChanged(nameof(rdString));
				OnPropertyChanged(nameof(rdSemAlteracao));
			}
		}
		public bool rdString
		{
			get { return Saida == "string"; }
			set
			{
				if (value)
				{
					Saida = "string";
				}
				OnPropertyChanged(nameof(rdData));
				OnPropertyChanged(nameof(rdString));
				OnPropertyChanged(nameof(rdSemAlteracao));
			}
		}
		public bool rdSemAlteracao
		{
			get { return Saida == "semformatacao"; }
			set
			{
				if (value)
				{
					Saida = "semformatacao";
				}
				OnPropertyChanged(nameof(rdData));
				OnPropertyChanged(nameof(rdString));
				OnPropertyChanged(nameof(rdSemAlteracao));
			}
		}

		public ICommand GeraClick
		{
			get { return new CommandHandler(() => Gera()); }
		}

		public void Gera()
		{
			Destino = "";
			string[] linhas = Origem.Split(Environment.NewLine.ToCharArray());
			List<string> conversao = new List<string>();
			switch (Saida)
			{
				case "semformatacao":
					foreach (string linha in linhas)
					{
						if (!string.IsNullOrEmpty(linha))
							conversao.Add(linha);
					}

					break;
				case "string":
					foreach (string linha in linhas)
					{
						if (!string.IsNullOrEmpty(linha))
							conversao.Add(string.Format("'{0}'", linha.Replace("'", "''")));
					}

					break;
				case "data":
					DateTime iData;
					foreach (string linha in linhas)
					{
						if (DateTime.TryParse(linha, out iData))
							conversao.Add(string.Format("'{0}'", iData.ToString("yyyy-MM-dd HH:mm:ss")));
					}

					break;
			}
			if (conversao.Count > 0)
			{
				Destino = "(" + string.Join(",", conversao) + ")";
				System.Windows.Clipboard.SetText(Destino);

			}
			OnPropertyChanged(nameof(Destino));
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
