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

            productoViewModel.AjustarAtributosCodigoBarrasYNombre();

            if(productoViewModel.MisAtributosCodbarrasNombreTieneCaracteresPermitidos()  &&  productoViewModel.MisAtributosCodbarrasNombreTieneLongitudPermitida())
            {
                using (var ctx = new palominoEntities())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction(IsolationLevel.Serializable))
                    {
                        try
                        {   //Buscar los codigosBarras del usuario operador
                            var resumenTodosProductos = (from s in ctx.Producto
                                                         select new { s.Id, s.IdUsuarioAlta, s.Activo }).ToList();

                            var resumenProductosUsuario = (from s in resumenTodosProductos
                                                           where s.IdUsuarioAlta == productoViewModel.IdUsuarioAlta
                                                           select s).ToList();

                            var idsActivosUsuario = (from s in resumenProductosUsuario
                                                     where s.Activo == true
                                                     select s.Id).ToList();

                            var codigosBarrasUsuario = (from s in ctx.Producto
                                                        where idsActivosUsuario.Contains(s.Id)
                                                        select s.CodigoBarras).ToList();

                            bool existeProductoEnActivos = productoViewModel.ExisteEn(codigosBarrasUsuario);


                            //Busco el Id de usuario que necesitare, y ver que este activo
                            var resumenUsuarios = (from x in ctx.Usuario
                                                   select new { x.Id, x.Usuario1, x.Activo }).ToList();

                            var usuarioActivoBuscado = resumenUsuarios.SingleOrDefault(item => (item.Id == productoViewModel.IdUsuarioAlta) &&
                                                                                                (item.Activo == true));

                            if (!existeProductoEnActivos && (usuarioActivoBuscado != null))
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
                                mensaje = existeProductoEnActivos == true ? "Ya existe este código de barras en la BD" : "El usuario esta inactivo";
                            }
                            dbContextTransaction.Commit();
                        }

                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception("Excepcion cachada y lanzada en ProductoLN.Crear. " + ex.Message, ex);
                        }
                    }
                }
                return (mensaje);
            }

            else
            { return ("Verifica que el texto de entrada este en el formato permitido");  }
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


        public List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> MostrarActivosDeOperador(int idUsuarioOperador)
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

                        if (usuarioActivoBuscado != null)
                        {
                            //https://www.syncfusion.com/blogs/post/8-tips-writing-best-linq-to-entities-queries.aspx#use-AsNoTracking
                            var idsYEstadoDeProductosDeOperador = (from s in ctx.Producto.AsNoTracking()
                                                                   where s.IdUsuarioAlta == idUsuarioOperador
                                                                   select new { s.Id, s.Activo }).ToList();

                            var idsActivosDeOperador = (from s in idsYEstadoDeProductosDeOperador
                                                        where s.Activo == true
                                                        select s.Id).ToList();

                            var misProductos = (from s in ctx.Producto.AsNoTracking()
                                                where idsActivosDeOperador.Contains(s.Id)
                                                select new
                                                {
                                                    Producto_Id = s.Id,
                                                    Producto_CodigoBarras = s.CodigoBarras,
                                                    Producto_Activo = s.Activo
                                                }).ToList();


                            var miRelacionActivosEInactivos = (from s in misProductos
                                              join x in ctx.ProductoConDetalles.AsNoTracking()
                                              on s.Producto_Id equals x.IdProducto
                                              select new
                                              {
                                                  s.Producto_Id,
                                                  s.Producto_CodigoBarras,
                                                  s.Producto_Activo,
                                                  ProductoConDetalles_IdDetalles = x.IdDetalleProducto,
                                                  ProductoConDetalles_Activo = x.Activo
                                              }).ToList();

                            var miRelacionActivos = (from s in miRelacionActivosEInactivos
                                                     where s.ProductoConDetalles_Activo == true
                                                     select s).ToList();

                            res = (from d in miRelacionActivos
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

                            /*var idsDeOperador = (from s in ctx.Producto.AsNoTracking()
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
                                   }).ToList();   */
                        }


                        
                        dbContextTransaction.Commit();
                    }

                    catch (Exception ex)
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

            //Utilizo el metodo solo para verificar si el nuevo DetalleProducto_Nombre es valido (o sea sin caracteres raros, reducir espacios en blanco si utilizo muchos, si la longitud es correcta)
            productoConNuevosValores.AjustarAtributosCodigoBarrasYNombre();

            if (productoConNuevosValores.MisAtributosCodbarrasNombreTieneCaracteresPermitidos() && productoConNuevosValores.MisAtributosCodbarrasNombreTieneLongitudPermitida())
            {
                using (var ctx = new palominoEntities())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction(IsolationLevel.Serializable))
                    {
                        try
                        {   //verifico que los datos del Producto, ProductoConDetalles que vienen en la solicitud sigan siendo los mismos (con la excepcion del nombre en
                            // DetalleProducto) que estan en la BD 
                            var productoEnBD = ctx.Producto.Find(productoConNuevosValores.Producto_Id);   //regresa null si no lo encontro
                            var productoConDetallesEnBD = ctx.ProductoConDetalles.SingleOrDefault(item => (item.IdProducto == productoConNuevosValores.ProductoConDetalles_IdProducto)
                                && (item.IdDetalleProducto == productoConNuevosValores.ProductoConDetalles_IdDetalleProducto));  //regresa null si no lo encontro
                            var detalleProductoEnBD = ctx.DetalleProducto.Find(productoConNuevosValores.DetalleProducto_Id);  // regresa null si no lo encontro

                            RelojServidor miRelojServidor = new RelojServidor();
                            bool esMismaFechaAltaEnProducto = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(productoEnBD.FechaAlta, productoConNuevosValores.Producto_FechaAlta);
                            bool esMismaFechaModificacionEnProducto = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(productoEnBD.FechaModificacion, productoConNuevosValores.Producto_FechaModificacion);
                            bool esMismoCodigoBarrasEnProducto = productoEnBD.CodigoBarras == productoConNuevosValores.Producto_CodigoBarras;
                            bool esActivoProducto = (productoEnBD.Activo == true) ? true : false;
                            bool esValidoInfoEnProducto = (esMismaFechaAltaEnProducto == true) && (esMismaFechaModificacionEnProducto == true) && (esMismoCodigoBarrasEnProducto == true) && (esActivoProducto == true);

                            bool esMismaFechaAltaEnProductoConDetalles = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(productoConDetallesEnBD.FechaAlta, productoConNuevosValores.ProductoConDetalles_FechaAlta);
                            bool esMismaFechaModificacionEnProductoConDetalles = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(productoConDetallesEnBD.FechaModificacion, productoConNuevosValores.ProductoConDetalles_FechaModificacion);
                            bool esActivoProductoConDetalles = (productoConDetallesEnBD.Activo == true) ? true : false;
                            bool esValidoInfoEnProductoConDetalles = (esMismaFechaAltaEnProductoConDetalles == true) && (esMismaFechaModificacionEnProductoConDetalles == true) && (esActivoProductoConDetalles == true);

                            bool esMismaFechaAltaEnDetalleProducto = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(detalleProductoEnBD.FechaAlta, productoConNuevosValores.DetalleProducto_FechaAlta);
                            bool esMismafechaModificacionEnDetalleProducto = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(detalleProductoEnBD.FechaModificacion, productoConNuevosValores.DetalleProducto_FechaModificacion);
                            bool esActivoDetalleProducto = (detalleProductoEnBD.Activo == true) ? true : false;
                            bool esValidoInfoEnDetalleProducto = (esMismaFechaAltaEnDetalleProducto == true) && (esMismafechaModificacionEnDetalleProducto == true) && (esActivoDetalleProducto == true);



                            if ((productoEnBD != null) && (productoConDetallesEnBD != null) && (detalleProductoEnBD != null))
                            {
                                //Busco el Id de usuario que necesitare, y ver que este activo
                                var resumenUsuarios = (from x in ctx.Usuario
                                                       select new { x.Id, x.Usuario1, x.Activo }).ToList();
                                var usuarioActivoBuscado = resumenUsuarios.SingleOrDefault(item => (item.Id == productoConNuevosValores.DetalleProducto_IdUsuarioAlta) &&
                                                                            (item.Activo == true));
                                // Aqui se comprueba lo anterior
                                if ((esValidoInfoEnProducto) && (esValidoInfoEnProductoConDetalles) && (esValidoInfoEnDetalleProducto) && (usuarioActivoBuscado != null))
                                {
                                    var resumenDetalleProducto = (from c in ctx.DetalleProducto
                                                                  select new { c.Id, c.Activo }).ToList();

                                    DateTime horaActualEnServidor = DateTime.Now;
                                    detalleProductoEnBD.Activo = false;
                                    detalleProductoEnBD.IdUsuarioModifico = productoConNuevosValores.DetalleProducto_IdUsuarioModifico;
                                    detalleProductoEnBD.FechaModificacion = horaActualEnServidor;

                                    productoConDetallesEnBD.Activo = false;
                                    productoConDetallesEnBD.IdUsuarioModifico = productoConNuevosValores.DetalleProducto_IdUsuarioModifico;
                                    productoConDetallesEnBD.FechaModificacion = horaActualEnServidor;

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
                                    nuevoDetalleProducto.FechaAlta = horaActualEnServidor;
                                    nuevoDetalleProducto.FechaModificacion = horaActualEnServidor;
                                    ctx.DetalleProducto.Add(nuevoDetalleProducto);

                                    ProductoConDetalles productoConDetalles = new ProductoConDetalles();
                                    productoConDetalles.IdProducto = productoConNuevosValores.Producto_Id;
                                    productoConDetalles.IdDetalleProducto = idNuevoParaDetalleProducto;
                                    productoConDetalles.Activo = true;
                                    productoConDetalles.IdUsuarioAlta = productoConNuevosValores.DetalleProducto_IdUsuarioAlta;
                                    productoConDetalles.IdUsuarioModifico = productoConNuevosValores.DetalleProducto_IdUsuarioModifico;
                                    productoConDetalles.FechaAlta = horaActualEnServidor;
                                    productoConDetalles.FechaModificacion = horaActualEnServidor;
                                    ctx.ProductoConDetalles.Add(productoConDetalles);

                                    ctx.SaveChanges();
                                    res = "ok";
                                }
                                else
                                {
                                    res = "No esta disponible el producto a modificar";
                                }
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
                            throw new Exception("Excepcion cachada y lanzada en ProductoLN.Modificar" + ex.Message, ex);
                        }
                    }
                }

                return (res);
            }

            else
            { return ("Verifica que el texto de entrada este en el formato permitido"); }
        }



        public ProductoJoinProductoConDetallesJoinDetalleProductoViewModel BuscarProductoXCodigoBarras(string codigoBarrasBuscado, int idUsuarioOperador)
        {
            ProductoJoinProductoConDetallesJoinDetalleProductoViewModel productoJoinProductoConDetallesJoinDetalleProductoViewModel = new ProductoJoinProductoConDetallesJoinDetalleProductoViewModel();

            //Ajusto lo que ingreso el usuario como codigoBarrasBuscado, uso metodos de la clase ProductoViewModel
            ProductoJoinProductoConDetallesJoinDetalleProductoViewModel auxProductoJoinProductoConDetallesJoinDetalleProductoViewModel = new ProductoJoinProductoConDetallesJoinDetalleProductoViewModel();
            auxProductoJoinProductoConDetallesJoinDetalleProductoViewModel.Producto_CodigoBarras = codigoBarrasBuscado;
            auxProductoJoinProductoConDetallesJoinDetalleProductoViewModel.DetalleProducto_Nombre  = "nombre ficticio solo para poder usar los metodos de ProductoJoinProductoConDetallesJoinDetalleProductoViewModel";  //asigno un valor al atributo Nombre, solo para poder usar los metodos de ProductoJoinProductoConDetallesJoinDetalleProductoViewModel
            auxProductoJoinProductoConDetallesJoinDetalleProductoViewModel.AjustarAtributosCodigoBarrasYNombre();


            if (auxProductoJoinProductoConDetallesJoinDetalleProductoViewModel.MisAtributosCodbarrasNombreTieneCaracteresPermitidos() && auxProductoJoinProductoConDetallesJoinDetalleProductoViewModel.MisAtributosCodbarrasNombreTieneLongitudPermitida())
            {
                codigoBarrasBuscado = auxProductoJoinProductoConDetallesJoinDetalleProductoViewModel.Producto_CodigoBarras;   //Aqui ya esta el codigo de barras sin espacios, validado, etc.
                using (var ctx = new palominoEntities())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction(IsolationLevel.Serializable))
                    {
                        try
                        {   //Busco los Ids donde Activo = 1  , del usuario 
                            var listaIds_EstadoProductos = (from c in ctx.Producto
                                                            where c.IdUsuarioAlta == idUsuarioOperador
                                                            select new { c.Id, c.Activo }).ToList();

                            var listaIds_EstadoActivoProductos = (from s in listaIds_EstadoProductos
                                                                  where s.Activo == true
                                                                  select s.Id).ToList();

                            //Obtengo info de los productos del usuario operador
                            var listaIds_CodBarras_EstadoProductos = (from s in ctx.Producto
                                                                      where listaIds_EstadoActivoProductos.Contains(s.Id)
                                                                      select new { s.Id, s.CodigoBarras, s.FechaAlta, s.FechaModificacion, s.Activo }).ToList();


                            //Obtengo el unico elemento donde esta el codigoBarras buscado 
                            var producto = listaIds_CodBarras_EstadoProductos.SingleOrDefault(item => item.CodigoBarras.Equals(codigoBarrasBuscado));

                            //Busco el Id de usuario que necesitare, y ver que este activo
                            var resumenUsuarios = (from x in ctx.Usuario
                                                   select new { x.Id, x.Usuario1, x.Activo }).ToList();
                            var usuarioActivoBuscado = resumenUsuarios.SingleOrDefault(item => (item.Id == idUsuarioOperador) &&
                                                                        (item.Activo == true));

                            if ((producto != null) && (usuarioActivoBuscado != null))
                            {
                                var relacionActivosEInactivos = (from prodConDetalles in ctx.ProductoConDetalles
                                                                 where producto.Id == prodConDetalles.IdProducto
                                                                 select new
                                                                 {
                                                                    Producto_Id = producto.Id,
                                                                    Producto_CodigoBarras = producto.CodigoBarras,
                                                                    Producto_FechaAlta = producto.FechaAlta,
                                                                    Producto_FechaModificacion = producto.FechaModificacion,
                                                                    Producto_Activo = producto.Activo,
                                                                    ProductoConDetalles_IdProducto = prodConDetalles.IdProducto,
                                                                    ProductoConDetalles_IdDetalleProducto = prodConDetalles.IdDetalleProducto,
                                                                    ProductoConDetalles_FechaAlta = prodConDetalles.FechaAlta,
                                                                    ProductoConDetalles_FechaModificacion = prodConDetalles.FechaModificacion,
                                                                    ProductoConDetalles_Activo = prodConDetalles.Activo
                                                                 }).ToList();

                                var relacion = (from s in relacionActivosEInactivos
                                                where s.ProductoConDetalles_Activo == true
                                                select s).ToList();


                                //Debe de regresarse una lista con solo un elemento
                                var productoConDetalles = (from r in relacion
                                                           join d in ctx.DetalleProducto
                                                           on r.ProductoConDetalles_IdDetalleProducto equals d.Id
                                                           where d.Activo == true
                                                           select new { /*r.Id, r.CodigoBarras, r.IdDetalleProducto, d.Nombre*/
                                                               r.Producto_Id, r.Producto_CodigoBarras, r.Producto_FechaAlta, r.Producto_FechaModificacion, r.Producto_Activo,
                                                               r.ProductoConDetalles_IdProducto,  r.ProductoConDetalles_IdDetalleProducto, r.ProductoConDetalles_FechaAlta, 
                                                               r.ProductoConDetalles_FechaModificacion, r.ProductoConDetalles_Activo,
                                                               DetalleProducto_Id = d.Id,
                                                               DetalleProducto_Nombre = d.Nombre,
                                                               DetalleProducto_FechaAlta = d.FechaAlta,
                                                               DetalleProducto_FechaModificacion = d.FechaModificacion,
                                                               DetalleProducto_Activo = d.Activo 
                                                           }).ToList();

                                var unicoElemento = productoConDetalles.Single();

                                //armo la respuesta final
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.Producto_Id = unicoElemento.Producto_Id;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.Producto_CodigoBarras = unicoElemento.Producto_CodigoBarras;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.Producto_FechaAlta = unicoElemento.Producto_FechaAlta;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.Producto_FechaModificacion = unicoElemento.Producto_FechaModificacion;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.Producto_Activo = unicoElemento.Producto_Activo;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.ProductoConDetalles_IdProducto = unicoElemento.ProductoConDetalles_IdProducto;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.ProductoConDetalles_IdDetalleProducto = unicoElemento.ProductoConDetalles_IdDetalleProducto;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.ProductoConDetalles_FechaAlta = unicoElemento.ProductoConDetalles_FechaAlta;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.ProductoConDetalles_FechaModificacion = unicoElemento.ProductoConDetalles_FechaModificacion;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.ProductoConDetalles_Activo = unicoElemento.ProductoConDetalles_Activo;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.DetalleProducto_Id = unicoElemento.DetalleProducto_Id;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.DetalleProducto_Nombre = unicoElemento.DetalleProducto_Nombre;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.DetalleProducto_FechaAlta = unicoElemento.DetalleProducto_FechaAlta;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.DetalleProducto_FechaModificacion = unicoElemento.DetalleProducto_FechaModificacion;
                                productoJoinProductoConDetallesJoinDetalleProductoViewModel.DetalleProducto_Activo = unicoElemento.DetalleProducto_Activo;
                            }
                            dbContextTransaction.Commit();
                        }

                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception("Excepcion cachada y lanzada en ProductoLN.BuscarProductoXCodigoBarras. " + ex.Message, ex);
                        }
                    }
                }
            }

            else { throw new Exception("Verifique el formato de entrada");  }

            return (productoJoinProductoConDetallesJoinDetalleProductoViewModel);
        }



    }
}
