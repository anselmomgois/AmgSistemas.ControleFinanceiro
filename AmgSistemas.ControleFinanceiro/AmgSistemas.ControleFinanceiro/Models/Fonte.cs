using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AmgSistemas.ControleFinanceiro.Models
{
    public class Fonte
    {
        public string Identificador { get; set; }
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; }
        public string TipoValor { get; set; }
        public Nullable<Int32> DiaFechamento { get; set; }
        public bool CartaoCredito { get; set; }
        public string Codigo { get; set; }
    }
}