using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCaducidadesPromotor.Basicos
{
    public class RelojServidor
    {

        //----------------Methods
        public DateTime RegresarHoraEnServidor()
        {
            return (DateTime.Now);
        }

        public void ColocarMismaFechaHoraEnCamposFechaAltaYFechaModificacion(ProductoViewModel productoViewModel)
        {
            DateTime fechaHora = new DateTime();
            fechaHora = RegresarHoraEnServidor();

            productoViewModel.FechaAlta = fechaHora;
            productoViewModel.FechaModificacion = fechaHora;
        }

        public void ColocarMismaFechaHoraEnCamposFechaAltaYFechaModificacion(ProductoJoinProductoConDetallesJoinDetalleProductoViewModel productoJoinViewModel)
        {
            DateTime fechaHora = new DateTime();
            fechaHora = RegresarHoraEnServidor();

            productoJoinViewModel.Producto_FechaAlta = fechaHora;
            productoJoinViewModel.Producto_FechaModificacion = fechaHora;

            productoJoinViewModel.ProductoConDetalles_FechaAlta = fechaHora;
            productoJoinViewModel.ProductoConDetalles_FechaModificacion = fechaHora;

            productoJoinViewModel.DetalleProducto_FechaAlta = fechaHora;
            productoJoinViewModel.DetalleProducto_FechaModificacion = fechaHora;
        }

        public void ColocarMismaFechaHoraEnCamposFechaAltaYFechaModificacion(TiendaViewModel tiendaViewModel)
        {
            DateTime fechaHora = new DateTime();
            fechaHora = RegresarHoraEnServidor();

            tiendaViewModel.FechaAlta = fechaHora;
            tiendaViewModel.FechaModificacion = fechaHora;
        }

    }
}