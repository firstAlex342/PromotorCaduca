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
                UsuarioViewModel clsUsuario = new UsuarioViewModel();
                clsUsuario.Id = 0;

                cliente.BaseAddress = new Uri("http://localhost:51339/");
                var responseTask = cliente.GetAsync("api/ProductoAPI/MostrarTodosRegistradosDeOperador?idUsuarioOperador=" + clsUsuario.Id.ToString());

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
                UsuarioViewModel clsUsuario = new UsuarioViewModel();
                clsUsuario.Id= 0;
                productoViewModel.IdUsuarioAlta = clsUsuario.Id;
                productoViewModel.IdUsuarioModifico = clsUsuario.Id;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:51339/");

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<ProductoViewModel>("api/ProductoAPI", productoViewModel);

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
                        ProductoViewModel productoVM = new ProductoViewModel();
                        productoVM.Nombre = ModelState["productoViewModel.Nombre"].Value.AttemptedValue;
                        productoVM.CodigoBarras = ModelState["productoViewModel.CodigoBarras"].Value.AttemptedValue;

                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.Cache.SetNoStore();
                        return (View("MostrarFormAltaProducto", productoVM));
                    }
                }
            }

            else
            {
                ProductoViewModel productoVM = new ProductoViewModel();
                productoVM.Nombre = ModelState["productoViewModel.Nombre"].Value.AttemptedValue;
                productoVM.CodigoBarras = ModelState["productoViewModel.CodigoBarras"].Value.AttemptedValue;

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
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
            UsuarioViewModel clsUsuario = new UsuarioViewModel();
            clsUsuario.Id = 0;

            if (ModelState.IsValid)
            {
                using (var cliente = new HttpClient())
                {
                    cliente.BaseAddress = new Uri("http://localhost:51339/");
                    var responseTask = cliente.GetAsync("api/ProductoAPI/BuscarProductoxCodigoBarras?codigoBarrasBuscado=" + codigoBarrasBuscado + "&idUsuarioOperador=" + clsUsuario.Id.ToString());
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

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public JsonResult ModificarProducto(ProductoJoinProductoConDetallesJoinDetalleProductoViewModel pJoinViewModel)
        {
            string respuesta = "";
            UsuarioViewModel clsUsuario = new UsuarioViewModel();
            clsUsuario.Id = 0;
            pJoinViewModel.DetalleProducto_IdUsuarioAlta = clsUsuario.Id;
            pJoinViewModel.DetalleProducto_IdUsuarioModifico = clsUsuario.Id;

            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:51339/");

                    //HTTP PUT
                    var putTask = client.PutAsJsonAsync<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel>("api/ProductoAPI", pJoinViewModel);
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



    }
}