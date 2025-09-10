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
    public class CarritoItemService
    {
        private readonly TiendaContext tiendaContext;
        PedidoItemService pedidoItemService = new PedidoItemService();
        PedidoService pedidoServie = new PedidoService();

        public CarritoItemService()
        {
            tiendaContext = new TiendaContext();
        }
        #region BORRA EL CARRITO Y SUS ITEMS
        // Borra un carrito y sus items de la base de datos
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
                        using (SqlCommand command = new SqlCommand(borrarProductosQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@CarritoID", carritoId);
                            command.ExecuteNonQuery();
                        }

                        string borrarCarritoQuery = "DELETE FROM Carrito WHERE CarritoID = @CarritoID";
                        using (SqlCommand cmd = new SqlCommand(borrarCarritoQuery, connection, transaction))
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
        #endregion

        #region AGREGAR ITEMS AL CARRITO
        // Agrega un producto al carrito con la cantidad indicada
        public void AgregarProductoAlCarrito(Carrito carrito)
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
        #endregion

        #region MUESTRA LOS ITEMS DEL CARRITO
        // Muestra los items contenidos en el carrito
        public void MostrarCarrito(Carrito carrito)
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
        #endregion

        #region BORRAR ITEMS DEL CARRITO
        // Quita un producto del carrito por su ID
        public void QuitarProductoDelCarrito(Carrito carrito)
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
        #endregion

        #region FINALIZA LA COMPRA DEL CARRITO
        // Finaliza la compra, calcula total y aplica descuentos
        public void FinalizarCompra(Carrito carrito)
        {
            decimal total = 0;
            List<PedidoItem> listaItems = new List<PedidoItem>();

            using (var connection = tiendaContext.GetConnection())
            {
                connection.Open();

                string queryCarritoItems = @"SELECT ci.Cantidad, ci.PrecioUnitario, ci.ProductoID
                                             FROM CarritoItem ci
                                             WHERE ci.CarritoID = @carritoId";

                SqlCommand command = new SqlCommand(queryCarritoItems, connection);
                command.Parameters.AddWithValue("@carritoId", carrito.CarritoID);

                using (var reader = command.ExecuteReader())
                {
                    int itemID = 1;
                    while (reader.Read())
                    {
                        int cantidad = reader.GetInt32(0);
                        decimal precio = reader.GetDecimal(1);
                        int productoID = reader.GetInt32(2);

                        total += cantidad * precio;
                        listaItems.Add(new PedidoItem(itemID, 0, productoID, cantidad, precio));
                        itemID++;
                    }
                }
            }

            Console.WriteLine("\n############# SELECCIONAR DESCUENTO #############");
            Console.WriteLine("1 - SIN DESCUENTO");
            Console.WriteLine("2 - DESCUENTO FIJO ($500)");
            Console.WriteLine("3 - DESCUENTO PORCENTUAL (10%)");
            int opcion = GuardClause.GuardClause.ValidarOpcion(1, 3);

            switch (opcion)
            {
                case 1:
                    carrito.DescuentoStrategy = new DescuentoStrategy();
                    break;
                case 2:
                    carrito.DescuentoStrategy = new DescuentoFijo(500);
                    break;
                case 3:
                    carrito.DescuentoStrategy = new DescuentoPorcentual(10);
                    break;
            }

            decimal totalConDescuento = carrito.DescuentoStrategy.AplicarDescuento(total);

            int pedidoID = pedidoServie.AgregarPedido(carrito.ClienteID, totalConDescuento);

            pedidoItemService.InsertarItemsPedido(pedidoID, listaItems);

            Console.WriteLine($"\nCompra finalizada y pedido confirmada");
            Console.WriteLine($"Pedido ID: {pedidoID}");
            Console.WriteLine($"Total a pagar (descuento aplicado): ${totalConDescuento}");
        }
        #endregion

        #region METODOS LINQ
        // Devuelve los 5 productos más caros disponibles
        public List<Producto> Top5ProductosPorPrecio()
        {
            ProductoService productoService = new ProductoService();
            List<Producto> productos = productoService.ObtenerTodosLosProductos();

            var top5 = productos.OrderByDescending(p => p.Precio).Take(5).ToList();
            return top5;
        }

        // Muestra el stock total por cada categoría de producto
        public void StockPorCategoria()
        {
            ProductoService productoService = new ProductoService();
            List<Producto> productos = productoService.ObtenerTodosLosProductos();

            var stockPorCategoria = productos.GroupBy(p => p.Categoria).Select(g => new { Categoria = g.Key, TotalStock = g.Sum(p => p.Stock) });

            Console.WriteLine("\n############# CANTIDAD DE STOCK POR CATEGORIA #############");
            foreach (var item in stockPorCategoria)
            {
                Console.WriteLine($"{item.Categoria}: {item.TotalStock} unidades");
            }
        }
        #endregion
    }
}
