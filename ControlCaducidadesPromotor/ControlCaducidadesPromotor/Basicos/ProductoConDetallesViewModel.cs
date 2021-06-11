using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCaducidadesPromotor.Basicos
{
    public class ProductoConDetallesViewModel
    {
        //-----------Properties
        public int IdProducto { set; get; }
        public int IdDetalleProducto { set; get; }
        public int IdUsuarioAlta { set; get; }
        public DateTime FechaAlta { set; get; }
        public int IdUsuarioModifico { set; get; }
        public DateTime FechaModificacion { set; get; }
        public bool Activo { set; get; }

        //-------------Constructor
        public ProductoConDetallesViewModel()
        {
            this.IdProducto = 0;
            this.IdDetalleProducto = 0;
            this.IdUsuarioAlta = 0;
            this.FechaAlta = DateTime.MinValue;
            this.IdUsuarioModifico = 0;
            this.FechaModificacion = DateTime.MinValue;
            this.Activo = false;
        }//parameterless constructor
    }
}