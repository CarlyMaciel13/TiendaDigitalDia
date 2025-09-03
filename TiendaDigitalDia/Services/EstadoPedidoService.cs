using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    }
}
