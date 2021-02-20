using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmgSistemas.ControleFinanceiro.Models
{
    public class RelatoriCategoria
    {
        public string Ano { get; set; }
        public string Mes { get; set; }
        public List<Models.RegistroRelatorioCategoria> Registros { get; set; }
    }
}