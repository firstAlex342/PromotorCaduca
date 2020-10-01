using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Security;
using ControlCaducidadesPromotor.Basicos;

namespace ControlCaducidadesPromotor.Controllers
{
    public class AuthenticationController : Controller
    {
        // GET: Authentication
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Login()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return View("Login", new UsuarioViewModel());
        }


        [HttpPost]
        public ActionResult DoLogin(UsuarioViewModel miUsuarioViewModel)
        {
            if(ModelState.IsValid)
            {
                if(PuedeIniciarSesionUsuario(miUsuarioViewModel))
                {
                    FormsAuthentication.SetAuthCookie(miUsuarioViewModel.Usuario, false); 
                    return (RedirectToAction("Index", "Tienda"));
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Su usuario - contraseña no fue valido");
                    UsuarioViewModel usuarioDummyViewModel;
                    usuarioDummyViewModel = new UsuarioViewModel(0, "", ModelState["miUsuarioViewModel.Usuario"].Value.AttemptedValue, ModelState["miUsuarioViewModel.Password"].Value.AttemptedValue, false);
                    return (View("Login", usuarioDummyViewModel) );
                }
            }

            else
            {
                UsuarioViewModel usuarioViewModel;
                usuarioViewModel = new UsuarioViewModel(0,"",ModelState["miUsuarioViewModel.Usuario"].Value.AttemptedValue, ModelState["miUsuarioViewModel.Password"].Value.AttemptedValue, false);
                return View("Login", usuarioViewModel);
            }           
        }


        [Authorize]
        [HttpGet]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return (RedirectToAction("Login"));
        }



        private bool PuedeIniciarSesionUsuario(UsuarioViewModel usuarioViewModel)
        {
            bool respuesta = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:51339/");

                //HTTP Get
                var responseTask = client.GetAsync("api/UsuarioAPI/Get_UsuarioXUsuarioYPassword?usuario=" + usuarioViewModel.Usuario + "&password=" + usuarioViewModel.Password);

                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<UsuarioViewModel>();
                    readTask.Wait();

                    UsuarioViewModel usuarioBuscadoViewModel = readTask.Result;
                    respuesta = usuarioBuscadoViewModel.Activo == true ? true : false;
                }
            }

            return (respuesta);
        }
    }
}