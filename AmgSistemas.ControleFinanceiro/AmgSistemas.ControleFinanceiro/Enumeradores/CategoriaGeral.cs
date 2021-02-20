using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AmgSistemas.ControleFinanceiro.Atributos;

namespace AmgSistemas.ControleFinanceiro.Enumeradores
{
    public enum CategoriaGeral
    {
        [ValorEnum("DEE")]
        DESPESAESSENCIAL,
        [ValorEnum("DNE")]
        DESPESANAOESSENCIAL,
        [ValorEnum("INV")]
        INVESTIMENTO,
        [ValorEnum("DIV")]
        DIVIDA,
        [ValorEnum("LAZ")]
        LAZER
    }
}