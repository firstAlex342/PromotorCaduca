using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using ControlCaducidadesPromotor.Basicos;

namespace ControlCaducidadesPromotor.Controllers
{
    public class TiendaYProductoController : Controller
    {
        // GET: TiendasYProductos
        [Authorize]
        public ActionResult AgregarProductosATienda()
        {
            return ( View("AgregarProductosATienda"));
        }


        [Authorize]
        public JsonResult MostrarTiendasYProductosDeUsuario()
        {
            TiendasYProductosViewModel respuesta = new TiendasYProductosViewModel();
            UsuarioViewModel usuarioViewModel = LLamarApiBuscarUsuarioXUsuario(User.Identity.Name);


            if (ModelState.IsValid)
            {
                if (usuarioViewModel != null)
                {
                    using (var cliente = new HttpClient())
                    {
                        cliente.BaseAddress = new Uri("http://localhost:51339/");
                        var responseTask = cliente.GetAsync("api/TiendaYProductoAPI/Get_MostrarTiendasYProductos?idUsuarioOperador="+usuarioViewModel.Id.ToString());
                        responseTask.Wait();

                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<TiendasYProductosViewModel>();
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



        //---------------------------Methods
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