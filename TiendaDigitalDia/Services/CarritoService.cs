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
                Console.WriteLine("No existe un cliente con ese DNI. Debe crear una cuenta primero.");
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

            Console.WriteLine($"Carrito creado para {clienteExistente.Nombre} {clienteExistente.Apellido} con ID: {carrito.CarritoID} (RECUERDE SU ID)");
            return carrito;
        }
    }
}
