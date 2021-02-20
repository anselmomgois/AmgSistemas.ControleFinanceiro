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
    public class RelatorioCategoriaController : ApplicationController<Login>
    {
        // GET: Relatorio
        public ActionResult Index()
        {
            return View(BuscarRegistros(DateTime.Now.Year, DateTime.Now.Month));
        }

        [HttpPost]
        public ActionResult Buscar(AmgSistemas.ControleFinanceiro.Models.BuscaRegistros Filtro)
        {

            return View("Index", BuscarRegistros(Filtro.Ano, Filtro.Mes));
        }

        private Models.RelatoriCategoria BuscarRegistros(Int32 Ano, Int32 Mes)
        {
            string NomeUsuario = GetLogOnSessionModel().Nome;
            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            List<Models.Registro> objRegistros = RegistroFinanceiroController.BuscarRegistros(new BuscaRegistros() { Ano = Ano, Mes = Mes }, IdentificadorUsuario, true, false);

            List<Models.RegistroRelatorioCategoria> objDespesasRetorno = new List<RegistroRelatorioCategoria>();
            

            if (objRegistros != null && objRegistros.Count > 0)
            {
                string CodigoCategoriaGeralDespesaEssencial = Enumeradores.CategoriaGeral.DESPESAESSENCIAL.RecuperarValor();
                string CodigoReceita = Enumeradores.TipoFonte.CREDITO.RecuperarValor();



                objDespesasRetorno = (from Models.Registro r in objRegistros
                                     where r.CodigoTipoRegistro != CodigoReceita
                                     group r by r.TipoRegistro into Soma
                                     select new Models.RegistroRelatorioCategoria()
                                     {
                                         Categoria = Soma.First().TipoRegistro,
                                         Valor = Soma.Sum(vg => vg.Valor)
                                     }).ToList();
                
                
            }

            return new Models.RelatoriCategoria { Ano = Ano.ToString(), Mes = Mes.ToString(), Registros = objDespesasRetorno };

        }
    }
}