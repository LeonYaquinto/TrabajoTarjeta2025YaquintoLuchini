using System;
using TarjetaSube;

public class TarjetaComun : Tarjeta
{



    public decimal CalcularDescuentoTarjetaComun(decimal tarifaBase)
    {
        int viajesEsteMes = CantidadViajesEsteMes();

        if (viajesEsteMes >= 30 && viajesEsteMes <= 59)
        {
            return tarifaBase * 0.20m;
        }
        else if (viajesEsteMes >= 60 && viajesEsteMes <= 80)
        {
            return tarifaBase * 0.25m;
        }

        return 0m;
    }

    public override decimal CalcularMontoPasaje(decimal tarifaBase)
    {
        decimal descuento = CalcularDescuentoTarjetaComun(tarifaBase);
        return tarifaBase - descuento;
    }

    public override bool PuedePagar(decimal tarifaBase)
    {
        decimal montoPasaje = CalcularMontoPasaje(tarifaBase);
        return Saldo - montoPasaje >= -1200m; 
    }
}