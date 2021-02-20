using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AmgSistemas.ControleFinanceiro.Models
{
    public class TipoRegistro
    {

        public string Identificador { get; set; }
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; }
        public string TipoValor { get; set; }
        public string CodigoCategoriaGeral { get; set; }
        public bool Parcelamento { get; set; }
        public string Codigo { get; set; }
        public bool Salario { get; set; }

    }
}