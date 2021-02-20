using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AmgSistemas.ControleFinanceiro.Atributos;

namespace AmgSistemas.ControleFinanceiro.Enumeradores
{
    public enum TipoFonte
    {
        [ValorEnum("CD")]
        CREDITO,
        [ValorEnum("DE")]
        DEBITO,
        [ValorEnum("IV")]
        INVESTIMENTO
    }
}