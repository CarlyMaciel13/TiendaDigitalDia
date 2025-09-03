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
    public class PedidoService
    {
        private readonly TiendaContext tiendaContext;
        EstadoPedidoService estadoPedidoService = new EstadoPedidoService();

        public PedidoService()
        {
            tiendaContext = new TiendaContext();
        }

        public void MenuPedidos()
        {
            int opcion;
            do
            {
                Console.WriteLine("\n############## MENU PEDIDOS #############");
                Console.WriteLine("1 - MOSTRAR PEDIDOS");
                Console.WriteLine("2 - ELEVAR ESTADO DE UN PEDIDO");
                Console.WriteLine("0 - VOLVER AL MENU PRINCIPAL");
                Console.WriteLine("##########################################\n");

                opcion = GuardClause.GuardClause.ValidarOpcion(0, 2);

                switch (opcion)
                {
                    case 1:
                        List<Pedido> pedidos = VerPedidos();
                        foreach (var pedido in pedidos)
                        {
                            Console.WriteLine(pedido);
                        }
                        break;

                    case 2:
                        ElevarEstadoDePedido();
                        break;
                }

            } while (opcion != 0);
        }

        public int AgregarPedido(int clienteID, decimal total)
        {
            using (var connection = tiendaContext.GetConnection())
            {
                connection.Open();

                string query = @"INSERT INTO Pedido (ClienteID, EstadoID, Fecha, Total)
                                 VALUES (@clienteID, @estadoID, @fecha, @total);
                                 SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@clienteID", clienteID);
                command.Parameters.AddWithValue("@estadoID", 1);
                command.Parameters.AddWithValue("@fecha", DateTime.Now);
                command.Parameters.AddWithValue("@total", total);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public List<Pedido> VerPedidos()
        {
            List<Pedido> pedidos = new List<Pedido>();

            using (var connection = tiendaContext.GetConnection())
            {
                connection.Open();
                string query = @"SELECT PedidoID, ClienteID, EstadoID, Fecha, Total FROM Pedido";

                SqlCommand command = new SqlCommand(query, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int estadoID = reader.GetInt32(2);

                        pedidos.Add(new Pedido
                        {
                            PedidoID = reader.GetInt32(0),
                            ClienteID = reader.GetInt32(1),
                            Estado = estadoPedidoService.TraerEstadoPorID(estadoID),
                            FechaPedido = reader.GetDateTime(3),
                            TotalPedido = reader.GetDecimal(4)
                        });
                    }
                }
            }

            return pedidos;
        }

        public void ElevarEstadoDePedido()
        {
            List<Pedido> pedidos = VerPedidos();
            foreach (var p in pedidos)
            {
                Console.WriteLine($"ID: {p.PedidoID} - Cliente: {p.ClienteID} - Estado: {p.Estado.Descripcion}");
            }

            Console.Write("\nIngrese el ID para elevar al estado siguiente: ");
            if (int.TryParse(Console.ReadLine(), out int idPedido))
            {
                Pedido pedidoSeleccionado = pedidos.Find(p => p.PedidoID == idPedido);
                if (pedidoSeleccionado != null)
                {
                    estadoPedidoService.ElevarEstado(pedidoSeleccionado);
                }
                else
                {
                    Console.WriteLine("Pedido no encontrado");
                }
            }
            else
            {
                Console.WriteLine("ID no existente");
            }
        }
    }
}
