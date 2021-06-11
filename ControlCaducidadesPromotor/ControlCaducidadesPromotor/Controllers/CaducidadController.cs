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
                        return Json("aqui boludo", JsonRequestBehavior.AllowGet);
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
    }
}