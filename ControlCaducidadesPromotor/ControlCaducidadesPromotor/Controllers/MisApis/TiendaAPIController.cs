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
    public class TiendaAPIController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get_MostrarTodasTiendasDeUsuario(string usuario)
        {
            TiendaLN tiendaLN = new TiendaLN();

            IList<TiendaViewModel> res = tiendaLN.MostrarTodasTiendasDeUsuario(usuario);

            return (Ok(res));
        }


        [HttpPost]
        public IHttpActionResult Post_CrearTienda(TiendaViewModel tiendaViewModel)
        {
            if(ModelState.IsValid)
            {
                RelojServidor relojServidor = new RelojServidor();
                relojServidor.ColocarMismaFechaHoraEnCamposFechaAltaYFechaModificacion(tiendaViewModel);
                TiendaLN tiendaLN = new TiendaLN();

                string respuesta = tiendaLN.PostCrearTienda(tiendaViewModel);
                if(respuesta.Contains("ok"))
                {
                    return (Ok(respuesta));
                }

                else
                {
                    return (BadRequest(respuesta));
                }
            }

            else
            {
                return (BadRequest("Fallo el model binder en TiendaAPIController.Put_CrearTienda"));
            }
        }
    }
}
