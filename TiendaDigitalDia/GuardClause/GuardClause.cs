using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TiendaDigitalDia.GuardClause
{
    public class GuardClause
    {
        // Valida si una opcion ingresada por el usuario esta dentro del rango especificado
        // Solicita repetidamente un numero hasta que sea valido
        // Devuelve la opcion validada
        public static int ValidarOpcion(int minimo, int maximo)
        {
            bool pudo = false;
            int opcion = 0;
            while (!pudo)
            {
                pudo = int.TryParse(Console.ReadLine(), out opcion);
                if (!pudo || opcion < minimo || opcion > maximo)
                {
                    pudo = false;
                    Console.WriteLine(string.Concat("Solo numeros entre ", minimo, " y ", maximo, ".\nIntente nuevamente: "));
                }
            }
            return opcion;
        }

        public static string ValidarEspaciosYLetras()
        {
            string valor;
            Regex regex = new Regex(@"^[a-zA-Z\s]+$");

            do
            {
                valor = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(valor) || !regex.IsMatch(valor))
                {
                    Console.WriteLine("Este campo solo puede contener letras y espacios. Intente nuevamente.");
                }
                else break;
            } while (true);

            return valor;
        }

        public static decimal ValidarNumeroDecimal()
        {
            while (true)
            {
                if (decimal.TryParse(Console.ReadLine(), out decimal valor))
                    return valor;
                Console.WriteLine("Por favor, ingrese un numero valido (decimal).");
            }
        }

        public static int ValidarNumeroEntero()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int valor))
                    return valor;
                Console.WriteLine("Por favor, ingrese un numero entero valido.");
            }
        }
    }
}
