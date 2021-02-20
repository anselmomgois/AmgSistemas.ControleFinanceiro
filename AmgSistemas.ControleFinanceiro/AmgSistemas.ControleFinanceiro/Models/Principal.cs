using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmgSistemas.ControleFinanceiro.Models
{
    public class Principal
    {
        public string NomeUsuario { get; set; }
        public List<Models.SituacaoGeralMes> SituacaoGeralMes { get; set; }
        public Models.SituacaoGeralMes Receita { get; set; }
        public double TotalReceita { get; set; }
        public double TotalDespesa { get; set; }
        public double Saldo { get; set; }
        public Int32 SequenciaTotal { get; set; }
        public string Ano { get; set; }
        public string Mes { get; set; }
        public bool BuscarQuitado { get; set; }
    }
}