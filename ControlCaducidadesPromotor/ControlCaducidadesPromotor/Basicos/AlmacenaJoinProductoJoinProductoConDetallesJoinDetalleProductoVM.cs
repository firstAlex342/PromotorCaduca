using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCaducidadesPromotor.Basicos
{
    public class AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM 
        : ProductoJoinProductoConDetallesJoinDetalleProductoViewModel
    {
        public int Almacena_IdTienda { set; get; }
        public int Almacena_IdProducto { set; get; }
        public int Almacena_IdUsuarioAlta { set; get; }
        public DateTime Almacena_FechaAlta { set; get; }
        public int Almacena_IdUsuarioModifico { set; get; }
        public DateTime Almacena_FechaModificacion { set; get; }
        public bool Almacena_Activo { set; get; }


        //---------------Constructor
        public AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM() : base()
        {
            this.Almacena_IdTienda = 0;
            this.Almacena_IdProducto = 0;
            this.Almacena_IdUsuarioAlta = 0;
            this.Almacena_FechaAlta = DateTime.MinValue;
            this.Almacena_IdUsuarioModifico = 0;
            this.Almacena_FechaModificacion = DateTime.MinValue;
            this.Almacena_Activo = false;
        }

        //----------Methods
        /// <summary>
        /// Compara 2 objetos tipos AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM y regresa true o false, basado en si son iguales
        /// sus valores en Almacena_FechaAlta,Almacena_FechaModificacion,Producto_FechaAlta,Producto_FechaModificacion, ProductoConDetalles_FechaAlta,
        /// ProductoConDetalles_FechaModificacion,DetalleProducto_FechaAlta, DetalleProducto_FechaModificacion
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool EsIgualXFechaAltaYFechaModificacion(AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM  param)
        {
            RelojServidor miRelojServidor = new RelojServidor();
            bool res1 = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(this.Almacena_FechaAlta, param.Almacena_FechaAlta);
            bool res2 = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(this.Almacena_FechaModificacion, param.Almacena_FechaModificacion);
            bool res3 = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(this.Producto_FechaAlta, param.Producto_FechaAlta);
            bool res4 = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(this.Producto_FechaModificacion, param.Producto_FechaModificacion);
            bool res5 = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(this.ProductoConDetalles_FechaAlta, param.ProductoConDetalles_FechaAlta);
            bool res6 = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(this.ProductoConDetalles_FechaModificacion, param.ProductoConDetalles_FechaModificacion);
            bool res7 = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(this.DetalleProducto_FechaAlta, param.DetalleProducto_FechaAlta);
            bool res8 = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(this.DetalleProducto_FechaModificacion, param.DetalleProducto_FechaModificacion);

            return (res1 && res2 && res3 && res4 && res5 && res6 && res7 && res8);
        }
    }
}