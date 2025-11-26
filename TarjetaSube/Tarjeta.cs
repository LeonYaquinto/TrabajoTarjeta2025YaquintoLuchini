using System;
using System.Linq;

public class Tarjeta
{
    protected decimal saldo;
    protected decimal saldoPendiente;
    private const decimal LIMITE_SALDO = 56000m;
    private const decimal LIMITE_SALDO_NEGATIVO = -1200m;
    private const decimal TARIFA_BASICA = 1580m;
    private const decimal TARIFA_INTERURBANA = 3000m;
    protected DateTime ultimoViaje;
    protected int viajesHoy;
    protected DateTime fechaUltimoDia;
    private static int contadorId = 0;
    private int id;

    // Boleto de uso frecuente (solo tarjetas normales)
    private int viajesEnMes;
    private DateTime mesActual;

    // Trasbordos
    protected DateTime ultimoViajeParaTransbordo;
    protected string ultimaLineaUsada;
    protected bool esTransbordo;
    protected decimal ultimaTarifaCobrada;

    public decimal Saldo
    {
        get { return saldo; }
    }

    public decimal SaldoPendiente
    {
        get { return saldoPendiente; }
    }

    public int Id
    {
        get { return id; }
    }

    public bool EsTransbordo
    {
        get { return esTransbordo; }
    }

    public decimal UltimaTarifaCobrada
    {
        get { return ultimaTarifaCobrada; }
    }

    public Tarjeta()
    {
        saldo = 0m;
        saldoPendiente = 0m;
        ultimoViaje = DateTime.MinValue;
        viajesHoy = 0;
        fechaUltimoDia = DateTime.MinValue;
        id = ++contadorId;
        viajesEnMes = 0;
        mesActual = DateTime.MinValue;
        ultimoViajeParaTransbordo = DateTime.MinValue;
        ultimaLineaUsada = "";
        esTransbordo = false;
        ultimaTarifaCobrada = 0m;
    }

    public virtual bool Cargar(decimal monto)
    {
        decimal[] montosAceptados = { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };

        if (!montosAceptados.Contains(monto))
            return false;

        // Si hay saldo negativo, primero se descuenta de la carga
        if (saldo < 0)
        {
            decimal deuda = Math.Abs(saldo);
            if (monto <= deuda)
            {
                saldo += monto;
                return true;
            }
            else
            {
                decimal restante = monto - deuda;
                saldo = 0m;
                
                decimal espacioDisponible = LIMITE_SALDO - saldo;
                if (espacioDisponible >= restante)
                {
                    saldo += restante;
                }
                else
                {
                    saldo = LIMITE_SALDO;
                    saldoPendiente += (restante - espacioDisponible);
                }
                return true;
            }
        }

        // Carga normal cuando no hay saldo negativo
        decimal espacio = LIMITE_SALDO - saldo;

        if (espacio >= monto)
        {
            saldo += monto;
        }
        else
        {
            saldo = LIMITE_SALDO;
            saldoPendiente += (monto - espacio);
        }

        return true;
    }

    public void AcreditarCarga()
    {
        if (saldoPendiente <= 0)
            return;

        decimal espacioDisponible = LIMITE_SALDO - saldo;
        
        if (espacioDisponible >= saldoPendiente)
        {
            saldo += saldoPendiente;
            saldoPendiente = 0m;
        }
        else
        {
            saldo = LIMITE_SALDO;
            saldoPendiente -= espacioDisponible;
        }
    }

    public virtual bool Descontar(decimal monto)
    {
        // Permitir saldo negativo hasta el límite SOLO para tarjetas normales
        if (saldo - monto < LIMITE_SALDO_NEGATIVO)
            return false;

        saldo -= monto;
        AcreditarCarga();
        return true;
    }

    public virtual bool PagarPasaje()
    {
        return PagarPasaje("", false);
    }

