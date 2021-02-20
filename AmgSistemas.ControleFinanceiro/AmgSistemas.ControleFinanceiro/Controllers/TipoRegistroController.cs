using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmgSistemas.ControleFinanceiro.Models;
using AmgSistemas.ControleFinanceiro.Extensoes;
using AmgSistemas.ControleFinanceiro.Controllers.Shared;

namespace AmgSistemas.ControleFinanceiro.Controllers
{
    public class TipoRegistroController : ApplicationController<Login>
    {
        // GET: TipoRegistro
        public ActionResult Listar()
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            List<Models.TipoRegistro> objTipoRegistro = (from BD.AGCF_TIPOREGISTRO i in objBD.AGCF_TIPOREGISTRO
                                                         where i.IDUSUARIO == IdentificadorUsuario
                                                         select new Models.TipoRegistro()
                                                         {
                                                             Identificador = i.IDTIPOREGISTRO,
                                                             Nome = i.DESTIPOREGISTRO,
                                                             TipoValor = i.CODTIPOREGISTRO,
                                                             CodigoCategoriaGeral = i.CODCATEGORIAGERAL,
                                                             Codigo = i.CODTIPOREGISTROORIGINAL,
                                                             Parcelamento = i.BOLPARCELAMENTO != null && i.BOLPARCELAMENTO == true ? true : false
                                                         }).ToList();




            return View(objTipoRegistro);
        }

        public ActionResult Criar()
        {
            List<SelectListItem> objCategoriaGeralDropDown = new List<SelectListItem>();
            objCategoriaGeralDropDown.Add(new SelectListItem { Text = "Credito", Value = "CD" });
            objCategoriaGeralDropDown.Add(new SelectListItem { Text = "Debito", Value = "DE" });
            objCategoriaGeralDropDown.Add(new SelectListItem { Text = "Investimento", Value = "IV" });

            ViewBag.Categorias = objCategoriaGeralDropDown;

            List<SelectListItem> objTipoRegistroDropDown = new List<SelectListItem>();
            objTipoRegistroDropDown.Add(new SelectListItem { Text = "Despesa Essencial", Value = "DEE" });
            objTipoRegistroDropDown.Add(new SelectListItem { Text = "Despesa Não Essencial", Value = "DNE" });
            objTipoRegistroDropDown.Add(new SelectListItem { Text = "Divida", Value = "DIV" });
            objTipoRegistroDropDown.Add(new SelectListItem { Text = "Lazer", Value = "LAZ" });

            ViewBag.TipoRegistro = objTipoRegistroDropDown;

            return View();
        }
     

        [HttpPost]
        public ActionResult Criar(Models.TipoRegistro objTipoRegistro)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            BD.AGCF_TIPOREGISTRO objFontesBD = new BD.AGCF_TIPOREGISTRO()
            {
                DESTIPOREGISTRO = objTipoRegistro.Nome,
                IDUSUARIO = IdentificadorUsuario,
                IDTIPOREGISTRO = Guid.NewGuid().ToString(),
                CODTIPOREGISTRO = objTipoRegistro.TipoValor,
                CODCATEGORIAGERAL = objTipoRegistro.CodigoCategoriaGeral,
                BOLPARCELAMENTO = objTipoRegistro.Parcelamento,
                CODTIPOREGISTROORIGINAL = objTipoRegistro.Codigo,
                BOLSALARIO = objTipoRegistro.Salario
            };

            objBD.AGCF_TIPOREGISTRO.Add(objFontesBD);
            objBD.SaveChanges();


            List<Models.TipoRegistro> objTipoRegistroRetorno = (from BD.AGCF_TIPOREGISTRO i in objBD.AGCF_TIPOREGISTRO
                                                                where i.IDUSUARIO == IdentificadorUsuario
                                                                select new Models.TipoRegistro()
                                                                {
                                                                    Identificador = i.IDTIPOREGISTRO,
                                                                    Nome = i.DESTIPOREGISTRO,
                                                                    TipoValor = i.CODTIPOREGISTRO,
                                                                    CodigoCategoriaGeral = i.CODCATEGORIAGERAL,
                                                                    Codigo = i.CODTIPOREGISTROORIGINAL,
                                                                    Parcelamento = i.BOLPARCELAMENTO != null && i.BOLPARCELAMENTO == true ? true : false,
                                                                    Salario = i.BOLSALARIO != null && i.BOLSALARIO == true ? true : false
                                                                }).ToList();


