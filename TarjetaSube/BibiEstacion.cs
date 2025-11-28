using System;
using System.Collections.Generic;

public class BibiEstacion
{
    private const decimal TARIFA_DIARIA = 1777.50m;
    private const decimal MULTA_POR_EXCESO = 1000m;
    private const int TIEMPO_MAXIMO_MINUTOS = 60;
    
    private string nombre;
    private Dictionary<int, DateTime> retirosPorTarjeta; // ID tarjeta -> fecha/hora de retiro
    private Dictionary<int, int> multasPendientesPorTarjeta; // ID tarjeta -> cantidad de multas acumuladas en el día
    private Dictionary<int, DateTime> ultimoPagoPorTarjeta; // ID tarjeta -> fecha del último pago (para control diario)

    public BibiEstacion(string nombre)
    {
        this.nombre = nombre;
        this.retirosPorTarjeta = new Dictionary<int, DateTime>();
        this.multasPendientesPorTarjeta = new Dictionary<int, int>();
        this.ultimoPagoPorTarjeta = new Dictionary<int, DateTime>();
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
        
        // Registrar el pago del día
        ultimoPagoPorTarjeta[tarjeta.Id] = DateTime.Now;
        
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
            // Acumular multa
            ActualizarMultasDiarias(tarjeta.Id);
            
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

    // Calcular el monto a pagar (tarifa + multas pendientes)
    private decimal CalcularMontoAPagar(Tarjeta tarjeta)
    {
        ActualizarMultasDiarias(tarjeta.Id);
        
        decimal monto = TARIFA_DIARIA;
        
        // Agregar multas pendientes
        if (multasPendientesPorTarjeta.ContainsKey(tarjeta.Id))
        {
            int cantidadMultas = multasPendientesPorTarjeta[tarjeta.Id];
            monto += cantidadMultas * MULTA_POR_EXCESO;
        }

        return monto;
    }

    // Actualizar multas diarias (resetear si es un nuevo día)
    private void ActualizarMultasDiarias(int idTarjeta)
    {
        DateTime ahora = DateTime.Now;
        
        if (ultimoPagoPorTarjeta.ContainsKey(idTarjeta))
        {
            DateTime ultimoPago = ultimoPagoPorTarjeta[idTarjeta];
            
            // Si es un nuevo día, resetear las multas
            if (ultimoPago.Date != ahora.Date)
            {
                if (multasPendientesPorTarjeta.ContainsKey(idTarjeta))
                {
                    multasPendientesPorTarjeta[idTarjeta] = 0;
                }
            }
        }
    }

    // Métodos públicos para consultar estado
    public int ObtenerMultasPendientes(Tarjeta tarjeta)
    {
        if (tarjeta == null)
            return 0;

        ActualizarMultasDiarias(tarjeta.Id);
        
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
