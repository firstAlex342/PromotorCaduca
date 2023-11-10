using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using ControlCaducidadesPromotor.Basicos;

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

        [Authorize]
        public JsonResult InsertarNuevaCaducidad(List<TiendaJoinCaducidadesViewModel> coleccionViewModel)
        {
            string respuesta = "No se enlazo";
            if(ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:51339/");

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<List<TiendaJoinCaducidadesViewModel>>("api/CaducidadAPI/Post_InsertarNuevaCaducidad", coleccionViewModel);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.Cache.SetNoStore();
                        return Json("Haz ingresado una nueva caducidad", JsonRequestBehavior.AllowGet);
                    }

                    else
                    {
                        //https://www.thetopsites.net/article/53008477.shtml
                        var x = result.Content.ReadAsStringAsync();
                        x.Wait(); //x.Result tiene el resultado

                        ModelState.AddModelError(string.Empty, x.Result);
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.Cache.SetNoStore();
                        return Json(x.Result, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            else
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        public ActionResult VerCaducidad()
        {
            List<TiendaViewModel> respuesta = new List<TiendaViewModel>();
            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri("http://localhost:51339/");
                var responseTask = cliente.GetAsync("api/TiendaAPI/Get_MostrarTodasTiendasDeUsuario?usuario=" + User.Identity.Name);

                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<TiendaViewModel>>();
                    readTask.Wait();

                    respuesta = readTask.Result;
                }
                else
                {
                    var x = result.Content.ReadAsStringAsync();
                    x.Wait();
                    ModelState.AddModelError(string.Empty, x.Result);
                }
            }

            return View("VerCaducidad",respuesta);
        }


        [Authorize]
        public JsonResult BuscarCaducidad(ParametroBuscarCaducidadViewModel parametroBuscarCaducidadViewModel)
        {
            List<CaducaJoinProductoJoin__JoinPeriodoJoinPeriodoConUnidadJoinUnidadMedidaViewModel> respuestaColeccion = new List<CaducaJoinProductoJoin__JoinPeriodoJoinPeriodoConUnidadJoinUnidadMedidaViewModel>();

            if (ModelState.IsValid)
            {
                UsuarioViewModel usuarioInfoViewModel = LLamarApiBuscarUsuarioXUsuario(User.Identity.Name);
                
                if (usuarioInfoViewModel != null)
                {
                    parametroBuscarCaducidadViewModel.IdUsuarioAlta = usuarioInfoViewModel.Id;
                    parametroBuscarCaducidadViewModel.IdUsuarioModifico = usuarioInfoViewModel.Id;

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:51339/");

                        //HTTP POST
                        var postTask = client.PostAsJsonAsync<ParametroBuscarCaducidadViewModel>("api/CaducidadAPI/Post_BuscarCaducidad", parametroBuscarCaducidadViewModel);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<List<CaducaJoinProductoJoin__JoinPeriodoJoinPeriodoConUnidadJoinUnidadMedidaViewModel>>();
                            readTask.Wait();
                            respuestaColeccion = readTask.Result;
                            
                            return Json(new { success = true, laRespuesta = respuestaColeccion }, JsonRequestBehavior.AllowGet);
                        }

                        else
                        {
                            //https://www.thetopsites.net/article/53008477.shtml
                            var x = result.Content.ReadAsStringAsync();  
                            x.Wait(); //x.Result tiene el resultado

                            //ModelState.AddModelError(string.Empty, x.Result);  
                            //TiendaViewModel tiendaVM = new TiendaViewModel();
                            //InicializarTiendaViewModelConDatosQueLLegaronAlControlador(tiendaVM);
                            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            //Response.Cache.SetNoStore();
                            //return (View("MostrarFormAltaTienda", tiendaVM));

                            // return Json("Hola mundo !!", JsonRequestBehavior.AllowGet); 
                            return Json(new { success = false, responseText = x.Result }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                else
                {                    
                    return Json(new { success = false, responseText = "algo anda mal!!, el usuario no se encontro" }, JsonRequestBehavior.AllowGet);
                }
            }

            else
            {      //regresar mensaje              
                return Json(new { success = false, responseText = "algo anda mal!!, no se pudo enlazar el modelBinder en CaducidadController.BuscarCaducidad" }, JsonRequestBehavior.AllowGet);
            }
            
        }



        //--------------methods
        /// <summary>
        /// Regresa los detalles de un usuario de la tabla Usuario. 
        /// Si Activo = 0 o Si Activo = 1, se incluyen en el valor de retorno.
        /// </summary>
        /// <param name="usuario">Nombre de usuario que se desea localizar. El usuario puede estar activo o inactivo</param>
        /// <returns></returns>
        private UsuarioViewModel LLamarApiBuscarUsuarioXUsuario(string usuario)
        {
            UsuarioViewModel usuarioViewModel = null;

            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri("http://localhost:51339/");
                var responseTask = cliente.GetAsync("api/UsuarioAPI/Get_UsuarioXUsuario?usuario=" + User.Identity.Name);

                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<UsuarioViewModel>();
                    readTask.Wait();

                    usuarioViewModel = readTask.Result;
                }
            }

            return (usuarioViewModel);
        }
    }
}