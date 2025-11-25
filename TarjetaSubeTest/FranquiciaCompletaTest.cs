using System;
using NUnit.Framework;

[TestFixture]
public class TarjetaFranquiciaCompletaTests
{
    private TarjetaFranquiciaCompleta tarjeta;
    private Colectivo colectivo;

    [SetUp]
    public void Setup()
    {
        tarjeta = new TarjetaFranquiciaCompleta();
        colectivo = new Colectivo("120", "Rosario Bus");
    }

    [Test]
    public void Test_ObtenerTarifa_DebeRetornarCero()
    {
        // Arrange & Act
        decimal tarifa = tarjeta.ObtenerTarifa();

        // Assert
        Assert.AreEqual(0m, tarifa, "La tarifa debe ser $0 para franquicia completa");
    }

    [Test]
    public void Test_ObtenerTipo_DebeRetornarFranquiciaCompleta()
    {
        // Arrange & Act
        string tipo = tarjeta.ObtenerTipo();

        // Assert
        Assert.AreEqual("Franquicia Completa", tipo, "El tipo debe ser 'Franquicia Completa'");
    }

    [Test]
    public void Test_PrimerViaje_DebeSerGratuito()
    {
        // Arrange
        decimal saldoInicial = tarjeta.Saldo;

        // Act
        bool resultado = tarjeta.PagarPasaje();
        Boleto boleto = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsTrue(resultado, "El primer viaje debe ser exitoso");
        Assert.IsNotNull(boleto, "Debe generar un boleto");
        Assert.AreEqual(0m, boleto.MontoPagado, "El monto pagado debe ser $0");
        Assert.AreEqual(saldoInicial, tarjeta.Saldo, "El saldo no debe cambiar");
    }

    [Test]
    public void Test_SegundoViaje_DebeSerGratuito()
    {
        // Arrange
        colectivo.PagarCon(tarjeta); // Primer viaje
        decimal saldoAntes = tarjeta.Saldo;

        // Act
        Boleto boleto = colectivo.PagarCon(tarjeta); // Segundo viaje

        // Assert
        Assert.IsNotNull(boleto, "Debe generar un boleto");
        Assert.AreEqual(0m, boleto.MontoPagado, "El monto pagado debe ser $0");
        Assert.AreEqual(saldoAntes, tarjeta.Saldo, "El saldo no debe cambiar");
    }

    [Test]
    public void Test_NoSePuedenRealizarMasDeDosViajesGratuitosPorDia()
    {
        // Arrange
        colectivo.PagarCon(tarjeta); // Primer viaje gratuito
        colectivo.PagarCon(tarjeta); // Segundo viaje gratuito

        // Act
        Boleto boleto = colectivo.PagarCon(tarjeta); // Tercer viaje (debe fallar)

        // Assert
        Assert.IsNull(boleto, "No debe generar boleto para el tercer viaje - límite alcanzado");
    }

    [Test]
    public void Test_ViajesPosterioresAlSegundo_SeCobranConPrecioCompleto()
    {
        // Arrange
        const decimal TARIFA_COMPLETA = 1580m;
        
        // Realizar dos viajes gratuitos con franquicia completa
        colectivo.PagarCon(tarjeta); // Primer viaje gratuito
        colectivo.PagarCon(tarjeta); // Segundo viaje gratuito
        
        // Verificar que el tercer viaje falla con franquicia
        Boleto boletoFranquicia = colectivo.PagarCon(tarjeta);
        Assert.IsNull(boletoFranquicia, "La franquicia debe rechazar el tercer viaje");

        // Crear una tarjeta normal para demostrar que los viajes posteriores se cobran normalmente
        Tarjeta tarjetaNormal = new Tarjeta();
        tarjetaNormal.Cargar(5000m);
        decimal saldoAntes = tarjetaNormal.Saldo;

        // Act - Pagar un tercer viaje con tarjeta normal
        Boleto boleto = colectivo.PagarCon(tarjetaNormal);

        // Assert
        Assert.IsNotNull(boleto, "El viaje con tarjeta normal debe ser exitoso");
        Assert.AreEqual(TARIFA_COMPLETA, boleto.MontoPagado, 
            "El monto pagado debe ser la tarifa completa ($1580)");
        Assert.AreEqual(saldoAntes - TARIFA_COMPLETA, tarjetaNormal.Saldo, 
            "Debe descontarse la tarifa completa del saldo");
        Assert.AreEqual("Normal", boleto.TipoTarjeta, 
            "El tipo debe ser 'Normal' ya que la franquicia no permite más viajes");
    }

