using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCaducidadesPromotor.Basicos
{
    public class ParametroBuscarCaducidadViewModel
    {
        //-------------properties
        public int IdTienda { set; get; }
        public DateTime FechaInicial { set; get; }
        public DateTime FechaFinal { set; get; }
        public bool MostrarVigentes { set; get; } //true = muestra solo vigentes, false = muestra vigentes y no vigentes

        public int IdUsuarioAlta { set; get; }
        public DateTime FechaAlta { set; get; }
        public int IdUsuarioModifico { set; get; }
        public DateTime FechaModificacion { set; get; }
        public bool Activo { set; get; }

        //-----------constructor
        public ParametroBuscarCaducidadViewModel()
        {
            this.IdTienda = 0;
            this.FechaInicial = DateTime.MinValue;
            this.FechaFinal = DateTime.MinValue;
            this.MostrarVigentes = false;

            this.IdUsuarioAlta = 0;
            this.FechaAlta = DateTime.MinValue;
            this.IdUsuarioModifico = 0;
            this.FechaModificacion = DateTime.MinValue;
            this.Activo = false;
        }//parameterless constructor

    }
}