using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AmgSistemas.ControleFinanceiro.Models
{
    public class Integrantes
    {
        public string Identificador { get; set; }
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; }
        public bool PessoaExterna { get; set; }
    }
}