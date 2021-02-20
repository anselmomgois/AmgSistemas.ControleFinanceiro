using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AmgSistemas.ControleFinanceiro.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Usuario é obrigatório")]
        public string Usuario { get; set; }
        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Senha { get; set; }
        public string Identificador { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}