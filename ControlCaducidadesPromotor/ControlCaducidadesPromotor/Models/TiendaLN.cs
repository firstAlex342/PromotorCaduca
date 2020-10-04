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
    }
}