using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaDigitalDia.Models
{
    public class Cliente
    {
        public int ClienteID { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaRegistro { get; set; }

        public Cliente() { }

        public Cliente(int clienteID, string dni, string nombre, string apellido, string email, string telefono, DateTime fechaRegistro)
        {
            this.ClienteID = clienteID;
            this.Dni = dni;
            this.Nombre = nombre;
            this.Apellido = apellido;
            this.Email = email;
            this.Telefono = telefono;
            this.FechaRegistro = fechaRegistro;
        }

        public override string ToString()
        {
            return string.Concat($"Cliente ID: {ClienteID}\n",
                                 $"Dni: {Dni}\n", 
                                 $"Nombre: {Nombre}\n", 
                                 $"Apellido: {Apellido}\n", 
                                 $"Email: {Email}\n", 
                                 $"Telefono: {Telefono}\n", 
                                 $"Fecha de Registro: {FechaRegistro}\n",
                                 "---------------------------");
        }
    }
}
