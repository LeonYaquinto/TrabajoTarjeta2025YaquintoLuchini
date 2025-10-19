using System;
using System.Linq;
public class Tarjeta
{
    private decimal saldo;
    private const decimal LIMITE_SALDO = 40000m;
    private const decimal TARIFA_BASICA = 1580m;

    public decimal Saldo
    {
        get { return saldo; }
    }

    public Tarjeta()
    {
        saldo = 0m;
    }

    public bool Cargar(decimal monto)
    {
        decimal[] montosAceptados = { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };

        if (!montosAceptados.Contains(monto))
            return false;

        if (saldo + monto > LIMITE_SALDO)
            return false;

        saldo += monto;
        return true;
    }

    public bool Descontar(decimal monto)
    {
        if (saldo < monto)
            return false;

        saldo -= monto;
        return true;
    }

    public bool PagarPasaje()
    {
        return Descontar(TARIFA_BASICA);
    }
}
