using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AmgSistemas.ControleFinanceiro.Extensoes;
using System.Web.Mvc;

namespace AmgSistemas.ControleFinanceiro.Classes
{
    public class Parametros
    {

        private static List<SelectListItem> _Anos = null;
        private static List<SelectListItem> _Meses = null;
        private static List<Tuple<string, string, double, Int32>> _CategoriasGerais = null;

        public static List<Tuple<string, string, double, Int32>> CategoriasGerais
        {
            get
            {
                if (_CategoriasGerais == null)
                {
                    _CategoriasGerais = new List<Tuple<string, string, double, Int32>>();

                    _CategoriasGerais.Add(new Tuple<string, string, double, Int32>(Enumeradores.CategoriaGeral.DESPESAESSENCIAL.RecuperarValor(), "Despesa Essencial", 40, 2));
                    _CategoriasGerais.Add(new Tuple<string, string, double, Int32>(Enumeradores.CategoriaGeral.DESPESANAOESSENCIAL.RecuperarValor(), "Despesa Não Essencial", 35, 3));
                    _CategoriasGerais.Add(new Tuple<string, string, double, Int32>(Enumeradores.CategoriaGeral.DIVIDA.RecuperarValor(), "Divida", 5, 4));
                    _CategoriasGerais.Add(new Tuple<string, string, double, Int32>(Enumeradores.CategoriaGeral.INVESTIMENTO.RecuperarValor(), "Investimento", 10, 5));
                    _CategoriasGerais.Add(new Tuple<string, string, double, Int32>(Enumeradores.CategoriaGeral.LAZER.RecuperarValor(), "Lazer", 10, 6));

                }

                return _CategoriasGerais;
            }
        }

        public static List<SelectListItem> Anos
        {
            get
            {
                if (_Anos == null || _Anos.Count == 0)
                {
                    _Anos = new List<SelectListItem>();

                    _Anos.Add(new SelectListItem { Text = (DateTime.Now.Year - 1).ToString(), Value = (DateTime.Now.Year - 1).ToString() });
                    _Anos.Add(new SelectListItem { Text = (DateTime.Now.Year).ToString(), Value = (DateTime.Now.Year).ToString() });
                    _Anos.Add(new SelectListItem { Text = (DateTime.Now.Year + 1).ToString(), Value = (DateTime.Now.Year + 1).ToString() });
                }

                return _Anos;
            }
        }

        public static List<SelectListItem> Meses
        {
            get
            {
                if (_Meses == null || _Meses.Count == 0)
                {
                    _Meses = new List<SelectListItem>();

                    _Meses.Add(new SelectListItem { Text = "Janeiro", Value = "1" });
                    _Meses.Add(new SelectListItem { Text = "Fevereiro", Value = "2" });
                    _Meses.Add(new SelectListItem { Text = "Março", Value = "3" });
                    _Meses.Add(new SelectListItem { Text = "Abril", Value = "4" });
                    _Meses.Add(new SelectListItem { Text = "Maio", Value = "5" });
                    _Meses.Add(new SelectListItem { Text = "Junho", Value = "6" });
                    _Meses.Add(new SelectListItem { Text = "Julho", Value = "7" });
                    _Meses.Add(new SelectListItem { Text = "Agosto", Value = "8" });
                    _Meses.Add(new SelectListItem { Text = "Setembro", Value = "9" });
                    _Meses.Add(new SelectListItem { Text = "Outubro", Value = "10" });
                    _Meses.Add(new SelectListItem { Text = "Novembro", Value = "11" });
                    _Meses.Add(new SelectListItem { Text = "Dezembro", Value = "12" });

                }

                return _Meses;
            }
        }
    }
}