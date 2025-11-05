using System;
using TarjetaSube;

public class Colectivo
{
    private const decimal TARIFA_BASICA = 1580m;
    private const decimal TARIFA_INTERURBANA = 3000m;
    private string linea;
    private bool esInterurbano;

    public Colectivo(string linea, bool esInterurbano = false)
    {
        this.linea = linea;
        this.esInterurbano = esInterurbano;
    }

    public decimal ObtenerTarifa()
    {
        return esInterurbano ? TARIFA_INTERURBANA : TARIFA_BASICA;
    }

    public Boleto PagarCon(Tarjeta tarjeta)
    {
        decimal tarifa = ObtenerTarifa();
        bool esTrasbordo = SistemaTrasbordos.PuedeRealizarTrasbordo(tarjeta.Id, this.linea);
        decimal montoPasaje = esTrasbordo ? 0m : tarjeta.CalcularMontoPasaje(tarifa);

        decimal saldoAnterior = tarjeta.Saldo;

        if (tarjeta.PuedePagar(tarifa) || esTrasbordo)
        {
            bool descuentoExitoso = tarjeta.Descontar(montoPasaje);

            if (descuentoExitoso || esTrasbordo)
            {
                decimal montoRecarga = 0;
                decimal totalAbonado = montoPasaje;

                if (!esTrasbordo && saldoAnterior < 0)
                {
                    montoRecarga = Math.Min(Math.Abs(saldoAnterior), montoPasaje);
                    totalAbonado = montoPasaje + montoRecarga;
                }

                Boleto? boletoOrigen = null;
                if (esTrasbordo)
                {
                    boletoOrigen = SistemaTrasbordos.ObtenerBoletoOrigenTrasbordo(tarjeta.Id, this.linea);
                }

                var boleto = new Boleto(
                    montoPasaje,
                    linea,
                    DateTime.Now,
                    true,
                    tarjeta.GetType().Name,
                    tarjeta.Saldo,
                    tarjeta.Id,
                    totalAbonado,
                    montoRecarga,
                    esTrasbordo,
                    boletoOrigen?.IdTarjeta
                );

                SistemaTrasbordos.RegistrarBoleto(boleto);
                return boleto;
            }
        }

        var boletoInvalido = new Boleto(
            montoPasaje,
            linea,
            DateTime.Now,
            false,
            tarjeta.GetType().Name,
            tarjeta.Saldo,
            tarjeta.Id
        );

        SistemaTrasbordos.RegistrarBoleto(boletoInvalido);
        return boletoInvalido;
    }
}