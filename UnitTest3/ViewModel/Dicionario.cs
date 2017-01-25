using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;

namespace UnitTest3.ViewModel
{
	[TestClass]
	public class Dicionario
	{
		[TestMethod]
		public void Conexao()
		{
			SqlConnection cn = new SqlConnection("server=sqlh117; database=infraestrutura; integrated security=true");
			cn.Open();
			Tools3.ViewModel.Banco.DicionarioViewModel dicionario = new Tools3.ViewModel.Banco.DicionarioViewModel();
			dicionario.CN = cn;
			
			Assert.AreEqual(dicionario.Base.Filhos.Count>0, true,"não acho schemas");
			cn.Close();
		}

		[TestMethod]
		public void SelecionaItem()
		{
			SqlConnection cn = new SqlConnection("server=sqlh117; database=infraestrutura; integrated security=true");
			cn.Open();
			Tools3.ViewModel.Banco.DicionarioViewModel dicionario = new Tools3.ViewModel.Banco.DicionarioViewModel();
			dicionario.CN = cn;
			dicionario.Selecionado = dicionario.Base.Filhos[2];
			cn.Close();

		}
	}
}
