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
                                             x.IdUsuarioAlta
                                         }).ToList();

                            var idsNecesitados = (from item in lista
                                                  where item.IdUsuarioAlta == idUsuario
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
                                                          select new { s.Id, s.Nombre, s.Activo }).ToList();    //De la tabla DetalleProducto obtengo las filas que me interesan

                            //Ya esta la info de las tablas fuera de EntityFramework, esta ahora en la memoria, asi que procedo a enlazar
                            //los registros tomados de las tablas Producto, ProductoConDetalles Y DetallesProducto; todos tiene el campo Activo = 1
                            var resumenProductosJOINresumenProductoConDetalles = (from s in resumenProductos
                                                                                  join x in resumenProductoConDetalles
                                                                                   on s.Id equals x.IdProducto
                                                                                  where (s.Activo == true) && (x.Activo == true)
                                                                                 select new { s.Id, s.CodigoBarras, x.IdProducto, x.IdDetalleProducto }).ToList();

                            var resumenProductosJOINresumenProductoConDetallesJOINresumenDetalleProducto = (from s in resumenProductosJOINresumenProductoConDetalles
                                                                                                            join x in resumenDetalleProducto
                                                                                                            on s.IdDetalleProducto equals x.Id
                                                                                                            where x.Activo == true
                                                                                                            select new 
                                                                                                            { s.Id, s.CodigoBarras, s.IdProducto, s.IdDetalleProducto,
                                                                                                            DetalleProd_Id = x.Id, x.Nombre
                                                                                                            }).ToList();

                            resViewModel = (from s in resumenProductosJOINresumenProductoConDetallesJOINresumenDetalleProducto
                                            select new AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM
                                            {  Almacena_IdTienda = idTienda,
                                               Almacena_IdProducto = s.Id,
                                               Producto_Id = s.Id,
                                               Producto_CodigoBarras = s.CodigoBarras,
                                               ProductoConDetalles_IdProducto = s.IdProducto,
                                               ProductoConDetalles_IdDetalleProducto = s.IdDetalleProducto,
                                               DetalleProducto_Id = s.DetalleProd_Id,
                                               DetalleProducto_Nombre = s.Nombre
                                           }).ToList();  //coloco el resultado en un objetoViewModel
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
    }
}