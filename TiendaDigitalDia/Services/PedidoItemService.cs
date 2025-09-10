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
    public class PedidoItemService
    {
        private readonly TiendaContext tiendaContext;

        public PedidoItemService()
        {
            tiendaContext = new TiendaContext();
        }
        #region AGREGAR ITEMS DE PEDIDO A LA BASE
        // Inserta los items de un pedido en la base de datos
        public void InsertarItemsPedido(int pedidoID, List<PedidoItem> listaItems)
        {
            using (var connection = tiendaContext.GetConnection())
            {
                connection.Open();

                foreach (var item in listaItems)
                {
                    string query = @"INSERT INTO PedidoItem (PedidoID, ProductoID, Cantidad, PrecioUnitario)
                                     VALUES (@pedidoID, @productoID, @cantidad, @precioUnitario)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@pedidoID", pedidoID);
                    command.Parameters.AddWithValue("@productoID", item.ProductoID);
                    command.Parameters.AddWithValue("@cantidad", item.Cantidad);
                    command.Parameters.AddWithValue("@precioUnitario", item.PrecioUnitario);

                    command.ExecuteNonQuery();
                }
            }
        }
        #endregion
    }
}
