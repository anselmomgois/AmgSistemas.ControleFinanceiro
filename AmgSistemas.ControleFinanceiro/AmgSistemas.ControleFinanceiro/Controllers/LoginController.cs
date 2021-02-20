using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmgSistemas.ControleFinanceiro.Controllers.Shared;
using AmgSistemas.ControleFinanceiro.Models;

namespace AmgSistemas.ControleFinanceiro.Controllers
{
    public class LoginController :  ApplicationController<Login>
    {
        // GET: Login
        public ActionResult Logar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Logar(Models.Login Login)
        {
            if(ModelState.IsValid)
            {

                BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

                var objLogin = (from BD.AGCF_USUARIO u in objBD.AGCF_USUARIO where u.DESLOGIN.ToUpper() == Login.Usuario.ToUpper() && u.DESSENHA == Login.Senha select u).FirstOrDefault();

                if(objLogin != null)
                {
                    Models.Login objLoginSessao = new Login()
                    {
                        Email =objLogin.DESEMAIL,
                        Identificador = objLogin.IDUSUARIO,
                        Nome = objLogin.DESNOME,
                        Usuario = objLogin.DESLOGIN
                    };

                    SetLogOnSessionModel(objLoginSessao);

                    return RedirectToAction("Index", "Principal");
                }
                else
                {
                    ModelState.AddModelError("", "Usuario ou senha incorretos.");
                }               
                
               
            }
            else
            {
                ModelState.AddModelError("", "Dados informados não estão corretos.");               
            }

            return View("Index", Login);
        }
    }
}
