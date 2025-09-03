using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaDigitalDia.Models
{
    public class PedidoItem
    {
        public int PedidoItemID { get; set; }
        public int PedidoID { get; set; }
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public PedidoItem() { }

        public PedidoItem(int pedidoItemID, int pedidoID, int productoID, int cantidad, decimal precioUnitario)
        {
            this.PedidoItemID = pedidoItemID;
            this.PedidoID = pedidoID;
            this.ProductoID = productoID;
            this.Cantidad = cantidad;
            this.PrecioUnitario = precioUnitario;
        }

        public override string ToString()
        {
            return string.Concat($"Pedido Iteme ID: {PedidoItemID}\n",
                                 $"Pedido ID: {PedidoID}\n",
                                 $"Prodcuto ID: {ProductoID}\n",
                                 $"Cantidad: {Cantidad}\n",
                                 $"Precio Unitario: {PrecioUnitario}\n");
        }
    }
}
