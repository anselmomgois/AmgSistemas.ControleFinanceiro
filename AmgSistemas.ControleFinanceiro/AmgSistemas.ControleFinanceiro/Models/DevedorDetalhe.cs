using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmgSistemas.ControleFinanceiro.Models
{
    public class DevedorDetalhe
    {
        public string IdentificadorIntegrante { get; set; }
        public string NomeIntegrante { get; set; }
        public List<ItemIntegrante> ItemsGastos { get; set; }
        public List<ItemIntegrante> PagamentosEfetuados { get; set; }
    }
}