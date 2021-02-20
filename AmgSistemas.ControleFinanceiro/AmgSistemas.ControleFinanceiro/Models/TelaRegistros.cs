using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmgSistemas.ControleFinanceiro.Models
{
    public class TelaRegistros
    {

       public List<Models.Registro> Registros { get; set; }
        public string Ano { get; set; }
        public string Mes { get; set; }
        public string TipoRegistro { get; set; }
        public string Titulo { get; set; }
       
    }
}