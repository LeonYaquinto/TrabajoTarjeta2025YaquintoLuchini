using System;
using System.Collections.Generic;

public class BibiEstacion
{
    private const decimal TARIFA_DIARIA = 1777.50m;
    private const decimal MULTA_POR_EXCESO = 1000m;
    private const int TIEMPO_MAXIMO_MINUTOS = 60;
    
    private string nombre;
    private Dictionary<int, DateTime> retirosPorTarjeta; // ID tarjeta -> fecha/hora de retiro
    private Dictionary<int, int> multasPendientesPorTarjeta; // ID tarjeta -> cantidad de multas acumuladas (se pagan en el próximo retiro)

    public BibiEstacion(string nombre)
    {
        this.nombre = nombre;
        this.retirosPorTarjeta = new Dictionary<int, DateTime>();
        this.multasPendientesPorTarjeta = new Dictionary<int, int>();
    }

    public string Nombre
    {
        get { return nombre; }
    }

    // Método para retirar una bicicleta
    public bool RetirarBici(Tarjeta tarjeta)
    {
        if (tarjeta == null)
            return false;

        // Verificar si ya tiene una bici retirada
        if (retirosPorTarjeta.ContainsKey(tarjeta.Id))
            return false;

        // Calcular el monto total a cobrar
        decimal montoTotal = CalcularMontoAPagar(tarjeta);

        // Intentar cobrar
        if (!tarjeta.Descontar(montoTotal))
            return false;

        // Registrar el retiro
        retirosPorTarjeta[tarjeta.Id] = DateTime.Now;
        
        // Limpiar las multas después de pagarlas
        if (multasPendientesPorTarjeta.ContainsKey(tarjeta.Id))
        {
            multasPendientesPorTarjeta[tarjeta.Id] = 0;
        }

        return true;
    }

    // Método para devolver una bicicleta
    public bool DevolverBici(Tarjeta tarjeta)
    {
        if (tarjeta == null)
            return false;

        // Verificar si tiene una bici retirada
        if (!retirosPorTarjeta.ContainsKey(tarjeta.Id))
            return false;

        DateTime horaRetiro = retirosPorTarjeta[tarjeta.Id];
        DateTime horaDevolucion = DateTime.Now;
        
        // Calcular tiempo de uso en minutos
        TimeSpan tiempoUso = horaDevolucion - horaRetiro;
        double minutosUsados = tiempoUso.TotalMinutes;

        // Verificar si excedió el tiempo máximo
        if (minutosUsados > TIEMPO_MAXIMO_MINUTOS)
        {
            // Acumular multa (se pagará en el próximo retiro)
            if (!multasPendientesPorTarjeta.ContainsKey(tarjeta.Id))
            {
                multasPendientesPorTarjeta[tarjeta.Id] = 0;
            }
            multasPendientesPorTarjeta[tarjeta.Id]++;
        }

        // Eliminar el registro de retiro
        retirosPorTarjeta.Remove(tarjeta.Id);

        return true;
    }

    // Calcular el monto a pagar (tarifa + multas pendientes acumuladas)
    private decimal CalcularMontoAPagar(Tarjeta tarjeta)
    {
        decimal monto = TARIFA_DIARIA;
        
        // Agregar TODAS las multas pendientes (sin resetear por cambio de día)
        if (multasPendientesPorTarjeta.ContainsKey(tarjeta.Id))
        {
            int cantidadMultas = multasPendientesPorTarjeta[tarjeta.Id];
            monto += cantidadMultas * MULTA_POR_EXCESO;
        }

        return monto;
    }

    // Métodos públicos para consultar estado
    public int ObtenerMultasPendientes(Tarjeta tarjeta)
    {
        if (tarjeta == null)
            return 0;
        
        if (multasPendientesPorTarjeta.ContainsKey(tarjeta.Id))
        {
            return multasPendientesPorTarjeta[tarjeta.Id];
        }
        return 0;
    }

    public bool TieneBiciRetirada(Tarjeta tarjeta)
    {
        if (tarjeta == null)
            return false;

        return retirosPorTarjeta.ContainsKey(tarjeta.Id);
    }

    public decimal ObtenerMontoAPagar(Tarjeta tarjeta)
    {
        if (tarjeta == null)
            return 0m;

        return CalcularMontoAPagar(tarjeta);
    }

    public static decimal TarifaDiaria
    {
        get { return TARIFA_DIARIA; }
    }

    public static decimal MultaPorExceso
    {
        get { return MULTA_POR_EXCESO; }
    }

    public static int TiempoMaximoMinutos
    {
        get { return TIEMPO_MAXIMO_MINUTOS; }
    }
}
