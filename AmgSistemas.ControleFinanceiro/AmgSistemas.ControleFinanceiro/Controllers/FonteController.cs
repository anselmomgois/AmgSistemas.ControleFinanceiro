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
    public class FonteController : ApplicationController<Login>
    {
        // GET: Fonte
        public ActionResult Listar()
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            List<Models.Fonte> objFonte = (from BD.AGCF_FONTES i in objBD.AGCF_FONTES
                                           where i.IDUSUARIO == IdentificadorUsuario
                                           select new Models.Fonte()
                                           {
                                               Identificador = i.IDFONTES,
                                               Nome = i.DESNOME,
                                               DiaFechamento = i.NELDIAFECHAMENTO,
                                               TipoValor = i.CODTIPOFONTE,
                                               Codigo = i.CODFONTE,
                                               CartaoCredito = i.BOLCARTAOCREDITO != null && i.BOLCARTAOCREDITO == true ? true : false
                                           }).ToList();

            return View(objFonte);
        }

        public ActionResult Criar()
        {
            List<SelectListItem> objCategoriaGeralDropDown = new List<SelectListItem>();
            objCategoriaGeralDropDown.Add(new SelectListItem { Text = "Credito", Value = "CD" });
            objCategoriaGeralDropDown.Add(new SelectListItem { Text = "Debito", Value = "DE" });
            objCategoriaGeralDropDown.Add(new SelectListItem { Text = "Investimento", Value = "IV" });

            ViewBag.Categorias = objCategoriaGeralDropDown;

            return View();
        }

        [HttpPost]
        public ActionResult Criar(Models.Fonte objIntegrante)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            BD.AGCF_FONTES objFontesBD = new BD.AGCF_FONTES()
            {
                DESNOME = objIntegrante.Nome,
                IDUSUARIO = IdentificadorUsuario,
                IDFONTES = Guid.NewGuid().ToString(),
                CODTIPOFONTE = objIntegrante.TipoValor,
                NELDIAFECHAMENTO = objIntegrante.DiaFechamento,
                BOLCARTAOCREDITO = objIntegrante.CartaoCredito,
                CODFONTE = objIntegrante.Codigo
            };

            objBD.AGCF_FONTES.Add(objFontesBD);
            objBD.SaveChanges();

            string CodigoTipoFonteDebito = Enumeradores.TipoFonte.DEBITO.RecuperarValor();
            string CodigoTipoFonteCredito = Enumeradores.TipoFonte.CREDITO.RecuperarValor();
            string CodigoTipoFonteInvestimento = Enumeradores.TipoFonte.INVESTIMENTO.RecuperarValor();

            List<Models.Fonte> objFontes = (from BD.AGCF_FONTES i in objBD.AGCF_FONTES
                                            where i.IDUSUARIO == IdentificadorUsuario
                                            select new Models.Fonte()
                                            {
                                                Identificador = i.IDFONTES,
                                                Nome = i.DESNOME,
                                                DiaFechamento = i.NELDIAFECHAMENTO,
                                                TipoValor = i.CODTIPOFONTE,
                                                Codigo = i.CODFONTE,
                                                CartaoCredito = i.BOLCARTAOCREDITO != null && i.BOLCARTAOCREDITO == true ? true : false
                                            }).ToList();


            return View("Listar", objFontes);
        }

        public ActionResult Editar(string id)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            string CodigoTipoFonteDebito = Enumeradores.TipoFonte.DEBITO.RecuperarValor();
            string CodigoTipoFonteCredito = Enumeradores.TipoFonte.CREDITO.RecuperarValor();
            string CodigoTipoFonteInvestimento = Enumeradores.TipoFonte.INVESTIMENTO.RecuperarValor();

            Models.Fonte objFonte = (from BD.AGCF_FONTES i in objBD.AGCF_FONTES
                                     where i.IDFONTES == id
                                     select new Models.Fonte()
                                     {
                                         Identificador = i.IDFONTES,
                                         Nome = i.DESNOME,
                                         DiaFechamento = i.NELDIAFECHAMENTO,
                                         TipoValor = i.CODTIPOFONTE,
                                         Codigo = i.CODFONTE,
                                         CartaoCredito = i.BOLCARTAOCREDITO != null && i.BOLCARTAOCREDITO == true ? true : false
                                     }).FirstOrDefault();

            List<SelectListItem> objCategoriaGeralDropDown = new List<SelectListItem>();
            objCategoriaGeralDropDown.Add(new SelectListItem { Text = "Credito", Value = "CD" });
            objCategoriaGeralDropDown.Add(new SelectListItem { Text = "Debito", Value = "DE" });
            objCategoriaGeralDropDown.Add(new SelectListItem { Text = "Investimento", Value = "IV" });

            ViewBag.Categorias = objCategoriaGeralDropDown;


            return View(objFonte);
        }

        [HttpPost]
        public ActionResult Editar(Models.Fonte fonte)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            BD.AGCF_FONTES objFonteBD = (from BD.AGCF_FONTES i in objBD.AGCF_FONTES where i.IDFONTES == fonte.Identificador select i).FirstOrDefault();

            if (objFonteBD != null)
            {
                objFonteBD.DESNOME = fonte.Nome;
                objFonteBD.CODTIPOFONTE = fonte.TipoValor;
                objFonteBD.NELDIAFECHAMENTO = fonte.DiaFechamento;
                objFonteBD.BOLCARTAOCREDITO = fonte.CartaoCredito;
                objFonteBD.CODFONTE = fonte.Codigo;
            }

            objBD.SaveChanges();

            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;
            string CodigoTipoFonteDebito = Enumeradores.TipoFonte.DEBITO.RecuperarValor();
            string CodigoTipoFonteCredito = Enumeradores.TipoFonte.CREDITO.RecuperarValor();
            string CodigoTipoFonteInvestimento = Enumeradores.TipoFonte.INVESTIMENTO.RecuperarValor();

            List<Models.Fonte> objFonte = (from BD.AGCF_FONTES i in objBD.AGCF_FONTES
                                           where i.IDUSUARIO == IdentificadorUsuario
                                           select new Models.Fonte()
                                           {
                                               Identificador = i.IDFONTES,
                                               Nome = i.DESNOME,
                                               DiaFechamento = i.NELDIAFECHAMENTO,
                                               TipoValor = i.CODTIPOFONTE,
                                               Codigo = i.CODFONTE,
                                               CartaoCredito = i.BOLCARTAOCREDITO != null && i.BOLCARTAOCREDITO == true ? true : false
                                           }).ToList();

            return View("Listar", objFonte);
        }
        public ActionResult Excluir(string id)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            Models.Fonte objFonte = (from BD.AGCF_FONTES i in objBD.AGCF_FONTES
                                     where i.IDFONTES == id
                                     select new Models.Fonte()
                                     {
                                         Identificador = i.IDFONTES,
                                         Nome = i.DESNOME,
                                         DiaFechamento = i.NELDIAFECHAMENTO
                                     }).FirstOrDefault();

            return View(objFonte);
        }

        [HttpPost]
        public ActionResult Excluir(Models.Integrantes objIntegrante)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            BD.AGCF_FONTES objIntegranteBD = (from BD.AGCF_FONTES i in objBD.AGCF_FONTES where i.IDFONTES == objIntegrante.Identificador select i).FirstOrDefault();

            if (objIntegranteBD != null)
            {
                objBD.AGCF_FONTES.Remove(objIntegranteBD);
            }

            objBD.SaveChanges();

            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;
            string CodigoTipoFonteDebito = Enumeradores.TipoFonte.DEBITO.RecuperarValor();
            string CodigoTipoFonteCredito = Enumeradores.TipoFonte.CREDITO.RecuperarValor();
            string CodigoTipoFonteInvestimento = Enumeradores.TipoFonte.INVESTIMENTO.RecuperarValor();

            List<Models.Fonte> objFonte = (from BD.AGCF_FONTES i in objBD.AGCF_FONTES
                                           where i.IDUSUARIO == IdentificadorUsuario
                                           select new Models.Fonte()
                                           {
                                               Identificador = i.IDFONTES,
                                               Nome = i.DESNOME,
                                               DiaFechamento = i.NELDIAFECHAMENTO,
                                               TipoValor = i.CODTIPOFONTE,
                                               CartaoCredito = i.BOLCARTAOCREDITO != null && i.BOLCARTAOCREDITO == true ? true : false
                                           }).ToList();

            return View("Listar", objFonte);
        }
    }
}