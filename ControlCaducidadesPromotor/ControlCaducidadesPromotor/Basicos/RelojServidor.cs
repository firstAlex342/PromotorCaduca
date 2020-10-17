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


        /// <summary>
        /// Compara 2 DateTime, la fecha, parte de la hora HH:MM:SS (observa no compara los milisegundos). True sin son
        /// iguales, false en caso de no ser iguales.
        /// </summary>
        /// <param name="fechaHora1"></param>
        /// <param name="fechaHora2"></param>
        /// <returns>bool</returns>
        public bool EsMismaFechaYHoraSinMilisegundos(DateTime fechaHora1, DateTime fechaHora2)
        {
            bool respuesta = false;

            bool esMismoAño = fechaHora1.Year == fechaHora2.Year;
            bool esMismoMes = fechaHora1.Month == fechaHora2.Month;
            bool esMismoDia = fechaHora1.Day == fechaHora2.Day;

            bool esMismaHora = fechaHora1.Hour == fechaHora2.Hour;
            bool sonMismosMinutos = fechaHora1.Minute == fechaHora2.Minute;
            bool sonMismosSegundos = fechaHora1.Second == fechaHora2.Second;

            respuesta = esMismoAño && esMismoMes && esMismoDia && esMismaHora && sonMismosMinutos && sonMismosSegundos;
            return (respuesta);
        }

    }
}