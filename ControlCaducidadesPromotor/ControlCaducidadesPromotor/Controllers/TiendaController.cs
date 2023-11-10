using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using ControlCaducidadesPromotor.Basicos;

namespace ControlCaducidadesPromotor.Controllers
{
    public class TiendaController : Controller
    {
        // GET: Tienda
        [Authorize]
        public ActionResult Index()
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
                    x.Wait(); //x.Result tiene el resultado
                    ModelState.AddModelError(string.Empty, x.Result);
                }
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return View("Index", respuesta);
        }


        [Authorize]
        public ActionResult MostrarFormAltaTienda()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return View("MostrarFormAltaTienda", new TiendaViewModel());
        }


        [Authorize]
        [System.Web.Mvc.HttpPost]
        public ActionResult CrearTienda(TiendaViewModel tiendaViewModel)
        {
            if (ModelState.IsValid)
            {
                UsuarioViewModel usuarioInfoViewModel = LLamarApiBuscarUsuarioXUsuario(User.Identity.Name);

                if (usuarioInfoViewModel != null)
                {
                    tiendaViewModel.IdUsuarioAlta = usuarioInfoViewModel.Id;
                    tiendaViewModel.IdUsuarioModifico = usuarioInfoViewModel.Id;

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:51339/");

                        //HTTP POST
                        var postTask = client.PostAsJsonAsync<TiendaViewModel>("api/TiendaAPI/Post_CrearTienda", tiendaViewModel);
                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index");
                        }

                        else
                        {
                            //https://www.thetopsites.net/article/53008477.shtml
                            var x = result.Content.ReadAsStringAsync();
                            x.Wait(); //x.Result tiene el resultado

                            ModelState.AddModelError(string.Empty, x.Result);
                            TiendaViewModel tiendaVM = new TiendaViewModel();
                            InicializarTiendaViewModelConDatosQueLLegaronAlControlador(tiendaVM);
                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Response.Cache.SetNoStore();
                            return (View("MostrarFormAltaTienda", tiendaVM));
                        }
                    }
                }

                else
                {
                    ModelState.AddModelError("", "No se pudo localizar a usuario " + User.Identity.Name);
                    TiendaViewModel tiendaVM = new TiendaViewModel();
                    InicializarTiendaViewModelConDatosQueLLegaronAlControlador(tiendaVM);
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Cache.SetNoStore();
                    return (View("MostrarFormAltaTienda", tiendaVM));
                }              
            }

            else
            {
                TiendaViewModel tiendaVM = new TiendaViewModel();
                InicializarTiendaViewModelConDatosQueLLegaronAlControlador(tiendaVM);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                return (View("MostrarFormAltaTienda", tiendaVM));
            }
        }



        [Authorize]
        [System.Web.Mvc.HttpGet]
        public ActionResult MostrarFormModificarTienda()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return (View("MostrarFormModificarTienda"));
        }


        [Authorize]
        public JsonResult BuscarTiendaXNombreDeTienda(string nombreDeTienda)
        {
            TiendaViewModel tiendaBuscadaViewModel = new TiendaViewModel();
            UsuarioViewModel usuarioViewModel = LLamarApiBuscarUsuarioXUsuario(User.Identity.Name);
            TiendaViewModel auxTiendaViewModel = new TiendaViewModel();

            if(ModelState.IsValid)
            {
                if (usuarioViewModel != null)
                {
                    //Debo de eliminar caracteres especiales antes de llamar al API, puesto que si lo envio como esta, corro el riesgo de que el navegador no acepte los caracteres al recibir el URL.
                    auxTiendaViewModel.Nombre = nombreDeTienda;
                    //Falto inicializar supmza, manzana, lote, calle, nombre ?????
                    auxTiendaViewModel.AjustarAtributosSupmzaMzaLteCalleNombre();
                    bool nombreTieneLongitudPermitida = auxTiendaViewModel.Nombre.Length > 0;

                    if (auxTiendaViewModel.ContieneCaracteresPermitidos(auxTiendaViewModel.Nombre) && nombreTieneLongitudPermitida)
                    {
                        using (var cliente = new HttpClient())
                        {
                            cliente.BaseAddress = new Uri("http://localhost:51339/");
                            var responseTask = cliente.GetAsync("api/TiendaAPI/Get_BuscarTiendaDeUsuarioXNombre?nombreTienda=" + auxTiendaViewModel.Nombre + "&idUsuarioOperador=" + usuarioViewModel.Id.ToString());
                            responseTask.Wait();

                            var result = responseTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var readTask = result.Content.ReadAsAsync<TiendaViewModel>();
                                readTask.Wait();

                                tiendaBuscadaViewModel = readTask.Result;
                                return Json(new { success = true, laRespuesta = tiendaBuscadaViewModel }, JsonRequestBehavior.AllowGet);
                            }
                            else //web api sent error response 
                            {
                                var x = result.Content.ReadAsStringAsync();
                                x.Wait(); //x.Result tiene el resultado
                                //ModelState.AddModelError(string.Empty, x.Result);   //Falta agregar el ModelState.Summary en la GUI
                                return Json(new { success = false, responseText = x.Result }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    else { return Json(new { success = false, responseText = "Verifique el formato de entrada" }, JsonRequestBehavior.AllowGet);
                    }
                }

                else { return Json(new { success = false, responseText = "Verifique el nombre de usuario y su estado" }, JsonRequestBehavior.AllowGet);
                } 
            }

            else
            {
                //La GUI evalua el objeto json regresado, lo interpreta
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.Cache.SetNoStore();
                return (Json(new { success = false, responseText = "Fallo en el modelBinder TiendaController.BuscarTiendaXNombreDeTienda" }, JsonRequestBehavior.AllowGet));
            }
        }


        [Authorize]
        public JsonResult ActualizarTiendaDeUsuario(TiendaViewModel tiendaViewModel)
        {
            string respuesta = "";
            if (ModelState.IsValid)
            {
                UsuarioViewModel usuarioViewModel = LLamarApiBuscarUsuarioXUsuario(User.Identity.Name);
                tiendaViewModel.IdUsuarioAlta = usuarioViewModel.Id;
                tiendaViewModel.IdUsuarioModifico = usuarioViewModel.Id;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:51339/");
                    
                    //HTTP PUT
                    var putTask = client.PutAsJsonAsync<TiendaViewModel>("api/TiendaAPI/Put_ActualizarTiendaDeUsuario", tiendaViewModel);
                    putTask.Wait();
                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        respuesta = "ok";
                    }

                    else
                    {
                        var x = result.Content.ReadAsStringAsync();
                        x.Wait(); // x.Result tiene el resultado
                        respuesta = x.Result;
                    }
                }
            }

            else
            { respuesta="No se pudo enlazar los datos para actualizar"; }

            return (Json(respuesta, JsonRequestBehavior.AllowGet));
        }



        /// <summary>
        /// Se invoca este metodo mediante AJAX en cliente, regresa un objeto JSON con las tiendas del usuario operador
        /// </summary>
        /// <returns>Json</returns>
        [Authorize]
        public JsonResult RecuperarTiendasDeUsuario()
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

                    return Json(new { success = true, elResultado = respuesta }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    var x = result.Content.ReadAsStringAsync();
                    x.Wait();
                    return Json(new { success = false, elResultado = x.Result }, JsonRequestBehavior.AllowGet);
                }
            }

            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetNoStore();
            //return (Json(respuesta, JsonRequestBehavior.AllowGet));
        }



        [Authorize]
        public JsonResult RecuperarProductosDeTiendaXId(TiendaViewModel tiendaViewModel)
        {
            TiendaYProductosViewModel respuesta = new TiendaYProductosViewModel();

            if (ModelState.IsValid)
            {
                UsuarioViewModel usuarioViewModel = LLamarApiBuscarUsuarioXUsuario(User.Identity.Name);
                tiendaViewModel.IdUsuarioAlta = usuarioViewModel.Id;
                tiendaViewModel.IdUsuarioModifico = usuarioViewModel.Id;

                using (var cliente = new HttpClient())
                {
                    cliente.BaseAddress = new Uri("http://localhost:51339/");
                    var responseTask = cliente.PostAsJsonAsync<TiendaViewModel>("api/TiendaAPI/Post_RecuperarProductosDeTiendaXId", tiendaViewModel);

                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<TiendaYProductosViewModel>();
                        readTask.Wait();

                        respuesta = readTask.Result;
                        return Json(new { success = true, laRespuesta = respuesta }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var x = result.Content.ReadAsStringAsync();
                        x.Wait(); //x.Result tiene el resultado
                        //ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        return Json(new { success=false, laRespuesta = x.Result }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            else
            {
               //Response.Cache.SetCacheability(HttpCacheability.NoCache); mantener comentada esta linea
                //Response.Cache.SetNoStore(); mantener comentada esta linea
                return Json(new { success = false, laRespuesta = "Fallo en el model binder TiendaController.RecuperarProductosDeTiendaXId" }, JsonRequestBehavior.AllowGet);
            }
        }


        
        /// <summary>
        /// Agrega productos a la tienda desde una list, el id del usuario lo asigna esta función
        /// </summary>
        /// <param name="tiendaYProductosColecc"></param>
        /// <returns></returns>
        [Authorize]
        [System.Web.Mvc.HttpPost]
        public JsonResult AgregarProductosATienda(TiendaYProductosViewModel tiendaYProductosColecc)
        {
            string respuesta="";
            
            if(ModelState.IsValid)
            {
                //crear api y llamarla
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:51339/");

                    AgregarUserIdAObjeto(tiendaYProductosColecc);

                    //HTTP POST
                    //var postTask = client.PostAsJsonAsync<List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM>>("api/TiendaAPI/Post_AgregarProductosATienda", tiendaYProductosColecc);
                    var postTask = client.PostAsJsonAsync<TiendaYProductosViewModel>("api/TiendaAPI/Post_AgregarProductosATienda", tiendaYProductosColecc);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        respuesta = "ok";
                    }

                    else
                    {
                        //https://www.thetopsites.net/article/53008477.shtml
                        var x = result.Content.ReadAsStringAsync();
                        x.Wait(); //x.Result tiene el resultado

                        ModelState.AddModelError(string.Empty, x.Result);
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.Cache.SetNoStore();
                        respuesta = x.Result;
                    }
                }
            }

            else
            {
                respuesta = "Fallo en el model binder en Tienda.AgregarProductosATienda";
            }
            
            return (Json(respuesta, JsonRequestBehavior.AllowGet));
        }





        //---------------------------Methods
        private UsuarioViewModel LLamarApiBuscarUsuarioXUsuario(string usuario)
        {
            UsuarioViewModel usuarioViewModel =  null; 

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


        private void InicializarTiendaViewModelConDatosQueLLegaronAlControlador(TiendaViewModel tiendaVM)
        {
            tiendaVM.Nombre = ModelState["tiendaViewModel.Nombre"].Value.AttemptedValue;
            tiendaVM.Supmza = ModelState["tiendaViewModel.Supmza"].Value.AttemptedValue;
            tiendaVM.Manzana = ModelState["tiendaViewModel.Manzana"].Value.AttemptedValue;
            tiendaVM.Lote = ModelState["tiendaViewModel.Lote"].Value.AttemptedValue;
            tiendaVM.Calle = ModelState["tiendaViewModel.Calle"].Value.AttemptedValue;
        }


        private void AgregarUserIdAObjeto(TiendaYProductosViewModel tiendaYProductosColecc)
        {
            UsuarioViewModel usuarioViewModel = LLamarApiBuscarUsuarioXUsuario(User.Identity.Name);
            tiendaYProductosColecc.InfoTienda.IdUsuarioAlta = usuarioViewModel.Id;
        }

    }
}