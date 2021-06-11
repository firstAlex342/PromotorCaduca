using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCaducidadesPromotor.Basicos
{
    public class TiendaJoinCaducidadesViewModel
    {
        //---------------properties
        public TiendaViewModel MiTiendaViewModel { set; get; }
        public AlmacenaViewModel MiAlmacenaViewModel { set; get; }
        public ProductoViewModel MiProductoViewModel { set; get; }
        public ProductoConDetallesViewModel MiProductoConDetallesViewModel { set; get; }
        public DetalleProductoViewModel MiDetalleProductoViewModel { set; get; }
        public CaducaViewModel MiCaducaViewModel { set; get; }
        public PeriodoViewModel MiPeriodoViewModel { set; get; }
        public PeriodoConUnidadViewModel MiPeriodoConUnidadViewModel { set; get; }
        public UnidadMedidaViewModel MiUnidadMedidaViewModel { set; get; }

        //-------------constructor
        public TiendaJoinCaducidadesViewModel()
        {
            this.MiTiendaViewModel = new TiendaViewModel();
            this.MiAlmacenaViewModel = new AlmacenaViewModel();
            this.MiProductoViewModel = new ProductoViewModel();
            this.MiProductoConDetallesViewModel = new ProductoConDetallesViewModel();
            this.MiDetalleProductoViewModel = new DetalleProductoViewModel();
            this.MiCaducaViewModel = new CaducaViewModel();
            this.MiPeriodoViewModel = new PeriodoViewModel();
            this.MiPeriodoConUnidadViewModel = new PeriodoConUnidadViewModel();
            this.MiUnidadMedidaViewModel = new UnidadMedidaViewModel();
        }

    }
}