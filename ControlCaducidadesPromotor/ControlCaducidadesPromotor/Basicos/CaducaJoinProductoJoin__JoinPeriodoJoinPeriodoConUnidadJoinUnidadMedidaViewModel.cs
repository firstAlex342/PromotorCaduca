using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCaducidadesPromotor.Basicos
{
    /// <summary>
    /// Representa un join entre las clases
    /// Caduca, Producto, ProductoConDetalles, DetalleProducto, 
    /// Periodo, PeriodoConUnidad, UnidadMedida
    /// </summary>
    public class CaducaJoinProductoJoin__JoinPeriodoJoinPeriodoConUnidadJoinUnidadMedidaViewModel
    {
        //--------------Properties   
        public CaducaViewModel MiCaducaViewModel { set; get; }
        public ProductoJoinProductoConDetallesJoinDetalleProductoViewModel MiProductoJoinProductoConDetallesJoinDetalleProductoViewModel { set; get; }
        public PeriodoViewModel MiPeriodoViewModel { set; get; }
        public PeriodoConUnidadViewModel MiPeriodoConUnidadViewModel { set; get; }
        public UnidadMedidaViewModel MiUnidadMedidaViewModel { set; get; }


        //-----------Constructor
        public CaducaJoinProductoJoin__JoinPeriodoJoinPeriodoConUnidadJoinUnidadMedidaViewModel()
        {
            this.MiCaducaViewModel = new CaducaViewModel();
            this.MiProductoJoinProductoConDetallesJoinDetalleProductoViewModel = new ProductoJoinProductoConDetallesJoinDetalleProductoViewModel();
            this.MiPeriodoViewModel = new PeriodoViewModel();
            this.MiPeriodoConUnidadViewModel = new PeriodoConUnidadViewModel();
            this.MiUnidadMedidaViewModel = new UnidadMedidaViewModel();
        }//parameterless constructor

    }
}