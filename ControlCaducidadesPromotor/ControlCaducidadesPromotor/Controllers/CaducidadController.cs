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
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
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
                            return Json(respuestaColeccion, JsonRequestBehavior.AllowGet);
                        }

                        else
                        {
                            //https://www.thetopsites.net/article/53008477.shtml
                            var x = result.Content.ReadAsStringAsync();
                            x.Wait(); //x.Result tiene el resultado

                            ModelState.AddModelError(string.Empty, x.Result);
                            //TiendaViewModel tiendaVM = new TiendaViewModel();
                            //InicializarTiendaViewModelConDatosQueLLegaronAlControlador(tiendaVM);
                            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            //Response.Cache.SetNoStore();
                            //return (View("MostrarFormAltaTienda", tiendaVM));

                            return Json("Hola mundo !!", JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                else
                {
                    return Json("algo anda mal !!", JsonRequestBehavior.AllowGet);
                }
            }

            else
            {      //regresar mensaje
                return Json("algo anda mal !!", JsonRequestBehavior.AllowGet);
            }
            
        }



        //--------------methods
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