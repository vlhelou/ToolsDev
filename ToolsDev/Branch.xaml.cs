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
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Data;

namespace ToolsDev
{
	/// <summary>
	/// Interaction logic for Branch.xaml
	/// </summary>
	public partial class Branch : Window
	{
		private XmlDocument Doc = new XmlDocument();
		private XmlNode Ponto;
		//private ImageList myImageList = new ImageList();

		private struct stcSVN
		{
			public bool Alterado;
			public int Branch;
		}

		public Branch()
		{
			InitializeComponent();

			string pathXML = System.AppDomain.CurrentDomain.BaseDirectory + "branch.xml";

			lbPath.Content = pathXML;
			try
			{
				Doc.Load(pathXML);
				PopulaTree();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				this.Close();
			}

		}

		private void PopulaTree()
		{
			tvProjetos.Items.Clear();
			int i = 0;
			foreach (XmlNode Solucao in Doc.DocumentElement.ChildNodes)
			{
				TreeViewItem ndSolucao = new TreeViewItem() { Header = Solucao.Attributes["Nome"].Value, Tag = i };
				ndSolucao.Expanded += new RoutedEventHandler(TreeViewItem_Expanded);
				int ii = 0;
				foreach (XmlNode Fontes in Solucao.ChildNodes)
				{
					TreeViewItem ndFontes = new TreeViewItem() { Header = Fontes.Attributes["Nome"].Value, Tag = ii };

					ndSolucao.Items.Add(ndFontes);
					ii++;
				}
				tvProjetos.Items.Add(ndSolucao);
				i++;
			}
		}

		private void tvProjetos_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{

			TreeViewItem ponto = (TreeViewItem)tvProjetos.SelectedItem;
			bool AtivaBotoes = false;
			if (!(ponto.Parent is TreeView))
			{
				TreeViewItem pai = (TreeViewItem)ponto.Parent;
				lbXMLPath.Content = string.Format("{0}\\{1}", pai.Header, ponto.Header);

				Ponto = Doc.DocumentElement.ChildNodes[Convert.ToInt16(pai.Tag)].ChildNodes[Convert.ToInt16(ponto.Tag)];
				Atribui();
				//SVNVersao();
				AtivaBotoes = true;
			}
			else
			{
				lbXMLPath.Content = string.Format("{0}", ponto.Header);
				Ponto = null;
				lbXMLPath.Foreground = Brushes.Black;
				gDetalhes.ItemsSource = null;
				gSVN.ItemsSource = null;
			}


			btnReverte.IsEnabled = AtivaBotoes;
			btnSWHMG.IsEnabled = AtivaBotoes;
			btnSWSTL.IsEnabled = AtivaBotoes;

		}

		private void Atribui()
		{
			//gDetalhes.Rows.Clear();
			//gSVN.Rows.Clear();
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Campo"));
			dt.Columns.Add(new DataColumn("Valor"));
			if (Ponto != null)
			{
				DataRow ln = dt.NewRow();
				ln["Campo"] = "Nome";
				ln["Valor"] = Ponto.Attributes["Nome"].Value;
				dt.Rows.Add(ln);

				ln = dt.NewRow();
				ln["Campo"] = "Local";
				ln["Valor"] = Ponto.Attributes["Local"].Value;
				dt.Rows.Add(ln);

				ln = dt.NewRow();
				ln["Campo"] = "STABLE";
				ln["Valor"] = Ponto.Attributes["STABLE"].Value;
				dt.Rows.Add(ln);

				ln = dt.NewRow();
				ln["Campo"] = "HMG";
				ln["Valor"] = Ponto.Attributes["HMG"].Value;
				dt.Rows.Add(ln);
				gDetalhes.ItemsSource = dt.DefaultView;
				SVNVersao();



			}


		}

