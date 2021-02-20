using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmgSistemas.ControleFinanceiro.Models
{
    public class ItemIntegrante
    {
        public string Identificador { get; set; }
        public string Descricao { get; set; }
        public double Valor { get; set; }
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
    }
}