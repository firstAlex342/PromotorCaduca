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

        public string EliminarExcesoDeEspaciosEnBlanco(string texto)
        {
            string[] separators = new string[] { " " };
            var coleccion = texto.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            string textoConEspacionEnBlancoNecesarios = "";
            foreach (var item in coleccion)
            {
                textoConEspacionEnBlancoNecesarios = textoConEspacionEnBlancoNecesarios + item + " ";
            }

            return (textoConEspacionEnBlancoNecesarios.TrimEnd());
        }

        public string EliminarTodosEspaciosEnBlanco(string texto)
        {
            string[] separators = new string[] { " " };
            var coleccion = texto.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            string textoSinEspaciosEnBlanco = "";
            foreach (var item in coleccion)
            {
                textoSinEspaciosEnBlanco = textoSinEspaciosEnBlanco + item;
            }

            return (textoSinEspaciosEnBlanco.TrimEnd());
        }

        public void SuprimirEspaciosEnBlancoEnCodbarrasYNombre()
        {
            this.CodigoBarras = EliminarTodosEspaciosEnBlanco(this.CodigoBarras);
            this.Nombre = EliminarExcesoDeEspaciosEnBlanco(this.Nombre);
        }

        public void PonerMisAtributosAMinusculas()
        {
            this.CodigoBarras = this.CodigoBarras.ToLower();
            this.Nombre = this.Nombre.ToLower();
        }

        public void AjustarAtributosCodigoBarrasYNombre()
        {
            SuprimirEspaciosEnBlancoEnCodbarrasYNombre();
            PonerMisAtributosAMinusculas();
        }

        public bool ContieneCaracteresPermitidos(string texto)
        {
            bool respuesta = false;

            var textoVar = texto.ToArray<char>();

            string caracteresPermitidosString = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ abcdefghijklmnñopqrstuvwxyz1234567890";
            var caracteresPermitidosVar = caracteresPermitidosString.ToArray<char>();

            foreach (var caracter in textoVar)
            {
                if (caracteresPermitidosVar.Contains(caracter) == true)
                {
                    respuesta = true;
                }

                else
                {
                    respuesta = false;
                    break;
                }
            }
            return (respuesta);
        }

        public bool MisAtributosCodbarrasNombreTieneCaracteresPermitidos()
        {
            bool respuesta = false;
            bool EsPermitidoCaracteresEnCodbarras = ContieneCaracteresPermitidos(this.CodigoBarras);
            bool EsPermitidoCaracteresEnNombre = ContieneCaracteresPermitidos(this.Nombre);

            if(EsPermitidoCaracteresEnCodbarras && EsPermitidoCaracteresEnNombre)
            { respuesta = true; }

            return (respuesta);
        }

        public bool MisAtributosCodbarrasNombreTieneLongitudPermitida()
        {
            int longitudCodBarras = this.CodigoBarras.Length;
            int longitudNombre = this.Nombre.Length;
            bool respuesta = false;

            if(  (longitudCodBarras > 0)   && (longitudNombre >0) )
            {
                respuesta = true;
            }
            return (respuesta);
        }
    }
}