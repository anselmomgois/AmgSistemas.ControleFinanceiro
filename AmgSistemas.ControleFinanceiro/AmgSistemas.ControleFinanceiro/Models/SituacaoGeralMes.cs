using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmgSistemas.ControleFinanceiro.Models
{
    public class SituacaoGeralMes
    {
        public string DescricaoCategoriaGeral { get; set; }
        private double _ValorGasto;
        public double ValorGasto
        {
            get
            {
                return _ValorGasto;
            }
            set
            {
                _ValorGasto = value;
            }

        }
        private double _PorcentagemGasta;
        public double PorcentagemGasta
        {
            get
            {
                return _PorcentagemGasta;
            }
            set
            {
                _PorcentagemGasta = value;
            }
        }
        private double _PorcentagemIdeal;
        public double PorcentagemIdeal
        {
            get
            {
                return _PorcentagemIdeal;
            }
            set
            {
                _PorcentagemIdeal = value;
            }
        }

        private double _ValorIdeal;
        public double ValorIdeal
        {
            get
            {
                return _ValorIdeal;
            }
            set
            {
                _ValorIdeal = value;
            }
        }
        public double PorcentagemDiferenca
        {
            get
            {
                return (_PorcentagemIdeal - _PorcentagemGasta);
            }
        }
        public double ValorDiferenca
        {
            get
            {
                return (_ValorIdeal - _ValorGasto);
            }
        }
        public Int32 Ordem { get; set; }
    }
}