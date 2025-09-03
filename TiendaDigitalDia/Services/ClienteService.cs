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
    public class ClienteService
    {
        private readonly TiendaContext tiendaContext;

        public ClienteService()
        {
            tiendaContext = new TiendaContext();
        }

        public void AgregarClienteASQL(Cliente cliente)
        {
            try
            {
                using (var connection = tiendaContext.GetConnection()) 
                {
                    connection.Open();

                    string query = @"INSERT INTO Cliente (Dni, Nombre, Apellido, Email, Telefono, FechaRegistro) 
                                     VALUES (@dni, @nombre, @apellido, @email, @telefono, @fechaRegistro)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@dni", cliente.Dni);
                    command.Parameters.AddWithValue("@nombre", cliente.Nombre);
                    command.Parameters.AddWithValue("@apellido", cliente.Apellido);
                    command.Parameters.AddWithValue("@email", cliente.Email);
                    command.Parameters.AddWithValue("@telefono", cliente.Telefono);
                    command.Parameters.AddWithValue("@fechaRegistro", cliente.FechaRegistro);

                    command.ExecuteNonQuery();
                }
            }catch (Exception)
            {
                Console.WriteLine("\nError al cargar los datos del cliente");
            }
        }

        public void CrearCuentaDeCliente()
        {
            int opcionSeguir;
            do
            {
                Console.WriteLine("Ingresar Dni: ");
                string dni = Console.ReadLine();
                Console.WriteLine("Ingresar Nombre: ");
                string nombre = Console.ReadLine();
                Console.WriteLine("Ingresar Apellido: ");
                string apellido = Console.ReadLine();
                Console.WriteLine("Ingresar Email: ");
                string email = Console.ReadLine();
                Console.WriteLine("Ingresar telefono: ");
                string telefono = Console.ReadLine();
                DateTime dateTime = DateTime.Now;

                Cliente cliente = null;

                cliente = new Cliente
                {
                    Dni = dni,
                    Nombre = nombre,
                    Apellido = apellido,
                    Email = email,
                    Telefono = telefono,
                    FechaRegistro = dateTime,
                };

                AgregarClienteASQL(cliente);

                Console.WriteLine("\n¿Desea agregar un cliente nuevo?");
                Console.WriteLine("1 - SI");
                Console.WriteLine("2 - NO");
                opcionSeguir = GuardClause.GuardClause.ValidarOpcion(1, 2);

            } while (opcionSeguir != 2);
        }

        public List<Cliente> ObtenerTodosLosDatosDeLosClientes()
        {
            List<Cliente> clientes = new List<Cliente>();

            string query = @"SELECT ClienteID, Dni, Nombre, Apellido, Email, Telefono, FechaRegistro FROM Cliente";

            using (SqlConnection connection = tiendaContext.GetConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Cliente cliente = new Cliente
                    {
                        ClienteID = reader.GetInt32(0),
                        Dni = reader.GetString(1),
                        Nombre = reader.GetString(2),
                        Apellido = reader.GetString(3),
                        Email = reader.GetString(4),
                        Telefono = reader.GetString(5),
                        FechaRegistro = reader.GetDateTime(6),
                    };
                    clientes.Add(cliente);
                }
             }
            return clientes;
        }

        public List<Cliente> BuscarClientePorDni(string filtro)
        {
            List<Cliente> clientes = new List<Cliente>();

            string query = @"SELECT ClienteID, Dni, Nombre, Apellido, Email, Telefono, FechaRegistro FROM Cliente WHERE Dni LIKE @dni";

            using (SqlConnection connection = tiendaContext.GetConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@dni", $"%{filtro}%");

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Cliente cliente = new Cliente
                    {
                        ClienteID = reader.GetInt32(0),
                        Dni = reader.GetString(1),
                        Nombre = reader.GetString(2),
                        Apellido = reader.GetString(3),
                        Email = reader.GetString(4),
                        Telefono = reader.GetString(5),
                        FechaRegistro = reader.GetDateTime(6)
                    };

                    clientes.Add(cliente);
                }

                reader.Close();
            }

            return clientes;
        }

        public Cliente SeleccionarClientePorDni()
        {
            Console.Write("Ingrese el DNI o parte del DNI del cliente a buscar: ");
            string buscarCliente = Console.ReadLine();

            List<Cliente> listaClientes = BuscarClientePorDni(buscarCliente);

            if (listaClientes.Count == 0)
            {
                Console.WriteLine("\nNo se encontro ningun cliente");
                return null;
            }

            if (listaClientes.Count == 1)
            {
                return listaClientes[0];
            }

            Console.WriteLine("\nSe encontraron varios clientes, seleccione uno:");
            for (int i = 0; i < listaClientes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {listaClientes[i].Dni} - {listaClientes[i].Nombre} - {listaClientes[i].Apellido} - {listaClientes[i].Email} - {listaClientes[i].Telefono}");
            }

            int seleccion = GuardClause.GuardClause.ValidarOpcion(1, listaClientes.Count);
            return listaClientes[seleccion - 1];
        }

        public void EditarCampoDelCliente(int id, string nombreCampo, object nuevoValor)
        {
            string query = $"UPDATE Cliente SET {nombreCampo} = @valor WHERE ClienteID = @id";

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

        public void ModificarClientePorDni()
        {
            int opcionSeguir;
            do
            {
                Cliente clienteSeleccionado = SeleccionarClientePorDni();

                if (clienteSeleccionado != null)
                {
                    Console.WriteLine("\nDatos actuales del cliente: ");
                    Console.WriteLine($"\nID: {clienteSeleccionado.ClienteID}" +
                                      $"\nDNI: {clienteSeleccionado.Dni}" +
                                      $"\nNombre: {clienteSeleccionado.Nombre}" +
                                      $"\nApellido: {clienteSeleccionado.Apellido}, " +
                                      $"\nEmail: {clienteSeleccionado.Email}" +
                                      $"\nTelefono: {clienteSeleccionado.Telefono}, " +
                                      $"\nFecha Registro: {clienteSeleccionado.FechaRegistro}");
                }

                Console.WriteLine("\nSeleccione el campo que desea modificar:");
                string[] camposDeCliente = { "Dni", "Nombre", "Apellido", "Email", "Telefono" };

                for (int i = 0; i < camposDeCliente.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {camposDeCliente[i]}");
                }

                Console.Write("Ingresar opcion: ");
                int opcionDeCampo = GuardClause.GuardClause.ValidarOpcion(1, camposDeCliente.Length);

                string campoSeleccionado = camposDeCliente[opcionDeCampo - 1];
                string nuevoValor;

                Console.Write($"\nIngrese nuevo valor para {campoSeleccionado}: ");
                nuevoValor = Console.ReadLine();

                EditarCampoDelCliente(clienteSeleccionado.ClienteID, campoSeleccionado, nuevoValor);

                Console.WriteLine("\nCampo modificado correctamente");

                Console.WriteLine("\n¿Desea modificar otro campo de este cliente?");
                Console.WriteLine("1 - SI");
                Console.WriteLine("2 - NO");
                opcionSeguir = GuardClause.GuardClause.ValidarOpcion(1, 2);

            } while (opcionSeguir != 2);
        }

        public void BorrarCliente(int id)
        {
            using (var connection = tiendaContext.GetConnection())
            {
                string query = "DELETE FROM Cliente WHERE ClienteID = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void BuscarYBorrarCliente()
        {
            int opcionSeguir;
            do
            {
                Cliente clienteSeleccionado = SeleccionarClientePorDni();

                if (clienteSeleccionado != null)
                {
                    Console.WriteLine("\nDatos actuales del cliente: ");
                    Console.WriteLine(clienteSeleccionado.ToString());

                    Console.WriteLine("\n¿Desea borrar este cliente?");
                    Console.WriteLine("1 - SI");
                    Console.WriteLine("2 - NO");
                    Console.Write("Ingresar opción: ");

                    int opcionBorrar = GuardClause.GuardClause.ValidarOpcion(1, 2);

                    if (opcionBorrar == 1)
                    {
                        BorrarCliente(clienteSeleccionado.ClienteID);
                        Console.WriteLine("\nCliente eliminado exitosamente");
                    }
                    else
                    {
                        Console.WriteLine("\nCliente no eliminado");
                    }
                }
                else
                {
                    Console.WriteLine("\nNo se encontro un cliente con ese DNI");
                }

                Console.WriteLine("\n¿Desea borrar otro cliente?");
                Console.WriteLine("1 - SI");
                Console.WriteLine("2 - NO");
                opcionSeguir = GuardClause.GuardClause.ValidarOpcion(1, 2);

            } while (opcionSeguir != 2);
        }

    }
}
