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
    public class ProductoAPIController : ApiController
    {

        [HttpGet]
        public IHttpActionResult MostrarTodosRegistradosDeOperador(int idUsuarioOperador)
        {
            ProductoLN p = new ProductoLN();
            IList<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> lista = p.MostrarTodosRegistradosDeOperador(idUsuarioOperador);

            return (Ok(lista));
        }

        [HttpGet]
        public IHttpActionResult MostrarActivosRegistradosDeOperador(int idUsuarioOperador)
        {
            ProductoLN p = new ProductoLN();
            IList<ProductoJoinProductoConDetallesJoinDetalleProductoViewModel> lista = p.MostrarActivosDeOperador(idUsuarioOperador);

            return (Ok(lista));
        }


        [HttpGet]
        public IHttpActionResult BuscarProductoxCodigoBarras(string codigoBarrasBuscado, int idUsuarioOperador)
        {   
            try
            {
                if (ModelState.IsValid)
                {
                    ProductoLN productoLN = new ProductoLN();
                    ProductoJoinProductoConDetallesJoinDetalleProductoViewModel pJoin = productoLN.BuscarProductoXCodigoBarras(codigoBarrasBuscado, idUsuarioOperador);
                    if (pJoin.Producto_Id != 0)
                    {
                        return (Ok(pJoin));
                    }

                    else
                    {
                        return (BadRequest("No se encontro codigo de barras"));
                    }
                }

                else
                {
                    return (BadRequest("ProductoAPIController.BuscarProductoXCodigoBarras no pudo recibir el parametro codigoBarrasBuscado"));
                }
            }

            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost]
        public IHttpActionResult CrearProducto(ProductoViewModel productoViewModel)
        {
            string respuesta = "";

            try
            {
                if (ModelState.IsValid)
                {                    
                    RelojServidor relojServidor = new RelojServidor();
                    relojServidor.ColocarMismaFechaHoraEnCamposFechaAltaYFechaModificacion(productoViewModel);

                    ProductoLN p = new ProductoLN();
                    respuesta = p.Crear(productoViewModel);

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
                    return (BadRequest(respuesta));
                }
            }

            catch(Exception ex)
            {
                return (BadRequest(ex.Message));
            }
        }



        [HttpPut]
        public IHttpActionResult ProductoActualizar(ProductoJoinProductoConDetallesJoinDetalleProductoViewModel pJoinViewModel)
        {
            if(ModelState.IsValid)
            {
                ProductoLN productoLN = new ProductoLN();
                string res = productoLN.Modificar(pJoinViewModel);
                
                if(res.Contains("ok"))
                {
                    return (Ok());
                }
                else
                {
                    return (BadRequest(res));
                }
            }

            return (BadRequest("ProductoAPIController.ProductoActualizar no pudo recibir el parametro productoViewModel"));
        }


    }
}
