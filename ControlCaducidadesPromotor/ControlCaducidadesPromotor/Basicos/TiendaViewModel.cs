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

        /// <summary>
        /// Suprime los espacios en blanco al principio y final de los atributos: Supmza, Manzana, Lote, Calle y Nombre
        /// </summary>
        public void SuprimirEspaciosEnBlancoEnSupmzaMzaLteCalleNombre()
        {
            this.Supmza = EliminarExcesoDeEspaciosEnBlanco(this.Supmza);
            this.Manzana = EliminarExcesoDeEspaciosEnBlanco(this.Manzana);
            this.Lote = EliminarExcesoDeEspaciosEnBlanco(this.Lote);
            this.Calle = EliminarExcesoDeEspaciosEnBlanco(this.Calle);
            this.Nombre = EliminarExcesoDeEspaciosEnBlanco(this.Nombre);
        }

        /// <summary>
        /// Coloca los atributos Supmza, manzana, lote, calle y nombre en minusculas
        /// </summary>
        public void PonerMisAtributosAMinusculas()
        {
            this.Supmza = this.Supmza.ToLower();
            this.Manzana = this.Manzana.ToLower();
            this.Lote = this.Lote.ToLower();
            this.Calle = this.Calle.ToLower();
            this.Nombre = this.Nombre.ToLower();
        }


        /// <summary>
        /// Suprime espacios en blanco al inicio y final de los atributos Supmza, Mza, Lote, Calle y nombre 
        /// y los pone a minusculas
        /// </summary>
        public void AjustarAtributosSupmzaMzaLteCalleNombre()
        {
            SuprimirEspaciosEnBlancoEnSupmzaMzaLteCalleNombre();
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

        public bool MisAtributosSupmzaMzaLteCalleNombreTieneCaracteresPermitidos()
        {
            bool EsPermitidoCaracteresEnSupmza = ContieneCaracteresPermitidos(this.Supmza);
            bool EsPermitidoCaracteresEnManzana = ContieneCaracteresPermitidos(this.Manzana);
            bool EsPermitidoCaracteresEnLote = ContieneCaracteresPermitidos(this.Lote);
            bool EsPermitidoCaracteresEnCalle = ContieneCaracteresPermitidos(this.Calle);
            bool EsPermitidoCaracteresEnNombre = ContieneCaracteresPermitidos(this.Nombre);
            bool respuesta = false;

            if(EsPermitidoCaracteresEnSupmza && EsPermitidoCaracteresEnManzana && EsPermitidoCaracteresEnLote && EsPermitidoCaracteresEnCalle && EsPermitidoCaracteresEnNombre)
            {
                respuesta = true;
            }

            return (respuesta);
        }

        public bool MisAtributosSupmzaMzaLteCalleNombreTieneLongitudPermitida()
        {
            int longitudSupmza = this.Supmza.Length;
            int longitudMza = this.Manzana.Length;
            int longitudLte = this.Lote.Length;
            int longitudCalle = this.Calle.Length;
            int longitudNombre = this.Nombre.Length;

            bool respuesta = false;
            if(  (longitudSupmza > 0) && (longitudMza > 0)  && (longitudLte > 0)  && (longitudCalle > 0) && (longitudNombre > 0)   )
            {
                respuesta = true;
            }

            return (respuesta);
        }

        public string EliminarExcesoDeEspaciosEnBlanco(string texto)
        {
            string[] separators = new string[] {" " };
            var coleccion = texto.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            string textoConEspacionEnBlancoNecesarios="";
            foreach(var item in coleccion)
            {
                textoConEspacionEnBlancoNecesarios = textoConEspacionEnBlancoNecesarios + item + " ";
            }

            return(textoConEspacionEnBlancoNecesarios.TrimEnd() );
        }
       
    }
}