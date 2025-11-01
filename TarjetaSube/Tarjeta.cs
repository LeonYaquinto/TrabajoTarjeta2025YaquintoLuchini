using System;
using System.Linq;

public class Tarjeta
{
    private decimal saldo;
    private const decimal LIMITE_SALDO = 40000m;
    private const decimal TARIFA_BASICA = 1580m;
    private const decimal SALDO_NEGATIVO_PERMITIDO = -1200m;

    public decimal Saldo
    {
        get { return saldo; }
    }

    public Tarjeta()
    {
        saldo = 0m;
    }

    public virtual bool Cargar(decimal monto)
    {
        decimal[] montosAceptados = { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };

        if (!montosAceptados.Contains(monto))
            return false;

        // Si hay saldo negativo, primero se descuenta de la carga
        if (saldo < 0)
        {
            decimal saldoNegativo = Math.Abs(saldo);
            if (monto <= saldoNegativo)
            {
                saldo += monto;
                return true;
            }
            else
            {
                decimal montoRestante = monto - saldoNegativo;
                saldo = 0;
                
                if (saldo + montoRestante > LIMITE_SALDO)
                    return false;
                
                saldo += montoRestante;
                return true;
            }
        }

        if (saldo + monto > LIMITE_SALDO)
            return false;

        saldo += monto;
        return true;
    }

    public bool Descontar(decimal monto)
    {
        // Ahora se permite saldo negativo hasta el límite
        if (saldo - monto < SALDO_NEGATIVO_PERMITIDO)
            return false;

        saldo -= monto;
        return true;
    }

    public virtual bool PagarPasaje()
    {
        return Descontar(TARIFA_BASICA);
    }

    public virtual decimal ObtenerTarifa()
    {
        return TARIFA_BASICA;
    }
}