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

        public CarritoService()
        {
            tiendaContext = new TiendaContext();
        }

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
                        AgregarProductoAlCarrito(carrito);
                        break;
                    case "2":
                        QuitarProductoDelCarrito(carrito);
                        break;
                    case "3":
                        MostrarCarrito(carrito);
                        break;
                    case "4":
                        FinalizarCompra(carrito);
                        finalizar = true;
                        break;
                    case "5":
                        var top5 = Top5ProductosPorPrecio();
                        Console.WriteLine("\n############# TOP 5 PRECIOS MAS CAROS #############");
                        foreach (var p in top5)
                        {
                            Console.WriteLine($"{p.Nombre} - {p.Categoria} - ${p.Precio} - Stock: {p.Stock}");
                        }
                        break;
                    case "6":
                        StockPorCategoria();
                        break;
                    case "0":
                        Console.WriteLine("Compra cancelada, el carrito no se guardara");
                        BorrarCarrito(carrito.CarritoID);
                        finalizar = true;
                        break;
                }
            }
        }

        public void BorrarCarrito(int carritoId)
        {
            using (SqlConnection connection = tiendaContext.GetConnection())
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string borrarProductosQuery = "DELETE FROM CarritoItem WHERE CarritoID = @CarritoID";
                        using (SqlCommand cmd = new SqlCommand(borrarProductosQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@CarritoID", carritoId);
                            cmd.ExecuteNonQuery();
                        }

                        string deleteCarritoQuery = "DELETE FROM Carrito WHERE CarritoID = @CarritoID";
                        using (SqlCommand cmd = new SqlCommand(deleteCarritoQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@CarritoID", carritoId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        Console.WriteLine("Carrito eliminado exitosamente");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error al borrar el carrito: " + ex.Message);
                    }
                }
            }
        }

        private void AgregarProductoAlCarrito(Carrito carrito)
        {
            ProductoService productoService = new ProductoService();
            var productos = productoService.ObtenerTodosLosProductos();

            Console.WriteLine("\n############# PRODUCTOS DISPONIBLES #############");
            for (int i = 0; i < productos.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {productos[i].Nombre} - ${productos[i].Precio} - Stock: {productos[i].Stock}");
            }

            Console.Write("Seleccione un producto: ");
            int seleccion = GuardClause.GuardClause.ValidarOpcion(1, productos.Count);

            Producto productoSeleccionado = productos[seleccion - 1];

            Console.Write("Ingrese la cantidad: ");
            int cantidad = GuardClause.GuardClause.ValidarNumeroEntero();

            if (cantidad > productoSeleccionado.Stock)
            {
                Console.WriteLine("No hay suficiente stock disponible para hacer esa compra, intentelo nuevamente");
                return;
            }

            using (var connection = tiendaContext.GetConnection())
            {
                connection.Open();

                string query = @"INSERT INTO CarritoItem (CarritoID, ProductoID, Cantidad, PrecioUnitario)
                                 VALUES (@carritoId, @productoId, @cantidad, @precioUnitario)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@carritoId", carrito.CarritoID);
                command.Parameters.AddWithValue("@productoId", productoSeleccionado.ProductoID);
                command.Parameters.AddWithValue("@cantidad", cantidad);
                command.Parameters.AddWithValue("@precioUnitario", productoSeleccionado.Precio);

                command.ExecuteNonQuery();
            }

            Console.WriteLine($"{cantidad} x {productoSeleccionado.Nombre} agregado al carrito exitosamente");
        }

        private void MostrarCarrito(Carrito carrito)
        {
            List<CarritoItem> items = new List<CarritoItem>();

            using (var connection = tiendaContext.GetConnection())
            {
                connection.Open();

                string query = @"SELECT ci.CarritoItemID, ci.ProductoID, p.Nombre, ci.Cantidad, ci.PrecioUnitario
                                 FROM CarritoItem ci
                                 JOIN Producto p ON ci.ProductoID = p.ProductoID
                                 WHERE ci.CarritoID = @carritoId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@carritoId", carrito.CarritoID);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"\nItemID: {reader.GetInt32(0)} - {reader.GetString(2)} x {reader.GetInt32(3)} - ${reader.GetDecimal(4)} c/u");
                    }
                }
            }
        }

        private void QuitarProductoDelCarrito(Carrito carrito)
        {
            MostrarCarrito(carrito);
            Console.Write("\nIngrese el ID del producto a quitar: ");
            int carritoItemId = GuardClause.GuardClause.ValidarNumeroEntero();

            using (var connection = tiendaContext.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM CarritoItem WHERE CarritoItemID = @id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", carritoItemId);
                
                int filas = command.ExecuteNonQuery();

                if (filas > 0)
                {
                    Console.WriteLine("Producto quitado del carrito exitosamente");
                }
                else
                {
                    Console.WriteLine("No se encontro el producto");
                }
            }
        }

        private void FinalizarCompra(Carrito carrito)
        {
            decimal total = 0;

            using (var connection = tiendaContext.GetConnection())
            {
                connection.Open();

                string query = @"SELECT ci.Cantidad, ci.PrecioUnitario, ci.ProductoID
                                 FROM CarritoItem ci
                                 WHERE ci.CarritoID = @carritoId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@carritoId", carrito.CarritoID);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int cantidad = reader.GetInt32(0);
                        decimal precio = reader.GetDecimal(1);
                        total += cantidad * precio;
                    }
                }
            }

            Console.WriteLine($"\nCompra finalizada, Total a pagar: ${total}");
        }

        public List<Producto> Top5ProductosPorPrecio()
        {
            ProductoService productoService = new ProductoService();
            List<Producto> productos = productoService.ObtenerTodosLosProductos();

            var top5 = productos.OrderByDescending(p => p.Precio).Take(5).ToList();
            return top5;
        }

        public void StockPorCategoria()
        {
            ProductoService productoService = new ProductoService();
            List<Producto> productos = productoService.ObtenerTodosLosProductos();

            var stockPorCategoria = productos
                .GroupBy(p => p.Categoria)
                .Select(g => new { Categoria = g.Key, TotalStock = g.Sum(p => p.Stock) });

            Console.WriteLine("\n############# CANTIDAD DE STOCK POR CATEGORIA #############");
            foreach (var item in stockPorCategoria)
            {
                Console.WriteLine($"{item.Categoria}: {item.TotalStock} unidades");
            }
        }

    }
}
