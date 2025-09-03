using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaDigitalDia.Context;
using TiendaDigitalDia.Models;

namespace TiendaDigitalDia.Services
{
    public class ProductoService
    {
        private readonly TiendaContext tiendaContext;

        public ProductoService()
        {
            tiendaContext = new TiendaContext();
        }
        #region CREA UN NUEVO PRODUCTO
        public void AgregarProductoASQL(Producto producto)
        {
            try
            {
                using (var connection = tiendaContext.GetConnection())
                {
                    connection.Open();

                    string query = @"INSERT INTO Producto (Nombre, Categoria, Precio, Stock) 
                                     VALUES (@Nombre, @Categoria, @Precio, @Stock)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Categoria", producto.Categoria);
                    command.Parameters.AddWithValue("@Precio", producto.Precio);
                    command.Parameters.AddWithValue("@Stock", producto.Stock);

                    command.ExecuteNonQuery();
                    
                    Console.WriteLine("\nEl producto se cargo exitosamente");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("\nError al cargar los datos del producto");
            }
        }

        public void CrearNuevoProducto()
        {
            int opcionSeguir;
            do
            {
                Console.WriteLine("Ingresar Nombre: ");
                string nombre = GuardClause.GuardClause.ValidarEspaciosYLetras();
                Console.WriteLine("Ingresar Categoria: ");
                string categoria = GuardClause.GuardClause.ValidarEspaciosYLetras();
                Console.Write("Ingresar Precio: ");
                decimal precio = GuardClause.GuardClause.ValidarNumeroDecimal();
                Console.Write("Ingresar Stock: ");
                int stock = GuardClause.GuardClause.ValidarNumeroEntero();

                Producto producto = null;

                producto = new Producto
                {
                    Nombre = nombre,
                    Categoria = categoria,
                    Precio = precio,
                    Stock = stock,
                };

                AgregarProductoASQL(producto);

                Console.WriteLine("\n¿Desea agregar un producto nuevo?");
                Console.WriteLine("1 - SI");
                Console.WriteLine("2 - NO");
                opcionSeguir = GuardClause.GuardClause.ValidarOpcion(1, 2);

            } while (opcionSeguir != 2);
        }
        #endregion

        #region OBTIENE TODOS LOS PRODUCTOS (SE AGREGARAN FILTROS)
        public List<Producto> ObtenerTodosLosProductos()
        {
            List<Producto> productos = new List<Producto>();

            string query = @"SELECT ProductoID, Nombre, Categoria, Precio, Stock FROM Producto";

            using (SqlConnection connection = tiendaContext.GetConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Producto producto = new Producto
                    {
                        ProductoID = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Categoria = reader.GetString(2),
                        Precio = reader.GetDecimal(3),
                        Stock = reader.GetInt32(4)
                    };
                    productos.Add(producto);
                }
            }
            return productos;
        }
        #endregion

        #region BUSCA EL PRODUCTO A EDITAR (CON FILTRO POR NOMBRE)
        public List<Producto> BuscarProductoPorNombre(string filtro)
        {
            List<Producto> productos = new List<Producto>();

            string query = @"SELECT productoID, nombre, categoria, precio, Stock FROM Producto WHERE Nombre LIKE @nombre";

            using (SqlConnection connection = tiendaContext.GetConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@nombre", $"%{filtro}%");

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Producto producto = new Producto
                    {
                        ProductoID = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Categoria = reader.GetString(2),
                        Precio = reader.GetDecimal(3),
                        Stock = reader.GetInt32(4),
                    };

                    productos.Add(producto);
                }

                reader.Close();
            }

            return productos;
        }

        public Producto SeleccionarProductoPorNombre()
        {
            Console.Write("Ingrese el nombre o parte del nombre del producto a buscar: ");
            string buscarProducto = GuardClause.GuardClause.ValidarEspaciosYLetras();

            List<Producto> listaProductos = BuscarProductoPorNombre(buscarProducto);

            if (listaProductos.Count == 0)
            {
                Console.WriteLine("\nNo se encontro ningun producto");
                return null;
            }

            if (listaProductos.Count == 1)
            {
                return listaProductos[0];
            }

            Console.WriteLine("\nSe encontraron varios productos, seleccione uno:");
            for (int i = 0; i < listaProductos.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {listaProductos[i].Nombre} - {listaProductos[i].Categoria} - ${listaProductos[i].Precio} - {listaProductos[i].Stock}");
            }

            int seleccion = GuardClause.GuardClause.ValidarOpcion(1, listaProductos.Count);
            return listaProductos[seleccion - 1];
        }

