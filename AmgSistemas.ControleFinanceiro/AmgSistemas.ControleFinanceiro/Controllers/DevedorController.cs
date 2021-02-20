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
    public class DevedorController : ApplicationController<Login>
    {
        // GET: Devedor
        public ActionResult Listar()
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            List<BD.AGCF_INTEGRANTES> objIntegrantes = (from BD.AGCF_INTEGRANTES i in objBD.AGCF_INTEGRANTES where i.IDUSUARIO == IdentificadorUsuario && i.BOLEXTERNO == true select i).ToList();
            List<Models.Devedor> objDevedor = new List<Devedor>();
            if(objIntegrantes != null)
            {
                List<string> IdentificadoresIntegrantes = objIntegrantes.Select(i => i.IDINTEGRANTE).ToList();
                string CodigoReceita = Enumeradores.TipoFonte.CREDITO.RecuperarValor();
                List<Models.Registro> objRegistros = RegistroFinanceiroController.BuscarRegistrosDevedores(IdentificadoresIntegrantes,IdentificadorUsuario);

                foreach (var i in objIntegrantes)
                {

                    objDevedor.Add(new Devedor()
                    {
                        IdentificadorIntegrante = i.IDINTEGRANTE,
                        NomeIntegrante = i.DESNOME,
                        ValorTotalPagamento = (from Models.Registro r in objRegistros
                                               where r.CodigoTipoRegistro == CodigoReceita && r.IdentificadorIntegrante == i.IDINTEGRANTE
                                              select r.Valor).Sum(),
                        ValorTotalDivida = (from Models.Registro r in objRegistros
                                            where r.CodigoTipoRegistro != CodigoReceita && r.IdentificadorIntegrante == i.IDINTEGRANTE
                                             select r.Valor).Sum()
                    });
                }
            }

            return View(objDevedor);
        }
    }
}