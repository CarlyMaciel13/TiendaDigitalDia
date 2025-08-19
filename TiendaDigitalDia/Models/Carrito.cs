using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaDigitalDia.Models
{
    public class Carrito
    {
        public int CarritoID { get; set; }
        public int ClienteID { get; set; }

        public Carrito() { }

        public Carrito(int carritoID, int clienteID)
        {
            this.CarritoID = carritoID;
            this.ClienteID = clienteID;
        }

        public override string ToString()
        {
            return base.ToString() + string.Concat($"\nCarrito ID: {CarritoID}", 
                                                   $"\nCliente ID: {ClienteID}");
        }
    }
}
