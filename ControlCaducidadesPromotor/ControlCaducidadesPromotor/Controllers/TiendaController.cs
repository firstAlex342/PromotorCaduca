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
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
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
        [HttpPost]
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

    }
}