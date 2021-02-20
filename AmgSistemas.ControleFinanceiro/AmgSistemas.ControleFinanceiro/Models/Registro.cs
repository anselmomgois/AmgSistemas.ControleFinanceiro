using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AmgSistemas.ControleFinanceiro.Models
{
    public class Registro
    {
        private double _Valor;

        public string Identificador { get; set; }
        [Required(ErrorMessage = "Descrição é obrigatória")]
        public string Descricao { get; set; }
        public string Observacao { get; set; }
        public bool Quitado { get; set; }
        public bool Pago { get; set; }
        public string CategoriaGeral { get; set; }
        public Nullable<Int32> Parcela { get; set; }
        public Nullable<Int32> ParcelaFinal { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataRegistro { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        private DateTime _DataCompra;
        public DateTime DataCompra
        {
            get
            {
                return _DataCompra;
            }

            set
            {
                _DataCompra = value;
            }
        }

        public string DataCompraTela
        {
            get
            {
                return _DataCompra.ToString("yyyy-MM-dd");
            }

            set
            {
                if(!string.IsNullOrEmpty(value))
                {
                    _DataCompra =  Convert.ToDateTime(value);
                }
                
            }
        }
        public bool BolSalario { get; set; }
        public string IdentificadorFonte { get; set; }
        public string IdentificadorRegistroPai { get; set; }
        public string Fonte { get; set; }
        public string IdentificadorTipoRegistro { get; set; }
        public string TipoRegistro { get; set; }
        public string CodigoTipoRegistro { get; set; }
        public string IdentificadorIntegrante { get; set; }
        public string NomeIntegrante { get; set; }
        [DataType(DataType.Currency, ErrorMessage = "Valor deve ser uma quantia de dinheiro válida")]
        [Range(0.01, 999999.00)]
        public double Valor
        {
            get
            {
                return _Valor;
            }
            set
            {
                _Valor = value;
            }
        }
        public decimal ValorDecimal
        {
            get
            {
                return Convert.ToDecimal(_Valor);
            }
            set
            {
                _Valor = Convert.ToDouble(value);
            }
        }
    }
}