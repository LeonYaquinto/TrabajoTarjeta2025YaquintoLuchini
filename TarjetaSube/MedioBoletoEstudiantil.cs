using System;
using System.Collections.Generic;
using System.Linq;
using TarjetaSube;

public class MedioBoletoEstudiantil : Tarjeta
{
    private Dictionary<DateTime, int> viajesPorDia = new Dictionary<DateTime, int>();
    private DateTime? ultimoViaje = null;
    private readonly Func<DateTime> _nowProvider;

    public MedioBoletoEstudiantil() : this(() => DateTime.Now)
    {
    }

    public MedioBoletoEstudiantil(Func<DateTime> nowProvider)
    {
        _nowProvider = nowProvider;
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

        var hoy = GetHoy();
        int viajesHoy = viajesPorDia.GetValueOrDefault(hoy, 0);


        if (viajesHoy >= 2)
        {

            return false;
        }

        decimal montoPasaje = CalcularMontoPasaje(tarifaBase);
        bool puedePagar = Saldo - montoPasaje >= -1200m;

        return puedePagar;
    }

    public void DebugInfo()
    {
        Console.WriteLine($"DEBUG - Último viaje: {ultimoViaje}, Viajes hoy: {viajesPorDia.GetValueOrDefault(GetHoy(), 0)}");
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

        if (!viajesPorDia.ContainsKey(hoy))
            viajesPorDia[hoy] = 0;

        bool aplicarMedioBoleto = viajesPorDia[hoy] < 2 && EstaDentroDeFranjaHoraria();


        return aplicarMedioBoleto ? tarifaBase / 2 : tarifaBase;
    }

    

    public override bool Descontar(decimal monto)
    {


        if (!PuedePagar(monto))
        {

            return false;
        }

        var hoy = GetHoy();
        if (!viajesPorDia.ContainsKey(hoy))
            viajesPorDia[hoy] = 0;

        viajesPorDia[hoy]++;
        ultimoViaje = GetAhora();

       


        bool resultado = base.Descontar(monto);


        return resultado;
    }

    private bool EstaDentroDeFranjaHoraria()
    {
        DateTime ahora = GetAhora();
        DayOfWeek dia = ahora.DayOfWeek;
        int hora = ahora.Hour;

        bool enFranja = true; 
        
        return enFranja;
    }
}