using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ControlCaducidadesPromotor.AccesoADatos;
using ControlCaducidadesPromotor.Basicos;


namespace ControlCaducidadesPromotor.Models
{
    public class ProductoLN
    {
        //---------------------------Methods
        public string Crear(ProductoViewModel productoViewModel)
        {
            string mensaje = "";
            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {   //Buscar los codigosBarras del usuario operador
                        var codigosBarrasExisten = (from s in ctx.Producto
                                                    where s.IdUsuarioAlta == productoViewModel.IdUsuarioAlta
                                                    select s.CodigoBarras
                                                    ).ToList();
                        bool existeProductoEnActivosOInactivos = productoViewModel.ExisteEn(codigosBarrasExisten);

                        //Busco el Id de usuario que necesitare, y ver que este activo
                        var resumenUsuarios = (from x in ctx.Usuario
                                               select new { x.Id, x.Usuario1, x.Activo }).ToList();

                        var usuarioActivoBuscado = resumenUsuarios.SingleOrDefault(item => (item.Id == productoViewModel.IdUsuarioAlta) &&
                                                                                            (item.Activo == true));

                        if (!existeProductoEnActivosOInactivos && (usuarioActivoBuscado != null)   )
                        {
                            var listaIdsProducto = (from s in ctx.Producto
                                                    select s.Id).ToList();
                            int idUltimoEnProducto = listaIdsProducto.Count() == 0 ? 0 : listaIdsProducto.Max();
                            Producto p = new Producto();   //Producto es una clase del edmx                                                          
                            p.Id = idUltimoEnProducto + 1;
                            p.CodigoBarras = productoViewModel.CodigoBarras;
                            p.IdUsuarioAlta = productoViewModel.IdUsuarioAlta;
                            p.FechaAlta = productoViewModel.FechaAlta;
                            p.IdUsuarioModifico = productoViewModel.IdUsuarioModifico;
                            p.FechaModificacion = productoViewModel.FechaModificacion;
                            p.Activo = true;
                            ctx.Producto.Add(p);


                            var idsDetalleProducto = (from s in ctx.DetalleProducto
                                                      select s.Id).ToList();
                            int idUltimoEnDetalleProduto = idsDetalleProducto.Count() == 0 ? 0 : idsDetalleProducto.Max();
                            DetalleProducto detalleProducto = new DetalleProducto();
                            detalleProducto.Id = idUltimoEnDetalleProduto + 1;
                            detalleProducto.Nombre = productoViewModel.Nombre;
                            detalleProducto.IdUsuarioAlta = productoViewModel.IdUsuarioAlta;
                            detalleProducto.IdUsuarioModifico = productoViewModel.IdUsuarioModifico;
                            detalleProducto.FechaAlta = productoViewModel.FechaAlta;
                            detalleProducto.FechaModificacion = productoViewModel.FechaModificacion;
                            detalleProducto.Activo = true;
                            ctx.DetalleProducto.Add(detalleProducto);

                            ProductoConDetalles productoConDetalles = new ProductoConDetalles();
                            productoConDetalles.IdProducto = p.Id;
                            productoConDetalles.IdDetalleProducto = detalleProducto.Id;
                            productoConDetalles.IdUsuarioAlta = productoViewModel.IdUsuarioAlta;
                            productoConDetalles.FechaAlta = productoViewModel.FechaAlta;
                            productoConDetalles.IdUsuarioModifico = productoViewModel.IdUsuarioModifico;
                            productoConDetalles.FechaModificacion = productoViewModel.FechaModificacion;
                            productoConDetalles.Activo = true;
                            ctx.ProductoConDetalles.Add(productoConDetalles);

                            ctx.SaveChanges();                            
                            mensaje = "ok";
                        }

                        else
                        { 
                            mensaje = existeProductoEnActivosOInactivos == true ? "Ya existe este código de barras en la BD" : "El usuario esta inactivo";
                        }
                        dbContextTransaction.Commit();
                    }

                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion cachada y lanzada en ProductoLN.Crear", ex);
                    }
                }
            }
            return (mensaje);
        }


        public List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> MostrarTodosRegistradosDeOperador(int idUsuarioOperador)
        {
            List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> res = new List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel>();

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {   //Busco el Id de usuario que necesitare, y ver que este activo
                        var resumenUsuarios = (from x in ctx.Usuario
                                               select new { x.Id, x.Usuario1, x.Activo }).ToList();
                        var usuarioActivoBuscado = resumenUsuarios.SingleOrDefault(item => (item.Id == idUsuarioOperador) &&
                                                                    (item.Activo == true));

                        if(usuarioActivoBuscado != null)
                        {
                            //https://www.syncfusion.com/blogs/post/8-tips-writing-best-linq-to-entities-queries.aspx#use-AsNoTracking
                            var idsDeOperador = (from s in ctx.Producto.AsNoTracking()
                                                 where s.IdUsuarioAlta == idUsuarioOperador
                                                 select s.Id).ToList();


                            var misProductos = (from s in ctx.Producto.AsNoTracking()
                                                where idsDeOperador.Contains(s.Id)
                                                select new
                                                {
                                                    Producto_Id = s.Id,
                                                    Producto_CodigoBarras = s.CodigoBarras,
                                                    Producto_Activo = s.Activo
                                                }).ToList();

                            var miRelacion = (from s in misProductos
                                              join x in ctx.ProductoConDetalles.AsNoTracking()
                                              on s.Producto_Id equals x.IdProducto
                                              select new
                                              {
                                                  s.Producto_Id,
                                                  s.Producto_CodigoBarras,
                                                  s.Producto_Activo,
                                                  ProductoConDetalles_IdDetalles = x.IdDetalleProducto
                                              }).ToList();

                            res = (from d in miRelacion
                                   join x in ctx.DetalleProducto.AsNoTracking()
                                   on d.ProductoConDetalles_IdDetalles equals x.Id
                                   where x.Activo == true
                                   select new ProductoJoinProductoConDetallesJoinDetalleProductoViewModel
                                   {
                                       Producto_Id = d.Producto_Id,
                                       Producto_CodigoBarras = d.Producto_CodigoBarras,
                                       Producto_Activo = d.Producto_Activo,
                                       DetalleProducto_Nombre = x.Nombre
                                   }).ToList();
                        }



                        dbContextTransaction.Commit();
                    }

                    catch(Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion cachada y lanzada en ProductoLN.MostrarTodosLosActivos ", ex);
                    }
                }
            }

            return (res);
        }



        public string Modificar(ProductoJoinProductoConDetallesJoinDetalleProductoViewModel productoConNuevosValores)
        {
            string res = "false";

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {   //Verifico que el Producto este activo
                        var listaIdsYEstado = (from s in ctx.Producto
                                               select new { s.Id, s.Activo }).ToList();

                        var listaIdsActivos = (from item in listaIdsYEstado
                                               where item.Activo == true
                                               select item.Id).ToList();

                        int idBuscado = listaIdsActivos.Find(id => id == productoConNuevosValores.Producto_Id);

                        //Verifico que el DetalleProducto este activo
                        var listaIdsDetalleProducto = (from x in ctx.DetalleProducto
                                                       select new { x.Id, x.Activo }).ToList();

                        var listaIdsDetalleProductoActivos = (from x in listaIdsDetalleProducto
                                                              where x.Activo == true
                                                              select x.Id).ToList();

                        var otroIdBuscado = listaIdsDetalleProductoActivos.Find(item => item == productoConNuevosValores.DetalleProducto_Id);

                        //Busco el Id de usuario que necesitare, y ver que este activo
                        var resumenUsuarios = (from x in ctx.Usuario
                                               select new { x.Id, x.Usuario1, x.Activo }).ToList();
                        var usuarioActivoBuscado = resumenUsuarios.SingleOrDefault(item => (item.Id == productoConNuevosValores.DetalleProducto_IdUsuarioAlta) &&
                                                                    (item.Activo == true));

                        if (  (idBuscado > 0) &&  (otroIdBuscado > 0)  &&  (usuarioActivoBuscado!=null)   ) // Aqui se comprueba lo anterior
                        {
                            var resumenProductoConDetalles = (from s in ctx.ProductoConDetalles
                                                       where s.IdProducto == productoConNuevosValores.Producto_Id
                                                       select new {  s.IdDetalleProducto }).ToList();

                            var resumenDetalleProducto = (from c in ctx.DetalleProducto
                                      select new { c.Id, c.Activo }).ToList();

                            var fila = (from a in resumenProductoConDetalles
                                        join b in resumenDetalleProducto
                                        on a.IdDetalleProducto equals b.Id
                                        where b.Activo == true
                                        select b.Id).ToList();

                            var elementoUnico = fila.Single();

                            DetalleProducto detalleProducto = ctx.DetalleProducto.Find(elementoUnico);
                            detalleProducto.Activo = false;
                            detalleProducto.IdUsuarioModifico = productoConNuevosValores.DetalleProducto_IdUsuarioModifico;
                            detalleProducto.FechaModificacion = productoConNuevosValores.DetalleProducto_FechaModificacion;

                            //crear nuevo
                            var resumenIdsEnDetalleProducto = (from s in resumenDetalleProducto
                                                               select s.Id).ToList();

                            int idNuevoParaDetalleProducto = resumenIdsEnDetalleProducto.Max() + 1;
                            DetalleProducto nuevoDetalleProducto = new DetalleProducto();
                            nuevoDetalleProducto.Id = idNuevoParaDetalleProducto;
                            nuevoDetalleProducto.Nombre = productoConNuevosValores.DetalleProducto_Nombre;
                            nuevoDetalleProducto.Activo = true;
                            nuevoDetalleProducto.IdUsuarioAlta = productoConNuevosValores.DetalleProducto_IdUsuarioAlta;
                            nuevoDetalleProducto.IdUsuarioModifico = productoConNuevosValores.DetalleProducto_IdUsuarioModifico;
                            nuevoDetalleProducto.FechaAlta = productoConNuevosValores.DetalleProducto_FechaAlta;
                            nuevoDetalleProducto.FechaModificacion = productoConNuevosValores.DetalleProducto_FechaModificacion;
                            ctx.DetalleProducto.Add(nuevoDetalleProducto);

                            ProductoConDetalles productoConDetalles = new ProductoConDetalles();
                            productoConDetalles.IdProducto = productoConNuevosValores.Producto_Id;
                            productoConDetalles.IdDetalleProducto = idNuevoParaDetalleProducto;
                            productoConDetalles.Activo = true;
                            productoConDetalles.IdUsuarioAlta = productoConNuevosValores.DetalleProducto_IdUsuarioAlta;
                            productoConDetalles.IdUsuarioModifico = productoConNuevosValores.DetalleProducto_IdUsuarioModifico;
                            productoConDetalles.FechaAlta = productoConNuevosValores.DetalleProducto_FechaAlta;
                            productoConDetalles.FechaModificacion = productoConNuevosValores.DetalleProducto_FechaModificacion;
                            ctx.ProductoConDetalles.Add(productoConDetalles);

                            ctx.SaveChanges();
                            res = "ok";
                        }
                        else
                        {
                            res = "No esta disponible el producto a modificar";
                        }
                        dbContextTransaction.Commit();
                    }

                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion cachada y lanzada en ProductoLN.Modificar", ex);
                    }
                }
            }

            return (res);
        }



        public ProductoJoinProductoConDetallesJoinDetalleProductoViewModel BuscarProductoXCodigoBarras(string codigoBarrasBuscado, int idUsuarioOperador)
        {
            ProductoJoinProductoConDetallesJoinDetalleProductoViewModel productoJoinProductoConDetallesJoinDetalleProductoViewModel = new ProductoJoinProductoConDetallesJoinDetalleProductoViewModel();

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {   //Obtengo info de los productos del usuario operador
                        var listaIds_CodBarras_EstadoProductos = (from s in ctx.Producto
                                                                  where s.IdUsuarioAlta == idUsuarioOperador
                                                                  select new { s.Id, s.CodigoBarras, s.Activo }).ToList();

                        //Obtengo el unico elemento donde esta el codigoBarras buscado 
                        var producto = listaIds_CodBarras_EstadoProductos.SingleOrDefault(item => item.CodigoBarras.Equals(codigoBarrasBuscado) );

                        //Busco el Id de usuario que necesitare, y ver que este activo
                        var resumenUsuarios = (from x in ctx.Usuario
                                               select new { x.Id, x.Usuario1, x.Activo }).ToList();
                        var usuarioActivoBuscado = resumenUsuarios.SingleOrDefault(item => (item.Id == idUsuarioOperador) &&
                                                                    (item.Activo == true));

                        if ((producto != null)   &&   (usuarioActivoBuscado!= null)  )
                        {
                            var relacion = (from prodConDetalles in ctx.ProductoConDetalles
                                            where producto.Id == prodConDetalles.IdProducto
                                            select new
                                            {   producto.Id,
                                                producto.CodigoBarras,
                                                prodConDetalles.IdDetalleProducto
                                            }).ToList();

                            //Debe de regresarse una lista con solo un elemento
                            var productoConDetalles = (from r in relacion
                                                       join d in ctx.DetalleProducto
                                                       on r.IdDetalleProducto equals d.Id
                                                       where d.Activo == true
                                                       select new { r.Id, r.CodigoBarras,r.IdDetalleProducto, d.Nombre }).ToList();

                            var unicoElemento = productoConDetalles.Single();

                            //armo la respuesta final
                            productoJoinProductoConDetallesJoinDetalleProductoViewModel.Producto_Id = unicoElemento.Id;
                            productoJoinProductoConDetallesJoinDetalleProductoViewModel.Producto_CodigoBarras = unicoElemento.CodigoBarras;
                            productoJoinProductoConDetallesJoinDetalleProductoViewModel.DetalleProducto_Id = unicoElemento.IdDetalleProducto;
                            productoJoinProductoConDetallesJoinDetalleProductoViewModel.DetalleProducto_Nombre = unicoElemento.Nombre;
                        }
                        dbContextTransaction.Commit();
                    }

                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion cachada y lanzada en ProductoLN.BuscarProductoXCodigoBarras", ex);
                    }
                }
            }
            return (productoJoinProductoConDetallesJoinDetalleProductoViewModel);
        }



    }
}
