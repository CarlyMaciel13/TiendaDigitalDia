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
        public EstadoPedido Estado { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal TotalPedido { get; set; }

        private List<IObservador> observadores = new List<IObservador>();

        public Pedido() { }

        public Pedido(int pedidoID, int clienteID, EstadoPedido estado, DateTime fechaPedido, decimal totalPedido)
        {
            PedidoID = pedidoID;
            ClienteID = clienteID;
            Estado = estado;
            FechaPedido = fechaPedido;
            TotalPedido = totalPedido;
        }

        // Cambia el estado del pedido y notifica a todos los observadores registrados
        public void CambiarEstado(EstadoPedido nuevoEstado)
        {
            Estado = nuevoEstado;

            foreach (var observador in observadores)
            {
                observador.Actualizar(this, Estado);
            }
        }

        public override string ToString()
        {
            return string.Concat($"Pedido ID: {PedidoID}\n",
                                 $"Cliente ID: {ClienteID}\n",
                                 $"Estado: {Estado.Descripcion}\n",
                                 $"Fecha Pedido: {FechaPedido}\n",
                                 $"Total Pedido: {TotalPedido}\n",
                                 "--------------------------------");
        }
    }
}
