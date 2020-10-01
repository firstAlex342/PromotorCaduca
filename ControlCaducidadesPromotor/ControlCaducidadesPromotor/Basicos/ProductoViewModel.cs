using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ControlCaducidadesPromotor.Basicos
{
    public class ProductoViewModel
    {
        public int Id { set; get; }

        [Required(ErrorMessage ="Introduzca código de barras")]
        public string CodigoBarras { set; get; }

        [Required(ErrorMessage= "Introduzca nombre")]
        public string Nombre { set; get; }

        public int IdUsuarioAlta { set; get; }
        public DateTime FechaAlta { set; get; }
        public int IdUsuarioModifico { set; get; }
        public DateTime FechaModificacion { set; get; }
        public bool Activo { set; get; }


        //--------------Constructor
        public ProductoViewModel()
        {
            this.Id = 0;
            this.CodigoBarras = "";
            this.Nombre = "";
            this.IdUsuarioAlta = 0;
            this.FechaAlta = DateTime.MinValue;
            this.IdUsuarioModifico = 0;
            this.FechaModificacion = DateTime.MinValue;
            this.Activo = true;
        }


        //-----------------Methods
        public bool ExisteEn(List<string> codigosDeBarras)
        {
            bool respuesta = false;

            respuesta = codigosDeBarras.Contains(this.CodigoBarras);

            return (respuesta);
        }
    }
}