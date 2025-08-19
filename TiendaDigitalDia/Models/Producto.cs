using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TiendaDigitalDia.Models
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        public Producto() { }

        public Producto(int productoID, string nombre, string categoria, decimal precio, int stock)
        {
            this.ProductoID = productoID;
            this.Nombre = nombre;
            this.Categoria = categoria;
            this.Precio = precio;
            this.Stock = stock;
        }

        public override string ToString()
        {
            return string.Concat($"ProductoID: {ProductoID}\n",
                                 $"Nombre: {Nombre}\n",
                                 $"Categoria: {Categoria}\n",
                                 $"Precio: ${Precio}\n",
                                 $"Stock: {Stock}\n",                    
                                 "---------------------------");
        }
    }
}
