//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ControlCaducidadesPromotor.AccesoADatos
{
    using System;
    using System.Collections.Generic;
    
    public partial class DetalleProducto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DetalleProducto()
        {
            this.ProductoConDetalles = new HashSet<ProductoConDetalles>();
            this.Caduca = new HashSet<Caduca>();
        }
    
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int IdUsuarioAlta { get; set; }
        public System.DateTime FechaAlta { get; set; }
        public int IdUsuarioModifico { get; set; }
        public System.DateTime FechaModificacion { get; set; }
        public bool Activo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductoConDetalles> ProductoConDetalles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Caduca> Caduca { get; set; }
    }
}
