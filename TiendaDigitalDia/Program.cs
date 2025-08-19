using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaDigitalDia.Models;
using TiendaDigitalDia.Services;

namespace TiendaDigitalDia
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductoService productoService = new ProductoService();

            int opcion;
            do
            {
                opcion = Menu();

                switch (opcion)
                {
                    case 1:
                        productoService.CrearNuevoProducto();
                        break;
                    case 2:
                        List<Producto> productos = productoService.ObtenerTodosLosProductos();
                        foreach (var producto in productos)
                        {
                            Console.WriteLine(producto);
                        }
                        break;
                    case 3:
                        productoService.ModificarProductoPorNombre();
                        break;
                    case 4:
                        productoService.BuscarYBorrarProducto();
                        break;
                }
            } while (opcion != 0);
        }

        private static int Menu()
        {
            Console.WriteLine("\n############# MENU PRINCIPAL #############");
            Console.WriteLine("1 - AGREGAR PRODUCTO");
            Console.WriteLine("2 - MOSTRAR PRODUCTOS");
            Console.WriteLine("3 - MODIFICAR PRODUCTO");
            Console.WriteLine("4 - ELIMINAR PRODUCTO");
            Console.WriteLine("0 - SALIR");
            Console.WriteLine("############# MENU PRINCIPAL #############\n");

            int opcion = GuardClause.GuardClause.ValidarOpcion(0, 4);

            return opcion;
        }
    }
}
