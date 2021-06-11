using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ControlCaducidadesPromotor.Models;
using ControlCaducidadesPromotor.Basicos;

namespace ControlCaducidadesPromotor.Controllers.MisApis
{
    public class CaducidadAPIController : ApiController
    {
        [System.Web.Http.HttpPost]
        public IHttpActionResult Post_InsertarNuevaCaducidad(List<TiendaJoinCaducidadesViewModel> coleccionViewModel)
        {
            string respuesta = "";
            if (ModelState.IsValid)
            {
                TiendaLN tiendaLN = new TiendaLN();
                respuesta = tiendaLN.Post_AgregarNuevaCaducidad(coleccionViewModel);

                if (respuesta.Contains("ok"))
                {
                    return (Ok());
                }

                else
                {
                    return (BadRequest(respuesta));
                }
            }

            else
            {
                return (BadRequest("Fallo en el model binder CaducidadAPIController.Post_InsertarNuevaCaducidad"));
            }
        }
    }
}