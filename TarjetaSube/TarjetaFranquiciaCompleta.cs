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

    public override bool PagarPasaje(string linea, bool esInterurbano)
    {
        ActualizarContadorViajes();

        // Verificar trasbordo
        bool esTrasbordo = VerificarTransbordo(linea);
        
        decimal tarifaAPagar;
        if (esTrasbordo)
        {
            tarifaAPagar = 0m;
            esTransbordo = true;
        }
        else
        {
            // Si ya se usaron los 2 viajes gratuitos, cobrar tarifa completa
            if (viajesHoy >= MAX_VIAJES_GRATUITOS_DIARIOS)
            {
                if (esInterurbano)
                {
                    tarifaAPagar = ObtenerTarifaInterurbana();
                }
                else
                {
                    tarifaAPagar = base.ObtenerTarifa();
                }
                
                bool resultadoPago = Descontar(tarifaAPagar);
                if (!resultadoPago)
                    return false;
                    
                esTransbordo = false;
            }
            else
            {
                // Viajes gratuitos (primeros 2 del día)
                tarifaAPagar = 0m;
                esTransbordo = false;
            }
        }

        viajesHoy++;
        ultimoViaje = DateTime.Now;
        ultimoViajeParaTransbordo = DateTime.Now;
        ultimaLineaUsada = linea;
        ultimaTarifaCobrada = tarifaAPagar;
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

    public override decimal ObtenerTarifaInterurbana()
    {
        ActualizarContadorViajes();
        
        // Si ya se usaron los 2 viajes gratuitos, devolver tarifa completa
        if (viajesHoy >= MAX_VIAJES_GRATUITOS_DIARIOS)
        {
            return base.ObtenerTarifaInterurbana();
        }
        
        return 0m;
    }

    public override string ObtenerTipo()
    {
        return "Franquicia Completa";
    }

    public override bool PuedePagarEnHorario()
    {
        DateTime ahora = DateTime.Now;
        
        // Lunes a viernes de 6 a 22
        if (ahora.DayOfWeek >= DayOfWeek.Monday && ahora.DayOfWeek <= DayOfWeek.Friday)
        {
            if (ahora.Hour >= 6 && ahora.Hour < 22)
            {
                return true;
            }
        }
        
        return false;
    }
}
