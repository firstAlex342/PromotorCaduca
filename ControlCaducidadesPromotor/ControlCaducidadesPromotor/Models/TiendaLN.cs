using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControlCaducidadesPromotor.Basicos;
using ControlCaducidadesPromotor.AccesoADatos;

namespace ControlCaducidadesPromotor.Models
{
    public class TiendaLN
    {
        public List<TiendaViewModel> MostrarTodasTiendasDeUsuario(string usuario)
        {
            List<TiendaViewModel> tiendasViewModel = new List<TiendaViewModel>();

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        //Obtengo el id del usuario del que necesitare info y veo si esta activo al momento de esta consulta
                        var resumenUsuarios = (from s in ctx.Usuario.AsNoTracking()
                                           select new { s.Id, s.Usuario1, s.Activo }).ToList();

                        var usuarioBuscado = resumenUsuarios.SingleOrDefault(item => (item.Usuario1 == usuario)   &&  (item.Activo == true));

                        if(usuarioBuscado != null)
                        {
                            var idUsuario = usuarioBuscado.Id;

                            //Operaciones comunes
                            var lista = (from x in ctx.Tienda.AsNoTracking()
                                         select new
                                         {
                                             x.Id,
                                             x.IdUsuarioAlta, 
                                             x.Activo
                                         }).ToList();

                            var idsNecesitados = (from item in lista
                                                  where (item.IdUsuarioAlta == idUsuario) && (item.Activo == true)
                                                  select item.Id).ToList();

                            tiendasViewModel = (from s in ctx.Tienda
                                                where idsNecesitados.Contains(s.Id)
                                                select new TiendaViewModel
                                                {
                                                    Id = s.Id,
                                                    Supmza = s.Supmza,
                                                    Manzana = s.Manzana,
                                                    Lote = s.Lote,
                                                    Calle = s.Calle,
                                                    Nombre = s.Nombre,
                                                    IdUsuarioAlta = s.IdUsuarioAlta,
                                                    FechaAlta = s.FechaAlta,
                                                    IdUsuarioModifico = s.IdUsuarioModifico,
                                                    FechaModificacion = s.FechaModificacion,
                                                    Activo = s.Activo
                                                }).ToList();
                        }

                        dbContextTransaction.Commit();
                    }


