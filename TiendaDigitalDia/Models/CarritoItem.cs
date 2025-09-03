using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaDigitalDia.Models
{
    public class CarritoItem
    {
        public int CarritoItemID { get; set; }
        public int CarritoID { get; set; }
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public CarritoItem() { }

        public CarritoItem(int carritoItemID, int carritoID, int productoID, int cantidad, decimal precioUnitario)
        {
            this.CarritoItemID = carritoItemID;
            this.CarritoID = carritoID;
            this.ProductoID = productoID;
            this.Cantidad = cantidad;
            this.PrecioUnitario = precioUnitario;
        }

        public override string ToString()
        {
            return string.Concat($"Carrito Item ID: {CarritoItemID}\n",
                                 $"Carrito ID: {CarritoID}\n",
                                 $"¨Producto ID: {ProductoID}\n",
                                 $"Cantidad: {Cantidad}\n",
                                 $"Precio Unitario: {PrecioUnitario}\n",
                                 "--------------------------------");
        }
    }
}
