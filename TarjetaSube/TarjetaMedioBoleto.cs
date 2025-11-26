using System;

public class TarjetaMedioBoleto : Tarjeta
{
    private const decimal DESCUENTO = 0.5m;
    private const int MINUTOS_ESPERA = 5;
    private const int MAX_VIAJES_MEDIO_BOLETO_DIARIOS = 2;

    // Override Descontar para NO permitir saldo negativo en medio boleto
    public override bool Descontar(decimal monto)
    {
        // Medio boleto NO permite saldo negativo
        if (saldo < monto)
            return false;

        saldo -= monto;
        AcreditarCarga();
        return true;
    }

    public override bool PagarPasaje(string linea, bool esInterurbano)
    {
        ActualizarContadorViajes();

        // Verificar si es trasbordo primero
        bool esTrasbordo = VerificarTransbordo(linea);
        
        // Si NO es trasbordo, verificar intervalo mínimo de 5 minutos
        if (!esTrasbordo && ultimoViaje != DateTime.MinValue)
        {
            TimeSpan tiempoTranscurrido = DateTime.Now - ultimoViaje;
            if (tiempoTranscurrido.TotalMinutes < MINUTOS_ESPERA)
            {
                return false;
            }
        }
        
        decimal tarifaAPagar;
        if (esTrasbordo)
        {
            tarifaAPagar = 0m;
            esTransbordo = true;
        }
        else
        {
            // Verificar si ya se usaron los 2 medio boletos del día
            if (viajesHoy >= MAX_VIAJES_MEDIO_BOLETO_DIARIOS)
            {
                // A partir del tercer viaje, cobrar tarifa completa
                if (esInterurbano)
                {
                    tarifaAPagar = ObtenerTarifaInterurbana();
                }
                else
                {
                    tarifaAPagar = base.ObtenerTarifa();
                }
            }
            else
            {
                // Primeros 2 viajes con medio boleto
                if (esInterurbano)
                {
                    tarifaAPagar = ObtenerTarifaInterurbana();
                }
                else
                {
                    tarifaAPagar = ObtenerTarifa();
                }
            }
            esTransbordo = false;
        }

        bool resultado = Descontar(tarifaAPagar);
        if (resultado)
        {
            viajesHoy++;
            ultimoViaje = DateTime.Now;
            ultimoViajeParaTransbordo = DateTime.Now;
            ultimaLineaUsada = linea;
            ultimaTarifaCobrada = tarifaAPagar;
        }
        return resultado;
    }

    public override decimal ObtenerTarifa()
    {
        ActualizarContadorViajes();
        
        // Si ya se usaron los 2 medio boletos del día, devolver tarifa completa
        if (viajesHoy >= MAX_VIAJES_MEDIO_BOLETO_DIARIOS)
        {
            return base.ObtenerTarifa();
        }
        
        return base.ObtenerTarifa() * DESCUENTO;
    }

    public override decimal ObtenerTarifaInterurbana()
    {
        ActualizarContadorViajes();
        
        // Si ya se usaron los 2 medio boletos del día, devolver tarifa completa
        if (viajesHoy >= MAX_VIAJES_MEDIO_BOLETO_DIARIOS)
        {
            return base.ObtenerTarifaInterurbana();
        }
        
        return base.ObtenerTarifaInterurbana() * DESCUENTO;
    }

    public override string ObtenerTipo()
    {
        return "Medio Boleto";
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