    public virtual bool PagarPasaje(string linea, bool esInterurbano)
    {
        ActualizarContadorViajes();
        
        // Verificar si es trasbordo
        bool esTrasbordo = VerificarTransbordo(linea);
        
        decimal tarifaAPagar;
        if (esTrasbordo)
        {
            tarifaAPagar = 0m; // Trasbordo gratuito
            esTransbordo = true;
        }
        else
        {
            if (esInterurbano)
            {
                tarifaAPagar = ObtenerTarifaInterurbana();
            }
            else
            {
                tarifaAPagar = ObtenerTarifa();
            }
            esTransbordo = false;
        }

        bool resultado = Descontar(tarifaAPagar);
        if (resultado)
        {
            ultimoViaje = DateTime.Now;
            ultimoViajeParaTransbordo = DateTime.Now;
            ultimaLineaUsada = linea;
            viajesHoy++;
            ultimaTarifaCobrada = tarifaAPagar;
            
            // Incrementar contador mensual DESPUÉS de pagar (para el próximo viaje)
            if (this.GetType() == typeof(Tarjeta) && !esTrasbordo)
            {
                ActualizarContadorMensual();
                viajesEnMes++;
            }
        }
        return resultado;
    }

    protected bool VerificarTransbordo(string lineaActual)
    {
        // Trasbordos solo de lunes a sábado de 7:00 a 22:00
        DateTime ahora = DateTime.Now;
        
        if (ahora.DayOfWeek == DayOfWeek.Sunday)
            return false;

        if (ahora.Hour < 7 || ahora.Hour >= 22)
            return false;

        // Debe haber un viaje anterior
        if (ultimoViajeParaTransbordo == DateTime.MinValue)
            return false;

        // Debe ser dentro de 1 hora
        TimeSpan tiempoTranscurrido = ahora - ultimoViajeParaTransbordo;
        if (tiempoTranscurrido.TotalMinutes > 60)
            return false;

        // Debe ser en línea diferente
        if (string.IsNullOrEmpty(lineaActual) || string.IsNullOrEmpty(ultimaLineaUsada))
            return false;

        if (lineaActual == ultimaLineaUsada)
            return false;

        return true;
    }

    public virtual decimal ObtenerTarifa()
    {
        // Boleto de uso frecuente (solo para tarjetas normales)
        if (this.GetType() == typeof(Tarjeta))
        {
            ActualizarContadorMensual();
            
            // El contador se incrementa DESPUÉS del viaje
            // Por eso, el viaje 30 tendrá viajesEnMes=29
            if (viajesEnMes >= 29 && viajesEnMes < 59)
            {
                return TARIFA_BASICA * 0.8m; // 20% descuento (viajes 30-59)
            }
            else if (viajesEnMes >= 59 && viajesEnMes < 80)
            {
                return TARIFA_BASICA * 0.75m; // 25% descuento (viajes 60-80)
            }
        }
        
        return TARIFA_BASICA;
    }

    public virtual decimal ObtenerTarifaInterurbana()
    {
        return TARIFA_INTERURBANA;
    }

    public virtual string ObtenerTipo()
    {
        return "Normal";
    }

    public virtual bool PuedePagarEnHorario()
    {
        // Tarjetas normales pueden pagar siempre
        return true;
    }

    public virtual bool PuedeUsarSaldoNegativo(decimal monto)
    {
        // Solo tarjetas normales pueden usar saldo negativo
        if (this.GetType() == typeof(Tarjeta))
        {
            return (saldo - monto >= LIMITE_SALDO_NEGATIVO);
        }
        return false;
    }

    protected void ActualizarContadorViajes()
    {
        DateTime ahora = DateTime.Now;
        if (fechaUltimoDia.Date != ahora.Date)
        {
            viajesHoy = 0;
            fechaUltimoDia = ahora;
        }
    }

    protected void ActualizarContadorMensual()
    {
        DateTime ahora = DateTime.Now;
        if (mesActual.Year != ahora.Year || mesActual.Month != ahora.Month)
        {
            viajesEnMes = 0;
            mesActual = ahora;
        }
    }

    public int ViajesEnMes
    {
        get 
        { 
            ActualizarContadorMensual();
            return viajesEnMes; 
        }
    }
}
