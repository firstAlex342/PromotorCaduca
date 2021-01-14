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
    public class TiendaYProductoAPIController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get_MostrarTiendasYProductos(int idUsuarioOperador)
        {
            if(ModelState.IsValid) 
            {  /*Main succes Scenario*/
                UsuarioViewModel usuarioViewModel;
                UsuarioLN usuarioLN = new UsuarioLN();
                usuarioViewModel = usuarioLN.Get_DetallesDeUsuarioXId(idUsuarioOperador);

                List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> productosYDetallesVM = new List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel>();
                ProductoLN productoLN = new ProductoLN();
                productosYDetallesVM = productoLN.MostrarTodosRegistradosDeOperador(usuarioViewModel.Id);

                List<TiendaViewModel> tiendasVM = new List<TiendaViewModel>();
                TiendaLN tiendaLN = new TiendaLN();
                tiendasVM = tiendaLN.MostrarTodasTiendasDeUsuario(usuarioViewModel.Usuario);

                TiendasYProductosViewModel tiendasYProductosViewModel = new TiendasYProductosViewModel(tiendasVM, productosYDetallesVM);
                return (Ok(tiendasYProductosViewModel));                
            }

            else
            {
                return (BadRequest("Fallo el model binder en TiendaYProductoAPI.Get_MostrarTiendasYProductos"));
            }
        }
    }
}
