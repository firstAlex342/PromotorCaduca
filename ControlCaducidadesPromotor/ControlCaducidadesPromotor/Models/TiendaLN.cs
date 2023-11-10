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
        /// <summary>
        /// Regresa una lista con contenido, una lista vacia ó una excepcion
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
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

                            dbContextTransaction.Commit();
                        }

                        else { throw new Exception("Verifique que el usuario exista y este activo"); }
                    }


                    catch(Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion cachada y lanzada en TiendaLN.MostrarTodasTiendasDeUsuario. "+ ex.Message , ex);
                    }
                }
            }

            return (tiendasViewModel);
        }


        public string PostCrearTienda(TiendaViewModel tiendaViewModel)
        {
            string respuesta = "";

            tiendaViewModel.AjustarAtributosSupmzaMzaLteCalleNombre();
            if(tiendaViewModel.MisAtributosSupmzaMzaLteCalleNombreTieneCaracteresPermitidos() && tiendaViewModel.MisAtributosSupmzaMzaLteCalleNombreTieneLongitudPermitida())
            {
                using (var ctx = new palominoEntities())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                    {
                        try
                        {   //Ver si existe un nombre de tienda similar, en lo registros tienda del usuarioOperador, si existe no hago algo
                            var idsDeUsuarioOperador = (from item in ctx.Tienda
                                                        where item.IdUsuarioAlta == tiendaViewModel.IdUsuarioAlta
                                                        select new { item.Id, item.Activo }).ToList();

                            var idsDeUsuarioOperadorActivos = (from item in idsDeUsuarioOperador
                                                               where item.Activo == true
                                                               select item.Id).ToList();

                            var s = (from item in ctx.Tienda
                                     where idsDeUsuarioOperadorActivos.Contains(item.Id)
                                     select item.Nombre).ToList();

                            bool existeNombreDeTiendaEnBD = s.Contains(tiendaViewModel.Nombre);



                            //Busco el Id de usuario que necesitare, y ver que este activo
                            var resumenUsuarios = (from x in ctx.Usuario
                                                   select new { x.Id, x.Usuario1, x.Activo }).ToList();

                            var usuarioActivoBuscado = resumenUsuarios.SingleOrDefault(item => (item.Id == tiendaViewModel.IdUsuarioAlta) &&
                                                                                                (item.Activo == true));

                            if ((existeNombreDeTiendaEnBD == false) && (usuarioActivoBuscado != null))
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

                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception("Excepción lanzada y cachada en TiendaLN.PostCrearTienda" + ex.Message , ex);
                        }
                    }
                }

                return (respuesta);
            }

            else
            { return ("Verifica que el texto de entrada este en el formato permitido.");  }

        }




        public TiendaViewModel Get_BuscarTiendaDeUsuarioXNombre(TiendaViewModel tiendaViewModel)
        {
            TiendaViewModel tiendaBuscadaVM = null;

            //Eliminar espacios en blanco innecesarios en el atributo nombre de la tienda, y verificar que el nombre de la tienda contenga caracteres permitidos y que NO este vacia.          
            tiendaViewModel.AjustarAtributosSupmzaMzaLteCalleNombre();
            bool nombreTiendaContieneLongitudPermitida = (tiendaViewModel.Nombre.Length > 0) ? true : false;

            if (tiendaViewModel.ContieneCaracteresPermitidos(tiendaViewModel.Nombre) && nombreTiendaContieneLongitudPermitida)
            {
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

                            if (usuarioActivoBuscado != null)
                            {
                                //Busco las tiendas del usuario operador
                                var idsTiendasActivas = (from s in ctx.Tienda
                                                         where s.Activo == true
                                                         select new { s.Id, s.Activo, s.IdUsuarioAlta }).ToList();  //nueva linea

                                var idsTiendasActivasDeUsuarioOperador = (from s in idsTiendasActivas
                                                                          where s.IdUsuarioAlta == tiendaViewModel.IdUsuarioAlta
                                                                          select s.Id).ToList();    //nueva linea

                                var detallesTiendasActivasUsuarioOperador = (from s in ctx.Tienda
                                                                             where idsTiendasActivasDeUsuarioOperador.Contains(s.Id)
                                                                             select new { s.Id, s.Nombre }).ToList();  //nueva linea

                                var resumenTiendaBuscada = detallesTiendasActivasUsuarioOperador.SingleOrDefault(item => item.Nombre == tiendaViewModel.Nombre); //Nueva linea

                                if (resumenTiendaBuscada != null)
                                {
                                    Tienda tiendaX = ctx.Tienda.Find(resumenTiendaBuscada.Id);
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

                            else
                            {  throw new Exception("El usuario no se encontro ó no esta activo");   }

                            dbContextTransaction.Commit();
                        }

                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception("Excepción lanzada y cachada en TiendaLN.Get_BuscarTiendaDeUsuarioXNombre " + ex.Message , ex);
                        }
                    }
                }
                return (tiendaBuscadaVM);
            }

            else
            {  throw new Exception("Excepción lanzada y cachada en TiendaLN.Get_BuscarTiendaDeUsuarioXNombre. Verifique el formato de entrada."); }                       
        }



        public string Put_ActualizarTiendaDeUsuario(TiendaViewModel tiendaViewModel)
        {
            string respuesta = "";

            tiendaViewModel.AjustarAtributosSupmzaMzaLteCalleNombre(); 

            if(tiendaViewModel.MisAtributosSupmzaMzaLteCalleNombreTieneCaracteresPermitidos() && tiendaViewModel.MisAtributosSupmzaMzaLteCalleNombreTieneLongitudPermitida())
            {
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
                            if (usuarioActivoBuscado != null)
                            {
                                //Buscar la tienda almacenada en la BD y ver que este activa
                                Tienda tiendaOriginal = ctx.Tienda.Find(tiendaViewModel.Id);
                                if (tiendaOriginal != null)
                                {   //Veo que la tienda este activo
                                    if (tiendaOriginal.Activo == true)
                                    {
                                        //Verifico que la tiendaViewModel que viene llegando en la solicitud( que es la que este viendo el usuario)
                                        //sea la que esta almacenada en la bd, a través de la fechas y hora
                                        RelojServidor relojServidor = new RelojServidor();
                                        if ((relojServidor.EsMismaFechaYHoraSinMilisegundos(tiendaViewModel.FechaAlta, tiendaOriginal.FechaAlta))
                                            && (relojServidor.EsMismaFechaYHoraSinMilisegundos(tiendaViewModel.FechaModificacion, tiendaOriginal.FechaModificacion)))
                                        {
                                            //ver que el nombre de tienda que viene en la solicitud este disponible
                                            var resumenTiendas = (from s in ctx.Tienda
                                                                  select new { s.Id, s.IdUsuarioAlta, s.Activo }).ToList();

                                            var resumenTiendasActivas = (from s in resumenTiendas
                                                                         where s.Activo == true
                                                                         select s).ToList();
                                            var resumenTiendasActivasUsuarioOperador = (from s in resumenTiendasActivas
                                                                                        where s.IdUsuarioAlta == tiendaViewModel.IdUsuarioAlta
                                                                                        select s).ToList();

                                            var idsTiendasARecuperar = (from s in resumenTiendasActivasUsuarioOperador
                                                                        where s.Id != tiendaViewModel.Id
                                                                        select s.Id).ToList();


                                            var nombresAVerificar = (from s in ctx.Tienda
                                                                     where idsTiendasARecuperar.Contains(s.Id)
                                                                     select s.Nombre).ToList();
                                            //Ver que el nombre de tienda que viene en la solicitud este disponible
                                            /*var resumenTiendas = (from s in ctx.Tienda
                                                                 select new { s.Id, s.Nombre, s.IdUsuarioAlta }).ToList();

                                            var tiendasDeUsuarioOperador = (from s in resumenTiendas
                                                                            where s.IdUsuarioAlta == tiendaViewModel.IdUsuarioAlta
                                                                            select s).ToList();

                                            var nombresAVerificar = (from s in tiendasDeUsuarioOperador
                                                                     where s.Id != tiendaViewModel.Id
                                                                     select s.Nombre).ToList();*/

                                            if (nombresAVerificar.Contains(tiendaViewModel.Nombre) == false)
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
                            throw new Exception("Excepción lanzada y cachada en TiendaLN.Put_ActualizarTienda" + ex.Message, ex);
                        }
                    }
                }

                return (respuesta);
            }
            
            else
            { return ("Verifica que el texto de entrada este en el formato permitido."); }
        }



        public TiendaYProductosViewModel Get_RecuperarProductosDeTienda(TiendaViewModel tiendaViewModel)
        {
            List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM> productosDisponiblesEnTienda = new List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM>();
            List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> productosDisponiblesParaAniadir = new List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel>();
            TiendaYProductosViewModel resViewModel = new TiendaYProductosViewModel(); 

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        //Obtengo el id del usuario del que necesitare info y veo si esta activo al momento de esta consulta
                        var resumenUsuarios = (from s in ctx.Usuario.AsNoTracking()
                                               select new { s.Id, s.Usuario1, s.Activo }).ToList();

                        var usuarioBuscado = resumenUsuarios.SingleOrDefault(item => (item.Id == tiendaViewModel.IdUsuarioAlta ) && (item.Activo == true));

                        //Verifico que la tienda este activa al momento de esta consulta
                        Tienda tiendaBuscada = ctx.Tienda.Find(tiendaViewModel.Id);    
                        bool esTiendaValida= false;
                        if (tiendaBuscada != null) { 
                            if (tiendaBuscada.Activo == true)
                            {
                                //Verifico que la tiendaViewModel que viene llegando en la solicitud( que es la que este viendo el usuario)
                                //sea la que esta almacenada en la bd, a través de la fechas y hora
                                RelojServidor relojServidor = new RelojServidor();
                                esTiendaValida = (relojServidor.EsMismaFechaYHoraSinMilisegundos(tiendaViewModel.FechaAlta, tiendaBuscada.FechaAlta)) &&
                                    (relojServidor.EsMismaFechaYHoraSinMilisegundos(tiendaViewModel.FechaModificacion, tiendaBuscada.FechaModificacion)); 
                            }
                            else
                            { esTiendaValida = false; }
                        }
                        
                        else { esTiendaValida = false; }

                        if ((usuarioBuscado != null) && (esTiendaValida == true)) //Aqui se comprueba
                        {
                            var idUsuario = usuarioBuscado.Id;

                            var resumenAlmacena = (from s in ctx.Almacena
                                                   where (s.IdTienda == tiendaViewModel.Id) && (s.Activo == true)
                                                   select s).ToList();

                            var idsDeProductosMeInteresan = (from s in resumenAlmacena
                                                             select s.IdProducto).ToList();  //obtengo los ids de los productos de la tienda requerida

                            var resumenProductos = (from s in ctx.Producto
                                                    where (idsDeProductosMeInteresan.Contains(s.Id))   &&  (s.Activo==true)
                                                    select s).ToList();  //De la tabla Producto obtengo el Id y el codigoBarras de los productos que me interesan

                            var resumenProductoConDetalles = (from s in ctx.ProductoConDetalles
                                                             where (idsDeProductosMeInteresan.Contains(s.IdProducto))  && (s.Activo == true)  //De la tabla ProductoConDetalles obtengo los IdProducto e IdDetalleProducto que me interesan 
                                                             select s).ToList();


                            var idsDetalleProductoMeInteresan = (from s in resumenProductoConDetalles
                                                                select s.IdDetalleProducto).ToList();

                            var resumenDetalleProducto = (from s in ctx.DetalleProducto
                                                          where (idsDetalleProductoMeInteresan.Contains(s.Id))  &&  (s.Activo==true)
                                                          select s).ToList();    //De la tabla DetalleProducto obtengo las filas que me interesan


                            //Ya esta la info de las tablas fuera de EntityFramework, esta ahora en la memoria, asi que procedo a enlazar
                            //los registros tomados de las tablas Producto, ProductoConDetalles Y DetallesProducto; todos tiene el campo Activo = 1
                            productosDisponiblesEnTienda = EnlazarAlmacenaProductoProductoConDetallesDetalleProducto(resumenAlmacena, resumenProductos, resumenProductoConDetalles, resumenDetalleProducto);

                            //Ahora comienzo a obtener la info de los productos que se pueden aañadir a la tienda
                            var idsProductosDisponiblesUsuario = (from s in ctx.Producto
                                                                  where (s.IdUsuarioAlta == tiendaViewModel.IdUsuarioAlta) && (s.Activo == true)
                                                                  select s.Id).ToList();

                            var idsProductosQSePuedenAniadir = (from s in idsProductosDisponiblesUsuario
                                                                where idsDeProductosMeInteresan.Contains(s) == false
                                                                select s).ToList();         //aqui obtengo los ids de los productos que se pueden agregar a la tienda

                            var resumenProductosKSePuedenAniadir = (from s in ctx.Producto
                                                    where (idsProductosQSePuedenAniadir.Contains(s.Id)) && (s.Activo == true)
                                                    select s).ToList();    //Obtengo de la tabla Producto los productos que se pueden añadir

                            var resumenProductoConDetallesKSePuedenAniadir = (from s in ctx.ProductoConDetalles
                                                              where (idsProductosQSePuedenAniadir.Contains(s.IdProducto)) && (s.Activo == true)  
                                                              select s).ToList();     //Obtengo de la tabla ProductoConDetalles los items que necesitare

                            var idsDetalleProductoKSePuedenAniadir= (from s in resumenProductoConDetallesKSePuedenAniadir
                                                                     select s.IdDetalleProducto).ToList();

                            var resumenDetalleProductoKSePuedenAniadir = (from s in ctx.DetalleProducto
                                                                          where (idsDetalleProductoKSePuedenAniadir.Contains(s.Id)) && (s.Activo == true)
                                                                          select s).ToList();   //Obtengo de la tabla DetalleProducto los items que necesitare

                            //Ya extrai la informacion de los productos que se pueden añadir, la info ya esta fuera de EntityFramework, esta en memoria, procedo a enlazar las listas
                            productosDisponiblesParaAniadir = EnlazarProductoProductoConDetallesDetalleProducto(resumenProductosKSePuedenAniadir, resumenProductoConDetallesKSePuedenAniadir, resumenDetalleProductoKSePuedenAniadir);

                            resViewModel.ProductosDeTienda = productosDisponiblesEnTienda;
                            resViewModel.ProductosKSePuedenAniadirATienda = productosDisponiblesParaAniadir;

                            dbContextTransaction.Commit();
                        }

                        else
                        { throw new Exception("Verifica que el usuario y la tienda se encuentren en estado correcto."); }                       
                    }

                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion cachada y lanzada en TiendaLN.Get_RecuperarProductosDeTienda. " + ex.Message, ex);
                    }
                }
            }

            return (resViewModel);
        }


        //public string Post_AgregarProductosATienda(List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM>  elementosOrigen)
        public string Post_AgregarProductosATienda(TiendaYProductosViewModel elementosOrigen)
        {
            string respuesta = "inicializar..";

            using (var ctx = new palominoEntities())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        //Ver que la tienda esta viendo el usuario en la pantalla sea la misma en BD y este activa
                        //AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM filaUnica = elementosOrigen.First();
                        var idsTienda = (from s in ctx.Tienda
                                        select new { s.Id, s.Activo }).ToList();

                        var idsTiendaActivos = (from s in idsTienda
                                               where s.Activo.Equals(true)
                                               select s.Id).ToList();
                        var idsTiendasActivasYFechas = (from s in ctx.Tienda
                                                        where idsTiendaActivos.Contains(s.Id)
                                                        select new { s.Id, s.FechaAlta, s.FechaModificacion }).ToList();

                        var tiendaBuscadaEnBD = idsTiendasActivasYFechas.FirstOrDefault(s => s.Id == elementosOrigen.InfoTienda.Id );
                        RelojServidor miRelojServidor = new RelojServidor();
                        bool esTiendaActiva = (miRelojServidor.EsMismaFechaYHoraSinMilisegundos(tiendaBuscadaEnBD.FechaAlta, elementosOrigen.InfoTienda.FechaAlta)) &&
                            (miRelojServidor.EsMismaFechaYHoraSinMilisegundos(tiendaBuscadaEnBD.FechaModificacion, elementosOrigen.InfoTienda.FechaModificacion));
                        
                        
                        //Ver que el usuario operador este activo
                        var resumenUsuarios = (from x in ctx.Usuario
                                               select new { x.Id, x.Usuario1, x.Activo }).ToList();

                        var resumenUsuariosActivos = (from s in resumenUsuarios
                                                      where s.Activo == true
                                                      select s.Id).ToList();
                       
                        bool esUsuarioOperadorActivo = resumenUsuariosActivos.Contains(elementosOrigen.InfoTienda.IdUsuarioAlta);
                        
                       if(esTiendaActiva && esUsuarioOperadorActivo) //Aqui compruebo que la tienda y el usuario operador están activos
                       {

                            //---Buscar todos los proiductos que ya tiene la tienda
                            //Extraigo de la tabla Almacena las relaciones activas que pertenezcan al usuario operador
                            var resumenAlmacenaActivosEInactivos = (from s in ctx.Almacena
                                                                    where s.IdTienda == elementosOrigen.InfoTienda.Id
                                                                    select s).ToList();

                            var resumenAlmacena_ParaEnlazar = (from s in resumenAlmacenaActivosEInactivos
                                                          where s.Activo == true
                                                          select s).ToList(); //aqui ya tengo la info de la tabla Almacena fuera de EntityFramework, listo para enlazar

                            var idsResumenAlmacenaParaEnlazar = (from s in resumenAlmacena_ParaEnlazar
                                                                 select s.IdProducto).ToList();

                            var resumenProducto_ParaEnlazar = (from s in ctx.Producto
                                                               where (s.Activo == true) && (idsResumenAlmacenaParaEnlazar.Contains(s.Id))
                                                               select s).ToList();  //aqui ya tengo la info de la tabla Producto fuera de EntityFramework, listo para enlazar

                            var productoConDetallesParaEnlazar = (from s in ctx.ProductoConDetalles
                                                                  where (s.Activo == true) && (idsResumenAlmacenaParaEnlazar.Contains(s.IdProducto))
                                                                  select s).ToList(); //aqui ya tengo la info de tabla ProductoConDetalles fuera de EntityFramework, listo para enlazar

                            var idsDetallesProductosMeInteresan = (from s in productoConDetallesParaEnlazar
                                                                   select s.IdDetalleProducto).ToList();

                            var resumenDetalleProducto_ParaEnlazar = (from s in ctx.DetalleProducto
                                                                      where (s.Activo == true) && (idsDetallesProductosMeInteresan.Contains(s.Id))
                                                                      select s).ToList();  //aqui ya tengo la info de la tabla DetalleProducto fuera de EntityFramework.

                            List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM> productosEnTiendaAlMomento = EnlazarAlmacenaProductoProductoConDetallesDetalleProducto(resumenAlmacena_ParaEnlazar, resumenProducto_ParaEnlazar, productoConDetallesParaEnlazar, resumenDetalleProducto_ParaEnlazar);
                            //Aqui compruebo que el contenido de la tabla Almacena que el usuario ve en la GUI es lo que esta al momento de la consulta el la BD
                            bool sonLosMismosProductosKYaEstanEnTienda = elementosOrigen.EsIgualProductosDeTiendaXFechaAltaYFechaModif(productosEnTiendaAlMomento);

                            //--------------Ahora verificar si los productos que se quieren aniadir) que aparecen en la GUI son 
                            //los que están en la BD al momento de realizar la consulta

                            var idsTodosProductos = (from s in ctx.Producto
                                                     select new { s.Id, s.IdUsuarioAlta, s.Activo }).ToList();

                            var idsProductosActivos = (from s in idsTodosProductos
                                                       where s.Activo == true
                                                       select s).ToList();

                            var idsProductosActivosUsuario = (from s in idsProductosActivos
                                                              where s.IdUsuarioAlta == elementosOrigen.InfoTienda.IdUsuarioAlta
                                                              select s.Id).ToList();

                            var idsProductosKSePuedenAniadir = (from s in idsProductosActivosUsuario
                                                                where (idsResumenAlmacenaParaEnlazar.Contains(s) == false)
                                                                select s).ToList();

                            var productosKSePuedenAniadir = (from s in ctx.Producto
                                                             where idsProductosKSePuedenAniadir.Contains(s.Id)
                                                             select s).ToList();    //Aqui ya tengo los items de Producto listo para enlazar

                            var productosConDetallesKSePuedenAniadir = (from s in ctx.ProductoConDetalles
                                                                          where (idsProductosKSePuedenAniadir.Contains(s.IdProducto)) && (s.Activo == true)
                                                                          select s).ToList();    //Aqui ya tengo los items de ProductocConDetalles listo para enlazar

                            var idsDetalleProductosKSePuedenAniadir = (from s in productosConDetallesKSePuedenAniadir
                                                                       select s.IdDetalleProducto).ToList();

                            var detalleProductosKSePuedenAniadir = (from s in ctx.DetalleProducto
                                                                    where (idsDetalleProductosKSePuedenAniadir.Contains(s.Id))  && (s.Activo== true)
                                                                    select s).ToList();  // Aqui ya tengo los items de DetalleProducto listo para enlazar

                            List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> productosKSePuedenAniadirAlMomento = EnlazarProductoProductoConDetallesDetalleProducto(productosKSePuedenAniadir, productosConDetallesKSePuedenAniadir, detalleProductosKSePuedenAniadir);
                            //Aqui compruebo que lo(s) productos que se quieren aniadir a la tienda coincidan con los que se recupero desde la BD
                            bool productosKSePuedenAniadirEstanOk = elementosOrigen.EsIgualProductosKSePuedenAniadirATienda(productosKSePuedenAniadirAlMomento);

                            //Al controlador TienDaController.AgregarProductosATienda   agregarle try catch, cambie javascript  a var con let
                            if (  (sonLosMismosProductosKYaEstanEnTienda == true)  &&  (productosKSePuedenAniadirEstanOk == true)   )
                            {
                                var resumenInactivosDeTienda = (from s in resumenAlmacenaActivosEInactivos
                                                        where s.Activo == false
                                                        select s).ToList();

                                foreach (var item in elementosOrigen.ProductosKSePuedenAniadirATienda)
                                {
                                    var relacionBuscada = resumenInactivosDeTienda.FirstOrDefault(s => s.IdProducto == item.Producto_Id);

                                    if(relacionBuscada!=null)
                                    {   //La relacion ya existio en el pasado, pero esta desactivada, se procede a reactivar
                                        relacionBuscada.Activo = true;
                                        relacionBuscada.FechaModificacion = DateTime.Now;
                                        ctx.SaveChanges();
                                    }

                                    else
                                    {   //La relación no existe, asi que se crea
                                        DateTime fechaHoraActual = DateTime.Now;
                                        Almacena almacena = new Almacena();
                                        almacena.IdTienda = elementosOrigen.InfoTienda.Id;
                                        almacena.IdProducto = item.Producto_Id;
                                        almacena.IdUsuarioAlta = elementosOrigen.InfoTienda.IdUsuarioAlta;
                                        almacena.IdUsuarioModifico = elementosOrigen.InfoTienda.IdUsuarioAlta;
                                        almacena.FechaAlta = fechaHoraActual;
                                        almacena.FechaModificacion = fechaHoraActual;
                                        almacena.Activo = true;
                                        ctx.Almacena.Add(almacena);
                                        ctx.SaveChanges();
                                    }                                    
                                }
                                respuesta = "ok";
                            }

                            else
                            {
                                respuesta = "La info de tienda que esta viendo el usuario ya no coincide con la almacenada en BD";
                            }
                              
                        }

                        else
                           { //La info de tienda que esta viendo el usuario ya no coincide con la almacenada en BD ó el usuario esta inactivo.
                               respuesta = "La info de tienda que esta viendo el usuario ya no coincide con la almacenada en BD ó el usuario esta inactivo";
                           }

                        if (respuesta.Contains("ok"))
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
                        throw new Exception("Excepcion lanzada y cachada en TiendaLN.Post_AgregarProductosATienda " + ex.Message ,ex);
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
                                                coleccionViewModel.ForEach(item =>      //Aparentemente aqui deberia de hacerse la modificacion para insertar !!!
                                                {
                                                    //Obtener nuevo id para tabla Periodo
                                                    var idsPeriodo = (from elemento in ctx.Periodo
                                                                      select elemento.Id).ToList();

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
                        //verificamos si al momento de la solicitud la tienda y el usuario estan activos
                        List<TiendaViewModel> resumenTiendas = new List<TiendaViewModel>();
                        resumenTiendas = (from s in ctx.Tienda
                                                   select new TiendaViewModel { Id = s.Id, Activo = s.Activo }).ToList();

                        RulesEngineLN rulesEngineLN = new RulesEngineLN();

                        var resumenUsuarios = (from s in ctx.Usuario
                                               select new UsuarioViewModel { Id = s.Id, Activo = s.Activo }).ToList();

                        bool esActivoUsuarioOperador = rulesEngineLN.EsActivoIdUsuario(resumenUsuarios, parametroBuscarCaducidadViewModel.IdUsuarioAlta);
                        bool esActivaTiendaBuscada = rulesEngineLN.EsActivaTienda(resumenTiendas, parametroBuscarCaducidadViewModel.IdTienda);
                        if(esActivaTiendaBuscada && esActivoUsuarioOperador)
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

                            respuestaFinal = (  respuestaFinal.OrderByDescending(item => item.MiPeriodoViewModel.FechaCaducidad.Year)
                                .ThenByDescending(item => item.MiPeriodoViewModel.FechaCaducidad.Month)
                                .ThenByDescending(item=> item.MiPeriodoViewModel.FechaCaducidad.Day)  ).ToList();//.ThenByDescending;

                            //var x = from s in caducaJoinProductoJoinProductoConDetallesJoinDetalleProductoJoinPeriodoJoinPeriodoConUnidadJoinUnidadMedida
                            //        select new CaducaJoinProductoJoin__JoinPeriodoJoinPeriodoConUnidadJoinUnidadMedidaViewModel() { MiCaducaViewModel = new CaducaViewModel(),
                            //            MiCaducaViewModel};

                        }

                        else
                        {
                            //No es activa tienda buscada o usuario esta deshabilitado
                            throw new Exception("Tienda deshabilitada o usuario no habilitado");
                        }
                        
                    }

                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Excepcion lanzada y cachada en TiendaLN.Get_BuscarCaducidadEnTiendaFrom." + ex.Message , ex);
                    }
                }
            }

            return (respuestaFinal);
        }


        //----------------------------Metodos privados
        private List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM> EnlazarAlmacenaProductoProductoConDetallesDetalleProducto(List<Almacena> resumenAlmacena, List<Producto> resumenProductos, List<ProductoConDetalles> resumenProductoConDetalles, List<DetalleProducto> resumenDetalleProducto)
        {
            List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM> resViewModel = new List<AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM>();

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
                                                           Producto_FechaAlta = c.FechaAlta,
                                                           Producto_FechaModificacion = c.FechaModificacion,
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
                                                                                         s.Producto_FechaAlta,
                                                                                         s.Producto_FechaModificacion,
                                                                                         s.Producto_Activo,
                                                                                         ProductoConDetalles_IdProducto = c.IdProducto,
                                                                                         ProductoConDetalles_IdDetalleProducto = c.IdDetalleProducto,
                                                                                         ProductoConDetalles_FechaAlta = c.FechaAlta,
                                                                                         ProductoConDetalles_FechaModificacion = c.FechaModificacion,
                                                                                         ProductoConDetalles_Activo = c.Activo    ///----aqui me quede 3 mayo 2023
                                                                                     }).ToList();

            var resumenAlmacenaJOINresumenProductosJOINresumenProductoConDetallesJOINDetalleProducto = (from s in resumenAlmacenaJOINresumenProductosJOINresumenProductoConDetalles
                                                                                                        join x in resumenDetalleProducto
                                                                                                        on s.ProductoConDetalles_IdDetalleProducto equals x.Id
                                                                                                        select new
                                                                                                        {
                                                                                                            s.Almacena_IdTienda,
                                                                                                            s.Almacena_IdProducto,
                                                                                                            s.Almacena_FechaAlta,
                                                                                                            s.Almacena_FechaModificacion,
                                                                                                            s.Producto_Id,
                                                                                                            s.Producto_CodigoBarras,
                                                                                                            s.Producto_FechaAlta,
                                                                                                            s.Producto_FechaModificacion,
                                                                                                            s.Producto_Activo,
                                                                                                            s.ProductoConDetalles_IdProducto,
                                                                                                            s.ProductoConDetalles_IdDetalleProducto,
                                                                                                            s.ProductoConDetalles_FechaAlta,
                                                                                                            s.ProductoConDetalles_FechaModificacion,
                                                                                                            s.ProductoConDetalles_Activo,
                                                                                                            DetalleProducto_Id = x.Id,
                                                                                                            DetalleProducto_Nombre = x.Nombre,
                                                                                                            DetalleProducto_FechaAlta = x.FechaAlta,
                                                                                                            DetalleProducto_FechaModificacion = x.FechaModificacion,
                                                                                                            DetalleProducto_IdUsuarioAlta = x.IdUsuarioAlta,
                                                                                                            DetalleProducto_Activo = x.Activo
                                                                                                        }).ToList();

            resViewModel = (from c in resumenAlmacenaJOINresumenProductosJOINresumenProductoConDetallesJOINDetalleProducto
                            select new AlmacenaJoinProductoJoinProductoConDetallesJoinDetalleProductoVM
                            {
                                Almacena_IdTienda = c.Almacena_IdTienda,
                                Almacena_IdProducto = c.Almacena_IdProducto,
                                Almacena_FechaAlta = c.Almacena_FechaAlta,
                                Almacena_FechaModificacion = c.Almacena_FechaModificacion,
                                Producto_Id = c.Producto_Id,
                                Producto_CodigoBarras = c.Producto_CodigoBarras,
                                Producto_FechaAlta = c.Producto_FechaAlta,
                                Producto_FechaModificacion = c.Producto_FechaModificacion,
                                ProductoConDetalles_IdProducto = c.ProductoConDetalles_IdProducto,
                                ProductoConDetalles_IdDetalleProducto = c.ProductoConDetalles_IdDetalleProducto,
                                ProductoConDetalles_FechaAlta = c.ProductoConDetalles_FechaAlta,
                                ProductoConDetalles_FechaModificacion = c.ProductoConDetalles_FechaModificacion,
                                DetalleProducto_Id = c.DetalleProducto_Id,
                                DetalleProducto_Nombre = c.DetalleProducto_Nombre,
                                DetalleProducto_FechaAlta = c.DetalleProducto_FechaAlta,
                                DetalleProducto_FechaModificacion = c.DetalleProducto_FechaModificacion,
                                DetalleProducto_IdUsuarioAlta = c.DetalleProducto_IdUsuarioAlta,
                            }).ToList();  //<---aqui obtengo los productos que actualmente tiene ya añadidos la tienda

            return (resViewModel);
        }


        private List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> EnlazarProductoProductoConDetallesDetalleProducto(List<Producto> resumenProductos, List<ProductoConDetalles> resumenProductoConDetalles, List<DetalleProducto> resumenDetalleProducto)
        {
            List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> respuesta = new List<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel>();

            var resumenProductosJOINresumenProductoConDetalles = (from s in resumenProductos
                                                                  join c in resumenProductoConDetalles
                                                                  on s.Id equals c.IdProducto
                                                                  select new
                                                                  {
                                                                      Producto_Id = s.Id,
                                                                      Producto_CodigoBarras = s.CodigoBarras,
                                                                      Producto_FechaAlta = s.FechaAlta,
                                                                      Producto_FechaModificacion = s.FechaModificacion,
                                                                      ProductoConDetalles_IdProducto = c.IdProducto,
                                                                      ProductoConDetalles_IdDetalleProducto = c.IdDetalleProducto,
                                                                      ProductoConDetalles_FechaAlta = c.FechaAlta,
                                                                      ProductoConDetalles_FechaModificacion = c.FechaModificacion
                                                                  }).ToList();

            var resumenProductosJOINresumenProductoConDetallesJOINDetalleProducto = (from s in resumenProductosJOINresumenProductoConDetalles
                                                                                     join c in resumenDetalleProducto
                                                                                     on s.ProductoConDetalles_IdDetalleProducto equals c.Id
                                                                                     select new
                                                                                     {
                                                                                         s.Producto_Id,
                                                                                         s.Producto_CodigoBarras,
                                                                                         s.Producto_FechaAlta,
                                                                                         s.Producto_FechaModificacion,
                                                                                         s.ProductoConDetalles_IdProducto,
                                                                                         s.ProductoConDetalles_IdDetalleProducto,
                                                                                         s.ProductoConDetalles_FechaAlta,
                                                                                         s.ProductoConDetalles_FechaModificacion,
                                                                                         DetalleProducto_Id = c.Id,
                                                                                         DetalleProducto_Nombre = c.Nombre,
                                                                                         DetalleProducto_FechaAlta = c.FechaAlta,
                                                                                         DetalleProducto_FechaModificacion = c.FechaModificacion,
                                                                                         DetalleProducto_IdUsuarioAlta = c.IdUsuarioAlta
                                                                                     }).ToList();

            respuesta = (from s in resumenProductosJOINresumenProductoConDetallesJOINDetalleProducto
                        select new ProductoJoinProductoConDetallesJoinDetalleProductoViewModel
                        {
                            Producto_Id = s.Producto_Id,
                            Producto_CodigoBarras = s.Producto_CodigoBarras,
                            Producto_FechaAlta = s.Producto_FechaAlta,
                            Producto_FechaModificacion = s.Producto_FechaModificacion,
                            ProductoConDetalles_IdProducto = s.ProductoConDetalles_IdProducto,
                            ProductoConDetalles_IdDetalleProducto = s.ProductoConDetalles_IdDetalleProducto,
                            ProductoConDetalles_FechaAlta = s.ProductoConDetalles_FechaAlta,
                            ProductoConDetalles_FechaModificacion = s.ProductoConDetalles_FechaModificacion,
                            DetalleProducto_Id = s.DetalleProducto_Id,
                            DetalleProducto_Nombre = s.DetalleProducto_Nombre,
                            DetalleProducto_FechaAlta = s.DetalleProducto_FechaAlta,
                            DetalleProducto_FechaModificacion = s.DetalleProducto_FechaModificacion,
                            DetalleProducto_IdUsuarioAlta =s.DetalleProducto_IdUsuarioAlta
                        }).ToList();

            return (respuesta);
        }
    }
}