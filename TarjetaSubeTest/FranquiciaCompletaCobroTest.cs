using System;
using NUnit.Framework;

[TestFixture]
public class TarjetaFranquiciaCompletaCobroTest
{
    private TarjetaFranquiciaCompleta tarjeta;
    private Colectivo colectivo;
    private const decimal TARIFA_COMPLETA = 1580m;

    [SetUp]
    public void Setup()
    {
        tarjeta = new TarjetaFranquiciaCompleta();
        colectivo = new Colectivo("120", "Rosario Bus");
    }

    [Test]
    public void Test_TercerViajeDelDia_CobraTarifaCompleta()
    {
        // Arrange
        tarjeta.Cargar(5000m); // Cargar saldo para el tercer viaje

        // Realizar dos viajes gratuitos
        colectivo.PagarCon(tarjeta); // Viaje 1 - gratuito
        colectivo.PagarCon(tarjeta); // Viaje 2 - gratuito

        decimal saldoAntesTercerViaje = tarjeta.Saldo;

        // Act - Tercer viaje
        Boleto boleto3 = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNotNull(boleto3, "Debe generar boleto para el tercer viaje");
        Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado, 
            "El tercer viaje debe cobrar tarifa completa ($1580)");
        Assert.AreEqual(saldoAntesTercerViaje - TARIFA_COMPLETA, tarjeta.Saldo,
            "Debe descontarse la tarifa completa del saldo");
    }

    [Test]
    public void Test_TercerViajeConSaldoInsuficiente_NoGeneraBoleto()
    {
        // Arrange
        tarjeta.Cargar(2000m);
        tarjeta.Descontar(500m); // Dejar con 1500 (menos que la tarifa completa)

        // Realizar dos viajes gratuitos
        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);

        // Act - Tercer viaje sin saldo suficiente
        Boleto boleto3 = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNull(boleto3, "No debe generar boleto si no tiene saldo suficiente");
        Assert.AreEqual(1500m, tarjeta.Saldo, "El saldo no debe cambiar");
    }

    [Test]
    public void Test_CuartoYQuintoViaje_TambienCobranTarifaCompleta()
    {
        // Arrange
        tarjeta.Cargar(10000m);

        // Dos viajes gratuitos
        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);

        decimal saldoDespuesDeGratuitos = tarjeta.Saldo;

        // Act - Viajes 3, 4 y 5
        Boleto boleto3 = colectivo.PagarCon(tarjeta);
        Boleto boleto4 = colectivo.PagarCon(tarjeta);
        Boleto boleto5 = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado);
        Assert.AreEqual(TARIFA_COMPLETA, boleto4.MontoPagado);
        Assert.AreEqual(TARIFA_COMPLETA, boleto5.MontoPagado);
        Assert.AreEqual(saldoDespuesDeGratuitos - (3 * TARIFA_COMPLETA), tarjeta.Saldo);
    }

    [Test]
    public void Test_ObtenerTarifa_DevuelveCorrectamenteSegunViajes()
    {
        // Arrange
        tarjeta.Cargar(5000m);

        // Assert - Antes de viajes
        Assert.AreEqual(0m, tarjeta.ObtenerTarifa(), "Tarifa debe ser 0 antes de viajes");

        // Primer viaje
        tarjeta.PagarPasaje();
        Assert.AreEqual(0m, tarjeta.ObtenerTarifa(), "Tarifa debe ser 0 después del primer viaje");

        // Segundo viaje
        tarjeta.PagarPasaje();
        Assert.AreEqual(TARIFA_COMPLETA, tarjeta.ObtenerTarifa(), 
            "Tarifa debe ser completa después del segundo viaje");
    }

    [Test]
    public void Test_TercerViajeConSaldoExacto()
    {
        // Arrange
        tarjeta.Cargar(2000m);
        tarjeta.Descontar(420m); // Dejar exactamente 1580

        // Dos viajes gratuitos
        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);

        // Act - Tercer viaje con saldo exacto
        Boleto boleto3 = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNotNull(boleto3);
        Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado);
        Assert.AreEqual(0m, tarjeta.Saldo, "El saldo debe quedar en 0");
    }

    [Test]
    public void Test_SecuenciaCompletaConSaldo()
    {
        // Arrange
        tarjeta.Cargar(10000m);
        decimal saldoInicial = 10000m;

        // Act & Assert
        // Viaje 1 - gratuito
        Boleto b1 = colectivo.PagarCon(tarjeta);
        Assert.AreEqual(0m, b1.MontoPagado);
        Assert.AreEqual(saldoInicial, tarjeta.Saldo);

        // Viaje 2 - gratuito
        Boleto b2 = colectivo.PagarCon(tarjeta);
        Assert.AreEqual(0m, b2.MontoPagado);
        Assert.AreEqual(saldoInicial, tarjeta.Saldo);

        // Viaje 3 - tarifa completa
        Boleto b3 = colectivo.PagarCon(tarjeta);
        Assert.AreEqual(TARIFA_COMPLETA, b3.MontoPagado);
        Assert.AreEqual(saldoInicial - TARIFA_COMPLETA, tarjeta.Saldo);

        // Viaje 4 - tarifa completa
        Boleto b4 = colectivo.PagarCon(tarjeta);
        Assert.AreEqual(TARIFA_COMPLETA, b4.MontoPagado);
        Assert.AreEqual(saldoInicial - (2 * TARIFA_COMPLETA), tarjeta.Saldo);
    }

    [Test]
    public void Test_BoletoDelTercerViaje_ContieneInformacionCorrecta()
    {
        // Arrange
        tarjeta.Cargar(5000m);
        int idTarjeta = tarjeta.Id;

        // Dos viajes gratuitos
        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);

        // Act - Tercer viaje
        Boleto boleto = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNotNull(boleto);
        Assert.AreEqual(TARIFA_COMPLETA, boleto.MontoPagado);
        Assert.AreEqual("120", boleto.LineaColectivo);
        Assert.AreEqual("Rosario Bus", boleto.Empresa);
        Assert.AreEqual(5000m - TARIFA_COMPLETA, boleto.SaldoRestante);
        Assert.AreEqual("Franquicia Completa", boleto.TipoTarjeta);
        Assert.AreEqual(idTarjeta, boleto.IdTarjeta);
    }

    [Test]
    public void Test_TercerViajeSinSaldo_NoModificaSaldo()
    {
        // Arrange - Sin cargar saldo
        
        // Dos viajes gratuitos
        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);

        // Act - Tercer viaje sin saldo
        Boleto boleto3 = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNull(boleto3);
        Assert.AreEqual(0m, tarjeta.Saldo);
    }

    [Test]
    public void Test_VariosViajesConTarifaCompleta_DescuentanCorrectamente()
    {
        // Arrange
        tarjeta.Cargar(20000m);

        // Realizar 10 viajes (2 gratuitos + 8 pagos)
        for (int i = 1; i <= 10; i++)
        {
            Boleto boleto = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto, $"El viaje {i} debe generar boleto");

            if (i <= 2)
            {
                Assert.AreEqual(0m, boleto.MontoPagado, $"Viaje {i} debe ser gratuito");
            }
            else
            {
                Assert.AreEqual(TARIFA_COMPLETA, boleto.MontoPagado, 
                    $"Viaje {i} debe cobrar tarifa completa");
            }
        }

        // Assert - Verificar saldo final
        decimal saldoEsperado = 20000m - (8 * TARIFA_COMPLETA); // 8 viajes pagos
        Assert.AreEqual(saldoEsperado, tarjeta.Saldo);
    }

    [Test]
    public void Test_TercerViajeConSaldoPendiente()
    {
        // Arrange
        tarjeta.Cargar(30000m);
        tarjeta.Cargar(30000m); // Genera saldo pendiente

        Assert.AreEqual(56000m, tarjeta.Saldo);
        Assert.AreEqual(4000m, tarjeta.SaldoPendiente);

        // Dos viajes gratuitos
        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);

        // Act - Tercer viaje (debe descontar y acreditar pendiente)
        Boleto boleto3 = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNotNull(boleto3);
        Assert.AreEqual(TARIFA_COMPLETA, boleto3.MontoPagado);
        Assert.AreEqual(56000m, tarjeta.Saldo); // Se acreditó el pendiente
        Assert.AreEqual(2420m, tarjeta.SaldoPendiente); // Quedó pendiente: 4000 - 1580
    }

    [Test]
    public void Test_IntegracionCompleta_GratuitosYPagos()
    {
        // Arrange
        tarjeta.Cargar(15000m);
        decimal saldoInicial = tarjeta.Saldo;

        // Act - Realizar 7 viajes
        Boleto[] boletos = new Boleto[7];
        for (int i = 0; i < 7; i++)
        {
            boletos[i] = colectivo.PagarCon(tarjeta);
        }

        // Assert
        // Primeros 2 gratuitos
        Assert.AreEqual(0m, boletos[0].MontoPagado);
        Assert.AreEqual(0m, boletos[1].MontoPagado);

        // Siguientes 5 con tarifa completa
        for (int i = 2; i < 7; i++)
        {
            Assert.AreEqual(TARIFA_COMPLETA, boletos[i].MontoPagado, 
                $"Viaje {i + 1} debe cobrar tarifa completa");
        }

        // Saldo final
        decimal saldoEsperado = saldoInicial - (5 * TARIFA_COMPLETA);
        Assert.AreEqual(saldoEsperado, tarjeta.Saldo);
    }
}
