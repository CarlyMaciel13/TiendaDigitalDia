using System;
using System.Collections.Generic;
using TiendaDigitalDia.Models;
using TiendaDigitalDia.Services;

namespace TiendaDigitalDia
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductoService productoService = new ProductoService();
            ClienteService clienteService = new ClienteService();
            CarritoService carritoService = new CarritoService();
            PedidoService pedidoService = new PedidoService();
            int opcion;
            do
            {
                opcion = MenuPrincipal();

                switch (opcion)
                {
                    case 1:
                        MenuProductos(productoService);
                        break;
                    case 2:
                        MenuClientes(clienteService);
                        break;
                    case 3:
                        carritoService.GestionarCarrito();
                        break;
                    case 4:
                        pedidoService.MenuPedidos();
                        break;
                }
            } while (opcion != 0);
        }

        private static int MenuPrincipal()
        {
            Console.WriteLine("\n############# MENU PRINCIPAL #############");
            Console.WriteLine("1 - GESTIONAR PRODUCTOS");
            Console.WriteLine("2 - GESTIONAR CLIENTES");
            Console.WriteLine("3 - GESTIONAR CARRITO");
            Console.WriteLine("4 - GESTIONAR PEDIDOS");
            Console.WriteLine("0 - SALIR");
            Console.WriteLine("##########################################\n");

            int opcion = GuardClause.GuardClause.ValidarOpcion(0, 4);
            return opcion;
        }

        private static void MenuProductos(ProductoService productoService)
        {
            int opcion;
            do
            {
                Console.WriteLine("\n############# MENU PRODUCTOS #############");
                Console.WriteLine("1 - AGREGAR PRODUCTO");
                Console.WriteLine("2 - MOSTRAR PRODUCTOS");
                Console.WriteLine("3 - MODIFICAR PRODUCTO");
                Console.WriteLine("4 - ELIMINAR PRODUCTO");
                Console.WriteLine("0 - VOLVER AL MENU PRINCIPAL");
                Console.WriteLine("##########################################\n");

                opcion = GuardClause.GuardClause.ValidarOpcion(0, 4);

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

        private static void MenuClientes(ClienteService clienteService)
        {
            int opcion;
            do
            {
                Console.WriteLine("\n############## MENU CLIENTES #############");
                Console.WriteLine("1 - AGREGAR CLIENTE");
                Console.WriteLine("2 - MOSTRAR TODOS LOS CLIENTES");
                Console.WriteLine("3 - MODIFICAR CLIENTE");
                Console.WriteLine("4 - ELIMINAR CLIENTE");
                Console.WriteLine("0 - VOLVER AL MENU PRINCIPAL");
                Console.WriteLine("##########################################\n");

                opcion = GuardClause.GuardClause.ValidarOpcion(0, 4);

                switch (opcion)
                {
                    case 1:
                        clienteService.CrearCuentaDeCliente();
                        break;
                    case 2:
                        List<Cliente> clientes = clienteService.ObtenerTodosLosDatosDeLosClientes();
                        foreach (var cliente in clientes)
                        {
                            Console.WriteLine(cliente);
                        }
                        break;
                    case 3:
                        clienteService.ModificarClientePorDni();
                        break;
                    case 4:
                        clienteService.BuscarYBorrarCliente();
                        break;
                }

            } while (opcion != 0);
        }
    }
}
