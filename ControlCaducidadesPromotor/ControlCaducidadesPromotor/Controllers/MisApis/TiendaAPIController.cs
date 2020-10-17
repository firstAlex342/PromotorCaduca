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



        [HttpGet]
        public IHttpActionResult Get_BuscarTiendaDeUsuarioXNombre(string nombreTienda, int idUsuarioOperador)
        {
            if(ModelState.IsValid)
            {
                TiendaViewModel tiendaViewModel = new TiendaViewModel();
                tiendaViewModel.Nombre = nombreTienda;
                tiendaViewModel.IdUsuarioAlta = idUsuarioOperador;
                TiendaLN tiendaLN = new TiendaLN();
                TiendaViewModel tiendaBuscadaViewModel = tiendaLN.Get_BuscarTiendaDeUsuarioXNombre(tiendaViewModel); 
                if( tiendaBuscadaViewModel != null)
                {
                    return (Ok(tiendaBuscadaViewModel));
                }

                else
                {
                    return (BadRequest("No se encontro la tienda buscada"));
                }
            }

            else
            {
                return (BadRequest("Fallo en el model binder TiendaAPIController.Get_BuscarTiendaDeUsuarioXNombre"));
            }
        }



        [HttpPut]
        public IHttpActionResult Put_ActualizarTiendaDeUsuario(TiendaViewModel tiendaViewModel)
        {
            if(ModelState.IsValid)
            {
                TiendaLN tiendaLN = new TiendaLN();
                string res = tiendaLN.Put_ActualizarTiendaDeUsuario(tiendaViewModel);

                if(res.Contains("ok"))
                {
                    return (Ok());
                }

                else
                {
                    return (BadRequest(res));
                }
            }

            else
            {
                return (BadRequest("Fallo el model binder en TiendaAPIController.Put_ActualizarTiendaDeUsuario"));
            }
        }
    }
}
