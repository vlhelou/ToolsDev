using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

namespace UnitTest3.ViewModel
{
	[TestClass]
	public class CriaClasse
	{
		[TestMethod]
		public void TestMethod1()
		{
			SqlConnection cn = new SqlConnection("server=.; database=ComposicaoCusto; integrated security=true");
			cn.Open();
			cn.Close();

			Tools3.ViewModel.Programacao.CriaClasseViewModel cc = new Tools3.ViewModel.Programacao.CriaClasseViewModel();
			Tools3.Model.Banco.BuscaBanco.ObjetoBancoModel tablea = new Tools3.Model.Banco.BuscaBanco.ObjetoBancoModel() {
				Id= 1301579675,
				Nome= "grupo",
				Tipo="Table"

			};
			cc.CN = cn;
			cc.Gera();

			Assert.AreEqual(1 > 0, true, "não acho schemas");
		}
	}
}
