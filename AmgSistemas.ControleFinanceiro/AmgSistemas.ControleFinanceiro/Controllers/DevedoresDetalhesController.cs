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
    public class DevedoresDetalhesController : ApplicationController<Login>
    {
        public ActionResult Listar()
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            List<BD.AGCF_INTEGRANTES> objIntegrantes = (from BD.AGCF_INTEGRANTES i in objBD.AGCF_INTEGRANTES where i.IDUSUARIO == IdentificadorUsuario && i.BOLEXTERNO == true select i).ToList();
            List<Models.DevedorDetalhe> objDevedor = new List<DevedorDetalhe>();
            if (objIntegrantes != null)
            {
                List<string> IdentificadoresIntegrantes = objIntegrantes.Select(i => i.IDINTEGRANTE).ToList();
                string CodigoReceita = Enumeradores.TipoFonte.CREDITO.RecuperarValor();
                List<Models.Registro> objRegistros = RegistroFinanceiroController.BuscarRegistrosDevedores(IdentificadoresIntegrantes, IdentificadorUsuario);

                foreach (var i in objIntegrantes)
                {

                    objDevedor.Add(new DevedorDetalhe()
                    {
                        IdentificadorIntegrante = i.IDINTEGRANTE,
                        NomeIntegrante = i.DESNOME,
                        PagamentosEfetuados = (from Models.Registro r in objRegistros
                                               where r.CodigoTipoRegistro == CodigoReceita && r.IdentificadorIntegrante == i.IDINTEGRANTE
                                               select new Models.ItemIntegrante() {
                                                   Identificador = r.Identificador,
                                                   Descricao = r.Descricao,
                                                   Valor = r.Valor,
                                                   Data = r.DataRegistro
                                               }).ToList(),
                        ItemsGastos = (from Models.Registro r in objRegistros
                                       where r.CodigoTipoRegistro != CodigoReceita && r.IdentificadorIntegrante == i.IDINTEGRANTE
                                       select new Models.ItemIntegrante()
                                       {
                                           Identificador = r.Identificador,
                                           Descricao = r.Descricao,
                                           Valor = r.Valor,
                                           Data = r.DataRegistro
                                       }).ToList()
                    });
                }
            }

            return View(objDevedor);
        }
    }
}