using System;
using NUnit.Framework;

[TestFixture]
public class TrasbordosTest
{
    [SetUp]
    public void Setup()
    {
        var field = typeof(SistemaTrasbordos).GetField("boletosPorTarjeta",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var dic = (System.Collections.IDictionary)field.GetValue(null);
        dic.Clear();
    }

    [Test]
    public void ObtenerBoletoOrigenTrasbordo_DeberiaRetornarNull_CuandoTarjetaNoExiste()
    {
        var resultado = SistemaTrasbordos.ObtenerBoletoOrigenTrasbordo(999, "132");


        Assert.IsNull(resultado);
    }


    
    [Test]
    public void ObtenerBoletoOrigenTrasbordo_DeberiaIgnorarBoletosInvalidosOTrasbordo()
    {

        var ahora = DateTime.Now;
        var boletoValido = new Boleto(
            monto: 120m,
            lineaColectivo: "120",
            fechaHora: ahora.AddMinutes(-10),
            esValido: true,
            tipoTarjeta: "SUBE",
            saldoRestante: 100m,
            idTarjeta: 3,
            esTrasbordo: false
        );

        var boletoInvalido = new Boleto(
            monto: 150m,
            lineaColectivo: "130",
            fechaHora: ahora.AddMinutes(-5),
            esValido: false, 
            tipoTarjeta: "SUBE",
            saldoRestante: 80m,
            idTarjeta: 3,
            esTrasbordo: false
        );

        var boletoTrasbordo = new Boleto(
            monto: 150m,
            lineaColectivo: "131",
            fechaHora: ahora.AddMinutes(-2),
            esValido: true,
            tipoTarjeta: "SUBE",
            saldoRestante: 80m,
            idTarjeta: 3,
            esTrasbordo: true 
        );

        SistemaTrasbordos.RegistrarBoleto(boletoValido);
        SistemaTrasbordos.RegistrarBoleto(boletoInvalido);
        SistemaTrasbordos.RegistrarBoleto(boletoTrasbordo);


        var resultado = SistemaTrasbordos.ObtenerBoletoOrigenTrasbordo(3, "150");

        Assert.IsNotNull(resultado);
        Assert.AreEqual("120", resultado.LineaColectivo, "Debe retornar solo boletos válidos y no trasbordo");
    }

    [Test]
    public void ObtenerBoletoOrigenTrasbordo_DeberiaRetornarBoletoMasReciente_ValidoYDistintaLinea()
    {

        var ahora = DateTime.Now;
        var boleto1 = new Boleto(
            monto: 120m,
            lineaColectivo: "120",
            fechaHora: ahora.AddMinutes(-50),
            esValido: true,
            tipoTarjeta: "SUBE",
            saldoRestante: 100m,
            idTarjeta: 2
        );

        var boleto2 = new Boleto(
            monto: 150m,
            lineaColectivo: "133",
            fechaHora: ahora.AddMinutes(-20),
            esValido: true,
            tipoTarjeta: "SUBE",
            saldoRestante: 80m,
            idTarjeta: 2
        );

        SistemaTrasbordos.RegistrarBoleto(boleto1);
        SistemaTrasbordos.RegistrarBoleto(boleto2);


        var resultado = SistemaTrasbordos.ObtenerBoletoOrigenTrasbordo(2, "140");


        Assert.IsNotNull(resultado);
        Assert.AreEqual("133", resultado.LineaColectivo, "Debe retornar el boleto más reciente y válido de línea distinta");
    }


    [Test]
    public void ObtenerBoletoOrigenTrasbordo_DeberiaRetornarNull_CuandoNoCumpleCondiciones()
    {
        var ahora = DateTime.Now;
        var boletoViejo = new Boleto(
            monto: 120m,
            lineaColectivo: "132",
            fechaHora: ahora.AddHours(-2),
            esValido: true,
            tipoTarjeta: "SUBE",
            saldoRestante: 100m,
            idTarjeta: 1
        );

        var boletoMismaLinea = new Boleto(
            monto: 130m,
            lineaColectivo: "132",
            fechaHora: ahora.AddMinutes(-30),
            esValido: true,
            tipoTarjeta: "SUBE",
            saldoRestante: 200m,
            idTarjeta: 1
        );

        SistemaTrasbordos.RegistrarBoleto(boletoViejo);
        SistemaTrasbordos.RegistrarBoleto(boletoMismaLinea);


        var resultado = SistemaTrasbordos.ObtenerBoletoOrigenTrasbordo(1, "132");

        Assert.IsNull(resultado, "No debería haber boleto válido para trasbordo");
    }

   

}
