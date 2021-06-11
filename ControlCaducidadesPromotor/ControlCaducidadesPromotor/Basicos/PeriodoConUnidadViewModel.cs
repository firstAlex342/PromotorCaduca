using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCaducidadesPromotor.Basicos
{
    public class PeriodoConUnidadViewModel
    {
        //---------properties
        public int IdPeriodo { set; get; }
        public int IdUnidad { set; get; }
        public int IdUsuarioAlta { set; get; }
        public DateTime FechaAlta { set; get; }
        public int IdUsuarioModifico { set; get; }
        public DateTime FechaModificacion { set; get; }
        public bool Activo { set; get; }

        //--------------constructor
        public PeriodoConUnidadViewModel()
        {
            this.IdPeriodo = 0;
            this.IdUnidad = 0;
            this.IdUsuarioAlta = 0;
            this.FechaAlta = DateTime.MinValue;
            this.IdUsuarioModifico = 0;
            this.FechaModificacion = DateTime.MinValue;
            this.Activo = false;
        }
    }
}