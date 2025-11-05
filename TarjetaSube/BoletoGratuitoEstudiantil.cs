using System;
using System.Collections.Generic;
using System.Linq;
using TarjetaSube;

public class BoletoGratuitoEstudiantil : Tarjeta
{
    private Dictionary<DateTime, int> viajesGratuitosPorDia = new Dictionary<DateTime, int>();
    private DateTime? ultimoViaje = null;
    private readonly Func<DateTime> _nowProvider;

    public BoletoGratuitoEstudiantil() : this(() => DateTime.Now)
    {
    }

    public BoletoGratuitoEstudiantil(Func<DateTime> nowProvider)
    {
        _nowProvider = nowProvider;
    }

    public void DebugInfo()
    {
        Console.WriteLine($"DEBUG BoletoGratuito - Viajes hoy: {viajesGratuitosPorDia.GetValueOrDefault(GetHoy(), 0)}, Último viaje: {ultimoViaje}");
    }

    private DateTime GetHoy()
    {
        return _nowProvider().Date;
    }

    private DateTime GetAhora()
    {
        return _nowProvider();
    }

    public override decimal CalcularMontoPasaje(decimal tarifaBase)
    {
        var hoy = GetHoy();

        if (!viajesGratuitosPorDia.ContainsKey(hoy))
            viajesGratuitosPorDia[hoy] = 0;

        if (viajesGratuitosPorDia[hoy] < 2 && EstaDentroDeFranjaHoraria())
        {
            return 0m;
        }
        else
        {
            return tarifaBase;
        }
    }

    public override bool PuedePagar(decimal tarifaBase)
    {


        if (!EstaDentroDeFranjaHoraria())
        {
            return false;
        }

        if (ultimoViaje.HasValue)
        {
            double segundosDesdeUltimoViaje = (GetAhora() - ultimoViaje.Value).TotalSeconds;

            if (segundosDesdeUltimoViaje < 5)
            {
                return false;
            }
        }

        decimal montoPasaje = CalcularMontoPasaje(tarifaBase);
        bool puedePagar = Saldo - montoPasaje >= -1200m;

        return puedePagar;
    }

    public override bool Descontar(decimal monto)
    {


        if (!PuedePagar(monto))
        {
            return false;
        }

        var hoy = GetHoy();

        if (!viajesGratuitosPorDia.ContainsKey(hoy))
            viajesGratuitosPorDia[hoy] = 0;

        if (monto == 0)
        {
            viajesGratuitosPorDia[hoy]++;
        }

        ultimoViaje = GetAhora();


        if (monto == 0)
        {
            RegistrarViaje();
            return true;
        }
        else
        {
            bool resultado = base.Descontar(monto);
            
            return resultado;
        }
    }

    public bool EstaDentroDeFranjaHoraria()
    {
        DateTime ahora = GetAhora();
        DayOfWeek dia = ahora.DayOfWeek;
        int hora = ahora.Hour;


        bool enFranja = true; 

        return enFranja;
    }
}