        public void EditarCampoDelProducto(int id, string nombreCampo, object nuevoValor)
        {
            string query = $"UPDATE Producto SET {nombreCampo} = @valor WHERE ProductoID = @id";

            using (var connection = tiendaContext.GetConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@valor", nuevoValor);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void ModificarProductoPorNombre()
        {
            int opcionSeguir;
            do
            {
                Producto productoSeleccionado = SeleccionarProductoPorNombre();

                if (productoSeleccionado != null)
                {
                    Console.WriteLine("\nDatos actuales del producto: ");
                    Console.WriteLine($"\nID: {productoSeleccionado.ProductoID}\nNombre: {productoSeleccionado.Nombre}" +
                                      $"\nCategoría: {productoSeleccionado.Categoria}\nPrecio: {productoSeleccionado.Precio}\nStock: {productoSeleccionado.Stock}");
                }

                Console.WriteLine("\nSeleccione el campo que desea modificar:");
                string[] camposDeProducto = { "Nombre", "Categoria", "Precio", "Stock" };

                for (int i = 0; i < camposDeProducto.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {camposDeProducto[i]}");
                }

                Console.Write("Ingresar opcion: ");
                int opcionDeCampo = GuardClause.GuardClause.ValidarOpcion(1, camposDeProducto.Length);

                string campoSeleccionado = camposDeProducto[opcionDeCampo - 1];
                string nuevoValor;

                Console.Write($"\nIngrese nuevo valor para {campoSeleccionado}: ");
                nuevoValor = Console.ReadLine();

                EditarCampoDelProducto(productoSeleccionado.ProductoID, campoSeleccionado, nuevoValor);

                Console.WriteLine("\nCampo modificado correctamente");

                Console.WriteLine("\n¿Desea modificar otro campo de este producto?");
                Console.WriteLine("1 - SI");
                Console.WriteLine("2 - NO");
                opcionSeguir = GuardClause.GuardClause.ValidarOpcion(1, 2);

            } while (opcionSeguir != 2);
        }
        #endregion

        #region BORRA EL PRODUCTO (CON FILTRO POR NOMBRE)
        public void BorrarProducto(int id)
        {
            using (var connection = tiendaContext.GetConnection())
            {
                string query = "DELETE FROM Producto WHERE ProductoID = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void BuscarYBorrarProducto()
        {
            int opcionSeguir;
            do
            {
                Producto productoSeleccionado = SeleccionarProductoPorNombre();

                if (productoSeleccionado != null)
                {
                    Console.WriteLine("\nDatos actuales del producto: ");
                    Console.WriteLine(productoSeleccionado.ToString());

                    Console.WriteLine("\n¿Desea borrar este producto?");
                    Console.WriteLine("1 - SI");
                    Console.WriteLine("2 - NO");
                    Console.Write("Ingresar opción: ");

                    int opcionBorrar = GuardClause.GuardClause.ValidarOpcion(1, 2);

                    if (opcionBorrar == 1)
                    {
                        BorrarProducto(productoSeleccionado.ProductoID);
                        Console.WriteLine("\nProducto eliminado exitosamente");
                    }
                    else
                    {
                        Console.WriteLine("\nProducto no eliminado");
                    }
                }
                else
                {
                    Console.WriteLine("\nNo se encontro un producto con ese nombre");
                }

                Console.WriteLine("\n¿Desea borrar otro producto?");
                Console.WriteLine("1 - SI");
                Console.WriteLine("2 - NO");
                opcionSeguir = GuardClause.GuardClause.ValidarOpcion(1, 2);

            } while (opcionSeguir != 2);
        }
        #endregion
    }
}
