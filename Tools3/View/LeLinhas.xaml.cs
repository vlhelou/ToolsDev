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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tools3.View
{
	/// <summary>
	/// Interaction logic for LeLinhas.xaml
	/// </summary>
	public partial class LeLinhas : Page
	{
		Tools3.ViewModel.LeLinhasViewModel viewmodel = new ViewModel.LeLinhasViewModel();
		public LeLinhas()
		{
			InitializeComponent();
			DataContext = viewmodel;
		}

		public void Arquivo_Focus (object sender, RoutedEventArgs e)
		{
			var fileDialog = new System.Windows.Forms.OpenFileDialog();
			var result = fileDialog.ShowDialog();
			switch (result)
			{
				case System.Windows.Forms.DialogResult.OK:
					var file = fileDialog.FileName;
					
					arquivo.Text = file;
					
					break;
				case System.Windows.Forms.DialogResult.Cancel:
				default:
					arquivo.Text = null;
					break;
			}
		}
	}
}
