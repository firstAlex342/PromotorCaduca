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