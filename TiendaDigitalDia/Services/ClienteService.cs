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

                    string query = @"INSERT INTO Cliente (dni, nombre, apellido, email, telefono, fechaRegistro) 
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
    }
}
