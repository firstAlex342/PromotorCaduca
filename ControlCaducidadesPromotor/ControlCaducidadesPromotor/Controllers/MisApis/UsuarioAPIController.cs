using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ControlCaducidadesPromotor.Models;
using ControlCaducidadesPromotor.Basicos;

namespace ControlCaducidadesPromotor.Controllers.MisApis
{
    public class UsuarioAPIController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get_UsuarioXUsuarioYPassword(string usuario, string password)
        {
            if(ModelState.IsValid)
            {
                UsuarioViewModel usuarioViewModel = new UsuarioViewModel(0, "", usuario, password, false);
                UsuarioViewModel usuarioBuscadoVM = null;
                UsuarioLN usuarioLN = new UsuarioLN();
                usuarioBuscadoVM = usuarioLN.Get_UsuarioXUsuarioYPassword(usuarioViewModel);
                if(usuarioBuscadoVM != null)
                {
                    return (Ok(usuarioBuscadoVM));
                }

                else
                {
                    return (BadRequest("No se encontro usuario ó password ó esta inactivo este usuario"));
                }
            }

            else
            {
                return (BadRequest("No se pudo enlazar los parametros en UsuarioAPIController.Get_EsUsuarioCorrectoYEstaActivo"));
            }
        }



        [HttpGet]
        public IHttpActionResult Get_UsuarioXUsuario(string usuario)
        {
            if (ModelState.IsValid)
            {
                UsuarioViewModel usuarioBuscadoVM = null;
                UsuarioLN usuarioLN = new UsuarioLN();

                usuarioBuscadoVM = usuarioLN.Get_DetallesDeUsuarioXusuario(usuario);
                if (usuarioBuscadoVM != null)
                {
                    return (Ok(usuarioBuscadoVM));
                }

                else
                {
                    return (BadRequest("No se encontro usuario"));
                }
            }

            else
            {
                return (BadRequest("No se pudo enlazar los parametros en UsuarioAPIController.Get_UsuarioXUsuario"));
            }
        }
    }
}
