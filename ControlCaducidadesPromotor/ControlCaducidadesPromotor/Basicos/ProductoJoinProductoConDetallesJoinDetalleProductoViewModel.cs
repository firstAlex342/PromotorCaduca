using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCaducidadesPromotor.Basicos
{
    public class ProductoJoinProductoConDetallesJoinDetalleProductoViewModel
    {
        public int Producto_Id { set; get; }
        public string Producto_CodigoBarras { set; get; }
        public int Producto_IdUsuarioAlta { set; get; }
        public DateTime Producto_FechaAlta { set; get; }
        public int Producto_IdUsuarioModifico { set; get; }
        public DateTime Producto_FechaModificacion { set; get; }
        public bool Producto_Activo { set; get; }

        public int ProductoConDetalles_IdProducto { set; get; }
        public int ProductoConDetalles_IdDetalleProducto { set; get; }
        public int ProductoConDetalles_IdUsuarioAlta { set; get; }
        public DateTime ProductoConDetalles_FechaAlta { set; get; }
        public int ProductoConDetalles_IdUsuarioModifico { set; get; }
        public DateTime ProductoConDetalles_FechaModificacion { set; get; }
        public bool ProductoConDetalles_Activo { set; get; }

        public int DetalleProducto_Id { set; get; }
        public string DetalleProducto_Nombre { set; get; }
        public int DetalleProducto_IdUsuarioAlta { set; get; }
        public DateTime DetalleProducto_FechaAlta { set; get; }
        public int DetalleProducto_IdUsuarioModifico { set; get; }
        public DateTime DetalleProducto_FechaModificacion { set; get; }
        public bool DetalleProducto_Activo { set; get; }

        //-------------------Constructor
        public ProductoJoinProductoConDetallesJoinDetalleProductoViewModel()
        {
            this.Producto_Id = 0;
            this.Producto_CodigoBarras = String.Empty;
            this.Producto_IdUsuarioAlta = 0;
            this.Producto_FechaAlta = DateTime.MinValue;
            this.Producto_IdUsuarioModifico = 0;
            this.Producto_FechaModificacion = DateTime.MinValue;
            this.Producto_Activo = false;

            this.ProductoConDetalles_IdProducto = 0;
            this.ProductoConDetalles_IdDetalleProducto = 0;
            this.ProductoConDetalles_IdUsuarioAlta = 0;
            this.ProductoConDetalles_FechaAlta = DateTime.MinValue;
            this.ProductoConDetalles_IdUsuarioModifico = 0;
            this.ProductoConDetalles_FechaModificacion = DateTime.MinValue;
            this.ProductoConDetalles_Activo = false;

            this.DetalleProducto_Id = 0;
            this.DetalleProducto_Nombre = String.Empty;
            this.DetalleProducto_IdUsuarioAlta = 0;
            this.DetalleProducto_FechaAlta = DateTime.MinValue;
            this.DetalleProducto_IdUsuarioModifico = 0;
            this.DetalleProducto_FechaModificacion = DateTime.MinValue;
            this.DetalleProducto_Activo = false;
        }
    }
}