using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaDigitalDia.Interfaces;

namespace TiendaDigitalDia.Models
{
    public class Carrito
    {
        public int CarritoID { get; set; }
        public int ClienteID { get; set; }
        public IDescuentoStrategy DescuentoStrategy { get; set; }

        public Carrito() { }

        public Carrito(int carritoID, int clienteID)
        {
            this.CarritoID = carritoID;
            this.ClienteID = clienteID;
        }

        public override string ToString()
        {
            return string.Concat($"Carrito ID: {CarritoID}\n", 
                                 $"Cliente ID: {ClienteID}\n",
                                 "--------------------------------");
        }
    }
}
