using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarjetaSube
{
    public class FranquiciaCompleta : Tarjeta
    {
        public override decimal CalcularMontoPasaje(decimal tarifaBase)
        {
            return 0m;
        }

        public override bool Descontar(decimal monto)
        {

            RegistrarViaje();
            return true;
        }

        public override bool PuedePagar(decimal tarifaBase)
        {
            return true;
        }


    }
}
