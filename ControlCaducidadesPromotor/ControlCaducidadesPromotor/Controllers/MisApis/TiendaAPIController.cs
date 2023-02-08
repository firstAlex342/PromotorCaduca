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
        /// <summary>
        /// Recupera todas las tiendas ACTIVAS de un usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public IHttpActionResult Get_MostrarTodasTiendasDeUsuario(string usuario)
        {
            TiendaLN tiendaLN = new TiendaLN();

            IList<TiendaViewModel> res = tiendaLN.MostrarTodasTiendasDeUsuario(usuario);

            return (Ok(res));
        }


        [System.Web.Http.HttpGet]
        public IHttpActionResult Get_RecuperarProductosDeTiendaXId(int idTienda, string usuario)
        {
            TiendaLN tiendaLN = new TiendaLN();
            IList<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM> res;
            res = tiendaLN.Get_RecuperarProductosDeTienda(idTienda, usuario);

            return (Ok(res));
        }


        [System.Web.Http.HttpPost]
        public IHttpActionResult Post_CrearTienda(TiendaViewModel tiendaViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RelojServidor relojServidor = new RelojServidor();
                    relojServidor.ColocarMismaFechaHoraEnCamposFechaAltaYFechaModificacion(tiendaViewModel);
                    TiendaLN tiendaLN = new TiendaLN();

                    string respuesta = tiendaLN.PostCrearTienda(tiendaViewModel);                    
                    if (respuesta.Contains("ok"))
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

            catch(Exception ex)
            {
                return (BadRequest(ex.Message));
            }
        }



        [System.Web.Http.HttpGet]
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



        [System.Web.Http.HttpPut]
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


        [System.Web.Http.HttpGet]
        public IHttpActionResult Get_RecuperarProductosNoPertenecenATienda(int idTienda, string usuario, int dummy)
        { //El parametro dummy solo es para que la firma de este metodo sea diferente a cualquier otro metodo de este mismo
            //controlador, ya que si 2 metodos de este controlador tiene la misma firma y son del mismo tipo de solocitud (get,put,delete, etc)
            //falla el controlador
            if (ModelState.IsValid)
            {
                List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> resViewModel = null;
                TiendaLN tiendaLN = new TiendaLN();

                resViewModel = tiendaLN.Get_RecuperarProductosNoPertenecenATienda(idTienda, usuario);
                if (resViewModel != null)
                {
                    return (Ok(resViewModel));
                }

                else
                {
                    return (BadRequest("No fue posible obtener la lista de productos buscada"));
                }
            }

            else
            {
                return BadRequest("Fallo en model binder en TiendaAPIController.Get_RecuperarProductosNoPertenecenATienda");
            }
        }


        [System.Web.Http.HttpPost]
        public IHttpActionResult Post_AgregarProductosATienda(List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM> coleccion)
        {
            if (ModelState.IsValid)
            {
                TiendaLN tiendaLN = new TiendaLN();
                string respuesta = tiendaLN.Post_AgregarProductosATienda(coleccion);

                if(respuesta.Contains("ok"))
                {
                    return (Ok(respuesta));
                }

                else
                {
                    return BadRequest(respuesta);
                }
                
            }

            else
            {
                return BadRequest("Fallo en el model binder TiendaAPIController.Post_AgregarProductosATienda");
            }
        }
    }
}
