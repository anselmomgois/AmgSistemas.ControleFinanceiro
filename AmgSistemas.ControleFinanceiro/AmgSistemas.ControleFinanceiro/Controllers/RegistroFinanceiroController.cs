using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AmgSistemas.ControleFinanceiro.Models;
using AmgSistemas.ControleFinanceiro.Extensoes;
using AmgSistemas.ControleFinanceiro.Controllers.Shared;
using System.Threading.Tasks;
using System.Web.Services;
using System.IO;
using System.Data;
using ExcelDataReader;
using System.Diagnostics;
using frmUtil = AmgSistemas.Framework.Utilitarios;

namespace AmgSistemas.ControleFinanceiro.Controllers
{
    public class RegistroFinanceiroController : ApplicationController<Login>
    {
        public ActionResult Listar(string id)
        {

            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            Models.TelaRegistros objTela = new TelaRegistros();
            objTela.Ano = DateTime.Now.Year.ToString();
            objTela.Mes = DateTime.Now.Month.ToString();

            objTela.Registros = BuscarRegistros(new BuscaRegistros()
            {
                id = id,
                Mes = DateTime.Now.Month,
                Ano = DateTime.Now.Year
            }, IdentificadorUsuario, true, false);


            if (id == Enumeradores.TipoFonte.CREDITO.RecuperarValor())
                objTela.Titulo = "Receita";
            else if (id == Enumeradores.TipoFonte.INVESTIMENTO.RecuperarValor())
                objTela.Titulo = "Investimento";
            else
                objTela.Titulo = "Despesa";

            objTela.TipoRegistro = id;

            return View(objTela);

        }

        public ActionResult CriarRegistros()
        {

            return View();

        }

