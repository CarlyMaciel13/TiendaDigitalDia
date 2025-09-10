using TiendaDigitalDia.Interfaces;

namespace TiendaDigitalDia.Models
{
    // Estrategia de descuento que no aplica ningun descuento

    public class DescuentoStrategy : IDescuentoStrategy
    {
        public decimal AplicarDescuento(decimal total) => total;
    }

    // Estrategia de descuento fijo que resta un monto fijo del total
    public class DescuentoFijo : IDescuentoStrategy
    {
        private readonly decimal monto;

        public DescuentoFijo(decimal monto)
        {
            this.monto = monto;
        }

        public decimal AplicarDescuento(decimal total)
        {
            return total - monto < 0 ? 0 : total - monto;
        }
    }

    // Estrategia de descuento porcentual que aplica un porcentaje sobre el total
    public class DescuentoPorcentual : IDescuentoStrategy
    {
        private readonly decimal porcentaje;

        public DescuentoPorcentual(decimal porcentaje)
        {
            this.porcentaje = porcentaje;
        }

        public decimal AplicarDescuento(decimal total)
        {
            return total - (total * porcentaje / 100);
        }
    }
}