            return View("Listar", objTipoRegistroRetorno);
        }

        public ActionResult Editar(string id)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            Models.TipoRegistro objTipoRegistro = (from BD.AGCF_TIPOREGISTRO i in objBD.AGCF_TIPOREGISTRO
                                                   where i.IDTIPOREGISTRO == id
                                                   select new Models.TipoRegistro()
                                                   {
                                                       Identificador = i.IDTIPOREGISTRO,
                                                       Nome = i.DESTIPOREGISTRO,
                                                       TipoValor = i.CODTIPOREGISTRO,
                                                       CodigoCategoriaGeral = i.CODCATEGORIAGERAL,
                                                       Codigo = i.CODTIPOREGISTROORIGINAL,
                                                       Parcelamento = i.BOLPARCELAMENTO != null && i.BOLPARCELAMENTO == true ? true : false,
                                                       Salario = i.BOLSALARIO != null && i.BOLSALARIO == true ? true : false
                                                   }).FirstOrDefault();

            List<SelectListItem> objCategoriaGeralDropDown = new List<SelectListItem>();
            objCategoriaGeralDropDown.Add(new SelectListItem { Text = "Credito", Value = "CD" });
            objCategoriaGeralDropDown.Add(new SelectListItem { Text = "Debito", Value = "DE" });
            objCategoriaGeralDropDown.Add(new SelectListItem { Text = "Investimento", Value = "IV" });

            ViewBag.Categorias = objCategoriaGeralDropDown;

            List<SelectListItem> objTipoRegistroDropDown = new List<SelectListItem>();
            objTipoRegistroDropDown.Add(new SelectListItem { Text = "Despesa Essencial", Value = "DEE" });
            objTipoRegistroDropDown.Add(new SelectListItem { Text = "Despesa Não Essencial", Value = "DNE" });
            objTipoRegistroDropDown.Add(new SelectListItem { Text = "Divida", Value = "DIV" });
            objTipoRegistroDropDown.Add(new SelectListItem { Text = "Lazer", Value = "LAZ" });

            ViewBag.TipoRegistro = objTipoRegistroDropDown;

            return View(objTipoRegistro);
        }

        [HttpPost]
        public ActionResult Editar(Models.TipoRegistro objTipoRegistro)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            BD.AGCF_TIPOREGISTRO objTipoRegistroBD = (from BD.AGCF_TIPOREGISTRO i in objBD.AGCF_TIPOREGISTRO where i.IDTIPOREGISTRO == objTipoRegistro.Identificador select i).FirstOrDefault();

            if (objTipoRegistroBD != null)
            {
                objTipoRegistroBD.DESTIPOREGISTRO = objTipoRegistro.Nome;
                objTipoRegistroBD.CODTIPOREGISTRO = objTipoRegistro.TipoValor;
                objTipoRegistroBD.CODCATEGORIAGERAL = objTipoRegistro.CodigoCategoriaGeral;
                objTipoRegistroBD.BOLPARCELAMENTO = objTipoRegistro.Parcelamento;
                objTipoRegistroBD.CODTIPOREGISTROORIGINAL = objTipoRegistro.Codigo;
                objTipoRegistroBD.BOLSALARIO = objTipoRegistro.Salario;
            }

            objBD.SaveChanges();

            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            List<Models.TipoRegistro> objTipoRegistroRetorno = (from BD.AGCF_TIPOREGISTRO i in objBD.AGCF_TIPOREGISTRO
                                                                where i.IDUSUARIO == IdentificadorUsuario
                                                                select new Models.TipoRegistro()
                                                                {
                                                                    Identificador = i.IDTIPOREGISTRO,
                                                                    Nome = i.DESTIPOREGISTRO,
                                                                    TipoValor = i.CODTIPOREGISTRO,
                                                                    CodigoCategoriaGeral = i.CODCATEGORIAGERAL,
                                                                    Codigo = i.CODTIPOREGISTROORIGINAL,
                                                                    Parcelamento = i.BOLPARCELAMENTO != null && i.BOLPARCELAMENTO == true ? true : false,
                                                                    Salario = i.BOLSALARIO != null && i.BOLSALARIO == true ? true : false
                                                                }).ToList();

            return View("Listar", objTipoRegistroRetorno);
        }

        public ActionResult Excluir(string id)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            Models.TipoRegistro objFonte = (from BD.AGCF_TIPOREGISTRO i in objBD.AGCF_TIPOREGISTRO
                                            where i.IDTIPOREGISTRO == id
                                            select new Models.TipoRegistro()
                                            {
                                                Identificador = i.IDTIPOREGISTRO,
                                                Nome = i.DESTIPOREGISTRO
                                            }).FirstOrDefault();

            return View(objFonte);
        }

        [HttpPost]
        public ActionResult Excluir(Models.TipoRegistro objIntegrante)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            BD.AGCF_TIPOREGISTRO objIntegranteBD = (from BD.AGCF_TIPOREGISTRO i in objBD.AGCF_TIPOREGISTRO where i.IDTIPOREGISTRO == objIntegrante.Identificador select i).FirstOrDefault();

            if (objIntegranteBD != null)
            {
                objBD.AGCF_TIPOREGISTRO.Remove(objIntegranteBD);
            }

            objBD.SaveChanges();

            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            List<Models.TipoRegistro> objTipoRegistro = (from BD.AGCF_TIPOREGISTRO i in objBD.AGCF_TIPOREGISTRO
                                                         where i.IDUSUARIO == IdentificadorUsuario
                                                         select new Models.TipoRegistro()
                                                         {
                                                             Identificador = i.IDTIPOREGISTRO,
                                                             Nome = i.DESTIPOREGISTRO,
                                                             TipoValor = i.CODTIPOREGISTRO,
                                                             CodigoCategoriaGeral = i.CODCATEGORIAGERAL,
                                                             Parcelamento = i.BOLPARCELAMENTO != null && i.BOLPARCELAMENTO == true ? true : false
                                                         }).ToList();

            return View("Listar", objTipoRegistro);
        }
    }
}