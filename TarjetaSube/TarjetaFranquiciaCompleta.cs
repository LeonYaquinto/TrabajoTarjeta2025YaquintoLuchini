using System;

public class TarjetaFranquiciaCompleta : Tarjeta
{
    private const int MAX_VIAJES_DIARIOS = 2;

    public override bool PagarPasaje()
    {
        ActualizarContadorViajes();

        if (viajesHoy >= MAX_VIAJES_DIARIOS)
        {
            return false;
        }

        viajesHoy++;
        ultimoViaje = DateTime.Now;
        return true;
    }

    public override decimal ObtenerTarifa()
    {
        return 0m;
    }

    public override string ObtenerTipo()
    {
        return "Franquicia Completa";
    }
}