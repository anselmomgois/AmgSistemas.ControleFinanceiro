using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmgSistemas.ControleFinanceiro.Controllers.Shared;
using AmgSistemas.ControleFinanceiro.Models;
using AmgSistemas.ControleFinanceiro.Extensoes;

namespace AmgSistemas.ControleFinanceiro.Controllers
{
    public class PrincipalController : ApplicationController<Login>
    {
        // GET: Principal
        public ActionResult Index()
        {
            
            return View(BuscarRegistros(DateTime.Now.Year, DateTime.Now.Month,false));
        }

        public ActionResult RelatorioCategoriaGeral(string BolBuscarQuitado)
        {

            return View("Index", BuscarRegistros(DateTime.Now.Year, DateTime.Now.Month, Convert.ToBoolean(BolBuscarQuitado)));
        }

        [HttpPost]
        public ActionResult Buscar(AmgSistemas.ControleFinanceiro.Models.BuscaRegistros Filtro)
        {

            return View("Index",BuscarRegistros(Filtro.Ano, Filtro.Mes, Filtro.BolBuscarQuitado));
        }

        private Models.Principal BuscarRegistros(Int32 Ano, Int32 Mes, bool BuscarQuitado)
        {
            string NomeUsuario = GetLogOnSessionModel().Nome;
            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            List<Models.Registro> objRegistros = RegistroFinanceiroController.BuscarRegistros(new BuscaRegistros() { Ano = Ano, Mes = Mes }, IdentificadorUsuario, !BuscarQuitado, BuscarQuitado);

            List<Models.SituacaoGeralMes> objDespesasRetorno = new List<SituacaoGeralMes>();
            Models.SituacaoGeralMes objDespesasReceita = null;
            double ValorReceita = 0;
            double ValorDespesa = 0;
            double ValorTotalReceita = 0;

            if (objRegistros != null && objRegistros.Count > 0)
            {
                string CodigoCategoriaGeralDespesaEssencial = Enumeradores.CategoriaGeral.DESPESAESSENCIAL.RecuperarValor();
                string CodigoReceita = Enumeradores.TipoFonte.CREDITO.RecuperarValor();
                List<Models.SituacaoGeralMes> objDespesasGastos = null;

               
                    objDespesasReceita = (from Models.Registro r in objRegistros
                                          where r.CodigoTipoRegistro == CodigoReceita //&& r.BolSalario == true
                                          group r by r.CodigoTipoRegistro into Soma
                                          select new Models.SituacaoGeralMes()
                                          {
                                              DescricaoCategoriaGeral = "Receita",
                                              ValorGasto = Soma.Sum(vg => vg.Valor),
                                              Ordem = 1
                                          }).FirstOrDefault();


                if (objDespesasReceita == null) objDespesasReceita = new SituacaoGeralMes()
                {
                    DescricaoCategoriaGeral = "Receita",
                    ValorGasto = 0
                };

                ValorReceita = objDespesasReceita.ValorGasto;

                ValorTotalReceita  = (from Models.Registro r in objRegistros
                                                          where r.CodigoTipoRegistro == CodigoReceita && r.BolSalario == true
                                                          group r by r.CodigoTipoRegistro into Soma
                                                          select  Soma.Sum(vg => vg.Valor)).FirstOrDefault();

                objDespesasReceita.ValorGasto = ValorTotalReceita;

                objDespesasGastos = (from Models.Registro r in objRegistros
                                     where r.CodigoTipoRegistro != CodigoReceita
                                     group r by r.CategoriaGeral into Soma
                                     select new Models.SituacaoGeralMes()
                                     {
                                         DescricaoCategoriaGeral = (from Tuple<string, string, double, Int32> v in Classes.Parametros.CategoriasGerais
                                                                    where v.Item1 == Soma.First().CategoriaGeral
                                                                    select v.Item2).FirstOrDefault(),
                                         ValorGasto = Soma.Sum(vg => vg.Valor),
                                         PorcentagemIdeal = (from Tuple<string, string, double, Int32> v in Classes.Parametros.CategoriasGerais
                                                             where v.Item1 == Soma.First().CategoriaGeral
                                                             select v.Item3).FirstOrDefault(),
                                         PorcentagemGasta = ((Soma.Sum(vg => vg.Valor) * 100) / ValorTotalReceita),
                                         ValorIdeal = (from Tuple<string, string, double, Int32> v in Classes.Parametros.CategoriasGerais
                                                       where v.Item1 == Soma.First().CategoriaGeral
                                                       select (v.Item3 * ValorTotalReceita) / 100).FirstOrDefault(),
                                         Ordem = (from Tuple<string, string, double, Int32> v in Classes.Parametros.CategoriasGerais
                                                  where v.Item1 == Soma.First().CategoriaGeral
                                                  select v.Item4).FirstOrDefault()
                                     }).ToList();

                if (objDespesasGastos != null && objDespesasGastos.Count > 0)
                {
                    objDespesasRetorno.AddRange(objDespesasGastos);

                    ValorDespesa = (from Models.Registro r in objRegistros
                                    where r.CodigoTipoRegistro != CodigoReceita
                                    select r.Valor).Sum();

                }

                if (objDespesasRetorno == null) objDespesasGastos = new List<SituacaoGeralMes>();
                foreach (var tv in Classes.Parametros.CategoriasGerais)
                {
                    if (!objDespesasRetorno.Exists(d => d.DescricaoCategoriaGeral == tv.Item2))
                    {
                        objDespesasRetorno.Add(new SituacaoGeralMes()
                        {
                            DescricaoCategoriaGeral = tv.Item2,
                            ValorGasto = 0,
                            PorcentagemIdeal = tv.Item3,
                            PorcentagemGasta = ((0 * 100) / ValorTotalReceita),
                            ValorIdeal = (tv.Item3 * ValorTotalReceita) / 100,
                            Ordem = tv.Item4
                        });
                    }
                }
            }

            return new Models.Principal() { NomeUsuario = NomeUsuario, BuscarQuitado = BuscarQuitado, Ano = Ano.ToString(), Mes = Mes.ToString(), SituacaoGeralMes = objDespesasRetorno, Receita = objDespesasReceita, TotalDespesa = ValorDespesa, SequenciaTotal = 100, Saldo = (ValorReceita - ValorDespesa) };

        }


        [HttpPost]
        public ActionResult RelatorioPorCategoria(AmgSistemas.ControleFinanceiro.Models.BuscaRegistros Filtro)
        {

            return View("Index", BuscarRegistros(Filtro.Ano, Filtro.Mes, Filtro.BolBuscarQuitado));
        }

    }
}