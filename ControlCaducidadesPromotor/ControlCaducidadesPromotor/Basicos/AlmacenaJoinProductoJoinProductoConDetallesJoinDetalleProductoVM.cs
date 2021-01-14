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
    }
}