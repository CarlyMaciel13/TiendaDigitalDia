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
        ClienteService clienteService = new ClienteService();

        public PedidoService()
        {
            tiendaContext = new TiendaContext();
        }
        #region MENU PEDIDOS
        // Muestra el menu de pedidos con opciones para mostrar, filtrar por estado y elevar estado
        public void MenuPedidos()
        {
            int opcion;
            do
            {
                Console.WriteLine("\n############## MENU PEDIDOS #############");
                Console.WriteLine("1 - MOSTRAR PEDIDOS");
                Console.WriteLine("2 - MOSTRAR PEDIDOS POR ESTADO");
                Console.WriteLine("3 - ELEVAR ESTADO DE UN PEDIDO");
                Console.WriteLine("0 - VOLVER AL MENU PRINCIPAL");
                Console.WriteLine("##########################################\n");

                opcion = GuardClause.GuardClause.ValidarOpcion(0, 3);

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
                        Console.Write("Ingrese el ID del estado a filtrar (1 al 4): ");
                        int estadoID = GuardClause.GuardClause.ValidarOpcion(0, 4);

                        List<Pedido> pedidosEstado = VerPedidos();
                        List<Cliente> clientes = clienteService.ObtenerTodosLosDatosDeLosClientes();

                        estadoPedidoService.ReportePedidosPorEstado(pedidosEstado, clientes, estadoID);
                        break;
                    case 3:
                        ElevarEstadoDePedido();
                        break;
                }

            } while (opcion != 0);
        }
        #endregion

        #region AGREGAR PEDIDO A LA BASE
        // Inserta un nuevo pedido en la base y devuelve el ID generado
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
        #endregion

        #region VER TODOS LOS PEDIDOS
        // Devuelve todos los pedidos de la base con su estado correspondiente
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
        #endregion

        #region ELEVA ESTADOS DE PEDIDOS
        // Permite al usuario seleccionar un pedido y elevar su estado al siguiente
        public void ElevarEstadoDePedido()
        {
            List<Pedido> pedidos = VerPedidos();
            foreach (var pedido in pedidos)
            {
                Console.WriteLine($"ID: {pedido.PedidoID} - Cliente: {pedido.ClienteID} - Estado: {pedido.Estado.Descripcion}");
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
        #endregion
    }
}
