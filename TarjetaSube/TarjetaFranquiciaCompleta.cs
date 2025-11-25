using System;

public class TarjetaFranquiciaCompleta : Tarjeta
{
    private const int MAX_VIAJES_GRATUITOS_DIARIOS = 2;

    // Override Descontar para NO permitir saldo negativo en franquicia
    public override bool Descontar(decimal monto)
    {
        // Franquicia completa NO permite saldo negativo
        if (saldo < monto)
            return false;

        saldo -= monto;
        AcreditarCarga();
        return true;
    }

    public override bool PagarPasaje()
    {
        ActualizarContadorViajes();

        // Si ya se usaron los 2 viajes gratuitos, cobrar tarifa completa
        if (viajesHoy >= MAX_VIAJES_GRATUITOS_DIARIOS)
        {
            bool resultado = Descontar(base.ObtenerTarifa());
            if (resultado)
            {
                viajesHoy++;
                ultimoViaje = DateTime.Now;
            }
            return resultado;
        }

        // Viajes gratuitos (primeros 2 del día)
        viajesHoy++;
        ultimoViaje = DateTime.Now;
        return true;
    }

    public override decimal ObtenerTarifa()
    {
        ActualizarContadorViajes();
        
        // Si ya se usaron los 2 viajes gratuitos, devolver tarifa completa
        if (viajesHoy >= MAX_VIAJES_GRATUITOS_DIARIOS)
        {
            return base.ObtenerTarifa();
        }
        
        return 0m;
    }

    public override string ObtenerTipo()
    {
        return "Franquicia Completa";
    }
}
