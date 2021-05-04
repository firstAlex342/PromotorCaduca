using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlCaducidadesPromotor.Controllers
{
    public class CaducidadController : Controller
    {
        // GET: Caducidad
        [Authorize]
        public ActionResult AgregarCaducidad()
        {
            return View("AgregarCaducidad");
        }
    }
}