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

    public override bool PagarPasaje()
    {
        ActualizarContadorViajes();

        // Verificar intervalo mínimo de 5 minutos
        if (ultimoViaje != DateTime.MinValue)
        {
            TimeSpan tiempoTranscurrido = DateTime.Now - ultimoViaje;
            if (tiempoTranscurrido.TotalMinutes < MINUTOS_ESPERA)
            {
                return false;
            }
        }

        // Verificar si ya se usaron los 2 medio boletos del día
        if (viajesHoy >= MAX_VIAJES_MEDIO_BOLETO_DIARIOS)
        {
            // A partir del tercer viaje, cobrar tarifa completa
            bool resultado = Descontar(base.ObtenerTarifa());
            if (resultado)
            {
                viajesHoy++;
                ultimoViaje = DateTime.Now;
            }
            return resultado;
        }

        // Pagar con medio boleto (primeros 2 viajes del día)
        bool resultadoMedioBoleto = Descontar(ObtenerTarifa());
        if (resultadoMedioBoleto)
        {
            viajesHoy++;
            ultimoViaje = DateTime.Now;
        }
        return resultadoMedioBoleto;
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

    public override string ObtenerTipo()
    {
        return "Medio Boleto";
    }
}
