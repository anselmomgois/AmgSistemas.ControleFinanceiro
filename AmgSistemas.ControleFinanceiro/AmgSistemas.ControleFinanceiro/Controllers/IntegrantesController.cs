using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmgSistemas.ControleFinanceiro.Controllers.Shared;
using AmgSistemas.ControleFinanceiro.Models;

namespace AmgSistemas.ControleFinanceiro.Controllers
{
    public class IntegrantesController : ApplicationController<Login>
    {
        // GET: Participantes
        public ActionResult Listar()
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            List<Models.Integrantes> objIntegrantes = (from BD.AGCF_INTEGRANTES i in objBD.AGCF_INTEGRANTES
                                                       where i.IDUSUARIO == IdentificadorUsuario
                                                       select new Models.Integrantes()
                                                       {
                                                           Identificador = i.IDINTEGRANTE,
                                                           Nome = i.DESNOME,
                                                           PessoaExterna = i.BOLEXTERNO != null && i.BOLEXTERNO == true ? true : false
                                                       }).ToList();

            return View(objIntegrantes);
        }

        public ActionResult Criar()
        {          

            return View();
        }

        public ActionResult Editar(string id)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            Models.Integrantes objIntegrante = (from BD.AGCF_INTEGRANTES i in objBD.AGCF_INTEGRANTES
                                                       where i.IDINTEGRANTE == id
                                                       select new Models.Integrantes()
                                                       {
                                                           Identificador = i.IDINTEGRANTE,
                                                           Nome = i.DESNOME,
                                                           PessoaExterna = i.BOLEXTERNO != null && i.BOLEXTERNO == true ? true : false
                                                       }).FirstOrDefault();

            return View(objIntegrante);
        }

        public ActionResult Excluir(string id)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            Models.Integrantes objIntegrante = (from BD.AGCF_INTEGRANTES i in objBD.AGCF_INTEGRANTES
                                                where i.IDINTEGRANTE == id
                                                select new Models.Integrantes()
                                                {
                                                    Identificador = i.IDINTEGRANTE,
                                                    Nome = i.DESNOME
                                                }).FirstOrDefault();

            return View(objIntegrante);
        }

        [HttpPost]
        public ActionResult Excluir(Models.Integrantes objIntegrante)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            BD.AGCF_INTEGRANTES objIntegranteBD = (from BD.AGCF_INTEGRANTES i in objBD.AGCF_INTEGRANTES where i.IDINTEGRANTE == objIntegrante.Identificador select i).FirstOrDefault();

            if (objIntegranteBD != null)
            {
                objBD.AGCF_INTEGRANTES.Remove(objIntegranteBD);
            }

            objBD.SaveChanges();

            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            List<Models.Integrantes> objIntegrantes = (from BD.AGCF_INTEGRANTES i in objBD.AGCF_INTEGRANTES
                                                       where i.IDUSUARIO == IdentificadorUsuario
                                                       select new Models.Integrantes()
                                                       {
                                                           Identificador = i.IDINTEGRANTE,
                                                           Nome = i.DESNOME
                                                       }).ToList();

            return View("Listar", objIntegrantes);
        }

        [HttpPost]
        public ActionResult Editar(Models.Integrantes objIntegrante)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            BD.AGCF_INTEGRANTES objIntegranteBD = (from BD.AGCF_INTEGRANTES i in objBD.AGCF_INTEGRANTES where i.IDINTEGRANTE == objIntegrante.Identificador select i).FirstOrDefault();

           if(objIntegranteBD != null)
            {
                objIntegranteBD.DESNOME = objIntegrante.Nome;
                objIntegranteBD.BOLEXTERNO = objIntegrante.PessoaExterna;
            }

            objBD.SaveChanges();

            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            List<Models.Integrantes> objIntegrantes = (from BD.AGCF_INTEGRANTES i in objBD.AGCF_INTEGRANTES
                                                       where i.IDUSUARIO == IdentificadorUsuario
                                                       select new Models.Integrantes()
                                                       {
                                                           Identificador = i.IDINTEGRANTE,
                                                           Nome = i.DESNOME,
                                                           PessoaExterna = i.BOLEXTERNO != null && i.BOLEXTERNO == true ? true : false
                                                       }).ToList();

            return View("Listar", objIntegrantes);
        }

        [HttpPost]
        public ActionResult Criar(Models.Integrantes objIntegrante)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            BD.AGCF_INTEGRANTES objIntegranteBD = new BD.AGCF_INTEGRANTES()
            {
                DESNOME = objIntegrante.Nome,
                IDUSUARIO = IdentificadorUsuario,
                BOLEXTERNO = objIntegrante.PessoaExterna,
                IDINTEGRANTE = Guid.NewGuid().ToString()
            };

            objBD.AGCF_INTEGRANTES.Add(objIntegranteBD);
            objBD.SaveChanges();

            List<Models.Integrantes> objIntegrantes = (from BD.AGCF_INTEGRANTES i in objBD.AGCF_INTEGRANTES
                                                       where i.IDUSUARIO == IdentificadorUsuario
                                                       select new Models.Integrantes()
                                                       {
                                                           Identificador = i.IDINTEGRANTE,
                                                           Nome = i.DESNOME,
                                                           PessoaExterna = i.BOLEXTERNO != null && i.BOLEXTERNO == true ? true : false
                                                       }).ToList();

            return View("Listar", objIntegrantes);
        }
    }
}