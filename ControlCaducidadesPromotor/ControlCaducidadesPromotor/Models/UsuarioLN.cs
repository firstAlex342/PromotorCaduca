using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControlCaducidadesPromotor.AccesoADatos;
using ControlCaducidadesPromotor.Basicos;

namespace ControlCaducidadesPromotor.Models
{
    public class UsuarioLN
    {
        public UsuarioViewModel Get_UsuarioXUsuarioYPassword(UsuarioViewModel usuarioViewModel)
        {
            UsuarioViewModel usuarioBuscadoVM = new UsuarioViewModel();
            usuarioBuscadoVM = null;

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {   //Obtengo nombres de usuario, password y activo de todos
                        var resumen = from s in ctx.Usuario
                                      select new { s.Id, s.Usuario1, s.Password, s.Nombre, s.Activo };

                        //Busco la coincidencia
                        var unicoElemento = resumen.SingleOrDefault(item => (item.Usuario1 == usuarioViewModel.Usuario) &&
                                                         (item.Password == usuarioViewModel.Password) );
                        if(unicoElemento != null)
                        {
                            UsuarioViewModel uvm = new UsuarioViewModel(unicoElemento.Id, unicoElemento.Nombre,
                                unicoElemento.Usuario1, unicoElemento.Password, unicoElemento.Activo);
                            usuarioBuscadoVM = uvm;
                        }

                        else
                        {
                            usuarioBuscadoVM = null;
                        }

                        dbContextTransaction.Commit();
                    }

                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion cachada y lanzada en UsuarioLN.Get_EsUsuarioCorrectoYEstaActivo", ex);
                    }
                }
            }

            return (usuarioBuscadoVM);
        }



        public UsuarioViewModel Get_DetallesDeUsuarioXusuario(string usuario)
        {
            UsuarioViewModel usuarioBuscadoVM = null;

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        var resumenUsuarios = (from s in ctx.Usuario
                                               select new { s.Id, s.Usuario1 }).ToList();

                        var resumenUsuarioBuscado = resumenUsuarios.Single(item => usuario == item.Usuario1);

                        Usuario usuarioQueNecesito = ctx.Usuario.Single(item => item.Id == resumenUsuarioBuscado.Id);
                        usuarioBuscadoVM = new UsuarioViewModel(usuarioQueNecesito.Id, usuarioQueNecesito.Nombre, usuarioQueNecesito.Usuario1, usuarioQueNecesito.Password, usuarioQueNecesito.Activo);

                        dbContextTransaction.Commit();
                    }

                    catch(Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion lanzada y cachada en Usuario.Get_DetallesDeUsuarioXusuario");
                    }
                }
            }

            return (usuarioBuscadoVM);
        }
    }
}