using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCaducidadesPromotor.Basicos
{
    /// <summary>
    /// Almacena una TiendaViewModel, 
    /// una lista de objetos ProductoJoinProductoConDetallesJoinDetalleProductoViewModel(representa los productos que se pueden agregar a la tienda),
    /// una lista de objetos AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM (representa los productos que ya han sido agregados a la tienda)
    /// </summary>
    public class TiendaYProductosViewModel
    {
        public List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM> ProductosDeTienda { set; get; }
        public List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> ProductosKSePuedenAniadirATienda { set; get; }
        public TiendaViewModel InfoTienda{ set; get; }

        //----------------------Constructor
        public TiendaYProductosViewModel()
        {
            this.ProductosDeTienda = new List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM>();
            this.ProductosKSePuedenAniadirATienda = new List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel>();
            this.InfoTienda = new TiendaViewModel();
        }//parameterless constructor

        public TiendaYProductosViewModel(TiendaViewModel tienda,
            List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM> productosDeTienda,
            List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> productosKSePuedenAniadirATienda)
        {
            this.InfoTienda = tienda;
            this.ProductosDeTienda = productosDeTienda;
            this.ProductosKSePuedenAniadirATienda = productosKSePuedenAniadirATienda;
        }

        //--------------------methods
        /// <summary>
        /// Compara la property ProductosDeTienda con el parametro productosEnTiendaAlMomento (compara los campos fechaAlta y fechaModificacion).
        /// Regresa true si son iguales. Regresa true sin ambas lista estan vacias.
        /// </summary>
        /// <param name="productosEnTiendaAlMomento"></param>
        /// <returns></returns>
        public bool EsIgualProductosDeTiendaXFechaAltaYFechaModif(List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM> productosEnTiendaAlMomento)
        {
            bool respuestaA = false;
            bool respuestaB = false;

            if (this.ProductosDeTienda.Count == 0 && productosEnTiendaAlMomento.Count== 0)
            { //Si las 2 listas estan vacías regresa true
                return (true);
            }

            //Verifico que cada item de productosEnTiendaAlMomento este en la lista ProductosDeTienda
            foreach (var item in productosEnTiendaAlMomento)
            {
                var elementoBuscado = this.ProductosDeTienda.SingleOrDefault(s => {
                    bool res1 = s.Almacena_IdTienda == item.Almacena_IdTienda;
                    bool res2 = s.Almacena_IdProducto == item.Almacena_IdProducto;
                    bool res3 = s.Producto_Id == item.Producto_Id;
                    bool res4 = s.ProductoConDetalles_IdProducto == item.ProductoConDetalles_IdProducto;
                    bool res5 = s.ProductoConDetalles_IdDetalleProducto == item.ProductoConDetalles_IdDetalleProducto;
                    bool res6 = s.DetalleProducto_Id == item.DetalleProducto_Id;

                    return (res1 && res2 && res3 && res4 && res5 && res6);
                });

                if (elementoBuscado != null)  {
                     bool sonIgualesXFecha = elementoBuscado.EsIgualXFechaAltaYFechaModificacion(item);
                    if(sonIgualesXFecha == true)  {
                        respuestaA = true;
                    }

                     else {
                        respuestaA = false;
                        break;
                      }
                }

                else { respuestaA = false; break; }
            }


            //Verifico que cada item de productosEnTiendaAlMomento este en la lista ProductosDeTienda
            foreach(var item in this.ProductosDeTienda)
            {
                var elementoBuscado = productosEnTiendaAlMomento.SingleOrDefault(s =>
                {
                    bool res1 = s.Almacena_IdTienda == item.Almacena_IdTienda;
                    bool res2 = s.Almacena_IdProducto == item.Almacena_IdProducto;
                    bool res3 = s.Producto_Id == item.Producto_Id;
                    bool res4 = s.ProductoConDetalles_IdProducto == item.ProductoConDetalles_IdProducto;
                    bool res5 = s.ProductoConDetalles_IdDetalleProducto == item.ProductoConDetalles_IdDetalleProducto;
                    bool res6 = s.DetalleProducto_Id == item.DetalleProducto_Id;

                    return (res1 && res2 && res3 && res4 && res5 && res6);
                });

                if (elementoBuscado != null)
                {
                    bool sonIgualesXFecha = elementoBuscado.EsIgualXFechaAltaYFechaModificacion(item);
                    if (sonIgualesXFecha == true)
                    {
                        respuestaB = true;
                    }

                    else
                    {
                        respuestaB = false;
                        break;
                    }
                }

                else { respuestaB = false; break; }
            }

            return (respuestaA && respuestaB);
        }

        public bool EsIgualProductosKSePuedenAniadirATienda(List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> productosKSePuedenAniadirAlMomento)
        {
            bool respuesta = false;
            
            foreach(var item in this.ProductosKSePuedenAniadirATienda)
            {
                var elementoBuscado = productosKSePuedenAniadirAlMomento.SingleOrDefault(s =>
                {
                    bool res1 = item.Producto_Id == s.Producto_Id;
                    bool res2 = (item.ProductoConDetalles_IdProducto == s.ProductoConDetalles_IdProducto) && (item.ProductoConDetalles_IdDetalleProducto == s.ProductoConDetalles_IdDetalleProducto);
                    bool res3 = item.DetalleProducto_Id == s.DetalleProducto_Id;

                    return (res1 && res2 && res3);
                });

                if(elementoBuscado != null)
                {
                    bool sonIgualesXFecha = item.EsIgualXFechaAltaYFechaModificacion(elementoBuscado);
                    if(sonIgualesXFecha == true)
                    {
                        respuesta = true;
                    }

                    else
                    {
                        respuesta = false;
                        break;
                    }
                }

                else { respuesta = false; break; }
            }

            return (respuesta);
        }
    }
}