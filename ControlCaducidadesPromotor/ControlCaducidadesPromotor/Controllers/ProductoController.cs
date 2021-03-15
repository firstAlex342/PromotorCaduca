using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using ControlCaducidadesPromotor.Basicos;

namespace WebApplication3.Controllers
{
    public class ProductoController : Controller
    {
        // GET: Raiz
        [Authorize]
        public ActionResult Index()
        {
            List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> respuesta = null;
            using (var cliente = new HttpClient())
            {
                UsuarioViewModel usuarioViewModel = LLamarApiBuscarUsuarioXUsuario(User.Identity.Name);
                if(usuarioViewModel != null)
                {
                    cliente.BaseAddress = new Uri("http://localhost:51339/");
                    var responseTask = cliente.GetAsync("api/ProductoAPI/MostrarTodosRegistradosDeOperador?idUsuarioOperador=" + usuarioViewModel.Id.ToString());
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel>>();
                        readTask.Wait();

                        respuesta = readTask.Result;
                    }
                    else
                    {
                        respuesta = new List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel>();
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "No se pudo encontrar detalles de usuario " + User.Identity.Name);
                }
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return View("Index",respuesta);
        }



        [Authorize]
        public ActionResult MostrarFormAltaProducto()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return View("MostrarFormAltaProducto", new ProductoViewModel());
        }


        [Authorize]
        [HttpPost]
        public ActionResult AniadirProducto(ProductoViewModel productoViewModel)
        {
            if (ModelState.IsValid)
            {
                UsuarioViewModel usuarioViewModel = LLamarApiBuscarUsuarioXUsuario(User.Identity.Name);
                
                if(usuarioViewModel != null)
                {
                    productoViewModel.IdUsuarioAlta = usuarioViewModel.Id;
                    productoViewModel.IdUsuarioModifico = usuarioViewModel.Id;

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:51339/");

                        //HTTP POST
                        var postTask = client.PostAsJsonAsync<ProductoViewModel>("api/ProductoAPI/CrearProducto", productoViewModel);

                        postTask.Wait();

                        var result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index");
                        }

                        else
                        {   //https://www.thetopsites.net/article/53008477.shtml
                            var x = result.Content.ReadAsStringAsync();
                            x.Wait(); //x.Result tiene el resultado

                            ModelState.AddModelError(string.Empty, x.Result);
                            ProductoViewModel productoVM = new ProductoViewModel();
                            InicializarProductoViewModelConValoresQueLLegaronAlControlador(productoVM);
                            EvitarAlmacenamientoDeRespuestaEnCacheNavegador();
                            return (View("MostrarFormAltaProducto", productoVM));
                        }
                    }
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "No se encontro el usuario " + User.Identity.Name);
                    ProductoViewModel productoVM = new ProductoViewModel();
                    InicializarProductoViewModelConValoresQueLLegaronAlControlador(productoVM);
                    EvitarAlmacenamientoDeRespuestaEnCacheNavegador();
                    return (View("MostrarFormAltaProducto", productoVM));
                }
            }

            else
            {
                ProductoViewModel productoVM = new ProductoViewModel();
                InicializarProductoViewModelConValoresQueLLegaronAlControlador(productoVM);
                EvitarAlmacenamientoDeRespuestaEnCacheNavegador();
                return (View("MostrarFormAltaProducto", productoVM));
            }
        }



        [Authorize]
        public ActionResult MostrarFormModificarProducto()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return View("MostrarFormModificarProducto");
        }


        [Authorize]
        public JsonResult BuscarProductoxCodigoBarras(string codigoBarrasBuscado)
        {
            ProductoJoinProductoConDetallesJoinDetalleProductoViewModel respuesta = new ProductoJoinProductoConDetallesJoinDetalleProductoViewModel();
            UsuarioViewModel usuarioViewModel = LLamarApiBuscarUsuarioXUsuario(User.Identity.Name);


            if (ModelState.IsValid)
            {
                if(usuarioViewModel != null)
                {
                    using (var cliente = new HttpClient())
                    {
                        cliente.BaseAddress = new Uri("http://localhost:51339/");
                        var responseTask = cliente.GetAsync("api/ProductoAPI/BuscarProductoxCodigoBarras?codigoBarrasBuscado=" + codigoBarrasBuscado + "&idUsuarioOperador=" + usuarioViewModel.Id.ToString());
                        responseTask.Wait();

                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel>();
                            readTask.Wait();

                            respuesta = readTask.Result;
                        }
                        else //web api sent error response 
                        {
                            var x = result.Content.ReadAsStringAsync();
                            x.Wait(); //x.Result tiene el resultado
                            ModelState.AddModelError(string.Empty, x.Result);
                        }
                    }
                }
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }



        [Authorize]
        public JsonResult ModificarProducto(ProductoJoinProductoConDetallesJoinDetalleProductoViewModel pJoinViewModel)
        {
            string respuesta = "";
            UsuarioViewModel usuarioViewModel = LLamarApiBuscarUsuarioXUsuario(User.Identity.Name);

            pJoinViewModel.DetalleProducto_IdUsuarioAlta = usuarioViewModel.Id;
            pJoinViewModel.DetalleProducto_IdUsuarioModifico = usuarioViewModel.Id;

            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:51339/");

                    //HTTP PUT
                    var putTask = client.PutAsJsonAsync<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel>("api/ProductoAPI/ProductoActualizar", pJoinViewModel);
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
            { respuesta = "No se pudo enlazar los datos para actualizar"; }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }




        //---------------------------Methods
        /// <summary>
        /// Busca un usuario basado en su "usuario" NO importa si esta activo o no, lo regresa tal como esta si lo encuentra.
        /// Regresa un UsuarioViewModel si lo encuentra, si no lo encuentra regresa NULL
        /// </summary>
        /// <param name="usuario"></param>
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


        private void InicializarProductoViewModelConValoresQueLLegaronAlControlador(ProductoViewModel productoViewModel)
        {
            productoViewModel.Nombre = ModelState["productoViewModel.Nombre"].Value.AttemptedValue;
            productoViewModel.CodigoBarras = ModelState["productoViewModel.CodigoBarras"].Value.AttemptedValue; 
        }


        private void EvitarAlmacenamientoDeRespuestaEnCacheNavegador()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
        }



    }
}