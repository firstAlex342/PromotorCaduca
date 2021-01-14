using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCaducidadesPromotor.Basicos
{
    /// <summary>
    /// Almacena una lista de objetos TiendaViewModel y una lista de objetos ProductoJoinProductoConDetallesJoinDetalleProductoViewModel
    /// </summary>
    public class TiendasYProductosViewModel
    {
        public List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> ProductosYDetallesViewModel { set; get; }
        public List<TiendaViewModel> TiendasViewModel{ set; get; }

        //----------------------Constructor
        public TiendasYProductosViewModel()
        {
            this.ProductosYDetallesViewModel = new List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel>();
            this.TiendasViewModel = new List<TiendaViewModel>();
        }//parameterless constructor

        public TiendasYProductosViewModel(List<TiendaViewModel> tiendasViewModel, 
            List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> productosYDetallesViewModel)
        {
            this.TiendasViewModel = tiendasViewModel;
            this.ProductosYDetallesViewModel = productosYDetallesViewModel;
        }
    }
}