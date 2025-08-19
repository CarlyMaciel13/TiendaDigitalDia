using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaDigitalDia.Interfaces;

namespace TiendaDigitalDia.Models
{
    public class Pedido
    {
        public int PedidoID { get; set; }
        public int ClienteID { get; set; }
        public int EstadoID { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal TotalPedido { get; set; }
        
        public Pedido() { }

        public Pedido(int pedidoID, int clienteID, int estadoID, DateTime fechaPedido, decimal totalPedido)
        {
            this.PedidoID = pedidoID;
            this.ClienteID = clienteID;
            this.EstadoID = estadoID;
            this.FechaPedido = fechaPedido;
            this.TotalPedido = totalPedido;
        }

        public override string ToString()
        {
            return base.ToString() + string.Concat($"\nPedido ID: {PedidoID}",
                                                   $"\nCliente ID: {ClienteID}",
                                                   $"\nEstado ID: {EstadoID}",
                                                   $"\nFecha Pedido: {FechaPedido}",
                                                   $"\nTotal Pedido: {TotalPedido}");
        }
    }
}
