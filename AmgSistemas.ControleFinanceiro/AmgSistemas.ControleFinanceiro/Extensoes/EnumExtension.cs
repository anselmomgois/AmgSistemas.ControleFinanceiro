using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmgSistemas.ControleFinanceiro.Extensoes
{
    public static class EnumExtension
    {

        public static string RecuperarValor(this System.Enum e)
        {
            Type tipoEnum = e.GetType();
            System.Reflection.FieldInfo field = tipoEnum.GetField(System.Enum.GetName(tipoEnum, e));
            if (field != null)
            {
                Atributos.ValorEnum atributo = (Atributos.ValorEnum)Attribute.GetCustomAttribute(field, typeof(Atributos.ValorEnum));
                if (atributo != null)
                {
                    return atributo.Valor;
                }
                throw new NotImplementedException(tipoEnum.ToString() + "." + field.Name);
            }
            return null;
        }

        public static TEnum RecuperarEnum<TEnum>(string valor)
        {
            Type tipoEnum = typeof(TEnum);
            if (!tipoEnum.IsEnum)
            {
                throw new ArgumentException("TEnum", "TEnum");
            }
            System.Reflection.FieldInfo[] fields = tipoEnum.GetFields();
            if (fields != null)
            {
                Atributos.ValorEnum atributo = default(Atributos.ValorEnum);
                foreach (System.Reflection.FieldInfo field in fields)
                {
                    atributo = (Atributos.ValorEnum)Attribute.GetCustomAttribute(field, typeof(Atributos.ValorEnum));
                    if (atributo != null)
                    {
                        if (atributo.Valor == valor)
                        {
                            return (TEnum)System.Enum.Parse(typeof(TEnum), field.Name);
                        }
                    }
                }

                return RecuperarEnum<TEnum>("NAO-RECONHECIDO");

            }
            throw new NotImplementedException(valor);
        }

    }
}