                    catch(Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion cachada y lanzada en TiendaLN.MostrarTodasTiendasDeUsuario", ex);
                    }
                }
            }

            return (tiendasViewModel);
        }


        public string PostCrearTienda(TiendaViewModel tiendaViewModel)
        {
            string respuesta = "";

            using (var ctx = new palominoEntities())
            {
                using(var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {   //Ver si existe un nombre de tienda similar, en lo registros tienda del usuarioOperador, si existe no hago algo
                        var idsDeUsuarioOperador = (from item in ctx.Tienda
                                                    where item.IdUsuarioAlta == tiendaViewModel.IdUsuarioAlta
                                                    select item.Id).ToList();

                        var s = (from item in ctx.Tienda
                                 where idsDeUsuarioOperador.Contains(item.Id)
                                 select item.Nombre).ToList();

                        bool existeNombreDeTiendaEnBD = s.Contains(tiendaViewModel.Nombre);


                        //Busco el Id de usuario que necesitare, y ver que este activo
                        var resumenUsuarios = (from x in ctx.Usuario
                                               select new { x.Id, x.Usuario1, x.Activo }).ToList();

                        var usuarioActivoBuscado = resumenUsuarios.SingleOrDefault(item => (item.Id == tiendaViewModel.IdUsuarioAlta)  && 
                                                                                            (item.Activo == true)  );

                        if ((existeNombreDeTiendaEnBD == false)   &&    (usuarioActivoBuscado != null)    )
                        {
                            var ids = (from item in ctx.Tienda
                                       select item.Id).ToList();

                            var nuevoId = ids.Count == 0 ? 1 : (ids.Max() + 1);

                            Tienda tienda = new Tienda();
                            tienda.Id = nuevoId;
                            tienda.Nombre = tiendaViewModel.Nombre;
                            tienda.Supmza = tiendaViewModel.Supmza;
                            tienda.Manzana = tiendaViewModel.Manzana;
                            tienda.Lote = tiendaViewModel.Lote;
                            tienda.Calle = tiendaViewModel.Calle;
                            tienda.IdUsuarioAlta = tiendaViewModel.IdUsuarioAlta;
                            tienda.IdUsuarioModifico = tiendaViewModel.IdUsuarioAlta;
                            tienda.FechaAlta = tiendaViewModel.FechaAlta;
                            tienda.FechaModificacion = tiendaViewModel.FechaModificacion;
                            tienda.Activo = true;

                            ctx.Tienda.Add(tienda);
                            ctx.SaveChanges();                           
                            respuesta = "ok";
                        }

                        else
                        {
                            respuesta = "Ya existe una tienda con un nombre similar ó tu nombre de usuario esta inactivo";
                        }

                        dbContextTransaction.Commit();
                    }

                    catch(Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepción lanzada y cachada en TiendaLN.PostCrearTienda",ex);
                    }
                }
            }

            return (respuesta);
        }




        public TiendaViewModel Get_BuscarTiendaDeUsuarioXNombre(TiendaViewModel tiendaViewModel)
        {
            TiendaViewModel tiendaBuscadaVM = null;

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        //Busco el Id de usuario que envio la solicitud, y ver que este activo
                        var resumenUsuarios = (from x in ctx.Usuario
                                               select new { x.Id, x.Usuario1, x.Activo }).ToList();

                        var usuarioActivoBuscado = resumenUsuarios.SingleOrDefault(item => (item.Id == tiendaViewModel.IdUsuarioAlta) &&
                                                                                            (item.Activo == true));

                        if(usuarioActivoBuscado != null)
                        {
                            //Busco las tiendas del usuario operador
                            var ids_Nombre_EstadoTiendas = (from s in ctx.Tienda
                                                            select new { s.Id, s.Nombre, s.IdUsuarioAlta }).ToList();

                            var ids_Nombre_Estado_TiendasDeUsuario = (from s in ids_Nombre_EstadoTiendas
                                                                     where s.IdUsuarioAlta == tiendaViewModel.IdUsuarioAlta
                                                                     select s).ToList();
                            //Dentro de las tiendas del usuario operador, busco la tienda que me interesa
                            var resumenTiendaBuscada = ids_Nombre_Estado_TiendasDeUsuario.SingleOrDefault(item => item.Nombre == tiendaViewModel.Nombre);
                            if(resumenTiendaBuscada != null)
                            {
                                Tienda tiendaX=  ctx.Tienda.Find(resumenTiendaBuscada.Id);
                                tiendaBuscadaVM = new TiendaViewModel();
                                tiendaBuscadaVM.Id = tiendaX.Id;
                                tiendaBuscadaVM.Nombre = tiendaX.Nombre;
                                tiendaBuscadaVM.Supmza = tiendaX.Supmza;
                                tiendaBuscadaVM.Manzana = tiendaX.Manzana;
                                tiendaBuscadaVM.Lote = tiendaX.Lote;
                                tiendaBuscadaVM.Calle = tiendaX.Calle;
                                tiendaBuscadaVM.IdUsuarioAlta = tiendaX.IdUsuarioAlta;
                                tiendaBuscadaVM.FechaAlta = tiendaX.FechaAlta;
                                tiendaBuscadaVM.IdUsuarioModifico = tiendaX.IdUsuarioModifico;
                                tiendaBuscadaVM.FechaModificacion = tiendaX.FechaModificacion;
                                tiendaBuscadaVM.Activo = tiendaX.Activo;
                            }

                        }

                        dbContextTransaction.Commit();
                    }

                    catch(Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepción lanzada y cachada en TiendaLN.Get_BuscarTiendaDeUsuarioXNombre", ex);
                    }
                }
            }

            return (tiendaBuscadaVM);
        }



        public string Put_ActualizarTiendaDeUsuario(TiendaViewModel tiendaViewModel)
        {
            string respuesta = "";

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        //Busco el Id de usuario que envio la solicitud, y ver que este activo
                        var resumenUsuarios = (from x in ctx.Usuario
                                               select new { x.Id, x.Usuario1, x.Activo }).ToList();

                        var usuarioActivoBuscado = resumenUsuarios.SingleOrDefault(item => (item.Id == tiendaViewModel.IdUsuarioAlta) &&
                                                                                            (item.Activo == true));
                        if(usuarioActivoBuscado != null)
                        {
                            //Buscar la tienda almacenada en la BD y ver que este activa
                            Tienda tiendaOriginal = ctx.Tienda.Find(tiendaViewModel.Id);
                            if (tiendaOriginal != null)
                            {   //Veo que la tienda este activo
                                if(tiendaOriginal.Activo == true)
                                {
                                    //Verifico que la tiendaViewModel que viene llegando en la solicitud( que es la que este viendo el usuario)
                                    //sea la que esta almacenada en la bd, a través de la fechas y hora
                                    RelojServidor relojServidor = new RelojServidor();
                                    if(   (relojServidor.EsMismaFechaYHoraSinMilisegundos(tiendaViewModel.FechaAlta, tiendaOriginal.FechaAlta)) 
                                        &&    (relojServidor.EsMismaFechaYHoraSinMilisegundos(tiendaViewModel.FechaModificacion, tiendaOriginal.FechaModificacion))  )
                                    {
                                        //Ver que el nombre de tienda que viene en la solicitud este disponible
                                        var resumenTiendas = (from s in ctx.Tienda
                                                             select new { s.Id, s.Nombre, s.IdUsuarioAlta }).ToList();

                                        var tiendasDeUsuarioOperador = (from s in resumenTiendas
                                                                        where s.IdUsuarioAlta == tiendaViewModel.IdUsuarioAlta
                                                                        select s).ToList();

                                        var nombresAVerificar = (from s in tiendasDeUsuarioOperador
                                                                 where s.Id != tiendaViewModel.Id
                                                                 select s.Nombre).ToList();

                                        if(nombresAVerificar.Contains(tiendaViewModel.Nombre) == false )
                                        {
                                            //Modifico
                                            tiendaOriginal.Supmza = tiendaViewModel.Supmza;
                                            tiendaOriginal.Manzana = tiendaViewModel.Manzana;
                                            tiendaOriginal.Lote = tiendaViewModel.Lote;
                                            tiendaOriginal.Calle = tiendaViewModel.Calle;
                                            tiendaOriginal.Nombre = tiendaViewModel.Nombre;
                                            tiendaOriginal.IdUsuarioAlta = tiendaViewModel.IdUsuarioAlta;
                                            tiendaOriginal.IdUsuarioModifico = tiendaViewModel.IdUsuarioModifico;
                                            tiendaOriginal.FechaAlta = tiendaOriginal.FechaAlta;
                                            tiendaOriginal.FechaModificacion = relojServidor.RegresarHoraEnServidor();
                                            tiendaOriginal.Activo = tiendaViewModel.Activo;
                                            ctx.SaveChanges();

                                            respuesta = "ok";
                                        }

                                        else
                                        {
                                            respuesta = "El nombre  de la tienda ya esta en uso";
                                        }
                                    }

                                    else
                                    {
                                        respuesta = "La tienda no se encuentra disponible en este momento";
                                    }
                                }

                                else
                                {
                                    respuesta = "La tienda no se encuentra disponible en este momento";
                                }
                            }

                            else
                            {
                                respuesta = "La tienda no esta activa en este momento";
                            }
                        }

                        else
                        {
                            respuesta = "El usuario operador no esta activo en este momento";
                        }
                        
                        dbContextTransaction.Commit();
                    }

                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepción lanzada y cachada en TiendaLN.Put_ActualizarTienda", ex);
                    }
                }
            }

            return (respuesta);
        }



        public List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM>  Get_RecuperarProductosDeTienda(int idTienda, string usuario)
        {
            List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM> resViewModel = new List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM>();

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        //Obtengo el id del usuario del que necesitare info y veo si esta activo al momento de esta consulta
                        var resumenUsuarios = (from s in ctx.Usuario.AsNoTracking()
                                               select new { s.Id, s.Usuario1, s.Activo }).ToList();

                        var usuarioBuscado = resumenUsuarios.SingleOrDefault(item => (item.Usuario1 == usuario) && (item.Activo == true));

                        //Verifico que la tienda este activa al momento de esta consulta
                        Tienda tiendaBuscada = ctx.Tienda.Find(idTienda);

                        if ((usuarioBuscado != null) && (tiendaBuscada.Activo == true)) //Aqui se comprueba
                        {
                            var idUsuario = usuarioBuscado.Id;

                            var resumenAlmacena = (from s in ctx.Almacena
                                                   where (s.IdTienda == idTienda) && (s.Activo == true)
                                                   select new { s.IdTienda, s.IdProducto, s.FechaAlta, s.FechaModificacion }).ToList();

                            var idsDeProductosMeInteresan = (from s in ctx.Almacena
                                                             where (s.IdTienda == idTienda) && (s.Activo == true)
                                                             select s.IdProducto).ToList();  //obtengo los ids de los productos de la tienda requerida

                            var resumenProductos = (from s in ctx.Producto
                                                    where idsDeProductosMeInteresan.Contains(s.Id)
                                                    select new { s.Id, s.CodigoBarras, s.Activo }).ToList();  //De la tabla Producto obtengo el Id y el codigoBarras de los productos que me interesan

                            var resumenProductoConDetalles = (from s in ctx.ProductoConDetalles
                                                             where idsDeProductosMeInteresan.Contains(s.IdProducto)   //De la tabla ProductoConDetalles obtengo los IdProducto e IdDetalleProducto que me interesan 
                                                             select new { s.IdProducto, s.IdDetalleProducto, s.Activo }).ToList();


                            var idsDetalleProductoMeInteresan = (from s in resumenProductoConDetalles
                                                                select s.IdDetalleProducto).ToList();

                            var resumenDetalleProducto = (from s in ctx.DetalleProducto
                                                          where idsDetalleProductoMeInteresan.Contains(s.Id)
                                                          select new { s.Id, s.Nombre, s.IdUsuarioAlta, s.Activo }).ToList();    //De la tabla DetalleProducto obtengo las filas que me interesan

                            //Ya esta la info de las tablas fuera de EntityFramework, esta ahora en la memoria, asi que procedo a enlazar
                            //los registros tomados de las tablas Producto, ProductoConDetalles Y DetallesProducto; todos tiene el campo Activo = 1
                            var resumenAlmacenaJOINresumenProductos = (from s in resumenAlmacena
                                                                       join c in resumenProductos
                                                                       on s.IdProducto equals c.Id
                                                                       select new
                                                                       {
                                                                           Almacena_IdTienda = s.IdTienda,
                                                                           Almacena_IdProducto = s.IdProducto,
                                                                           Almacena_FechaAlta = s.FechaAlta,
                                                                           Almacena_FechaModificacion = s.FechaModificacion,
                                                                           Producto_Id = c.Id,
                                                                           Producto_CodigoBarras = c.CodigoBarras,
                                                                           Producto_Activo = c.Activo
                                                                       }).ToList();

                            var resumenAlmacenaJOINresumenProductosJOINresumenProductoConDetalles = (from s in resumenAlmacenaJOINresumenProductos
                                                                                                     join c in resumenProductoConDetalles
                                                                                                     on s.Producto_Id equals c.IdProducto
                                                                                                     select new
                                                                                                     {
                                                                                                         s.Almacena_IdTienda,
                                                                                                         s.Almacena_IdProducto,
                                                                                                         s.Almacena_FechaAlta,
                                                                                                         s.Almacena_FechaModificacion,
                                                                                                         s.Producto_Id,
                                                                                                         s.Producto_CodigoBarras,
                                                                                                         s.Producto_Activo,
                                                                                                         ProductoConDetalles_IdProducto = c.IdProducto,
                                                                                                         ProductoConDetalles_IdDetalleProducto = c.IdDetalleProducto
                                                                                                     }).ToList();

                            var resumenAlmacenaJOINresumenProductosJOINresumenProductoConDetallesJOINDetalleProducto = (from s in resumenAlmacenaJOINresumenProductosJOINresumenProductoConDetalles
                                                                                                                        join x in resumenDetalleProducto
                                                                                                                        on s.ProductoConDetalles_IdDetalleProducto equals x.Id
                                                                                                                        where x.Activo == true  //<--------aqui me quede
                                                                                                                        select new
                                                                                                                        {
                                                                                                                            s.Almacena_IdTienda,
                                                                                                                            s.Almacena_IdProducto,
                                                                                                                            s.Almacena_FechaAlta,
                                                                                                                            s.Almacena_FechaModificacion,
                                                                                                                            s.Producto_Id,
                                                                                                                            s.Producto_CodigoBarras,
                                                                                                                            s.Producto_Activo,
                                                                                                                            s.ProductoConDetalles_IdProducto,
                                                                                                                            s.ProductoConDetalles_IdDetalleProducto,
                                                                                                                            DetalleProducto_Id = x.Id,
                                                                                                                            DetalleProducto_Nombre = x.Nombre,
                                                                                                                            DetalleProducto_IdUsuarioAlta = x.IdUsuarioAlta,
                                                                                                                            DetalleProducto_Activo = x.Activo
                                                                                                                        }).ToList();

                            resViewModel = (from c in resumenAlmacenaJOINresumenProductosJOINresumenProductoConDetallesJOINDetalleProducto
                                            select new AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM
                                            {
                                                Almacena_IdTienda = idTienda,
                                                Almacena_IdProducto = c.Almacena_IdProducto,
                                                Almacena_FechaAlta = c.Almacena_FechaAlta,
                                                Almacena_FechaModificacion = c.Almacena_FechaModificacion,
                                                Producto_Id = c.Producto_Id,
                                                Producto_CodigoBarras = c.Producto_CodigoBarras,
                                                ProductoConDetalles_IdProducto = c.ProductoConDetalles_IdProducto,
                                                ProductoConDetalles_IdDetalleProducto = c.ProductoConDetalles_IdDetalleProducto,
                                                DetalleProducto_Id = c.DetalleProducto_Id,
                                                DetalleProducto_Nombre = c.DetalleProducto_Nombre,
                                                DetalleProducto_IdUsuarioAlta = c.DetalleProducto_IdUsuarioAlta,
                                            }).ToList();  //<---este es el fin de mi nueva versión


                            //var resumenProductosJOINresumenProductoConDetalles = (from s in resumenProductos
                            //                                                      join x in resumenProductoConDetalles
                            //                                                       on s.Id equals x.IdProducto
                            //                                                      where (s.Activo == true) && (x.Activo == true)
                            //                                                     select new { s.Id, s.CodigoBarras, x.IdProducto, x.IdDetalleProducto }).ToList();

                            //var resumenProductosJOINresumenProductoConDetallesJOINresumenDetalleProducto = (from s in resumenProductosJOINresumenProductoConDetalles
                            //                                                                                join x in resumenDetalleProducto
                            //                                                                                on s.IdDetalleProducto equals x.Id
                            //                                                                                where x.Activo == true
                            //                                                                                select new 
                            //                                                                                { s.Id, s.CodigoBarras, s.IdProducto, s.IdDetalleProducto,
                            //                                                                                DetalleProd_Id = x.Id, x.Nombre, x.IdUsuarioAlta
                            //                                                                                }).ToList();

                            //resViewModel = (from s in resumenProductosJOINresumenProductoConDetallesJOINresumenDetalleProducto
                            //                select new AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM
                            //                {
                            //                    Almacena_IdTienda = idTienda,
                            //                    Almacena_IdProducto = s.Id,
                            //                    Producto_Id = s.Id,
                            //                    Producto_CodigoBarras = s.CodigoBarras,
                            //                    ProductoConDetalles_IdProducto = s.IdProducto,
                            //                    ProductoConDetalles_IdDetalleProducto = s.IdDetalleProducto,
                            //                    DetalleProducto_Id = s.DetalleProd_Id,
                            //                    DetalleProducto_Nombre = s.Nombre,
                            //                    DetalleProducto_IdUsuarioAlta = s.IdUsuarioAlta
                            //                }).ToList();  //coloco el resultado en un objetoViewModel
                        }

                        dbContextTransaction.Commit();
                    }

                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion cachada y lanzada en TiendaLN.Get_RecuperarProductosDeTienda", ex);
                    }
                }
            }

            return (resViewModel);
        }


        public List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> Get_RecuperarProductosNoPertenecenATienda(int idTienda, string usuario)
        {
            List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> resViewModel = new List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel>();

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        //Obtengo el id del usuario del que necesitare info y veo si esta activo al momento de esta consulta
                        var resumenUsuarios = (from s in ctx.Usuario.AsNoTracking()
                                               select new { s.Id, s.Usuario1, s.Activo }).ToList();

                        var usuarioBuscado = resumenUsuarios.SingleOrDefault(item => (item.Usuario1 == usuario) && (item.Activo == true));

                        //Verifico que la tienda este activa al momento de esta consulta
                        Tienda tiendaBuscada = ctx.Tienda.Find(idTienda);

                        if ((usuarioBuscado != null) && (tiendaBuscada.Activo == true)) //Aqui se comprueba
                        {   //Extraigo todos los productos que estan enlazados a la tienda en la tabla Almacena
                            var resumenActivosEInactivos = (from s in ctx.Almacena
                                                            where s.IdTienda == idTienda
                                                            select new { s.IdTienda, s.IdProducto, s.Activo }).ToList();

                            //Extraigo los ids delos productos que están activos en la tienda
                            var resumenIdsActivos = (from s in resumenActivosEInactivos
                                                     where s.Activo == true
                                                     select s.IdProducto).ToList();

                            //Extraigo los ids de los productos activos en la tabla Producto que pertenecen al usuario
                            var resumenTodosProductos = (from s in ctx.Producto
                                                         select new { s.Id, s.IdUsuarioAlta, s.Activo }).ToList();

                            var idsProductosActivos = (from s in resumenTodosProductos
                                                       where (s.Activo == true) && (s.IdUsuarioAlta == usuarioBuscado.Id)
                                                       select s.Id).ToList();

                            //Realizo una operacion para conocer los ids de los productos de los que necesito sus detalles
                            var idsProductoQueNoEstanEnTienda = (idsProductosActivos.Except(resumenIdsActivos)).ToList();

                            //Comienzo a obtener los detalles de los productos que necesitare
                            var resumenProductosAEnlazar = (from s in ctx.Producto
                                                            where idsProductoQueNoEstanEnTienda.Contains(s.Id)
                                                            select new { s.Id, s.CodigoBarras, s.FechaAlta, s.FechaModificacion }).ToList();
                        
                            var resumenProductoConDetallesAEnlazar = (from s in ctx.ProductoConDetalles
                                                         where idsProductoQueNoEstanEnTienda.Contains(s.IdProducto)
                                                         select new { s.IdProducto, s.IdDetalleProducto, s.Activo }).ToList();

                            var idsDetallesProductoNecesitare = (from s in resumenProductoConDetallesAEnlazar
                                                                 select s.IdDetalleProducto).ToList();

                            var resumenDetalleProductoAEnlazar = (from s in ctx.DetalleProducto
                                                                  where idsDetallesProductoNecesitare.Contains(s.Id)
                                                                  select new { s.Id, s.Nombre, s.FechaAlta, s.FechaModificacion, s.Activo }).ToList();

                            var productoJOINproductoConDetalles = (from a in resumenProductosAEnlazar
                                             join b in resumenProductoConDetallesAEnlazar
                                             on a.Id equals b.IdProducto
                                             select new
                                             {
                                                 a.Id,
                                                 a.CodigoBarras,
                                                 a.FechaAlta,
                                                 a.FechaModificacion,
                                                 b.IdProducto,
                                                 b.IdDetalleProducto,
                                                 b.Activo
                                             }).ToList();

                            var productoJOINproductoConDetallesJOINDetalleProducto = (from a in productoJOINproductoConDetalles
                                                                                      join b in resumenDetalleProductoAEnlazar
                                                                                      on a.IdDetalleProducto equals b.Id
                                                                                      where b.Activo == true
                                                                                      select new
                                                                                      {
                                                                                          a.Id,
                                                                                          a.CodigoBarras,
                                                                                          a.FechaAlta,
                                                                                          a.FechaModificacion,
                                                                                          a.IdProducto,
                                                                                          a.IdDetalleProducto,
                                                                                          a.Activo,
                                                                                          DetalleProducto_Id = b.Id,
                                                                                          DetalleProducto_Nombre = b.Nombre,
                                                                                          DetalleProducto_FechaAlta = b.FechaAlta,
                                                                                          DetalleProducto_FechaModificacion = b.FechaModificacion,
                                                                                          DetalleProducto_Activo = b.Activo
                                                                                      }).ToList();
                            //falta vaciar resultado en resViewModel
                            resViewModel = (from s in productoJOINproductoConDetallesJOINDetalleProducto
                                            select new ProductoJoinProductoConDetallesJoinDetalleProductoViewModel
                                            {
                                                Producto_Id = s.Id,
                                                Producto_CodigoBarras = s.CodigoBarras,
                                                Producto_FechaAlta = s.FechaAlta,
                                                Producto_FechaModificacion = s.FechaModificacion,
                                                ProductoConDetalles_IdProducto = s.IdProducto,
                                                ProductoConDetalles_IdDetalleProducto = s.IdDetalleProducto,
                                                ProductoConDetalles_Activo = s.Activo,
                                                DetalleProducto_Id = s.DetalleProducto_Id,
                                                DetalleProducto_Nombre = s.DetalleProducto_Nombre,
                                                DetalleProducto_FechaAlta = s.DetalleProducto_FechaAlta,
                                                DetalleProducto_FechaModificacion = s.DetalleProducto_FechaModificacion,
                                                DetalleProducto_Activo = s.DetalleProducto_Activo
                                            }).ToList();
                        }

                        dbContextTransaction.Commit();
                    }

                    catch(Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion lanzada y cachada en TiendaLN.Get_RecuperarProductosNoPertenecenATienda",ex);
                    }
                }
            }

            return (resViewModel);
        }


        public string Post_AgregarProductosATienda(List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM>  elementosOrigen)
        {
            string respuesta = "inicializar..";

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        //Ver que la tienda este activa 
                        AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM filaUnica = elementosOrigen.First();
                        var idsTienda = (from s in ctx.Tienda
                                        select new { s.Id, s.Activo }).ToList();

                        var idsTiendaActivos = (from s in idsTienda
                                               where s.Activo.Equals(true)
                                               select s.Id).ToList();

                        bool esTiendaActiva = idsTiendaActivos.Contains(filaUnica.Almacena_IdTienda);

                        //Ver que el usuario operador este activo
                        var resumenUsuarios = (from x in ctx.Usuario
                                               select new { x.Id, x.Usuario1, x.Activo }).ToList();

                        var resumenUsuariosActivos = (from s in resumenUsuarios
                                                      where s.Activo == true
                                                      select s.Id).ToList();

                        bool esUsuarioOperadorActivo = resumenUsuariosActivos.Contains(filaUnica.DetalleProducto_IdUsuarioAlta);

                        if(esTiendaActiva && esUsuarioOperadorActivo) //Aqui compruebo que la tienda y el usuario operador están activos
                        {   
                            //Extraigo de la tabla Almacena las relaciones activas que pertenezcan al usuario operador
                            var resumenAlmacena = (from s in ctx.Almacena
                                                  select new { s.IdTienda, s.IdProducto, s.IdUsuarioAlta, s.Activo }).ToList();

                            var resumenRelacionesUsuarioOperadorEnAlmacena = (from s in resumenAlmacena
                                                                              where (s.IdUsuarioAlta == filaUnica.DetalleProducto_IdUsuarioAlta) && (s.Activo == true)
                                                                              select s).ToList();


                            for (int i=0; i< elementosOrigen.Count; i++)
                            {
                                var itemDeOrigen = elementosOrigen[i];

                                //Verifico que en la tabla Almacena lo que el usuario ve, sea lo que esta al momento de ejecutar esta consulta
                                var elementoBuscado = resumenRelacionesUsuarioOperadorEnAlmacena.FirstOrDefault(s=> {
                                    return ((s.IdTienda == itemDeOrigen.Almacena_IdTienda) && (s.IdProducto == itemDeOrigen.Almacena_IdProducto) 
                                    && (s.Activo == true)
                                    );
                                });


                                if (elementoBuscado != null)  
                                {
                                    //Verifico ahora que en la tabla Producto, lo que ve el usuario sea lo que este al momento de realizar la consulta
                                    var idsProductos = (from s in ctx.Producto
                                                       select new { s.Id, s.Activo }).ToList();

                                    var idsProductosActivos = (from s in idsProductos
                                                               where s.Activo == true
                                                               select s.Id).ToList();

                                    bool esProductoActivo = idsProductosActivos.Contains(itemDeOrigen.Producto_Id);
                                    if(esProductoActivo == true)
                                    {
                                        //Verifico que en la tabla ProductoConDetalles, lo que ve el usuario sea lo que este al momento de realizar la consulta
                                        var resumenProductoConDetalles = (from s in ctx.ProductoConDetalles
                                                select new { s.IdProducto, s.IdDetalleProducto, s.Activo }).ToList();

                                        var resumenActivosProductoConDetalles = (from s in resumenProductoConDetalles
                                                                                 where s.Activo == true
                                                                                 select s).ToList();

                                        var relacionBuscadaEnProductoConDetalles = resumenActivosProductoConDetalles.FirstOrDefault(s => 
                                        (s.IdProducto == itemDeOrigen.ProductoConDetalles_IdProducto) && (s.IdDetalleProducto == itemDeOrigen.ProductoConDetalles_IdDetalleProducto)  );

                                        if(relacionBuscadaEnProductoConDetalles != null)
                                        {
                                            //Verifico que en la tabla DetalleProducto, lo que ve el usuario sea lo que este al momento de realizar la consulta
                                            var resumenDetalleProducto = (from s in ctx.DetalleProducto
                                                                          select new { s.Id, s.Activo }).ToList();

                                            var idsDetalleProductoActivos = (from s in resumenDetalleProducto
                                                                             where s.Activo == true
                                                                             select s.Id).ToList();

                                            bool esDetalleProductoElCorrecto = idsDetalleProductoActivos.Contains(itemDeOrigen.DetalleProducto_Id);
                                            if(esDetalleProductoElCorrecto)
                                            {
                                                //Todo esta bien los valores del objeto itemDeOrigen, coinciden los que el usuario ve en su pantalla con los
                                                //que estan en la BD
                                            }

                                            else
                                            {
                                                respuesta = "La información que usted ve en pantalla ha sido actualizada, recargue la página";
                                                break;
                                            }
                                        }

                                        else
                                        {
                                            respuesta = "La información que usted ve en pantalla ha sido actualizada, recargue la página";
                                            break;
                                        }
                                    }

                                    else
                                    {
                                        respuesta = "La información que usted ve en pantalla ha sido actualizada, recargue la página";
                                        break;
                                    }
                                }

                                else
                                {
                                    //No existe la relacion ó existe y esta inactiva  averiguarlo.....

                                    //Existe la relación, pero esta inactiva
                                    var resumenTodasRelacionesEnAlmacena = (from s in ctx.Almacena
                                                                          select new { s.IdTienda, s.IdProducto, s.FechaAlta, s.FechaModificacion, s.Activo }).ToList();

                                    var relacionBuscadaEnAlmacena = resumenTodasRelacionesEnAlmacena.FirstOrDefault(s =>
                                    (s.IdTienda == itemDeOrigen.Almacena_IdTienda) && (s.IdProducto == itemDeOrigen.Almacena_IdProducto) && (s.Activo == false) );

                                    if(relacionBuscadaEnAlmacena != null) //Se comprueba que la relación existe
                                    {   
                                        //Verifico que lo que ve el usuario en pantalla de la tabla Producto, este activo al moment de esta consulta
                                        var resumenEnProducto = (from s in ctx.Producto
                                                                 select new { s.Id, s.Activo }).ToList();

                                        var idsActivosEnProducto = (from s in resumenEnProducto
                                                                    where s.Activo == true
                                                                    select s.Id).ToList();

                                        if( (idsActivosEnProducto.Contains(itemDeOrigen.Producto_Id)) == true )
                                        {
                                            //Verifico que lo que ve en pantalla el usuario de la tabla ProductoConDetalles, este activo al momento de la consulta
                                            var resumenProductoConDetalles = (from s in ctx.ProductoConDetalles
                                                                              select new { s.IdProducto, s.IdDetalleProducto, s.Activo }).ToList();

                                            var idsActivosProductoConDetalles = (from s in resumenProductoConDetalles
                                                                                 where s.Activo == true
                                                                                 select new { s.IdProducto, s.IdDetalleProducto }).ToList();

                                            var relacionBuscadaEnProductoConDetalles = idsActivosProductoConDetalles.FirstOrDefault(s =>
                                            (s.IdProducto == itemDeOrigen.ProductoConDetalles_IdProducto) && (s.IdDetalleProducto == itemDeOrigen.ProductoConDetalles_IdDetalleProducto));

                                            if(relacionBuscadaEnProductoConDetalles != null)
                                            {
                                                //Verifico que lo que ve en pantalla el usuario de la tabla DetalleProducto, ese activo al momento de la consulta
                                                var resumenDetalleProducto = (from s in ctx.DetalleProducto
                                                                              select new { s.Id, s.Activo }).ToList();

                                                var idsActivosDetalleProducto = (from s in resumenDetalleProducto
                                                                                 where s.Activo == true
                                                                                 select s.Id).ToList();

                                                if(  idsActivosDetalleProducto.Contains(itemDeOrigen.DetalleProducto_Id) )
                                                {
                                                    //Veo una ultima condición para saber si de debe reactivar la relacion
                                                    DateTime fechaHoraCentinela = new DateTime(1985, 9, 15, 23, 59, 59);
                                                    RelojServidor miRelojServidor = new RelojServidor();

                                                    if ( miRelojServidor.EsMismaFechaYHoraSinMilisegundos(itemDeOrigen.Almacena_FechaAlta, fechaHoraCentinela) &&
                                                        miRelojServidor.EsMismaFechaYHoraSinMilisegundos(itemDeOrigen.Almacena_FechaModificacion, fechaHoraCentinela) ) 
                                                    {
                                                        //Todo esta bien, ahora solo se activo en la tabla Almacena
                                                        var itemsDeAlmacena = (from s in ctx.Almacena
                                                                               select s).ToList();

                                                        var relacionAActivarEnAlmacena = itemsDeAlmacena.First(s => (s.IdTienda == itemDeOrigen.Almacena_IdTienda) && (s.IdProducto == itemDeOrigen.Almacena_IdProducto));
                                                        relacionAActivarEnAlmacena.Activo = true;
                                                        relacionAActivarEnAlmacena.FechaModificacion = DateTime.Now;
                                                        ctx.SaveChanges();
                                                        respuesta = "ok";
                                                    }

                                                    else
                                                    {
                                                        bool esMismaFechaHoraAlta = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(relacionBuscadaEnAlmacena.FechaAlta, itemDeOrigen.Almacena_FechaAlta);
                                                        bool esMismaFechaHoraModificacion = miRelojServidor.EsMismaFechaYHoraSinMilisegundos(relacionBuscadaEnAlmacena.FechaModificacion, itemDeOrigen.Almacena_FechaModificacion);

                                                        if(esMismaFechaHoraAlta && esMismaFechaHoraModificacion)
                                                        {
                                                            //puede continuar el for :)
                                                        }

                                                        else
                                                        {
                                                            respuesta = "La información que usted ve en pantalla ha sido actualizada, recargue la página";
                                                            break;
                                                        }
                                                    }     
                                                }

                                                else
                                                {
                                                    respuesta = "La información que usted ve en pantalla ha sido actualizada, recargue la página";
                                                    break;
                                                }
                                            }

                                            else
                                            {
                                                respuesta = "La información que usted ve en pantalla ha sido actualizada, recargue la página";
                                                break;
                                            }
                                        }

                                        else
                                        {
                                            respuesta = "La información que usted ve en pantalla ha sido actualizada, recargue la página";
                                            break;
                                        }
                                    }

                                    else
                                    {
                                        //No existe la relacion, se crea aqui...

                                        //Verifico que en Producto este activo al momento de la consulta
                                        var idsProductos = (from s in ctx.Producto
                                                            select new { s.Id, s.Activo }).ToList();

                                        var idsActivosProductos = (from s in idsProductos
                                                                   where s.Activo == true
                                                                   select s.Id).ToList();

                                        if(idsActivosProductos.Contains(itemDeOrigen.Producto_Id))
                                        {
                                            //Verifico que en ProductoConDetalles lo que el usuario esta viendo al momento en la pantalla sea lo que este al momento de la consulta
                                            var resumProductoConDetalles = (from s in ctx.ProductoConDetalles
                                                                            select new { s.IdProducto, s.IdDetalleProducto, s.Activo }).ToList();

                                            var activosResumProductoConDetalles = (from s in resumProductoConDetalles
                                                                                   where s.Activo == true
                                                                                   select new { s.IdProducto, s.IdDetalleProducto }).ToList();

                                            var relBuscada = activosResumProductoConDetalles.FirstOrDefault(s =>
                                                (s.IdProducto == itemDeOrigen.ProductoConDetalles_IdProducto) && (s.IdDetalleProducto == itemDeOrigen.ProductoConDetalles_IdDetalleProducto));

                                            if(relBuscada != null)
                                            {
                                                //reviso en DetalleProducto para verficar que lo que ve el uuario en pantalla al momento de la consulta sea lo que este.
                                                var idsDetalleProducto = (from s in ctx.DetalleProducto
                                                                          select new { s.Id, s.Activo }).ToList();

                                                var idsActivosDetalleProducto = (from s in idsDetalleProducto
                                                                                 where s.Activo == true
                                                                                 select s.Id).ToList();
                                                if(idsActivosDetalleProducto.Contains(itemDeOrigen.DetalleProducto_Id))
                                                {
                                                    // se procede a crear la relacion
                                                    DateTime fechaHoraActual = DateTime.Now;

                                                    Almacena almacena = new Almacena();
                                                    almacena.IdTienda = itemDeOrigen.Almacena_IdTienda;
                                                    almacena.IdProducto = itemDeOrigen.Almacena_IdProducto;
                                                    almacena.IdUsuarioAlta = itemDeOrigen.DetalleProducto_IdUsuarioAlta;
                                                    almacena.FechaAlta = fechaHoraActual;
                                                    almacena.IdUsuarioModifico = itemDeOrigen.DetalleProducto_IdUsuarioAlta;
                                                    almacena.FechaModificacion = fechaHoraActual;
                                                    almacena.Activo = true;

                                                    ctx.Almacena.Add(almacena);
                                                    ctx.SaveChanges();
                                                    respuesta = "ok";
                                                }

                                                else
                                                {
                                                    respuesta = "La información que usted ve en pantalla ha sido actualizada, recargue la página";
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                respuesta = "La información que usted ve en pantalla ha sido actualizada, recargue la página";
                                                break;
                                            }
                                        }

                                        else
                                        {
                                            respuesta = "La información que usted ve en pantalla ha sido actualizada, recargue la página";
                                            break;
                                        }
                                    }
                                }                                                               
                            }//for
                        }

                        else
                        {
                            respuesta = "La tienda o el usuario ya no estan activos";
                        }

                        if(respuesta.Contains("ok"))
                        {
                            dbContextTransaction.Commit();
                        }

                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }

                    catch(Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion lanzada y cachada en TiendaLN.Post_AgregarProductosATienda",ex);
                    }
                }
            }

            return (respuesta);
        }

        public string Post_AgregarNuevaCaducidad(List<TiendaJoinCaducidadesViewModel> coleccionViewModel)
        {
            string respuesta = "Inicializar.";

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {   

                        TiendaJoinCaducidadesViewModel itemTiendaJoinCaducidadesViewModel = coleccionViewModel.First();
                        RelojServidor relojServidor = new RelojServidor();
                        DateTime fechaHoraEnServidor = relojServidor.RegresarHoraEnServidor();

                        //Verificar que estan activos: la tienda, usuario, producto y detalleProducto
                        RulesEngineLN rulesEngineLN = new RulesEngineLN();
                        var resumenUsuarios = (from s in ctx.Usuario
                                           select new UsuarioViewModel { Id = s.Id, Activo = s.Activo }).ToList();

                        bool esActivoUsuarioOperador = rulesEngineLN.EsActivoIdUsuario(resumenUsuarios, itemTiendaJoinCaducidadesViewModel.MiCaducaViewModel.IdUsuarioAlta);

                        var resumenTiendas = (from s in ctx.Tienda
                                             select new TiendaViewModel { Id = s.Id, Supmza = "", Manzana = "", Lote = "",
                                             Calle = "", Nombre = "", IdUsuarioAlta = 0, IdUsuarioModifico = 0,
                                             FechaAlta = DateTime.MinValue, FechaModificacion = DateTime.MinValue,
                                             Activo = s.Activo}).ToList();

                        bool esActivaTienda = rulesEngineLN.EsActivaTienda(resumenTiendas, itemTiendaJoinCaducidadesViewModel.MiCaducaViewModel.IdTienda);

                        var resumenAlmacena = (from s in ctx.Almacena
                                               select new AlmacenaViewModel { IdTienda = s.IdTienda, IdProducto = s.IdProducto, Activo = s.Activo }).ToList();

                        bool estanActivosTodosProductosEnTienda = rulesEngineLN.EstanActivosTodosLosProductosEnTienda(coleccionViewModel, resumenAlmacena);
                        
                        var resumenProducto = (from s in ctx.Producto
                                               select new ProductoViewModel { Id = s.Id, CodigoBarras="", Nombre="", IdUsuarioAlta = 0,
                                                   FechaAlta = DateTime.MinValue, IdUsuarioModifico = 0, FechaModificacion = DateTime.MinValue,
                                                   Activo = s.Activo  }).ToList();

                        bool estanActivosTodosProductos = rulesEngineLN.EstanActivosLosProductos(coleccionViewModel, resumenProducto);

                        var resumenProductoConDetalles = (from s in ctx.ProductoConDetalles
                                                          select new ProductoConDetallesViewModel
                                                          {
                                                              IdProducto = s.IdProducto,
                                                              IdDetalleProducto = s.IdDetalleProducto,
                                                              Activo = s.Activo
                                                          }).ToList();

                        bool estaActivaRelacionEnProductoConDetalles = rulesEngineLN.EstanActivaRelacionEnProductoConDetalles(coleccionViewModel, resumenProductoConDetalles);


                        var resumenDetalleProductoVM = (from s in ctx.DetalleProducto
                                                         select new DetalleProductoViewModel { Id = s.Id, Activo = s.Activo }).ToList();

                        bool estanActivosEnDetalleProducto = rulesEngineLN.EstanActivosLosIdDetalleProducto(coleccionViewModel, resumenDetalleProductoVM);
                        

                        if(esActivoUsuarioOperador)
                        {
                            if(esActivaTienda)
                            {
                                if(estanActivosTodosProductosEnTienda)
                                {
                                    if(estanActivosTodosProductos)
                                    {
                                        if(estaActivaRelacionEnProductoConDetalles)
                                        {
                                            if(estanActivosEnDetalleProducto)
                                            {
                                                //Obtener nuevo id para tabla Periodo
                                                var idsPeriodo = (from item in ctx.Periodo
                                                                  select item.Id).ToList();

                                                var nuevoIdDePeriodo = idsPeriodo.Count == 0 ? 1 : (idsPeriodo.Max() + 1);

                                                //Creo un item en Periodo
                                                Periodo periodo = new Periodo();
                                                periodo.Id = nuevoIdDePeriodo;
                                                periodo.FechaCaducidad = itemTiendaJoinCaducidadesViewModel.MiPeriodoViewModel.FechaCaducidad;
                                                periodo.NumeroUnidades = itemTiendaJoinCaducidadesViewModel.MiPeriodoViewModel.NumeroUnidades;
                                                periodo.Vigente = true;
                                                periodo.IdUsuarioAlta = itemTiendaJoinCaducidadesViewModel.MiCaducaViewModel.IdUsuarioAlta;
                                                periodo.IdUsuarioModifico = itemTiendaJoinCaducidadesViewModel.MiCaducaViewModel.IdUsuarioAlta;
                                                periodo.FechaAlta = fechaHoraEnServidor;
                                                periodo.FechaModificacion = fechaHoraEnServidor;
                                                periodo.Activo = true;

                                                ctx.Periodo.Add(periodo);
                                                ctx.SaveChanges();


                                                //Creo un item en PeriodoConunidad
                                                PeriodoConUnidad periodoConUnidad = new PeriodoConUnidad();
                                                periodoConUnidad.IdPeriodo = nuevoIdDePeriodo;
                                                periodoConUnidad.IdUnidad = itemTiendaJoinCaducidadesViewModel.MiUnidadMedidaViewModel.Id;
                                                periodoConUnidad.IdUsuarioAlta = itemTiendaJoinCaducidadesViewModel.MiCaducaViewModel.IdUsuarioAlta;
                                                periodoConUnidad.IdUsuarioModifico = itemTiendaJoinCaducidadesViewModel.MiCaducaViewModel.IdUsuarioAlta;
                                                periodoConUnidad.FechaAlta = fechaHoraEnServidor;
                                                periodoConUnidad.FechaModificacion = fechaHoraEnServidor;
                                                periodoConUnidad.Activo = true;

                                                ctx.PeriodoConUnidad.Add(periodoConUnidad);
                                                ctx.SaveChanges();

                                                coleccionViewModel.ForEach(item =>
                                                {
                                                    Caduca caduca = new Caduca();
                                                    caduca.IdProducto = item.MiCaducaViewModel.IdProducto;
                                                    caduca.IdDetalleProducto = item.MiCaducaViewModel.IdDetalleProducto;
                                                    caduca.IdPeriodo = nuevoIdDePeriodo;
                                                    caduca.IdTienda = item.MiCaducaViewModel.IdTienda;
                                                    caduca.IdUsuarioAlta = item.MiCaducaViewModel.IdUsuarioAlta;
                                                    caduca.IdUsuarioModifico = item.MiCaducaViewModel.IdUsuarioAlta;
                                                    caduca.FechaAlta = fechaHoraEnServidor;
                                                    caduca.FechaModificacion = fechaHoraEnServidor;
                                                    caduca.Activo = true;

                                                    ctx.Caduca.Add(caduca);
                                                    ctx.SaveChanges();
                                                }
                                                );

                                                respuesta = "ok";

                                                if (respuesta.Contains("ok"))
                                                {
                                                    dbContextTransaction.Commit();
                                                }

                                                else
                                                {
                                                    dbContextTransaction.Rollback();
                                                }
                                            }

                                            else
                                            {
                                                respuesta = "El detalle de algún(os) producto(s) no esta(n) disponible(s) en este momento en la tabla DetalleProducto";
                                            }
                                        }

                                        else
                                        {
                                            respuesta = "El detalle de algún(os) producto(s) no esta(n) disponible(s) en este momento en la tabla ProductoConDetalles";
                                        }
                                    }

                                    else
                                    {
                                        respuesta = "Algún(os) producto(s) no está(n) disponible(s) en este momento";
                                    }

                                }

                                else
                                {
                                    respuesta = "Algún(os) producto(s) no estan activos en la tienda en este momento.";
                                }

                            }

                            else
                            {
                                respuesta = "La tienda no esta activa en este momento";
                            }

                        }

                        else
                        {
                            respuesta = "El usuario operador no esta activo en este momento";
                        }

                    }

                    catch(Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion lanzada y cachada en TiendaLN.Post_AgregarNuevaCaducidad", ex);
                    }
                }
            }

            return (respuesta);
        }


        public List<CaducaJoinProductoJoin__JoinPeriodoJoinPeriodoConUnidadJoinUnidadMedidaViewModel> Get_BuscarCaducidadEnTiendaFrom(ParametroBuscarCaducidadViewModel parametroBuscarCaducidadViewModel)
        {
            List<CaducaJoinProductoJoin__JoinPeriodoJoinPeriodoConUnidadJoinUnidadMedidaViewModel> respuestaFinal = new List<CaducaJoinProductoJoin__JoinPeriodoJoinPeriodoConUnidadJoinUnidadMedidaViewModel>();
            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {   //Verificar como se comporta el tipo var, cuando un select regresa "vacio"
                        List<TiendaViewModel> resumenTiendas = new List<TiendaViewModel>();
                        resumenTiendas = (from s in ctx.Tienda
                                                   select new TiendaViewModel { Id = s.Id, Activo = s.Activo }).ToList();

                        RulesEngineLN rulesEngineLN = new RulesEngineLN();
                        bool esActivaTiendaBuscada = rulesEngineLN.EsActivaTienda(resumenTiendas, parametroBuscarCaducidadViewModel.IdTienda);
                        if(esActivaTiendaBuscada)
                        {
                            //Extrayendo info de la tabla caduca
                            var itemsActivosDeCaduca = (from s in ctx.Caduca
                                                 where s.Activo == true
                                                 select s).ToList();

                            var resumenCaduca = (from s in itemsActivosDeCaduca
                                                 where parametroBuscarCaducidadViewModel.IdTienda == s.IdTienda
                                                 select s).ToList();  //Esta coleccion se debe enlazar en consulta final

                            var resumenIdsIdProductoKNecesito = (from s in resumenCaduca
                                                                 select s.IdProducto).ToList();
                            
                            //Extrayendo info de la tabla Producto
                            var resumenProducto = (from s in ctx.Producto
                                                   where resumenIdsIdProductoKNecesito.Contains(s.Id) && s.Activo == true
                                                   select s).ToList();//esta coleccion se debe enlazar para consulta final

                            var resumenIdsProducto = (from s in resumenProducto
                                                      select s.Id).ToList();

                            //Extrayendo info de la tabla ProductoConDetalles
                            var resumenProductoConDetalles = (from s in ctx.ProductoConDetalles
                                                              where resumenIdsProducto.Contains(s.IdProducto) && s.Activo == true
                                                              select s).ToList();//esta coleccion se debe enlazar para consulta final

                            var resumenIdsIdDetalleProducto = (from s in resumenProductoConDetalles
                                                               select s.IdDetalleProducto).ToList();

                            //Extrayendo info de la tabla DetalleProducto
                            var resumenDetalleProducto = (from s in ctx.DetalleProducto
                                                          where resumenIdsIdDetalleProducto.Contains(s.Id) && s.Activo == true
                                                          select s).ToList(); //esta coleccion se debe enlazar para consulta final

                            //Se estrae info de la tabla Periodo
                            var idsPeriodosDeTienda = (from s in resumenCaduca
                                                       select s.IdPeriodo).ToList();

                            var itemsDPeriodoVigentesYActivos = (from s in ctx.Periodo
                                                                 where idsPeriodosDeTienda.Contains(s.Id) &&  s.Activo == true && s.Vigente == true 
                                                                 select s).ToList();

                            var resumenPeriodo = (from s in itemsDPeriodoVigentesYActivos
                                                  where s.FechaCaducidad >= parametroBuscarCaducidadViewModel.FechaInicial &&
                                                         s.FechaCaducidad <= parametroBuscarCaducidadViewModel.FechaFinal
                                                  select s).ToList();  //esta coleccion se debe enlazar para consulta final

                            var resumenIdsDeResumenPeriodo = (from s in resumenPeriodo
                                                              select s.Id).ToList();

                            //Se extrae info de la tabla PeriodoConUnidad
                            var resumenPeriodoConUnidad = (from s in ctx.PeriodoConUnidad
                                                           where resumenIdsDeResumenPeriodo.Contains(s.IdPeriodo)
                                                           select s).ToList();  //esta coleccion se debe enlazar para consulta final

                            var resumenIdsDeResumenPeriodoConUnidad = (from s in resumenPeriodoConUnidad
                                                                       select s.IdUnidad).ToList();

                            //Se extrae info de la tabla UnidadMedida
                            var resumenUnidadMedida = (from s in ctx.UnidadMedida
                                                       where resumenIdsDeResumenPeriodoConUnidad.Contains(s.Id)
                                                       select s).ToList(); //esta coleccion se debe enlazar para consulta final

                            //a partir de aqui se comienza a enlazar la consulta final
                            var caducaJoinProducto = (from resumCaduca in resumenCaduca
                                                      join resumProducto in resumenProducto on resumCaduca.IdProducto equals resumProducto.Id
                                                      select new { Caduca_IdProducto = resumCaduca.IdProducto, Caduca_IdPeriodo = resumCaduca.IdPeriodo,
                                                      Producto_Id = resumProducto.Id, Producto_CodigoBarras = resumProducto.CodigoBarras }).ToList();

                            var caducaJoinProductoJoinProductoConDetalles = (from s in caducaJoinProducto
                                                                             join c in resumenProductoConDetalles on s.Caduca_IdProducto equals c.IdProducto
                                                                             select new
                                                                             {
                                                                                 s.Caduca_IdProducto,
                                                                                 s.Caduca_IdPeriodo,
                                                                                 s.Producto_Id,
                                                                                 s.Producto_CodigoBarras,
                                                                                 ProductoConDetalles_IdProducto = c.IdProducto,
                                                                                 ProductoConDetalles_IdDetalleProducto = c.IdDetalleProducto
                                                                             }).ToList();

                            var caducaJoinProductoJoinProductoConDetallesJoinDetalleProducto = (from s in caducaJoinProductoJoinProductoConDetalles
                                                                                                join c in resumenDetalleProducto on s.ProductoConDetalles_IdDetalleProducto equals c.Id
                                                                                                select new
                                                                                                {
                                                                                                    s.Caduca_IdProducto,
                                                                                                    s.Caduca_IdPeriodo,
                                                                                                    s.Producto_Id,
                                                                                                    s.Producto_CodigoBarras,
                                                                                                    s.ProductoConDetalles_IdProducto,
                                                                                                    s.ProductoConDetalles_IdDetalleProducto,
                                                                                                    DetalleProducto_Id = c.Id,
                                                                                                    DetalleProducto_Nombre = c.Nombre
                                                                                                }).ToList();

                            var caducaJoinProductoJoinProductoConDetallesJoinDetalleProductoJoinPeriodo = (from s in caducaJoinProductoJoinProductoConDetallesJoinDetalleProducto
                                                                                                           join c in resumenPeriodo on s.Caduca_IdPeriodo equals c.Id
                                                                                                           select new {   s.Caduca_IdProducto,
                                                                                                                          s.Caduca_IdPeriodo,
                                                                                                                          s.Producto_Id,
                                                                                                                          s.Producto_CodigoBarras,
                                                                                                                          s.ProductoConDetalles_IdProducto,
                                                                                                                          s.ProductoConDetalles_IdDetalleProducto,
                                                                                                                          s.DetalleProducto_Id,
                                                                                                                          s.DetalleProducto_Nombre,
                                                                                                                          Periodo_Id = c.Id, 
                                                                                                                          Periodo_FechaCaducidad = c.FechaCaducidad,
                                                                                                                          Periodo_NumeroUnidades = c.NumeroUnidades,
                                                                                                                          Periodo_Vigente = c.Vigente
                                                                                                           }).ToList();

                            var caducaJoinProductoJoinProductoConDetallesJoinDetalleProductoJoinPeriodoJoinPeriodoConUnidad = (from s in caducaJoinProductoJoinProductoConDetallesJoinDetalleProductoJoinPeriodo
                                                                                                                               join c in resumenPeriodoConUnidad on s.Periodo_Id equals c.IdPeriodo
                                                                                                                               select new
                                                                                                                               {
                                                                                                                                   s.Caduca_IdProducto,
                                                                                                                                   s.Caduca_IdPeriodo,
                                                                                                                                   s.Producto_Id,
                                                                                                                                   s.Producto_CodigoBarras,
                                                                                                                                   s.ProductoConDetalles_IdProducto,
                                                                                                                                   s.ProductoConDetalles_IdDetalleProducto,
                                                                                                                                   s.DetalleProducto_Id,
                                                                                                                                   s.DetalleProducto_Nombre,
                                                                                                                                   s.Periodo_Id,
                                                                                                                                   s.Periodo_FechaCaducidad,
                                                                                                                                   s.Periodo_NumeroUnidades,
                                                                                                                                   s.Periodo_Vigente,
                                                                                                                                   PeriodoConUnidad_IdPeriodo = c.IdPeriodo,
                                                                                                                                   PeriodoConUnidad_IdUnidad = c.IdUnidad
                                                                                                                               }).ToList();  


                            var caducaJoinProductoJoinProductoConDetallesJoinDetalleProductoJoinPeriodoJoinPeriodoConUnidadJoinUnidadMedida = (from s in caducaJoinProductoJoinProductoConDetallesJoinDetalleProductoJoinPeriodoJoinPeriodoConUnidad
                                                                                                                                               join c in resumenUnidadMedida on s.PeriodoConUnidad_IdUnidad equals c.Id
                                                                                                                                               select new
                                                                                                                                               {
                                                                                                                                                   s.Caduca_IdProducto,
                                                                                                                                                   s.Caduca_IdPeriodo,
                                                                                                                                                   s.Producto_Id,
                                                                                                                                                   s.Producto_CodigoBarras,
                                                                                                                                                   s.ProductoConDetalles_IdProducto,
                                                                                                                                                   s.ProductoConDetalles_IdDetalleProducto,
                                                                                                                                                   s.DetalleProducto_Id,
                                                                                                                                                   s.DetalleProducto_Nombre,
                                                                                                                                                   s.Periodo_Id,
                                                                                                                                                   s.Periodo_FechaCaducidad,
                                                                                                                                                   s.Periodo_NumeroUnidades,
                                                                                                                                                   s.Periodo_Vigente,
                                                                                                                                                   s.PeriodoConUnidad_IdPeriodo,
                                                                                                                                                   s.PeriodoConUnidad_IdUnidad,
                                                                                                                                                   UnidadMedida_Id = c.Id,
                                                                                                                                                   UnidadMedida_Nombre = c.Nombre
                                                                                                                                               }).ToList();


                            
                            caducaJoinProductoJoinProductoConDetallesJoinDetalleProductoJoinPeriodoJoinPeriodoConUnidadJoinUnidadMedida.ForEach(item => {
                                CaducaJoinProductoJoin__JoinPeriodoJoinPeriodoConUnidadJoinUnidadMedidaViewModel c = new CaducaJoinProductoJoin__JoinPeriodoJoinPeriodoConUnidadJoinUnidadMedidaViewModel();
                                c.MiCaducaViewModel.IdProducto = item.Caduca_IdProducto;
                                c.MiCaducaViewModel.IdPeriodo = item.Caduca_IdPeriodo; 
                                c.MiProductoJoinProductoConDetallesJoinDetalleProductoViewModel.Producto_Id = item.Producto_Id;
                                c.MiProductoJoinProductoConDetallesJoinDetalleProductoViewModel.Producto_CodigoBarras = item.Producto_CodigoBarras;
                                c.MiProductoJoinProductoConDetallesJoinDetalleProductoViewModel.ProductoConDetalles_IdProducto = item.ProductoConDetalles_IdProducto;
                                c.MiProductoJoinProductoConDetallesJoinDetalleProductoViewModel.ProductoConDetalles_IdDetalleProducto = item.ProductoConDetalles_IdDetalleProducto;
                                c.MiProductoJoinProductoConDetallesJoinDetalleProductoViewModel.DetalleProducto_Id = item.DetalleProducto_Id;
                                c.MiProductoJoinProductoConDetallesJoinDetalleProductoViewModel.DetalleProducto_Nombre = item.DetalleProducto_Nombre;
                                c.MiPeriodoViewModel.Id = item.Periodo_Id;
                                c.MiPeriodoViewModel.FechaCaducidad = item.Periodo_FechaCaducidad;
                                c.MiPeriodoViewModel.NumeroUnidades = item.Periodo_NumeroUnidades;
                                c.MiPeriodoViewModel.Vigente = item.Periodo_Vigente;
                                c.MiPeriodoConUnidadViewModel.IdPeriodo = item.PeriodoConUnidad_IdPeriodo;
                                c.MiPeriodoConUnidadViewModel.IdUnidad = item.PeriodoConUnidad_IdUnidad;
                                c.MiUnidadMedidaViewModel.Id = item.UnidadMedida_Id;
                                c.MiUnidadMedidaViewModel.Nombre = item.UnidadMedida_Nombre;

                                respuestaFinal.Add(c);   
                                });

                            //var x = from s in caducaJoinProductoJoinProductoConDetallesJoinDetalleProductoJoinPeriodoJoinPeriodoConUnidadJoinUnidadMedida
                            //        select new CaducaJoinProductoJoin__JoinPeriodoJoinPeriodoConUnidadJoinUnidadMedidaViewModel() { MiCaducaViewModel = new CaducaViewModel(),
                            //            MiCaducaViewModel};

                        }

                        else
                        {
                            //No es activa tienda buscada
                        }
                        
                    }

                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion lanzada y cachada en TiendaLN.Get_BuscarCaducidadEnTiendaFrom", ex);
                    }
                }
            }

            return (respuestaFinal);
        }
    }
}