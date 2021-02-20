using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmgSistemas.ControleFinanceiro.Controllers.Shared
{
    public class BaseController : Controller
    {
        // GET: Base
        public const string SystemMessage = "MY_DIALOG";

        protected void ShowMessage(string htmlContent, string htmlTitle = "Mensagem do Sistema", Classes.MyDialog.DialogType type = Classes.MyDialog.DialogType.Info)
        {
            this.ShowMessage(new Classes.MyDialog { Title = htmlTitle, Content = htmlContent, @Type = type });
        }

        protected void ShowMessage(Classes.MyDialog dialog)
        {
            this.TempData[SystemMessage] = dialog.ToString();
        }
    }
}