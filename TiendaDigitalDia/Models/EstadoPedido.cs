using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaDigitalDia.Models
{
    public class EstadoPedido
    {
        public int EstadoID { get; set; }
        public string Descrpcion { get; set; }

        public EstadoPedido() { }

        public EstadoPedido(int estadoID, string descrpcion)
        {
            this.EstadoID = estadoID;
            this.Descrpcion = descrpcion;
        }

        public override string ToString()
        {
            return base.ToString() + string.Concat($"\nEstado ID: {EstadoID}", 
                                                   $"\nDescrpcion: {Descrpcion}");
        }
    }
}
