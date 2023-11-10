using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlCaducidadesPromotor.Basicos
{
    public class ProductoJoinProductoConDetallesJoinDetalleProductoViewModel
    {
        public int Producto_Id { set; get; }
        public string Producto_CodigoBarras { set; get; }
        public int Producto_IdUsuarioAlta { set; get; }
        public DateTime Producto_FechaAlta { set; get; }
        public int Producto_IdUsuarioModifico { set; get; }
        public DateTime Producto_FechaModificacion { set; get; }
        public bool Producto_Activo { set; get; }

        public int ProductoConDetalles_IdProducto { set; get; }
        public int ProductoConDetalles_IdDetalleProducto { set; get; }
        public int ProductoConDetalles_IdUsuarioAlta { set; get; }
        public DateTime ProductoConDetalles_FechaAlta { set; get; }
        public int ProductoConDetalles_IdUsuarioModifico { set; get; }
        public DateTime ProductoConDetalles_FechaModificacion { set; get; }
        public bool ProductoConDetalles_Activo { set; get; }

        public int DetalleProducto_Id { set; get; }
        public string DetalleProducto_Nombre { set; get; }
        public int DetalleProducto_IdUsuarioAlta { set; get; }
        public DateTime DetalleProducto_FechaAlta { set; get; }
        public int DetalleProducto_IdUsuarioModifico { set; get; }
        public DateTime DetalleProducto_FechaModificacion { set; get; }
        public bool DetalleProducto_Activo { set; get; }

        //-------------------Constructor
        public ProductoJoinProductoConDetallesJoinDetalleProductoViewModel()
        {
            this.Producto_Id = 0;
            this.Producto_CodigoBarras = String.Empty;
            this.Producto_IdUsuarioAlta = 0;
            this.Producto_FechaAlta = DateTime.MinValue;
            this.Producto_IdUsuarioModifico = 0;
            this.Producto_FechaModificacion = DateTime.MinValue;
            this.Producto_Activo = false;

            this.ProductoConDetalles_IdProducto = 0;
            this.ProductoConDetalles_IdDetalleProducto = 0;
            this.ProductoConDetalles_IdUsuarioAlta = 0;
            this.ProductoConDetalles_FechaAlta = DateTime.MinValue;
            this.ProductoConDetalles_IdUsuarioModifico = 0;
            this.ProductoConDetalles_FechaModificacion = DateTime.MinValue;
            this.ProductoConDetalles_Activo = false;

            this.DetalleProducto_Id = 0;
            this.DetalleProducto_Nombre = String.Empty;
            this.DetalleProducto_IdUsuarioAlta = 0;
            this.DetalleProducto_FechaAlta = DateTime.MinValue;
            this.DetalleProducto_IdUsuarioModifico = 0;
            this.DetalleProducto_FechaModificacion = DateTime.MinValue;
            this.DetalleProducto_Activo = false;
        }


        //-------------------Methods
        public void AjustarAtributosCodigoBarrasYNombre()
        {
            SuprimirEspaciosEnBlancoEnCodbarrasYNombre();
            PonerMisAtributosAMinusculasCodBarrasYNombre();
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

        public void SuprimirEspaciosEnBlancoEnCodbarrasYNombre()
        {
            this.Producto_CodigoBarras = EliminarTodosEspaciosEnBlanco(this.Producto_CodigoBarras);
            this.DetalleProducto_Nombre = EliminarExcesoDeEspaciosEnBlanco(this.DetalleProducto_Nombre);
        }

        public void PonerMisAtributosAMinusculasCodBarrasYNombre()
        {
            this.Producto_CodigoBarras = this.Producto_CodigoBarras.ToLower();
            this.DetalleProducto_Nombre = this.DetalleProducto_Nombre.ToLower();
        }

        public bool MisAtributosCodbarrasNombreTieneCaracteresPermitidos()
        {
            bool respuesta = false;
            bool EsPermitidoCaracteresEnCodbarras = ContieneCaracteresPermitidos(this.Producto_CodigoBarras);
            bool EsPermitidoCaracteresEnNombre = ContieneCaracteresPermitidos(this.DetalleProducto_Nombre);

            if (EsPermitidoCaracteresEnCodbarras && EsPermitidoCaracteresEnNombre)
            { respuesta = true; }

            return (respuesta);
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

        public bool MisAtributosCodbarrasNombreTieneLongitudPermitida()
        {
            int longitudCodBarras = this.Producto_CodigoBarras.Length;
            int longitudNombre = this.DetalleProducto_Nombre.Length;
            bool respuesta = false;

            if ((longitudCodBarras > 0) && (longitudNombre > 0))
            {
                respuesta = true;
            }
            return (respuesta);
        }

        public bool ExisteEn(List<string> codigosDeBarras)
        {
            bool respuesta = false;

            respuesta = codigosDeBarras.Contains(this.Producto_CodigoBarras);

            return (respuesta);
        }

        public bool EsIgualXFechaAltaYFechaModificacion(ProductoJoinProductoConDetallesJoinDetalleProductoViewModel param)
        {
            RelojServidor relojServidor = new RelojServidor();
            bool res1 = relojServidor.EsMismaFechaYHoraSinMilisegundos(this.Producto_FechaAlta, param.Producto_FechaAlta);
            bool res2 = relojServidor.EsMismaFechaYHoraSinMilisegundos(this.Producto_FechaModificacion, param.Producto_FechaModificacion);
            bool res3 = relojServidor.EsMismaFechaYHoraSinMilisegundos(this.ProductoConDetalles_FechaAlta, param.ProductoConDetalles_FechaAlta);
            bool res4 = relojServidor.EsMismaFechaYHoraSinMilisegundos(this.ProductoConDetalles_FechaModificacion, param.ProductoConDetalles_FechaModificacion);
            bool res5 = relojServidor.EsMismaFechaYHoraSinMilisegundos(this.DetalleProducto_FechaAlta, param.DetalleProducto_FechaAlta);
            bool res6 = relojServidor.EsMismaFechaYHoraSinMilisegundos(this.DetalleProducto_FechaModificacion, param.DetalleProducto_FechaModificacion);

            return (res1 && res2 && res3 && res4 && res5 && res6);
        }
    }
}