        [HttpPost]
        public ActionResult GravarRegistros()
        {

            if (Request.Files.Count > 0 && !string.IsNullOrEmpty(Request.Files[0].FileName))
            {

                var fileName = Path.GetFileName(Request.Files[0].FileName);
                var path = Path.Combine(Server.MapPath("~/Arquivos/"), fileName);

                //file.SaveAs(path);
                Request.Files[0].SaveAs(path);

                var extension = Path.GetExtension(path).ToLower();
                System.Data.DataSet ds;

                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    IExcelDataReader reader = null;
                    if (extension == ".xls")
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (extension == ".xlsx")
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else if (extension == ".csv")
                    {
                        reader = ExcelReaderFactory.CreateCsvReader(stream);
                    }

                    if (reader != null)
                    {

                        var openTiming = sw.ElapsedMilliseconds;
                        // reader.IsFirstRowAsColumnNames = firstRowNamesCheckBox.Checked;
                        using (reader)
                        {
                            ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                UseColumnDataType = false,
                                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            });
                        }

                        if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            string Data = string.Empty;
                            string Descricao = string.Empty;
                            string Valor = string.Empty;
                            string Fonte = string.Empty;
                            string Categoria = string.Empty;
                            string Integrante = string.Empty;
                            string Debitado = string.Empty;

                            DateTime DataConvertida = DateTime.Now;
                            decimal ValorConvertido = 0;
                            bool DebitadoConvertido = false;

                            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
                            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

                            List<BD.AGCF_FONTES> objFontes = (from BD.AGCF_FONTES f in objBD.AGCF_FONTES where f.IDUSUARIO == IdentificadorUsuario select f).ToList();
                            List<BD.AGCF_TIPOREGISTRO> objTiposRegistros = (from BD.AGCF_TIPOREGISTRO tr in objBD.AGCF_TIPOREGISTRO
                                                                            where tr.IDUSUARIO == IdentificadorUsuario select tr).ToList();
                            List<BD.AGCF_INTEGRANTES> objIntegrantes = (from BD.AGCF_INTEGRANTES tr in objBD.AGCF_INTEGRANTES
                                                                           where tr.IDUSUARIO == IdentificadorUsuario
                                                                            select tr).ToList();
                            foreach (DataRow dr in dt.Rows)
                            {
                                Valor = frmUtil.Util.AtribuirValorObj(dr["VALOR"], typeof(string)) as string;
                                Data = frmUtil.Util.AtribuirValorObj(dr["DATA"], typeof(string)) as string;
                                Descricao = frmUtil.Util.AtribuirValorObj(dr["DESCRICAO"], typeof(string)) as string;
                                Fonte = frmUtil.Util.AtribuirValorObj(dr["FONTE"], typeof(string)) as string;
                                Categoria = frmUtil.Util.AtribuirValorObj(dr["CATEGORIA"], typeof(string)) as string;
                                Integrante = frmUtil.Util.AtribuirValorObj(dr["INTEGRANTE"], typeof(string)) as string;
                                Debitado = frmUtil.Util.AtribuirValorObj(dr["DEBITADO"], typeof(string)) as string;
                                DataConvertida = Convert.ToDateTime(Data);
                                ValorConvertido = Convert.ToDecimal(Valor);
                                DebitadoConvertido = Convert.ToBoolean(Convert.ToInt32(Debitado));

                                if(ValorConvertido < 0)
                                {
                                    ValorConvertido = ValorConvertido * (-1);
                                }

                                if (Descricao.ToUpper() != "SALDO ANTERIOR" && Descricao.ToUpper() != "SALDO DO DIA")
                                {
                                    var Existe = (from BD.AGCF_REGISTRO r in objBD.AGCF_REGISTRO
                                                  where r.DTHCOMPRA == DataConvertida && r.DESREGISTRO == Descricao && r.NUMVALOR == ValorConvertido && r.IDUSUARIO == IdentificadorUsuario
                                                  select r).Count() > 0;

                                    if (!Existe)
                                    {
                                        Models.Registro objRegistro = new Registro()
                                        {
                                            DataCompra = DataConvertida,
                                            IdentificadorFonte = (from BD.AGCF_FONTES f in objFontes where f.CODFONTE.ToUpper() == Fonte.ToUpper() select f.IDFONTES).FirstOrDefault(),
                                            IdentificadorIntegrante = (from BD.AGCF_INTEGRANTES i in objIntegrantes where i.DESNOME.ToUpper() == Integrante.ToUpper() select i.IDINTEGRANTE).FirstOrDefault(),
                                            Descricao = Descricao,
                                            IdentificadorTipoRegistro = (from BD.AGCF_TIPOREGISTRO i in objTiposRegistros
                                                                         where i.CODTIPOREGISTROORIGINAL.ToUpper() == Categoria.ToUpper()
                                                                         select i.IDTIPOREGISTRO).FirstOrDefault(),
                                            Valor = Convert.ToDouble(ValorConvertido),
                                            Pago = DebitadoConvertido
                                        };

                                        if (!string.IsNullOrEmpty(objRegistro.IdentificadorFonte) && !string.IsNullOrEmpty(objRegistro.IdentificadorIntegrante) && 
                                            !string.IsNullOrEmpty(objRegistro.IdentificadorTipoRegistro))
                                        {
                                            CriarRegistro(objRegistro, ref objBD);
                                        }
                                    }
                                }
                            }

                            objBD.SaveChanges();
                            ModelState.AddModelError("SFS", "Planilha Importada com Sucesso!");
                        }
                    }
                }
                

            }

            return View("CriarRegistros");
        }

        [HttpPost]
        public ActionResult Buscar(AmgSistemas.ControleFinanceiro.Models.BuscaRegistros Filtro)
        {
            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            Models.TelaRegistros objTela = new TelaRegistros();

            objTela.Registros = BuscarRegistros(Filtro, IdentificadorUsuario, true, false);
            objTela.Ano = Filtro.Ano.ToString();
            objTela.Mes = Filtro.Mes.ToString();

            if (Filtro.id == Enumeradores.TipoFonte.CREDITO.RecuperarValor())
                objTela.Titulo = "Receita";
            else if (Filtro.id == Enumeradores.TipoFonte.INVESTIMENTO.RecuperarValor())
                objTela.Titulo = "Investimento";
            else
                objTela.Titulo = "Despesa";

            objTela.TipoRegistro = Filtro.id;

            return View("Listar", objTela);

        }

        public static List<Models.Registro> BuscarRegistros(AmgSistemas.ControleFinanceiro.Models.BuscaRegistros Filtro, string IdentificadorUsuario,
                                                            bool bolBuscarTodos, bool bolQuitado)
        {

            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            List<Models.Registro> objRegistros = null;


            string strMes = Filtro.Mes.ToString();
            string strAno = Filtro.Ano.ToString();

            if (strMes.Length < 2) strMes = string.Format("0{0}", strMes);

            if (bolBuscarTodos)
            {
                objRegistros = (from BD.AGCF_REGISTRO i in objBD.AGCF_REGISTRO
                                join BD.AGCF_TIPOREGISTRO tr in objBD.AGCF_TIPOREGISTRO on i.IDTIPOREGISTRO equals tr.IDTIPOREGISTRO
                                join BD.AGCF_FONTES fon in objBD.AGCF_FONTES on i.IDFONTES equals fon.IDFONTES
                                where i.IDUSUARIO == IdentificadorUsuario && (!string.IsNullOrEmpty(Filtro.id) ? tr.CODTIPOREGISTRO == Filtro.id : 1 == 1) && i.IDFONTES == fon.IDFONTES &&
                                i.CODMES == strMes && i.CODANO == strAno
                                select new Models.Registro()
                                {
                                    Identificador = i.IDREGISTRO,
                                    Descricao = i.DESREGISTRO,
                                    IdentificadorFonte = fon.IDFONTES,
                                    Fonte = fon.DESNOME,
                                    DataCompra = i.DTHCOMPRA,
                                    CategoriaGeral = tr.CODCATEGORIAGERAL,
                                    DataRegistro = i.DTHDATAREGISTRO,
                                    Quitado = i.BOLQUITADO,
                                    Pago = i.BOLPAGO != null && i.BOLPAGO == true ? true : false,
                                    Observacao = i.OBSDETALHES,
                                    Parcela = i.NELPARCELA,
                                    ParcelaFinal = i.NELPARCELAFINAL,
                                    IdentificadorTipoRegistro = tr.IDTIPOREGISTRO,
                                    CodigoTipoRegistro = tr.CODTIPOREGISTRO,
                                    TipoRegistro = tr.DESTIPOREGISTRO,
                                    BolSalario = tr.BOLSALARIO != null && tr.BOLSALARIO == true ? true : false,
                                    NomeIntegrante = (from BD.AGCF_INTEGRANTES it in objBD.AGCF_INTEGRANTES where it.IDINTEGRANTE == i.IDINTEGRANTE select it.DESNOME).FirstOrDefault(),
                                    IdentificadorIntegrante = i.IDINTEGRANTE,
                                    ValorDecimal = i.NUMVALOR
                                }).OrderBy(r => r.DataCompra).ToList();
            }
            else
            {
                objRegistros = (from BD.AGCF_REGISTRO i in objBD.AGCF_REGISTRO
                                join BD.AGCF_TIPOREGISTRO tr in objBD.AGCF_TIPOREGISTRO on i.IDTIPOREGISTRO equals tr.IDTIPOREGISTRO
                                join BD.AGCF_FONTES fon in objBD.AGCF_FONTES on i.IDFONTES equals fon.IDFONTES
                                where i.IDUSUARIO == IdentificadorUsuario && (!string.IsNullOrEmpty(Filtro.id) ? tr.CODTIPOREGISTRO == Filtro.id : 1 == 1) && i.IDFONTES == fon.IDFONTES &&
                                i.CODMES == strMes && i.CODANO == strAno && i.BOLPAGO == bolQuitado
                                select new Models.Registro()
                                {
                                    Identificador = i.IDREGISTRO,
                                    Descricao = i.DESREGISTRO,
                                    IdentificadorFonte = fon.IDFONTES,
                                    Fonte = fon.DESNOME,
                                    DataCompra = i.DTHCOMPRA,
                                    CategoriaGeral = tr.CODCATEGORIAGERAL,
                                    DataRegistro = i.DTHDATAREGISTRO,
                                    Quitado = i.BOLQUITADO,
                                    Pago = i.BOLPAGO != null && i.BOLPAGO == true ? true : false,
                                    Observacao = i.OBSDETALHES,
                                    Parcela = i.NELPARCELA,
                                    ParcelaFinal = i.NELPARCELAFINAL,
                                    IdentificadorTipoRegistro = tr.IDTIPOREGISTRO,
                                    CodigoTipoRegistro = tr.CODTIPOREGISTRO,
                                    TipoRegistro = tr.DESTIPOREGISTRO,
                                    BolSalario = tr.BOLSALARIO != null && tr.BOLSALARIO == true ? true : false,
                                    NomeIntegrante = (from BD.AGCF_INTEGRANTES it in objBD.AGCF_INTEGRANTES where it.IDINTEGRANTE == i.IDINTEGRANTE select it.DESNOME).FirstOrDefault(),
                                    IdentificadorIntegrante = i.IDINTEGRANTE,
                                    ValorDecimal = i.NUMVALOR
                                }).OrderBy(r => r.DataCompra).ToList();
            }

            return objRegistros;

        }

        public static List<Models.Registro> BuscarRegistrosDevedores(List<string> IdentificadoresIntegrantes, string IdentificadorUsuario)
        {

            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            List<Models.Registro> objRegistros = null;

            if (IdentificadoresIntegrantes != null && IdentificadoresIntegrantes.Count > 0)
                objRegistros = (from BD.AGCF_REGISTRO i in objBD.AGCF_REGISTRO
                                join BD.AGCF_TIPOREGISTRO tr in objBD.AGCF_TIPOREGISTRO on i.IDTIPOREGISTRO equals tr.IDTIPOREGISTRO
                                join BD.AGCF_FONTES fon in objBD.AGCF_FONTES on i.IDFONTES equals fon.IDFONTES
                                join string inte in IdentificadoresIntegrantes on i.IDINTEGRANTE equals inte
                                where i.IDUSUARIO == IdentificadorUsuario
                                select new Models.Registro()
                                {
                                    Identificador = i.IDREGISTRO,
                                    Descricao = i.DESREGISTRO,
                                    IdentificadorFonte = fon.IDFONTES,
                                    Fonte = fon.DESNOME,
                                    DataCompra = i.DTHCOMPRA,
                                    CategoriaGeral = tr.CODCATEGORIAGERAL,
                                    DataRegistro = i.DTHDATAREGISTRO,
                                    Quitado = i.BOLQUITADO,
                                    Pago = i.BOLPAGO != null && i.BOLPAGO == true ? true : false,
                                    Observacao = i.OBSDETALHES,
                                    Parcela = i.NELPARCELA,
                                    ParcelaFinal = i.NELPARCELAFINAL,
                                    IdentificadorTipoRegistro = tr.IDTIPOREGISTRO,
                                    CodigoTipoRegistro = tr.CODTIPOREGISTRO,
                                    TipoRegistro = tr.DESTIPOREGISTRO,
                                    NomeIntegrante = (from BD.AGCF_INTEGRANTES it in objBD.AGCF_INTEGRANTES where it.IDINTEGRANTE == i.IDINTEGRANTE select it.DESNOME).FirstOrDefault(),
                                    IdentificadorIntegrante = i.IDINTEGRANTE,
                                    ValorDecimal = i.NUMVALOR
                                }).OrderBy(r => r.DataCompra).ToList();


            return objRegistros;

        }


        public ActionResult Criar(string id)
        {
            try
            {

                //BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
                string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

                List<Models.TipoRegistro> objTipoRegistro = null;

                List<Models.Fonte> objFonte = null;

                List<Models.Integrantes> objIntegrantes = null;


                Task objTaskTipoRegistro = new Task(() => objTipoRegistro = RetornarTipoRegistro(id, IdentificadorUsuario));

                Task objTaskFonte = new Task(() => objFonte = RetornarFontes(id, IdentificadorUsuario));

                Task objTaskIntegrantes = new Task(() => objIntegrantes = RetornarIntegrantes(IdentificadorUsuario));

                objTaskTipoRegistro.Start();
                objTaskFonte.Start();
                objTaskIntegrantes.Start();

                Task.WaitAll(new Task[] { objTaskTipoRegistro, objTaskFonte, objTaskIntegrantes });

                ViewBag.Parcelamento = objTipoRegistro != null && objTipoRegistro.Count > 0 && objTipoRegistro.FirstOrDefault().Parcelamento;

                List<SelectListItem> objFontesDropDown = new List<SelectListItem>();
                List<SelectListItem> objCategoriasDropDown = new List<SelectListItem>();
                List<SelectListItem> objIntegrantesDropDown = new List<SelectListItem>();

                if (objFonte != null && objFonte.Count > 0)
                {
                    foreach (var f in objFonte)
                    {
                        objFontesDropDown.Add(new SelectListItem { Text = f.Nome, Value = f.Identificador });
                    }
                }

                if (objTipoRegistro != null && objTipoRegistro.Count > 0)
                {
                    foreach (var f in objTipoRegistro)
                    {
                        objCategoriasDropDown.Add(new SelectListItem { Text = f.Nome, Value = f.Identificador });
                    }
                }

                if (objIntegrantes != null && objIntegrantes.Count > 0)
                {
                    foreach (var f in objIntegrantes)
                    {
                        objIntegrantesDropDown.Add(new SelectListItem { Text = f.Nome, Value = f.Identificador });
                    }
                }

                ViewBag.Fontes = objFontesDropDown;
                ViewBag.Categorias = objCategoriasDropDown;
                ViewBag.Integrantes = objIntegrantesDropDown;

                if (id == Enumeradores.TipoFonte.CREDITO.RecuperarValor())
                    ViewBag.Titulo = "Receita";
                else if (id == Enumeradores.TipoFonte.INVESTIMENTO.RecuperarValor())
                    ViewBag.Titulo = "Investimento";
                else
                    ViewBag.Titulo = "Despesa";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
            }
            return View(new Models.Registro() { DataCompra = DateTime.Now });
        }

        private void CriarRegistro(Models.Registro objRegistro, ref BD.IGERENCEEntities objBD)
        {
            
            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;
            string IdentificadorRegistro = Guid.NewGuid().ToString();

            BD.AGCF_TIPOREGISTRO objTipoRegistroBD = (from BD.AGCF_TIPOREGISTRO tr in objBD.AGCF_TIPOREGISTRO
                                                      where tr.IDTIPOREGISTRO == objRegistro.IdentificadorTipoRegistro
                                                      select tr).FirstOrDefault();

            BD.AGCF_FONTES objFonte = (from BD.AGCF_FONTES f in objBD.AGCF_FONTES where f.IDFONTES == objRegistro.IdentificadorFonte select f).FirstOrDefault();
            string Mes = string.Empty;
            string Ano = string.Empty;

            if (objFonte != null)
            {


                if (objFonte.BOLCARTAOCREDITO != null && objFonte.BOLCARTAOCREDITO == true && objFonte.NELDIAFECHAMENTO != null)
                {
                    DateTime objDiaVencimento = DateTime.Now;
                    DateTime ultimoDiaDoMes = new DateTime(objRegistro.DataCompra.Year, objRegistro.DataCompra.Month, DateTime.DaysInMonth(objRegistro.DataCompra.Year, objRegistro.DataCompra.Month));

                    if (ultimoDiaDoMes.Day < objFonte.NELDIAFECHAMENTO)
                    {
                        objDiaVencimento = Convert.ToDateTime(string.Format("{0}/{1}/{2}", ultimoDiaDoMes.Day, objRegistro.DataCompra.Month, objRegistro.DataCompra.Year));
                    }
                    else
                    {
                        objDiaVencimento = Convert.ToDateTime(string.Format("{0}/{1}/{2}", objFonte.NELDIAFECHAMENTO, objRegistro.DataCompra.Month, objRegistro.DataCompra.Year));
                    }

                    if (objDiaVencimento.AddDays(-10) < objRegistro.DataCompra)
                    {
                        if (objDiaVencimento < objRegistro.DataCompra)
                        {
                            if (objDiaVencimento.AddMonths(1).AddDays(-10) < objRegistro.DataCompra)
                            {
                                objDiaVencimento = objDiaVencimento.AddMonths(2);
                                Mes = objDiaVencimento.Month.ToString();
                                Ano = objDiaVencimento.Year.ToString();
                            }
                            else
                            {
                                objDiaVencimento = objDiaVencimento.AddMonths(1);
                                Mes = objDiaVencimento.Month.ToString();
                                Ano = objDiaVencimento.Year.ToString();
                            }
                        }
                        else
                        {
                            objDiaVencimento = objDiaVencimento.AddMonths(1);
                            Mes = objDiaVencimento.Month.ToString();
                            Ano = objDiaVencimento.Year.ToString();
                        }
                    }
                    else
                    {
                        Mes = objRegistro.DataCompra.Month.ToString();
                        Ano = objRegistro.DataCompra.Year.ToString();
                    }
                }
                else
                {
                    Mes = objRegistro.DataCompra.Month.ToString();
                    Ano = objRegistro.DataCompra.Year.ToString();
                }

                if (Mes.Length < 2) Mes = string.Format("0{0}", Mes);
            }


            BD.AGCF_REGISTRO objRegistroBD = new BD.AGCF_REGISTRO()
            {
                DESREGISTRO = objRegistro.Descricao,
                IDUSUARIO = IdentificadorUsuario,
                IDREGISTRO = IdentificadorRegistro,
                DTHCOMPRA = objRegistro.DataCompra,
                DTHDATAREGISTRO = objRegistro.DataRegistro,
                IDFONTES = objRegistro.IdentificadorFonte,
                IDINTEGRANTE = objRegistro.IdentificadorIntegrante,
                BOLQUITADO = (objRegistro.Parcela == objRegistro.ParcelaFinal),
                BOLPAGO = objRegistro.Pago,
                NELPARCELA = objRegistro.Parcela,
                NELPARCELAFINAL = objRegistro.ParcelaFinal,
                IDTIPOREGISTRO = objRegistro.IdentificadorTipoRegistro,
                NUMVALOR = Convert.ToDecimal(objRegistro.Valor),
                OBSDETALHES = objRegistro.Observacao,
                CODMES = Mes,
                CODANO = Ano
            };

            objRegistroBD.DTHDATAREGISTRO = objRegistroBD.DTHCOMPRA;
            objRegistro.DataRegistro = objRegistroBD.DTHDATAREGISTRO;

            objBD.AGCF_REGISTRO.Add(objRegistroBD);


            if (objTipoRegistroBD != null && objTipoRegistroBD.BOLPARCELAMENTO != null && objTipoRegistroBD.BOLPARCELAMENTO == true &&
                objRegistro.Parcela != null && objRegistro.ParcelaFinal != null)
            {
                Int32 nelMes = Convert.ToInt32(Mes);
                Int32 nelAno = Convert.ToInt32(Ano);

                for (int i = Convert.ToInt32(objRegistro.Parcela + 1); i <= Convert.ToInt32(objRegistro.ParcelaFinal); i++)
                {

                    objRegistro.DataRegistro = objRegistro.DataRegistro.AddMonths(1);

                    nelMes += 1;

                    if (nelMes > 12)
                    {
                        nelMes = 1;
                        nelAno += 1;
                    }

                    BD.AGCF_REGISTRO objRegistroBDFilho = new BD.AGCF_REGISTRO()
                    {
                        DESREGISTRO = objRegistro.Descricao,
                        IDUSUARIO = IdentificadorUsuario,
                        IDREGISTROPAI = IdentificadorRegistro,
                        IDREGISTRO = Guid.NewGuid().ToString(),
                        DTHCOMPRA = objRegistro.DataCompra,
                        DTHDATAREGISTRO = objRegistro.DataRegistro,
                        IDFONTES = objRegistro.IdentificadorFonte,
                        IDINTEGRANTE = objRegistro.IdentificadorIntegrante,
                        BOLQUITADO = (objRegistro.Parcela == objRegistro.ParcelaFinal),
                        NELPARCELA = i,
                        NELPARCELAFINAL = objRegistro.ParcelaFinal,
                        IDTIPOREGISTRO = objRegistro.IdentificadorTipoRegistro,
                        NUMVALOR = Convert.ToDecimal(objRegistro.Valor),
                        OBSDETALHES = objRegistro.Observacao,
                        CODMES = nelMes.ToString().Length < 2 ? string.Format("0{0}", nelMes.ToString()) : nelMes.ToString(),
                        CODANO = nelAno.ToString()

                    };


                    objBD.AGCF_REGISTRO.Add(objRegistroBDFilho);
                }
            }
        }

        [HttpPost]
        public ActionResult Criar(Models.Registro objRegistro)
        {
            if (ModelState.IsValid)
            {
                BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
                string IdentificadorUsuario = GetLogOnSessionModel().Identificador;
                string IdentificadorRegistro = Guid.NewGuid().ToString();

                BD.AGCF_TIPOREGISTRO objTipoRegistroBD = (from BD.AGCF_TIPOREGISTRO tr in objBD.AGCF_TIPOREGISTRO
                                                          where tr.IDTIPOREGISTRO == objRegistro.IdentificadorTipoRegistro
                                                          select tr).FirstOrDefault();

                BD.AGCF_FONTES objFonte = (from BD.AGCF_FONTES f in objBD.AGCF_FONTES where f.IDFONTES == objRegistro.IdentificadorFonte select f).FirstOrDefault();
                string Mes = string.Empty;
                string Ano = string.Empty;

                if (objFonte != null)
                {


                    if (objFonte.BOLCARTAOCREDITO != null && objFonte.BOLCARTAOCREDITO == true && objFonte.NELDIAFECHAMENTO != null)
                    {
                        DateTime objDiaVencimento = DateTime.Now;
                        DateTime ultimoDiaDoMes = new DateTime(objRegistro.DataCompra.Year, objRegistro.DataCompra.Month, DateTime.DaysInMonth(objRegistro.DataCompra.Year, objRegistro.DataCompra.Month));

                        if (ultimoDiaDoMes.Day < objFonte.NELDIAFECHAMENTO)
                        {
                            objDiaVencimento = Convert.ToDateTime(string.Format("{0}/{1}/{2}", ultimoDiaDoMes.Day, objRegistro.DataCompra.Month, objRegistro.DataCompra.Year));
                        }
                        else
                        {
                            objDiaVencimento = Convert.ToDateTime(string.Format("{0}/{1}/{2}", objFonte.NELDIAFECHAMENTO, objRegistro.DataCompra.Month, objRegistro.DataCompra.Year));
                        }

                        if (objDiaVencimento.AddDays(-10) < objRegistro.DataCompra)
                        {
                            if (objDiaVencimento < objRegistro.DataCompra)
                            {
                                if (objDiaVencimento.AddMonths(1).AddDays(-10) < objRegistro.DataCompra)
                                {
                                    objDiaVencimento = objDiaVencimento.AddMonths(2);
                                    Mes = objDiaVencimento.Month.ToString();
                                    Ano = objDiaVencimento.Year.ToString();
                                }
                                else
                                {
                                    objDiaVencimento = objDiaVencimento.AddMonths(1);
                                    Mes = objDiaVencimento.Month.ToString();
                                    Ano = objDiaVencimento.Year.ToString();
                                }
                            }
                            else
                            {
                                objDiaVencimento = objDiaVencimento.AddMonths(1);
                                Mes = objDiaVencimento.Month.ToString();
                                Ano = objDiaVencimento.Year.ToString();
                            }
                        }
                        else
                        {
                            Mes = objRegistro.DataCompra.Month.ToString();
                            Ano = objRegistro.DataCompra.Year.ToString();
                        }
                    }
                    else
                    {
                        Mes = objRegistro.DataCompra.Month.ToString();
                        Ano = objRegistro.DataCompra.Year.ToString();
                    }

                    if (Mes.Length < 2) Mes = string.Format("0{0}", Mes);
                }


                BD.AGCF_REGISTRO objRegistroBD = new BD.AGCF_REGISTRO()
                {
                    DESREGISTRO = objRegistro.Descricao,
                    IDUSUARIO = IdentificadorUsuario,
                    IDREGISTRO = IdentificadorRegistro,
                    DTHCOMPRA = objRegistro.DataCompra,
                    DTHDATAREGISTRO = objRegistro.DataRegistro,
                    IDFONTES = objRegistro.IdentificadorFonte,
                    IDINTEGRANTE = objRegistro.IdentificadorIntegrante,
                    BOLQUITADO = (objRegistro.Parcela == objRegistro.ParcelaFinal),
                    BOLPAGO = objRegistro.Pago,
                    NELPARCELA = objRegistro.Parcela,
                    NELPARCELAFINAL = objRegistro.ParcelaFinal,
                    IDTIPOREGISTRO = objRegistro.IdentificadorTipoRegistro,
                    NUMVALOR = Convert.ToDecimal(objRegistro.Valor),
                    OBSDETALHES = objRegistro.Observacao,
                    CODMES = Mes,
                    CODANO = Ano
                };

                objRegistroBD.DTHDATAREGISTRO = objRegistroBD.DTHCOMPRA;
                objRegistro.DataRegistro = objRegistroBD.DTHDATAREGISTRO;

                objBD.AGCF_REGISTRO.Add(objRegistroBD);


                if (objTipoRegistroBD != null && objTipoRegistroBD.BOLPARCELAMENTO != null && objTipoRegistroBD.BOLPARCELAMENTO == true &&
                    objRegistro.Parcela != null && objRegistro.ParcelaFinal != null)
                {
                    Int32 nelMes = Convert.ToInt32(Mes);
                    Int32 nelAno = Convert.ToInt32(Ano);

                    for (int i = Convert.ToInt32(objRegistro.Parcela + 1); i <= Convert.ToInt32(objRegistro.ParcelaFinal); i++)
                    {

                        objRegistro.DataRegistro = objRegistro.DataRegistro.AddMonths(1);

                        nelMes += 1;

                        if (nelMes > 12)
                        {
                            nelMes = 1;
                            nelAno += 1;
                        }

                        BD.AGCF_REGISTRO objRegistroBDFilho = new BD.AGCF_REGISTRO()
                        {
                            DESREGISTRO = objRegistro.Descricao,
                            IDUSUARIO = IdentificadorUsuario,
                            IDREGISTROPAI = IdentificadorRegistro,
                            IDREGISTRO = Guid.NewGuid().ToString(),
                            DTHCOMPRA = objRegistro.DataCompra,
                            DTHDATAREGISTRO = objRegistro.DataRegistro,
                            IDFONTES = objRegistro.IdentificadorFonte,
                            IDINTEGRANTE = objRegistro.IdentificadorIntegrante,
                            BOLQUITADO = (objRegistro.Parcela == objRegistro.ParcelaFinal),
                            NELPARCELA = i,
                            NELPARCELAFINAL = objRegistro.ParcelaFinal,
                            IDTIPOREGISTRO = objRegistro.IdentificadorTipoRegistro,
                            NUMVALOR = Convert.ToDecimal(objRegistro.Valor),
                            OBSDETALHES = objRegistro.Observacao,
                            CODMES = nelMes.ToString().Length < 2 ? string.Format("0{0}", nelMes.ToString()) : nelMes.ToString(),
                            CODANO = nelAno.ToString()

                        };


                        objBD.AGCF_REGISTRO.Add(objRegistroBDFilho);
                    }
                }

                objBD.SaveChanges();

                Models.TelaRegistros objTela = new TelaRegistros();
                objTela.Ano = Ano.ToString();
                objTela.Mes = Mes.ToString();

                if (objTipoRegistroBD != null)
                {
                    objTela.Registros = BuscarRegistros(new BuscaRegistros() { Ano = Convert.ToInt32(objTela.Ano), Mes = Convert.ToInt32(objTela.Mes), id = objTipoRegistroBD.CODTIPOREGISTRO },
                  IdentificadorUsuario, true, false);


                    if (objTipoRegistroBD.CODTIPOREGISTRO == Enumeradores.TipoFonte.CREDITO.RecuperarValor())
                        objTela.Titulo = "Receita";
                    else if (objTipoRegistroBD.CODTIPOREGISTRO == Enumeradores.TipoFonte.INVESTIMENTO.RecuperarValor())
                        objTela.Titulo = "Investimento";
                    else
                        objTela.Titulo = "Despesa";

                    objTela.TipoRegistro = objTipoRegistroBD.CODTIPOREGISTRO;

                }
                return View("Listar", objTela);
            }
            else
            {
                ModelState.AddModelError("", "Por favor conferir os dados informados");

                return View();
            }
        }

        public ActionResult Editar(string id, string identificador)
        {
            Models.Registro objRegistro = null;
            try
            {

                string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

                List<Models.TipoRegistro> objTipoRegistro = null;

                List<Models.Fonte> objFonte = null;

                List<Models.Integrantes> objIntegrantes = null;

                Task objTaskTipoRegistro = new Task(() => objTipoRegistro = RetornarTipoRegistro(id, IdentificadorUsuario));

                Task objTaskFonte = new Task(() => objFonte = RetornarFontes(id, IdentificadorUsuario));

                Task objTaskIntegrantes = new Task(() => objIntegrantes = RetornarIntegrantes(IdentificadorUsuario));

                Task objTaskRegistro = new Task(() => objRegistro = RetornarRegistro(identificador));

                objTaskTipoRegistro.Start();
                objTaskFonte.Start();
                objTaskIntegrantes.Start();
                objTaskRegistro.Start();

                Task.WaitAll(new Task[] { objTaskTipoRegistro, objTaskFonte, objTaskIntegrantes, objTaskRegistro });

                ViewBag.Parcelamento = objTipoRegistro != null && objTipoRegistro.Count > 0 && objTipoRegistro.FirstOrDefault().Parcelamento;

                List<SelectListItem> objFontesDropDown = new List<SelectListItem>();
                List<SelectListItem> objCategoriasDropDown = new List<SelectListItem>();
                List<SelectListItem> objIntegrantesDropDown = new List<SelectListItem>();

                if (objFonte != null && objFonte.Count > 0)
                {
                    foreach (var f in objFonte)
                    {
                        objFontesDropDown.Add(new SelectListItem { Text = f.Nome, Value = f.Identificador });
                    }
                }

                if (objTipoRegistro != null && objTipoRegistro.Count > 0)
                {
                    foreach (var f in objTipoRegistro)
                    {
                        objCategoriasDropDown.Add(new SelectListItem { Text = f.Nome, Value = f.Identificador });
                    }
                }

                if (objIntegrantes != null && objIntegrantes.Count > 0)
                {
                    foreach (var f in objIntegrantes)
                    {
                        objIntegrantesDropDown.Add(new SelectListItem { Text = f.Nome, Value = f.Identificador });
                    }
                }

                ViewBag.Fontes = objFontesDropDown;
                ViewBag.Categorias = objCategoriasDropDown;
                ViewBag.Integrantes = objIntegrantesDropDown;

                if (id == Enumeradores.TipoFonte.CREDITO.RecuperarValor())
                    ViewBag.Titulo = "Receita";
                else if (id == Enumeradores.TipoFonte.INVESTIMENTO.RecuperarValor())
                    ViewBag.Titulo = "Investimento";
                else
                    ViewBag.Titulo = "Despesa";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
            }

            return View(objRegistro);
        }

        [HttpPost]
        public ActionResult Editar(Models.Registro objRegistro)
        {
            if (ModelState.IsValid)
            {
                BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
                string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

                BD.AGCF_TIPOREGISTRO objTipoRegistroBD = (from BD.AGCF_TIPOREGISTRO tr in objBD.AGCF_TIPOREGISTRO
                                                          where tr.IDTIPOREGISTRO == objRegistro.IdentificadorTipoRegistro
                                                          select tr).FirstOrDefault();

                BD.AGCF_FONTES objFonte = (from BD.AGCF_FONTES f in objBD.AGCF_FONTES where f.IDFONTES == objRegistro.IdentificadorFonte select f).FirstOrDefault();


                string Mes = string.Empty;
                string Ano = string.Empty;
                bool BolParcelamento = false;

                if (objFonte != null)
                {


                    if (objFonte.BOLCARTAOCREDITO != null && objFonte.BOLCARTAOCREDITO == true && objFonte.NELDIAFECHAMENTO != null)
                    {
                        DateTime objDiaVencimento = DateTime.Now;
                        DateTime ultimoDiaDoMes = new DateTime(objRegistro.DataCompra.Year, objRegistro.DataCompra.Month, DateTime.DaysInMonth(objRegistro.DataCompra.Year, objRegistro.DataCompra.Month));

                        if (objRegistro.Parcela != null && objRegistro.Parcela > 0)
                        {
                            ultimoDiaDoMes = new DateTime(objRegistro.DataRegistro.Year, objRegistro.DataRegistro.Month, DateTime.DaysInMonth(objRegistro.DataRegistro.Year, objRegistro.DataRegistro.Month));
                            BolParcelamento = true;
                        }

                        if (ultimoDiaDoMes.Day < objFonte.NELDIAFECHAMENTO)
                        {
                            if (BolParcelamento)
                                objDiaVencimento = Convert.ToDateTime(string.Format("{0}/{1}/{2}", ultimoDiaDoMes.Day, objRegistro.DataRegistro.Month, objRegistro.DataRegistro.Year));
                            else
                                objDiaVencimento = Convert.ToDateTime(string.Format("{0}/{1}/{2}", ultimoDiaDoMes.Day, objRegistro.DataCompra.Month, objRegistro.DataCompra.Year));
                        }
                        else
                        {
                            if (BolParcelamento)
                                objDiaVencimento = Convert.ToDateTime(string.Format("{0}/{1}/{2}", objFonte.NELDIAFECHAMENTO, objRegistro.DataRegistro.Month, objRegistro.DataRegistro.Year));
                            else
                                objDiaVencimento = Convert.ToDateTime(string.Format("{0}/{1}/{2}", objFonte.NELDIAFECHAMENTO, objRegistro.DataCompra.Month, objRegistro.DataCompra.Year));
                        }

                        if (objDiaVencimento.AddDays(-10) < objRegistro.DataCompra)
                        {
                            if (objDiaVencimento < objRegistro.DataCompra)
                            {
                                if (objDiaVencimento.AddMonths(1).AddDays(-10) < objRegistro.DataCompra)
                                {
                                    objDiaVencimento = objDiaVencimento.AddMonths(2);
                                    Mes = objDiaVencimento.Month.ToString();
                                    Ano = objDiaVencimento.Year.ToString();
                                }
                                else
                                {
                                    objDiaVencimento = objDiaVencimento.AddMonths(1);
                                    Mes = objDiaVencimento.Month.ToString();
                                    Ano = objDiaVencimento.Year.ToString();
                                }
                            }
                            else
                            {
                                objDiaVencimento = objDiaVencimento.AddMonths(1);
                                Mes = objDiaVencimento.Month.ToString();
                                Ano = objDiaVencimento.Year.ToString();
                            }
                        }
                        else
                        {
                            Mes = objRegistro.DataCompra.Month.ToString();
                            Ano = objRegistro.DataCompra.Year.ToString();
                        }
                    }
                    else
                    {
                        Mes = objRegistro.DataCompra.Month.ToString();
                        Ano = objRegistro.DataCompra.Year.ToString();
                    }

                    if (Mes.Length < 2) Mes = string.Format("0{0}", Mes.ToString());
                }

                if (BolParcelamento)
                {

                    var registrosfilhos = (from BD.AGCF_REGISTRO i in objBD.AGCF_REGISTRO
                                           where (i.IDREGISTROPAI == objRegistro.IdentificadorRegistroPai || i.IDREGISTRO == objRegistro.IdentificadorRegistroPai)
                                           select new Models.Registro()
                                           {
                                               Identificador = i.IDREGISTRO,
                                               CategoriaGeral = objRegistro.CategoriaGeral,
                                               CodigoTipoRegistro = objRegistro.CodigoTipoRegistro,
                                               DataCompra = i.DTHCOMPRA,
                                               DataRegistro = i.DTHDATAREGISTRO,
                                               Descricao = objRegistro.Descricao,
                                               Fonte = objRegistro.Fonte,
                                               IdentificadorFonte = objRegistro.IdentificadorFonte,
                                               IdentificadorIntegrante = objRegistro.IdentificadorIntegrante,
                                               IdentificadorTipoRegistro = objRegistro.IdentificadorTipoRegistro,
                                               NomeIntegrante = objRegistro.NomeIntegrante,
                                               Observacao = objRegistro.Observacao,
                                               Parcela = i.NELPARCELA,
                                               ParcelaFinal = i.NELPARCELAFINAL,
                                               Pago = i.BOLPAGO != null && i.BOLPAGO == true ? true : false,
                                               TipoRegistro = objRegistro.TipoRegistro,
                                               Valor = objRegistro.Valor,
                                               ValorDecimal = objRegistro.ValorDecimal
                                           }).ToList();

                    if (registrosfilhos != null && registrosfilhos.Count() > 0)
                    {
                        foreach (var rf in registrosfilhos)
                        {
                            EditarRegistro(rf, objFonte, objTipoRegistroBD, ref objBD);
                        }
                    }

                }
                else
                {
                    BD.AGCF_REGISTRO objRegistroBD = (from BD.AGCF_REGISTRO tr in objBD.AGCF_REGISTRO
                                                      where tr.IDREGISTRO == objRegistro.Identificador
                                                      select tr).FirstOrDefault();

                    if (objRegistroBD != null)
                    {
                        objRegistroBD.DESREGISTRO = objRegistro.Descricao;
                        objRegistroBD.IDFONTES = objRegistro.IdentificadorFonte;
                        objRegistroBD.IDINTEGRANTE = objRegistro.IdentificadorIntegrante;
                        objRegistroBD.BOLQUITADO = (objRegistro.Parcela == objRegistro.ParcelaFinal);
                        objRegistroBD.BOLPAGO = objRegistro.Pago;
                        objRegistroBD.NELPARCELA = objRegistro.Parcela;
                        objRegistroBD.NELPARCELAFINAL = objRegistro.ParcelaFinal;
                        objRegistroBD.IDTIPOREGISTRO = objRegistro.IdentificadorTipoRegistro;
                        objRegistroBD.NUMVALOR = Convert.ToDecimal(objRegistro.Valor);
                        objRegistroBD.OBSDETALHES = objRegistro.Observacao;
                        objRegistroBD.CODMES = Mes;
                        objRegistroBD.CODANO = Ano;

                        if (objTipoRegistroBD.BOLPARCELAMENTO != null && Convert.ToBoolean(objTipoRegistroBD.BOLPARCELAMENTO))
                        {
                            objRegistroBD.DTHCOMPRA = objRegistro.DataCompra;
                        }
                        else
                        {
                            objRegistroBD.DTHDATAREGISTRO = objRegistro.DataCompra;
                            objRegistroBD.DTHCOMPRA = objRegistro.DataCompra;
                        }
                    }
                }

                objBD.SaveChanges();

                Models.TelaRegistros objTela = new TelaRegistros();

                objTela.Ano = Ano.ToString();
                objTela.Mes = Mes.ToString();

                if (objTipoRegistroBD != null)
                {
                    objTela.Registros = BuscarRegistros(new BuscaRegistros() { Ano = Convert.ToInt32(objTela.Ano), Mes = Convert.ToInt32(objTela.Mes), id = objTipoRegistroBD.CODTIPOREGISTRO },
                  IdentificadorUsuario, true, false);


                    if (objTipoRegistroBD.CODTIPOREGISTRO == Enumeradores.TipoFonte.CREDITO.RecuperarValor())
                        objTela.Titulo = "Receita";
                    else if (objTipoRegistroBD.CODTIPOREGISTRO == Enumeradores.TipoFonte.INVESTIMENTO.RecuperarValor())
                        objTela.Titulo = "Investimento";
                    else
                        objTela.Titulo = "Despesa";

                    if (objTipoRegistroBD.CODTIPOREGISTRO == Enumeradores.TipoFonte.CREDITO.RecuperarValor())
                        objTela.TipoRegistro = Enumeradores.TipoFonte.CREDITO.RecuperarValor();
                    else if (objTipoRegistroBD.CODTIPOREGISTRO == Enumeradores.TipoFonte.DEBITO.RecuperarValor())
                        objTela.TipoRegistro = Enumeradores.TipoFonte.DEBITO.RecuperarValor();
                    else
                        objTela.TipoRegistro = Enumeradores.TipoFonte.INVESTIMENTO.RecuperarValor();

                }
                return View("Listar", objTela);

            }
            else
            {
                ModelState.AddModelError("", "Por favor conferir os dados informados");

                return View();
            }
        }

        public void EditarRegistro(Models.Registro objRegistro, BD.AGCF_FONTES objFonte, BD.AGCF_TIPOREGISTRO objTipoRegistroBD, ref BD.IGERENCEEntities objBD)
        {
            string Mes = string.Empty;
            string Ano = string.Empty;

            if (objFonte != null)
            {


                if (objFonte.BOLCARTAOCREDITO != null && objFonte.BOLCARTAOCREDITO == true && objFonte.NELDIAFECHAMENTO != null)
                {
                    DateTime objDiaVencimento = DateTime.Now;
                    DateTime ultimoDiaDoMes = new DateTime(objRegistro.DataCompra.Year, objRegistro.DataCompra.Month, DateTime.DaysInMonth(objRegistro.DataCompra.Year, objRegistro.DataCompra.Month));
                    bool BolParcelamento = false;

                    if (objRegistro.Parcela != null && objRegistro.Parcela > 0)
                    {
                        ultimoDiaDoMes = new DateTime(objRegistro.DataRegistro.Year, objRegistro.DataRegistro.Month, DateTime.DaysInMonth(objRegistro.DataRegistro.Year, objRegistro.DataRegistro.Month));
                        BolParcelamento = true;
                    }

                    if (ultimoDiaDoMes.Day < objFonte.NELDIAFECHAMENTO)
                    {
                        if (BolParcelamento)
                            objDiaVencimento = Convert.ToDateTime(string.Format("{0}/{1}/{2}", ultimoDiaDoMes.Day, objRegistro.DataRegistro.Month, objRegistro.DataRegistro.Year));
                        else
                            objDiaVencimento = Convert.ToDateTime(string.Format("{0}/{1}/{2}", ultimoDiaDoMes.Day, objRegistro.DataCompra.Month, objRegistro.DataCompra.Year));
                    }
                    else
                    {
                        if (BolParcelamento)
                            objDiaVencimento = Convert.ToDateTime(string.Format("{0}/{1}/{2}", objFonte.NELDIAFECHAMENTO, objRegistro.DataRegistro.Month, objRegistro.DataRegistro.Year));
                        else
                            objDiaVencimento = Convert.ToDateTime(string.Format("{0}/{1}/{2}", objFonte.NELDIAFECHAMENTO, objRegistro.DataCompra.Month, objRegistro.DataCompra.Year));
                    }

                    if (BolParcelamento)
                    {
                        if (objDiaVencimento.AddDays(-10) < objRegistro.DataRegistro)
                        {
                            if (objDiaVencimento < objRegistro.DataRegistro)
                            {
                                if (objDiaVencimento.AddMonths(1).AddDays(-10) < objRegistro.DataRegistro)
                                {
                                    objDiaVencimento = objDiaVencimento.AddMonths(2);
                                    Mes = objDiaVencimento.Month.ToString();
                                    Ano = objDiaVencimento.Year.ToString();
                                }
                                else
                                {
                                    objDiaVencimento = objDiaVencimento.AddMonths(1);
                                    Mes = objDiaVencimento.Month.ToString();
                                    Ano = objDiaVencimento.Year.ToString();
                                }
                            }
                            else
                            {
                                objDiaVencimento = objDiaVencimento.AddMonths(1);
                                Mes = objDiaVencimento.Month.ToString();
                                Ano = objDiaVencimento.Year.ToString();
                            }
                        }
                        else
                        {
                            Mes = objRegistro.DataRegistro.Month.ToString();
                            Ano = objRegistro.DataRegistro.Year.ToString();
                        }
                    }
                    else
                    {
                        if (objDiaVencimento.AddDays(-10) < objRegistro.DataCompra)
                        {
                            if (objDiaVencimento < objRegistro.DataCompra)
                            {
                                if (objDiaVencimento.AddMonths(1).AddDays(-10) < objRegistro.DataCompra)
                                {
                                    objDiaVencimento = objDiaVencimento.AddMonths(2);
                                    Mes = objDiaVencimento.Month.ToString();
                                    Ano = objDiaVencimento.Year.ToString();
                                }
                                else
                                {
                                    objDiaVencimento = objDiaVencimento.AddMonths(1);
                                    Mes = objDiaVencimento.Month.ToString();
                                    Ano = objDiaVencimento.Year.ToString();
                                }
                            }
                            else
                            {
                                objDiaVencimento = objDiaVencimento.AddMonths(1);
                                Mes = objDiaVencimento.Month.ToString();
                                Ano = objDiaVencimento.Year.ToString();
                            }
                        }
                        else
                        {
                            Mes = objRegistro.DataCompra.Month.ToString();
                            Ano = objRegistro.DataCompra.Year.ToString();
                        }
                    }
                }
                else
                {
                    Mes = objRegistro.DataCompra.Month.ToString();
                    Ano = objRegistro.DataCompra.Year.ToString();
                }

                if (Mes.Length < 2) Mes = string.Format("0{0}", Mes.ToString());
            }

            BD.AGCF_REGISTRO objRegistroBD = (from BD.AGCF_REGISTRO tr in objBD.AGCF_REGISTRO
                                              where tr.IDREGISTRO == objRegistro.Identificador
                                              select tr).FirstOrDefault();

            if (objRegistroBD != null)
            {
                objRegistroBD.DESREGISTRO = objRegistro.Descricao;
                objRegistroBD.IDFONTES = objRegistro.IdentificadorFonte;
                objRegistroBD.IDINTEGRANTE = objRegistro.IdentificadorIntegrante;
                objRegistroBD.BOLQUITADO = (objRegistro.Parcela == objRegistro.ParcelaFinal);
                objRegistroBD.BOLPAGO = objRegistro.Pago;
                objRegistroBD.NELPARCELA = objRegistro.Parcela;
                objRegistroBD.NELPARCELAFINAL = objRegistro.ParcelaFinal;
                objRegistroBD.IDTIPOREGISTRO = objRegistro.IdentificadorTipoRegistro;
                objRegistroBD.NUMVALOR = Convert.ToDecimal(objRegistro.Valor);
                objRegistroBD.OBSDETALHES = objRegistro.Observacao;
                objRegistroBD.CODMES = Mes;
                objRegistroBD.CODANO = Ano;

                if (objTipoRegistroBD.BOLPARCELAMENTO != null && Convert.ToBoolean(objTipoRegistroBD.BOLPARCELAMENTO))
                {
                    objRegistroBD.DTHCOMPRA = objRegistro.DataCompra;
                }
                else
                {
                    objRegistroBD.DTHDATAREGISTRO = objRegistro.DataCompra;
                    objRegistroBD.DTHCOMPRA = objRegistro.DataCompra;
                }
            }
        }

        public ActionResult Excluir(string id)
        {
            System.Threading.Thread.Sleep(1000);
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            Models.Registro objRegistro = (from BD.AGCF_REGISTRO i in objBD.AGCF_REGISTRO
                                           where i.IDREGISTRO == id
                                           select new Models.Registro()
                                           {
                                               Identificador = i.IDREGISTRO,
                                               IdentificadorTipoRegistro = i.IDTIPOREGISTRO
                                           }).FirstOrDefault();

            return View(objRegistro);
        }

        public ActionResult PagarItem(string id)
        {
            System.Threading.Thread.Sleep(1000);
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            Models.Registro objRegistro = (from BD.AGCF_REGISTRO i in objBD.AGCF_REGISTRO
                                           where i.IDREGISTRO == id
                                           select new Models.Registro()
                                           {
                                               Identificador = i.IDREGISTRO,
                                               IdentificadorTipoRegistro = i.IDTIPOREGISTRO,
                                               Pago = i.BOLPAGO != null && i.BOLPAGO == true ? true : false
                                           }).FirstOrDefault();

            objRegistro.Pago = !objRegistro.Pago;

            return View(objRegistro);
        }

        [HttpPost]
        public ActionResult PagarItem(Models.Registro objRegistro)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            BD.AGCF_REGISTRO objRegistroBD = (from BD.AGCF_REGISTRO i in objBD.AGCF_REGISTRO where i.IDREGISTRO == objRegistro.Identificador select i).FirstOrDefault();

            BD.AGCF_TIPOREGISTRO objTipoRegistroBD = (from BD.AGCF_TIPOREGISTRO tr in objBD.AGCF_TIPOREGISTRO
                                                      where tr.IDTIPOREGISTRO == objRegistro.IdentificadorTipoRegistro
                                                      select tr).FirstOrDefault(); ;

            if (objRegistroBD != null)
            {
                objRegistroBD.BOLPAGO = objRegistro.Pago;
            }

            objBD.SaveChanges();

            Models.TelaRegistros objTela = new TelaRegistros();
            objTela.Ano = objRegistroBD.CODANO;
            objTela.Mes = objRegistroBD.CODMES;

            if (objTipoRegistroBD != null)
            {
                objTela.Registros = BuscarRegistros(new BuscaRegistros() { Ano = Convert.ToInt32(objTela.Ano), Mes = Convert.ToInt32(objTela.Mes), id = objTipoRegistroBD.CODTIPOREGISTRO },
                    IdentificadorUsuario, true, false);


                if (objTipoRegistroBD.CODTIPOREGISTRO == Enumeradores.TipoFonte.CREDITO.RecuperarValor())
                    objTela.Titulo = "Receita";
                else if (objTipoRegistroBD.CODTIPOREGISTRO == Enumeradores.TipoFonte.INVESTIMENTO.RecuperarValor())
                    objTela.Titulo = "Investimento";
                else
                    objTela.Titulo = "Despesa";

                objTela.TipoRegistro = objTipoRegistroBD.CODTIPOREGISTRO;
            }
            return View("Listar", objTela);
        }

        [HttpPost]
        public ActionResult Excluir(Models.Registro objRegistro)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            string IdentificadorUsuario = GetLogOnSessionModel().Identificador;

            BD.AGCF_REGISTRO objRegistroBD = (from BD.AGCF_REGISTRO i in objBD.AGCF_REGISTRO where i.IDREGISTRO == objRegistro.Identificador select i).FirstOrDefault();

            BD.AGCF_TIPOREGISTRO objTipoRegistroBD = (from BD.AGCF_TIPOREGISTRO tr in objBD.AGCF_TIPOREGISTRO
                                                      where tr.IDTIPOREGISTRO == objRegistro.IdentificadorTipoRegistro
                                                      select tr).FirstOrDefault(); ;

            if (objRegistroBD != null)
            {
                var registrosfilhos = from BD.AGCF_REGISTRO i in objBD.AGCF_REGISTRO where i.IDREGISTROPAI == objRegistroBD.IDREGISTRO select i;

                if (registrosfilhos != null && registrosfilhos.Count() > 0)
                    objBD.AGCF_REGISTRO.RemoveRange(registrosfilhos);



                string IdentificadorRegistroPai = objRegistroBD.IDREGISTROPAI;

                objBD.AGCF_REGISTRO.Remove(objRegistroBD);

                if (!string.IsNullOrEmpty(IdentificadorRegistroPai) && (registrosfilhos == null || registrosfilhos.Count() == 0))
                {
                    objBD.AGCF_REGISTRO.RemoveRange(from BD.AGCF_REGISTRO i in objBD.AGCF_REGISTRO where i.IDREGISTROPAI == IdentificadorRegistroPai select i);
                }


            }

            objBD.SaveChanges();

            Models.TelaRegistros objTela = new TelaRegistros();
            objTela.Ano = DateTime.Now.Year.ToString();
            objTela.Mes = DateTime.Now.Month.ToString();

            if (objTipoRegistroBD != null)
            {
                objTela.Registros = BuscarRegistros(new BuscaRegistros() { Ano = Convert.ToInt32(objTela.Ano), Mes = Convert.ToInt32(objTela.Mes), id = objTipoRegistroBD.CODTIPOREGISTRO },
                                                    IdentificadorUsuario, true, false);


                if (objTipoRegistroBD.CODTIPOREGISTRO == Enumeradores.TipoFonte.CREDITO.RecuperarValor())
                    objTela.Titulo = "Receita";
                else if (objTipoRegistroBD.CODTIPOREGISTRO == Enumeradores.TipoFonte.INVESTIMENTO.RecuperarValor())
                    objTela.Titulo = "Investimento";
                else
                    objTela.Titulo = "Despesa";

                objTela.TipoRegistro = objTipoRegistroBD.CODTIPOREGISTRO;

            }
            return View("Listar", objTela);
        }

        private List<Models.TipoRegistro> RetornarTipoRegistro(string id, string IdentificadorUsuario)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            return (from BD.AGCF_TIPOREGISTRO i in objBD.AGCF_TIPOREGISTRO
                    where i.IDUSUARIO == IdentificadorUsuario && i.CODTIPOREGISTRO == id
                    select new Models.TipoRegistro()
                    {
                        Identificador = i.IDTIPOREGISTRO,
                        Nome = i.DESTIPOREGISTRO
                    }).ToList();
        }

        private List<Models.Fonte> RetornarFontes(string id, string IdentificadorUsuario)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            return (from BD.AGCF_FONTES i in objBD.AGCF_FONTES
                    where i.IDUSUARIO == IdentificadorUsuario && i.CODTIPOFONTE == id
                    select new Models.Fonte()
                    {
                        Identificador = i.IDFONTES,
                        Nome = i.DESNOME,
                        DiaFechamento = i.NELDIAFECHAMENTO,
                        TipoValor = i.CODTIPOFONTE,
                    }).ToList();
        }

        private List<Models.Integrantes> RetornarIntegrantes(string IdentificadorUsuario)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            return (from BD.AGCF_INTEGRANTES i in objBD.AGCF_INTEGRANTES
                    where i.IDUSUARIO == IdentificadorUsuario
                    select new Models.Integrantes()
                    {
                        Identificador = i.IDINTEGRANTE,
                        Nome = i.DESNOME
                    }).ToList();
        }

        private Models.Registro RetornarRegistro(string identificador)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();

            return (from BD.AGCF_REGISTRO i in objBD.AGCF_REGISTRO
                    join BD.AGCF_TIPOREGISTRO tr in objBD.AGCF_TIPOREGISTRO on i.IDTIPOREGISTRO equals tr.IDTIPOREGISTRO
                    join BD.AGCF_FONTES f in objBD.AGCF_FONTES on i.IDFONTES equals f.IDFONTES
                    where i.IDREGISTRO == identificador
                    select new Models.Registro()
                    {
                        Identificador = i.IDTIPOREGISTRO,
                        Descricao = i.DESREGISTRO,
                        IdentificadorFonte = f.IDFONTES,
                        Fonte = f.DESNOME,
                        DataCompra = i.DTHCOMPRA,
                        DataRegistro = i.DTHDATAREGISTRO,
                        Quitado = i.BOLQUITADO,
                        Observacao = i.OBSDETALHES,
                        Parcela = i.NELPARCELA,
                        ParcelaFinal = i.NELPARCELAFINAL,
                        IdentificadorRegistroPai = i.IDREGISTROPAI,
                        Pago = i.BOLPAGO != null && i.BOLPAGO == true ? true : false,
                        IdentificadorTipoRegistro = tr.IDTIPOREGISTRO,
                        TipoRegistro = tr.DESTIPOREGISTRO,
                        NomeIntegrante = (from BD.AGCF_INTEGRANTES it in objBD.AGCF_INTEGRANTES where it.IDINTEGRANTE == i.IDINTEGRANTE select it.DESNOME).FirstOrDefault(),
                        IdentificadorIntegrante = i.IDINTEGRANTE,
                        ValorDecimal = i.NUMVALOR
                    }).FirstOrDefault();
        }
        [HttpPost]
        public bool BuscarTipoRegistro(string id)
        {
            BD.IGERENCEEntities objBD = new BD.IGERENCEEntities();
            DadosTipoRegistro objTipoRegistro = null;

            try
            {
                return (from BD.AGCF_TIPOREGISTRO i in objBD.AGCF_TIPOREGISTRO
                        where i.IDTIPOREGISTRO == id && i.BOLPARCELAMENTO != null && i.BOLPARCELAMENTO == true
                        select i).Count() > 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }

        public class DadosTipoRegistro
        {
            public bool Parcelamento { get; set; }
        }
    }
}