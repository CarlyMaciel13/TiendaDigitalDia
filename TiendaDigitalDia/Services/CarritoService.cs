using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaDigitalDia.Context;
using TiendaDigitalDia.Models;

namespace TiendaDigitalDia.Services
{
    public class CarritoService
    {
        private readonly TiendaContext tiendaContext;
        CarritoItemService carritoItemService = new CarritoItemService();

        public CarritoService()
        {
            tiendaContext = new TiendaContext();
        }
        #region MENU CARRITO
        // Gestiona la logica principal del carrito: agregar, quitar, mostrar, finalizar compra
        public void GestionarCarrito()
        {
            Carrito carrito = IniciarCompraConDni();
            if (carrito == null) return;

            bool finalizar = false;
            while (!finalizar)
            {
                Console.WriteLine("\n############# GESTION DE CARRITO #############");
                Console.WriteLine("1 - AGREGAR PRODUCTO");
                Console.WriteLine("2 - QUITAR PRODUCTO");
                Console.WriteLine("3 - VAR CARRITO");
                Console.WriteLine("4 - FINALIZAR COMPRA");
                Console.WriteLine("5 - TOP 5 PRECIOS MAS CAROS");
                Console.WriteLine("6 - VER CANTIDAD DE STOCK POR CATEGORIAS");
                Console.WriteLine("0 - CANCELAR COMPRA");
                Console.Write("Seleccione una opcion: ");
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        carritoItemService.AgregarProductoAlCarrito(carrito);
                        break;
                    case "2":
                        carritoItemService.QuitarProductoDelCarrito(carrito);
                        break;
                    case "3":
                        carritoItemService.MostrarCarrito(carrito);
                        break;
                    case "4":
                        carritoItemService.FinalizarCompra(carrito);
                        finalizar = true;
                        break;
                    case "5":
                        var top5 = carritoItemService.Top5ProductosPorPrecio();
                        Console.WriteLine("\n############# TOP 5 PRECIOS MAS CAROS #############");
                        foreach (var p in top5)
                        {
                            Console.WriteLine($"{p.Nombre} - {p.Categoria} - ${p.Precio} - Stock: {p.Stock}");
                        }
                        break;
                    case "6":
                        carritoItemService.StockPorCategoria();
                        break;
                    case "0":
                        Console.WriteLine("Compra cancelada, el carrito no se guardara");
                        carritoItemService.BorrarCarrito(carrito.CarritoID);
                        finalizar = true;
                        break;
                }
            }
        }
        #endregion

        #region CREA UN CARRITO POR DNI
        // Inicia una compra solicitando DNI del cliente y creando un carrito
        public Carrito IniciarCompraConDni()
        {
            Console.WriteLine("Ingrese su DNI para iniciar la compra: ");
            string dni = Console.ReadLine();

            Cliente clienteExistente = null;

            List<Cliente> clientes = new List<Cliente>();
            using (var connection = tiendaContext.GetConnection())
            {
                connection.Open();

                string query = "SELECT clienteID, dni, nombre, apellido FROM Cliente";
                SqlCommand command = new SqlCommand(query, connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientes.Add(new Cliente
                        {
                            ClienteID = reader.GetInt32(0),
                            Dni = reader.GetString(1),
                            Nombre = reader.GetString(2),
                            Apellido = reader.GetString(3),
                        });
                    }
                }
            }

            // Buscar si existe el cliente
            clienteExistente = clientes.FirstOrDefault(c => c.Dni == dni);

            if (clienteExistente == null)
            {
                Console.WriteLine("No existe un cliente con ese DNI, Debe crear una cuenta primero");
                return null;
            }

            Carrito carrito = new Carrito
            {
                ClienteID = clienteExistente.ClienteID
            };

            using (var connection = tiendaContext.GetConnection())
            {
                connection.Open();
                string insertQuery = "INSERT INTO Carrito (clienteID) VALUES (@clienteId); SELECT SCOPE_IDENTITY();";
                SqlCommand command = new SqlCommand(insertQuery, connection);

                command.Parameters.AddWithValue("@clienteId", carrito.ClienteID);

                carrito.CarritoID = Convert.ToInt32(command.ExecuteScalar());
            }

            Console.WriteLine($"Carrito creado para {clienteExistente.Nombre} {clienteExistente.Apellido} con ID: {carrito.CarritoID}");
            return carrito;
        }
        #endregion
    }
}
