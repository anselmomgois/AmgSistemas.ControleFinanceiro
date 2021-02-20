using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmgSistemas.ControleFinanceiro.Models
{
    public class BuscaRegistros
    {
        public Int32 Ano { get; set; }
        public Int32 Mes { get; set; }

        public string id { get; set; }
        public bool BolBuscarQuitado { get; set; }
    }
}