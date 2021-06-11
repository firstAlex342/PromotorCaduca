using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCaducidadesPromotor.Basicos
{
    public class PeriodoViewModel
    {
        //---------------properties
        public int Id { set; get; }
        public DateTime FechaCaducidad { set; get; }
        public int NumeroUnidades { set; get; }
        public int IdUsuarioAlta { set; get; }
        public DateTime FechaAlta { set; get; }
        public int IdUsuarioModifico { set; get; }
        public DateTime FechaModificacion { set; get; }
        public bool Activo { set; get; }
        public bool Vigente { set; get; }

        //----------------Constructor
        public PeriodoViewModel()
        {
            this.Id = 0;
            this.FechaCaducidad = DateTime.MinValue;
            this.NumeroUnidades = 0;
            this.IdUsuarioAlta = 0;
            this.FechaAlta = DateTime.MinValue;
            this.IdUsuarioModifico = 0;
            this.FechaModificacion = DateTime.MinValue;
            this.Activo = false;
            this.Vigente = false;
        }
    }
}