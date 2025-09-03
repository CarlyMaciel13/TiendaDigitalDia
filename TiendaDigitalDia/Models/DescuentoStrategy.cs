using TiendaDigitalDia.Interfaces;

namespace TiendaDigitalDia.Models
{
    public class DescuentoStrategy : IDescuentoStrategy
    {
        public decimal AplicarDescuento(decimal total) => total;
    }

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
