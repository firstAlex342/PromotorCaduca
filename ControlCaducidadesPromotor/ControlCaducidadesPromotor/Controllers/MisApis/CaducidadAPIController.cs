﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ControlCaducidadesPromotor.Models;
using ControlCaducidadesPromotor.Basicos;

namespace ControlCaducidadesPromotor.Controllers.MisApis
{
    public class CaducidadAPIController : ApiController
    {
        [System.Web.Http.HttpPost]
        public IHttpActionResult Post_InsertarNuevaCaducidad(List<TiendaJoinCaducidadesViewModel> coleccionViewModel)
        {
            string respuesta = "";

            try
            {
                if (ModelState.IsValid)
                {
                    TiendaLN tiendaLN = new TiendaLN();
                    respuesta = tiendaLN.Post_AgregarNuevaCaducidad(coleccionViewModel);

                    if (respuesta.Contains("ok"))
                    {
                        return (Ok());
                    }

                    else
                    {
                        return (BadRequest(respuesta));
                    }
                }

                else
                {
                    return (BadRequest("Fallo en el model binder CaducidadAPIController.Post_InsertarNuevaCaducidad"));
                }
            }

            catch(Exception ex)
            {
                return (BadRequest(ex.Message));
            }
        }


        [System.Web.Http.HttpPost]
        public IHttpActionResult Post_BuscarCaducidad(ParametroBuscarCaducidadViewModel parametroBuscarCaducidadViewModel)
        {
            IList<CaducaJoinProductoJoin__JoinPeriodoJoinPeriodoConUnidadJoinUnidadMedidaViewModel> respuestaColeccion;

            try
            {
                if (ModelState.IsValid)
                {
                    TiendaLN tiendaLN = new TiendaLN();
                    respuestaColeccion = tiendaLN.Get_BuscarCaducidadEnTiendaFrom(parametroBuscarCaducidadViewModel);


                    return (Ok(respuestaColeccion));
                }

                else
                {
                    return (BadRequest("Fallo en el model binder CaducidadAPIController.Post_BuscarCaducidad"));
                }
            }

            catch(Exception ex)
            {
                return (BadRequest(ex.Message));
            }

        }
    }
}