using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ControlCaducidadesPromotor.Basicos
{
    public class TiendaViewModel
    {
        public int Id { set; get; }

        [Required(ErrorMessage ="Es necesario introducir la supermanzana")]
        public string Supmza { get; set; }

        [Required(ErrorMessage ="Es necesario introducir la manzana")]
        public string Manzana { get; set; }

        [Required(ErrorMessage ="Es necesario introducir el lote")]
        public string Lote { get; set; }

        [Required(ErrorMessage ="Es necesario introducir la calle")]
        public string Calle { get; set; }

        [Required(ErrorMessage ="Es necesario introducir un nombre")]
        public string Nombre { get; set; }

        public int IdUsuarioAlta { get; set; }
        public System.DateTime FechaAlta { get; set; }
        public int IdUsuarioModifico { get; set; }
        public System.DateTime FechaModificacion { get; set; }
        public bool Activo { get; set; }



        //-------------------Constructor
        public TiendaViewModel()
        {
            this.Id = 0;
            this.Supmza = String.Empty;
            this.Manzana = String.Empty;
            this.Lote = String.Empty;
            this.Calle = String.Empty;
            this.Nombre = String.Empty;

            this.IdUsuarioAlta = 0;
            this.FechaAlta = DateTime.MinValue;
            this.IdUsuarioModifico = 0;
            this.FechaModificacion = DateTime.MinValue;
            this.Activo = false;
        }//parameterless constructor


        //--------------Methods
        /// <summary>
        /// Regresa una representacion del objeto en tipos primitivos. La FechaAlta y FechaModificacion los muestra como string
        /// no como DateTime
        /// </summary>
        /// <returns>Object</returns>
        public object MostrarEnTiposPrimitivos()
        {
            var c = new { this.Id,
                this.Nombre,
                this.Supmza,
                this.Manzana,
                this.Lote,
                this.Calle,
                this.IdUsuarioAlta,
                //FechaAlta = this.FechaAlta.ToString(),
                this.FechaAlta,
                this.IdUsuarioModifico,
                //FechaModificacion = this.FechaModificacion.ToString(),
                this.FechaModificacion,
                this.Activo };

            return (c);
        }
       
    }
}