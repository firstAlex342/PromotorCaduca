﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class palominoEntities : DbContext
    {
        public palominoEntities()
            : base("name=palominoEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Producto> Producto { get; set; }
        public virtual DbSet<DetalleProducto> DetalleProducto { get; set; }
        public virtual DbSet<ProductoConDetalles> ProductoConDetalles { get; set; }
        public virtual DbSet<Caduca> Caduca { get; set; }
        public virtual DbSet<Periodo> Periodo { get; set; }
        public virtual DbSet<Almacena> Almacena { get; set; }
        public virtual DbSet<Tienda> Tienda { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<PeriodoConUnidad> PeriodoConUnidad { get; set; }
        public virtual DbSet<UnidadMedida> UnidadMedida { get; set; }
    }
}
