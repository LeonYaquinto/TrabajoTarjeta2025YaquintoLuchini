using System;

public class TarjetaMedioBoleto : Tarjeta
{
    private const decimal DESCUENTO = 0.5m;
    private const int MINUTOS_ESPERA = 5;

    public override bool PagarPasaje()
    {
        if (ultimoViaje != DateTime.MinValue)
        {
            TimeSpan tiempoTranscurrido = DateTime.Now - ultimoViaje;
            if (tiempoTranscurrido.TotalMinutes < MINUTOS_ESPERA)
            {
                return false;
            }
        }

        bool resultado = Descontar(ObtenerTarifa());
        if (resultado)
        {
            ultimoViaje = DateTime.Now;
        }
        return resultado;
    }

    public override decimal ObtenerTarifa()
    {
        return base.ObtenerTarifa() * DESCUENTO;
    }

    public override string ObtenerTipo()
    {
        return "Medio Boleto";
    }
}