		private void SVNVersao()
		{
			gDetalhes.UpdateLayout();
			DataTable dt = new DataTable();
			DataRow ln;
			dt.Columns.Add(new DataColumn("Campo"));
			dt.Columns.Add(new DataColumn("Valor"));

			string SVN = ExecCMD("svn info " + Ponto.Attributes["Local"].Value);
			string[] lines = SVN.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			string CaminhoURL = "";
			foreach (string Linha in lines)
			{
				if (Linha.StartsWith("URL: "))
				{
					ln = dt.NewRow();
					ln["Campo"] = "URL";
					CaminhoURL = Linha.Remove(0, 5);
					ln["Valor"] = CaminhoURL;
					if (ln["Valor"].ToString() == (string)Ponto.Attributes["STABLE"].Value)
					{

						DataGridRow firstRow = gDetalhes.ItemContainerGenerator.ContainerFromItem(gDetalhes.Items[2]) as DataGridRow;
						DataGridCell firstColumnInFirstRow = gDetalhes.Columns[1].GetCellContent(firstRow).Parent as DataGridCell;
						firstColumnInFirstRow.Background = Brushes.Aqua;
					}
					if (ln["Valor"].ToString() == (string)Ponto.Attributes["HMG"].Value)
					{
						DataGridRow firstRow = gDetalhes.ItemContainerGenerator.ContainerFromItem(gDetalhes.Items[3]) as DataGridRow;
						DataGridCell firstColumnInFirstRow = gDetalhes.Columns[1].GetCellContent(firstRow).Parent as DataGridCell;
						firstColumnInFirstRow.Background = Brushes.Aqua;

					}
					dt.Rows.Add(ln);
				}
				if (Linha.StartsWith("Revision: "))
				{
					ln = dt.NewRow();
					ln["Campo"] = "Revision";
					ln["Valor"] = Linha.Remove(0, 10);
					dt.Rows.Add(ln);
				}
			}
			if (!string.IsNullOrEmpty(CaminhoURL))
			{
				ln = dt.NewRow();
				ln["Campo"] = "Branch";
				string[] partes = CaminhoURL.Split('/');
				if (partes.Length > 2)
				{
					if (partes[partes.Length - 1].ToLower() == "fontes")
						ln["Valor"] = partes[partes.Length - 2];
					else
						ln["Valor"] = partes[partes.Length - 1];
				}
				dt.Rows.Add(ln);
			}
			gSVN.ItemsSource = dt.DefaultView;

		}

		private stcSVN SVNValores()
		{
			//Descobre se foi alterado ou não
			stcSVN Retorno = new stcSVN();
			string SVN = ExecCMD("svn st " + Ponto.Attributes["Local"].Value);
			string[] lines = SVN.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			foreach (string Linha in lines)
			{
				if ((!Linha.StartsWith("?")) & (Linha.Length > 3))
				{
					Retorno.Alterado = true;
					break;
				}
			}

			//Pega o Branch
			SVN = ExecCMD("svn info " + Ponto.Attributes["Local"].Value);
			lines = SVN.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			string CaminhoURL = "";
			foreach (string Linha in lines)
			{
				if (Linha.StartsWith("URL: "))
				{
					CaminhoURL = Linha.Remove(0, 5);
					break;
				}
			}
			Retorno.Branch = 1;
			if (CaminhoURL == (string)Ponto.Attributes["STABLE"].Value)
				Retorno.Branch = 3;
			if (CaminhoURL == (string)Ponto.Attributes["HMG"].Value)
				Retorno.Branch = 2;


			return Retorno;
		}


		private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
		{
			TreeViewItem tvi = e.OriginalSource as TreeViewItem;
			for (int i = 0; i < tvi.Items.Count; i++)
			{
				Ponto = Doc.DocumentElement.ChildNodes[Convert.ToInt16( tvi.Tag)].ChildNodes[i];
				stcSVN svnValor = SVNValores();

				TreeViewItem item;
				item = (TreeViewItem)tvi.Items[i];
				if (svnValor.Alterado)
				{
					item.Foreground = Brushes.Red;
				}
					
				else
				{
					item.Foreground = Brushes.Aqua;
				}
					
				//e.Node.Nodes[i].ImageIndex = svnValor.Branch;
				//e.Node.Nodes[i].SelectedImageIndex = svnValor.Branch;


			}
			
		}

		private string ExecCMD(string Comando)
		{
			string output = string.Empty;
			string error = string.Empty;

			ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c " + Comando);
			processStartInfo.CreateNoWindow = true;
			processStartInfo.RedirectStandardOutput = true;
			processStartInfo.RedirectStandardError = true;
			processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
			processStartInfo.UseShellExecute = false;

			Process process = Process.Start(processStartInfo);
			using (StreamReader streamReader = process.StandardOutput)
			{
				output = streamReader.ReadToEnd();
			}

			using (StreamReader streamReader = process.StandardError)
			{
				error = streamReader.ReadToEnd();
			}

			//txtInfo.Text = output;
			return output;
		}

		private void btnReverte_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Confirma Reversão?", "Reversão",MessageBoxButton.YesNo)==MessageBoxResult.Yes)
			{
				string SVN = ExecCMD("svn revert -R " + Ponto.Attributes["Local"].Value);
				MessageBox.Show(SVN);
			}

		}

		private void btnSWHMG_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Confirma Switch para HMG?", "Switch", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				string SVN = ExecCMD(string.Format("svn switch  {0} {1}", Ponto.Attributes["HMG"].Value, Ponto.Attributes["Local"].Value));
				MessageBox.Show(SVN);
			}


		}

		private void btnSWSTL_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Confirma Switch para STABLE?", "Switch", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				string SVN = ExecCMD(string.Format("svn switch  {0} {1}", Ponto.Attributes["STABLE"].Value, Ponto.Attributes["Local"].Value));
				MessageBox.Show(SVN);
			}

		}




	}
}
