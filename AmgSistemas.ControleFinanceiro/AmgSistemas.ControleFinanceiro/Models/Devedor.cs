using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmgSistemas.ControleFinanceiro.Models
{
    public class Devedor
    {
        public string IdentificadorIntegrante { get; set; }
        public string NomeIntegrante { get; set; }
        private double _ValorTotalDivida;
        public double ValorTotalDivida
        {
            get
            {
                return _ValorTotalDivida;
            }
            set
            {
                _ValorTotalDivida = value;
            }
        }
        private double _ValorTotalPagamento;
        public double ValorTotalPagamento
        {
            get
            {
                return _ValorTotalPagamento;
            }
            set
            {
                _ValorTotalPagamento = value;
            }
        }

        public double ValorRemanecente
        {
            get
            {
                return (ValorTotalDivida - ValorTotalPagamento);
            }
        }
    }
}