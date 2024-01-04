using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControlCaducidadesPromotor.Basicos;

namespace ControlCaducidadesPromotor.Models
{
    public class RulesEngineLN
    {
        //--------constructor
        public RulesEngineLN()
        {
        }//parameterless constructor


        //-----------Methods
        /// <summary>
        /// Regresa true si el idUsuarioBuscado existe en listUsuarioViewModel y esta activo
        /// </summary>
        /// <param name="listUsuarioViewModel">la lista de usuarios, vienen activos y no activos</param>
        /// <param name="idUsuarioBuscado">el id buscado</param>
        /// <returns></returns>
        public bool EsActivoIdUsuario(List<UsuarioViewModel> listUsuarioViewModel, int idUsuarioBuscado)
        {
            bool res = false;

            //Recupero solo los idsActivos
            var idsUsuariosActivos = (from s in listUsuarioViewModel
                                      where s.Activo == true
                                      select s.Id).ToList();

            res = idsUsuariosActivos.Contains(idUsuarioBuscado);
            return (res);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="idsTienda">una lista de los ids de tiendas</param>
        /// <param name="idTiendaBuscada">el id de tienda a buscar</param>
        /// <returns>bool</returns>
        public bool EsActivaTienda(List<TiendaViewModel> resumenTiendas, ParametroBuscarCaducidadViewModel parametroBuscarCaducidadViewModel)
        {
            bool res = false;

            /*var idsTiendasActivos = (from s in resumenTiendas       <----estas eran las lineas originales
                                     where s.Activo == true
                                     select s.Id).ToList();

            res = idsTiendasActivos.Contains(idTiendaBuscada);*/

            var resumenTiendasActivas = (from s in resumenTiendas
                                         where s.Activo == true
                                         select s).ToList();
            var elementoBuscado = resumenTiendasActivas.FirstOrDefault(s => { return (s.Id == parametroBuscarCaducidadViewModel.IdTienda); });
            if(elementoBuscado !=null)
            {
                RelojServidor relojServidor = new RelojServidor();
                bool esMismaFechaAlta = relojServidor.EsMismaFechaYHoraSinMilisegundos(elementoBuscado.FechaAlta, parametroBuscarCaducidadViewModel.FechaAlta);
                bool esMismaFechaModificacion = relojServidor.EsMismaFechaYHoraSinMilisegundos(elementoBuscado.FechaModificacion, parametroBuscarCaducidadViewModel.FechaModificacion);

                res = esMismaFechaAlta && esMismaFechaModificacion;
            }
            return (res);
        }


        /// <summary>
        /// Regresa false , si coleccionViewModel es una lista vacía
        /// </summary>
        /// <param name="coleccionViewModel"></param>
        /// <param name="resumenAlmacenaViewModel"></param>
        /// <returns></returns>
        public bool EstanActivosTodosLosProductosEnTienda(List<TiendaJoinCaducidadesViewModel> coleccionViewModel, List<AlmacenaViewModel> resumenAlmacenaViewModel)
        {
            bool res = false;

            var resumenActivosEnAlmacenaViewModel = (from s in resumenAlmacenaViewModel
                                                     where s.Activo == true
                                                     select s).ToList();

            if (coleccionViewModel.Count == 0)
            { return (false); } // Esta condicion es porque TrueForAll regresa por default true si coleccionViewModel es una lista vacía


            res = coleccionViewModel.TrueForAll(s => {
                var elementoBuscado = resumenActivosEnAlmacenaViewModel.FirstOrDefault(x => {
                    return(x.IdTienda == s.MiCaducaViewModel.IdTienda && x.IdProducto == s.MiCaducaViewModel.IdProducto);
                } );

                if(elementoBuscado != null)
                {
                    return (true);
                }

                else
                {
                    return (false);
                }
            });

            return (res);
        }


        /// <summary>
        /// Comprueba si todos los productos en coleccionViewModel estan activos. Regresa false si coleccionViewModel es una lista vacía.
        /// </summary>
        /// <param name="coleccionViewModel"></param>
        /// <param name="resumenProductoViewModel">los productos, activos y no activos, la funcion selecciona los activos y usa esa seleccion para la operación</param>
        /// <returns></returns>
        public bool EstanActivosLosProductos(List<TiendaJoinCaducidadesViewModel> coleccionViewModel, List<ProductoViewModel> resumenProductoViewModel)
        {
            bool res = false;

            if(coleccionViewModel.Count == 0)
            { return (false); } //Esta condicion es porque TrueForAll regresa true por default, si coleccionViewModel esta vacío

            var resumenActivosEnListProductoViewModel = (from s in resumenProductoViewModel
                                                         where s.Activo == true
                                                         select s.Id).ToList();

            res = coleccionViewModel.TrueForAll(s => resumenActivosEnListProductoViewModel.Contains(s.MiCaducaViewModel.IdProducto));

            return (res);
        }


        /// <summary>
        /// Regresa false, si coleccionViewModel es una lista vacía.
        /// </summary>
        /// <param name="coleccionViewModel"></param>
        /// <param name="resumenProductoConDetallesViewModel"></param>
        /// <returns></returns>
        public bool EstanActivaRelacionEnProductoConDetalles(List<TiendaJoinCaducidadesViewModel> coleccionViewModel, List<ProductoConDetallesViewModel> resumenProductoConDetallesViewModel)
        {
            bool res = false;
            var activosEnProductoConDetallesVM = (from s in resumenProductoConDetallesViewModel
                                                where s.Activo == true
                                                select s).ToList();

            if (coleccionViewModel.Count == 0)
            { return (false); } //Esta linea es porque TrueForAll regresa por default  true , si coleccionViewModel esta vacío.

            res = coleccionViewModel.TrueForAll(s =>
            {
                ProductoConDetallesViewModel productoConDetallesViewModel = null;
                productoConDetallesViewModel = activosEnProductoConDetallesVM.FirstOrDefault(m =>
                {
                    bool respTemporal = (m.IdProducto == s.MiCaducaViewModel.IdProducto) && (m.IdDetalleProducto == s.MiCaducaViewModel.IdDetalleProducto);
                    return (respTemporal);
                });

                if (productoConDetallesViewModel != null)
                { return (true); }

                else
                {
                    return (false);
                }
            });

            return (res);
        }


        /// <summary>
        /// Regresa false, si coleccionViewModel es una lista vacía.
        /// </summary>
        /// <param name="coleccionViewModel"></param>
        /// <param name="resumenDetalleProductoViewModel"></param>
        /// <returns></returns>
        public bool EstanActivosLosIdDetalleProducto(List<TiendaJoinCaducidadesViewModel> coleccionViewModel, List<DetalleProductoViewModel> resumenDetalleProductoViewModel)
        {
            bool res = false;

            var activosDetalleProductoViewModel = (from s in resumenDetalleProductoViewModel
                                                   where s.Activo == true
                                                   select s.Id).ToList();
            if (coleccionViewModel.Count == 0)
            { return (false);  } // Esta condición es porque TrueForAll regresa true por default si coleccionViewModel esta vacío.

            res = coleccionViewModel.TrueForAll(s =>  activosDetalleProductoViewModel.Contains(s.MiCaducaViewModel.IdDetalleProducto)  );

            return (res);
        }
    }
}