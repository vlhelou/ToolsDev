using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;


namespace ToolsDev
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private NotifyIcon iconTray;
		private ContextMenu mnTray;
		public MainWindow()
		{
			InitializeComponent();
			mnTray = new System.Windows.Forms.ContextMenu();

			System.Windows.Forms.MenuItem mnBanco = new System.Windows.Forms.MenuItem("Banco de Dados");
			mnBanco.MenuItems.Add(new MenuItem("Sql2Codigo", this.clickSql2Codigo));
			mnBanco.MenuItems.Add(new MenuItem("Busca Banco", this.ClickBuscaBanco));
			mnBanco.MenuItems.Add(new MenuItem("Dicionário", this.ClickDicionario));
			mnBanco.MenuItems.Add(new MenuItem("Monta IN", this.ClickMontaIN));
			mnBanco.MenuItems.Add(new MenuItem("Gera Insert", this.ClickGeraInsert));
			//ClickGeraInsert
			mnTray.MenuItems.Add(mnBanco);

			System.Windows.Forms.MenuItem mnProgramacao = new System.Windows.Forms.MenuItem("Programação");
			mnProgramacao.MenuItems.Add(new MenuItem("Cria Classe", this.clickCriaClasse));
			mnProgramacao.MenuItems.Add(new MenuItem("Branch", this.ClickBranch));
			mnTray.MenuItems.Add(mnProgramacao);


			System.Windows.Forms.MenuItem mnArquivo = new System.Windows.Forms.MenuItem("Banco de Dados");
			mnProgramacao.MenuItems.Add(new MenuItem("Lê Linhas", this.ClickLeLinhas));
			mnTray.MenuItems.Add(mnArquivo);

			iconTray = new System.Windows.Forms.NotifyIcon();
			iconTray.ContextMenu = mnTray;
			
			iconTray.Icon = new System.Drawing.Icon("Ferramenta.ico");
			//iconTray.ShowBalloonTip(2000);
			iconTray.Visible = true;

			//this.WindowState = WindowState.Minimized;
		}



		private void clickSql2Codigo(object sender, EventArgs e)
		{
			var aplic = new Sql2Codigo();
			aplic.Show();
			//MessageBox.Show("teste");
		}

		private void clickCriaClasse(object sender, EventArgs e)
		{
			var aplic = new CriaClasse(); 
			aplic.Show();

		}

		private void ClickBuscaBanco(object sender, EventArgs e)
		{
			var aplic = new BuscaBanco();
			aplic.Show();

		}
		private void ClickDicionario(object sender, EventArgs e)
		{
			var aplic = new Dicionario();
			aplic.Show();

		}

		private void ClickMontaIN(object sender, EventArgs e)
		{
			var aplic = new MontaIN();
			aplic.Show();

		}
		private void ClickGeraInsert(object sender, EventArgs e)
		{
			var aplic = new GeraInsert();
			aplic.Show();

		}
		private void ClickBranch(object sender, EventArgs e)
		{
			var aplic = new Branch();
			aplic.Show();

		}

		private void ClickLeLinhas(object sender, EventArgs e)
		{
			var aplic = new LeLinhas();
			aplic.Show();

		}

	}
}
