using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AmgSistemas.ControleFinanceiro.Models
{

    public class UsuarioCadastro
    {
        public string Identificador { get; set; }
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Usuario é obrigatório")]
        [Remote("ValidarUsuario", "UsuarioCadastro", ErrorMessage = "Usuário já existe")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Senha { get; set; }
        [Required(ErrorMessage = "Confirmar senha é obrigatório")]
        [System.ComponentModel.DataAnnotations.Compare("Senha", ErrorMessage = "As senhas não são iguais")]
        public string ConfirmarSenha { get; set; }
        [Required(ErrorMessage = "Email é obrigatório")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "O email informado não é valido")]
        public string Email { get; set; }
        public string Telefone { get; set; }
    }
}