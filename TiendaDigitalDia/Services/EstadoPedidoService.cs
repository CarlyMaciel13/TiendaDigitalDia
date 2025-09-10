using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TiendaDigitalDia.Context;
using TiendaDigitalDia.Models;

namespace TiendaDigitalDia.Services
{
    public class EstadoPedidoService
    {
        private readonly TiendaContext tiendaContext;

        public EstadoPedidoService()
        {
            tiendaContext = new TiendaContext();
        }
        #region BUSCA ESTADO POR ID
        // Busca y devuelve el estado de un pedido por su ID
        public EstadoPedido TraerEstadoPorID(int id)
        {
            using (var connection = tiendaContext.GetConnection())
            {
                connection.Open();

                string query = @"SELECT EstadoID, Descripcion FROM EstadoPedido WHERE EstadoID = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new EstadoPedido
                        {
                            EstadoID = reader.GetInt32(0),
                            Descripcion = reader.GetString(1)
                        };
                    }
                }
            }

            return new EstadoPedido(id, "Estado desconocido");
        }
        #endregion

        #region ELEVA ESTADOS DE PEDIDOS
        // Eleva el estado de un pedido al siguiente y actualiza la base de datos
        public void ElevarEstado(Pedido pedido)
        {
            if (pedido.Estado.EstadoID >= 4)
            {
                Console.WriteLine($"El pedido {pedido.PedidoID} ya esta en el ultimo estado ({pedido.Estado.Descripcion})");
                return;
            }

            using (var connection = tiendaContext.GetConnection())
            {
                connection.Open();

                int nuevoEstadoID = pedido.Estado.EstadoID + 1;

                string updateQuery = @"UPDATE Pedido SET EstadoID = @nuevoEstado WHERE PedidoID = @pedidoID";
                SqlCommand command = new SqlCommand(updateQuery, connection);

                command.Parameters.AddWithValue("@nuevoEstado", nuevoEstadoID);
                command.Parameters.AddWithValue("@pedidoID", pedido.PedidoID);

                command.ExecuteNonQuery();

                EstadoPedido nuevoEstado = TraerEstadoPorID(nuevoEstadoID);

                pedido.CambiarEstado(nuevoEstado);

                Console.WriteLine($"Pedido {pedido.PedidoID} actualizado al estado {nuevoEstado.EstadoID}: {nuevoEstado.Descripcion}");
            }
        }
        #endregion

        #region DA UN REPORTE DE PEDIDOS POR ESTADO
        // Genera un reporte de pedidos filtrados por un estado especifico
        public void ReportePedidosPorEstado(List<Pedido> pedidos, List<Cliente> clientes, int estadoID)
        {
            var estadoService = new EstadoPedidoService();
            EstadoPedido estado = estadoService.TraerEstadoPorID(estadoID);

            var reporte = from p in pedidos
                          join c in clientes on p.ClienteID equals c.ClienteID
                          where p.Estado.EstadoID == estadoID
                          select new
                          {
                              PedidoID = p.PedidoID,
                              Cliente = c.Nombre + " " + c.Apellido,
                              Estado = p.Estado.Descripcion,
                              Fecha = p.FechaPedido,
                              Total = p.TotalPedido
                          };

            if (reporte.Any())
            {
                Console.WriteLine($"\nPedidos en estado {estado.EstadoID} - {estado.Descripcion}: ");
                foreach (var item in reporte)
                {
                    Console.WriteLine($"Pedido: {item.PedidoID} | Cliente: {item.Cliente} | Estado: {item.Estado} | Fecha: {item.Fecha} | Total: {item.Total}");
                }
            }
            else
            {
                Console.WriteLine($"No hay pedidos en el estado {estado.Descripcion}");
            }
        }
        #endregion
    }
}