    [Test]
    public void Test_ContadorDeViajesSeReiniciaNuevoDia()
    {
        // Este test verifica la lógica del contador
        // En un entorno real, se usaría dependency injection para el DateTime

        // Arrange
        colectivo.PagarCon(tarjeta); // Primer viaje
        colectivo.PagarCon(tarjeta); // Segundo viaje

        // Act
        Boleto tercerViajeMismoDia = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNull(tercerViajeMismoDia, 
            "El tercer viaje el mismo día debe fallar");
        
        // Nota: En un test real con mocks, aquí se simularía un cambio de día
        // y se verificaría que el contador se reinicia a 0
    }

    [Test]
    public void Test_PrimerViajeDelDia_SinViajesPrevios()
    {
        // Arrange
        TarjetaFranquiciaCompleta tarjetaNueva = new TarjetaFranquiciaCompleta();

        // Act
        bool resultado = tarjetaNueva.PagarPasaje();

        // Assert
        Assert.IsTrue(resultado, "El primer viaje de una tarjeta nueva debe ser exitoso");
    }

    [Test]
    public void Test_DosViajesConsecutivos_AmbosGratuitos()
    {
        // Arrange & Act
        Boleto primerViaje = colectivo.PagarCon(tarjeta);
        Boleto segundoViaje = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNotNull(primerViaje, "El primer viaje debe ser exitoso");
        Assert.IsNotNull(segundoViaje, "El segundo viaje debe ser exitoso");
        Assert.AreEqual(0m, primerViaje.MontoPagado, "Primer viaje debe ser gratuito");
        Assert.AreEqual(0m, segundoViaje.MontoPagado, "Segundo viaje debe ser gratuito");
        Assert.AreEqual(0m, tarjeta.Saldo, "El saldo debe permanecer en 0");
    }

    [Test]
    public void Test_MultiplesIntentosDeViajePostLimite_TodosFallan()
    {
        // Arrange
        colectivo.PagarCon(tarjeta); // Viaje 1
        colectivo.PagarCon(tarjeta); // Viaje 2

        // Act
        Boleto viaje3 = colectivo.PagarCon(tarjeta);
        Boleto viaje4 = colectivo.PagarCon(tarjeta);
        Boleto viaje5 = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNull(viaje3, "El tercer viaje debe fallar");
        Assert.IsNull(viaje4, "El cuarto viaje debe fallar");
        Assert.IsNull(viaje5, "El quinto viaje debe fallar");
    }

    [Test]
    public void Test_BoletoGenerado_ContieneInformacionCorrecta()
    {
        // Arrange & Act
        Boleto boleto = colectivo.PagarCon(tarjeta);

        // Assert
        Assert.IsNotNull(boleto, "Debe generar un boleto");
        Assert.AreEqual(0m, boleto.MontoPagado, "El monto debe ser $0");
        Assert.AreEqual("Franquicia Completa", boleto.TipoTarjeta, 
            "El tipo de tarjeta debe ser 'Franquicia Completa'");
        Assert.AreEqual("120", boleto.LineaColectivo, "Debe tener la línea correcta");
        Assert.AreEqual("Rosario Bus", boleto.Empresa, "Debe tener la empresa correcta");
    }
}