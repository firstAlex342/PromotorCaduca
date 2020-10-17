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
    }
}