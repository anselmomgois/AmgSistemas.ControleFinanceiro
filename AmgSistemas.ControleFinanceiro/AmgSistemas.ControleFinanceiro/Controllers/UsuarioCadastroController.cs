using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmgSistemas.ControleFinanceiro.Controllers.Shared;
using AmgSistemas.ControleFinanceiro.Models;

namespace AmgSistemas.ControleFinanceiro.Controllers
{
    public class UsuarioCadastroController : BaseController
    {
        // GET: UsuarioCadastro

        public ActionResult Cadastrar()
        {
            return View("Cadastrar", new UsuarioCadastro());
        }

        [HttpPost]
        public ActionResult Cadastrar(UsuarioCadastro Usuario)
        {
            if (ModelState.IsValid)
            {
                BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

                BD.AGCF_USUARIO objUsuario = new BD.AGCF_USUARIO()
                {
                    DESEMAIL = Usuario.Email,
                    DESLOGIN = Usuario.Login,
                    DESNOME = Usuario.Nome,
                    DESSENHA = Usuario.Senha,
                    DESTELEFONE = Usuario.Telefone,
                    IDUSUARIO = Guid.NewGuid().ToString()
                };

                objBD.AGCF_USUARIO.Add(objUsuario);

                objBD.SaveChanges();

                this.ShowMessage("Usuário cadastro com sucesso.");


                //return View("Logar", "Login", new Login());
                //return View(new UsuarioCadastro());
                return RedirectToAction("Logar", "Login");
            }

            return View(Usuario);
        }

        public ActionResult ValidarUsuario(string Usuario)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            return Json(objBD.AGCF_USUARIO != null && !objBD.AGCF_USUARIO.All(x => x.DESLOGIN.ToUpper() == Usuario.ToUpper()), JsonRequestBehavior.AllowGet);
        }
    }
}