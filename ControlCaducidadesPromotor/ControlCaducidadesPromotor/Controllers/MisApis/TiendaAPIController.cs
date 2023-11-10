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

            try
            {
                IList<TiendaViewModel> res = tiendaLN.MostrarTodasTiendasDeUsuario(usuario);

                return (Ok(res));
            }

            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [System.Web.Http.HttpPost]
        public IHttpActionResult Post_RecuperarProductosDeTiendaXId(TiendaViewModel tiendaViewModel)
        {
            try
            {
                if(ModelState.IsValid)
                {                    
                    TiendaLN tiendaLN = new TiendaLN();
                    TiendaYProductosViewModel res;                   
                    res = tiendaLN.Get_RecuperarProductosDeTienda(tiendaViewModel);
                    return (Ok(res));
                }

                else
                {
                    return (BadRequest("Fallo en el model binder TiendaAPIController.Post_RecuperarProductosDeTiendaXId"));
                }
            }

            catch(Exception ex)
            {
                return (BadRequest(ex.Message));
            }
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
            try {
                if (ModelState.IsValid)
                {
                    TiendaViewModel tiendaViewModel = new TiendaViewModel();
                    tiendaViewModel.Nombre = nombreTienda;
                    tiendaViewModel.IdUsuarioAlta = idUsuarioOperador;
                    TiendaLN tiendaLN = new TiendaLN();
                    TiendaViewModel tiendaBuscadaViewModel = tiendaLN.Get_BuscarTiendaDeUsuarioXNombre(tiendaViewModel);
                    if (tiendaBuscadaViewModel != null)
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

            catch(Exception ex)
            {
                return (  BadRequest(ex.Message)  );
            }
        }



        [System.Web.Http.HttpPut]
        public IHttpActionResult Put_ActualizarTiendaDeUsuario(TiendaViewModel tiendaViewModel)
        {
            try{
                if (ModelState.IsValid)
                {
                    TiendaLN tiendaLN = new TiendaLN();
                    string res = tiendaLN.Put_ActualizarTiendaDeUsuario(tiendaViewModel);

                    if (res.Contains("ok"))
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

            catch(Exception ex)
            {
                return ( BadRequest(ex.Message)  );
            }
        }



        [System.Web.Http.HttpPost]
        public IHttpActionResult Post_AgregarProductosATienda(TiendaYProductosViewModel coleccion)
        {
            try {
                if (ModelState.IsValid)
                {
                    TiendaLN tiendaLN = new TiendaLN();
                    string respuesta = tiendaLN.Post_AgregarProductosATienda(coleccion);

                    if (respuesta.Contains("ok"))
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

            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
