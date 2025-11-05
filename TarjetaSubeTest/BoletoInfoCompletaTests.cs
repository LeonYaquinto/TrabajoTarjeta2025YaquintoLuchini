using System;
using NUnit.Framework;
using TarjetaSube;

[TestFixture]
public class BoletoInfoCompletaTests
{
    [Test]
    public void ObtenerInformacionCompleta_NoDeberiaIncluirNingunaCondicionOpcional()
    {
        var fecha = new DateTime(2025, 11, 3, 13, 0, 0);
        var boleto = new Boleto(
            monto: 60m,
            lineaColectivo: "115",
            fechaHora: fecha,
            esValido: false,
            tipoTarjeta: "Estudiantil",
            saldoRestante: 80m,
            idTarjeta: 5
        );

        var resultado = boleto.ObtenerInformacionCompleta();

        StringAssert.DoesNotContain("TRASBORDO", resultado);
        StringAssert.DoesNotContain("Total Abonado", resultado);
        StringAssert.DoesNotContain("Recarga Saldo Negativo", resultado);
    }

    [Test]
    public void Boleto_ObtenerInformacionCompleta_FormatoCorrecto()
    {
        var tarjeta = new TarjetaComun();
        tarjeta.Cargar(5000m);
        var colectivo = new Colectivo("132");

        var boleto = colectivo.PagarCon(tarjeta);
        string info = boleto.ObtenerInformacionCompleta();

        Assert.IsTrue(info.Contains("Línea: 132"));
        Assert.IsTrue(info.Contains("Tipo Tarjeta: TarjetaComun"));
        Assert.IsTrue(info.Contains("Monto Viaje: $1580"));
    }


    [Test]
    public void ToString_DeberiaIncluirTrasbordo_CuandoEsTrasbordoTrue()
    {
        var fecha = new DateTime(2025, 11, 3, 7, 0, 0);
        var boleto = new Boleto(
            monto: 80.00m,
            lineaColectivo: "153",
            fechaHora: fecha,
            esValido: false,
            tipoTarjeta: "Estudiantil",
            saldoRestante: 100.00m,
            idTarjeta: 6789,
            esTrasbordo: true
        );

        var resultado = boleto.ToString();

        StringAssert.Contains("TRASBORDO", resultado);
        StringAssert.Contains("Boleto - Línea: 153", resultado);
        StringAssert.Contains("Monto: $80", resultado);
        StringAssert.Contains("Fecha: 03/11/2025 07:00", resultado);
    }

    [Test]
    public void ObtenerInformacionCompleta_DeberiaIncluirTrasbordoSinIdOrigen()
    {
        var fecha = new DateTime(2025, 11, 3, 10, 0, 0);
        var boleto = new Boleto(
        monto: 100m,
        lineaColectivo: "120",
        fechaHora: fecha,
        esValido: true,
        tipoTarjeta: "SUBE",
        saldoRestante: 200m,
        idTarjeta: 1,
        esTrasbordo: true
        );

        var resultado = boleto.ObtenerInformacionCompleta();

        StringAssert.Contains("Tipo: TRASBORDO", resultado);
        StringAssert.DoesNotContain("ID Boleto Origen", resultado);
    }

    [Test]
    public void ObtenerInformacionCompleta_DeberiaIncluirTrasbordoConIdOrigen()
    {
        var fecha = new DateTime(2025, 11, 3, 10, 0, 0);
        var boleto = new Boleto(
            monto: 90m,
            lineaColectivo: "143",
            fechaHora: fecha,
            esValido: true,
            tipoTarjeta: "SUBE",
            saldoRestante: 150m,
            idTarjeta: 2,
            esTrasbordo: true,
            idBoletoOrigenTrasbordo: 999
        );

        var resultado = boleto.ObtenerInformacionCompleta();

        StringAssert.Contains("Tipo: TRASBORDO", resultado);
        StringAssert.Contains("ID Boleto Origen: 999", resultado);
    }



    [Test]
    public void Boleto_ContieneInformacionCompleta()
    {
        var tarjeta = new TarjetaComun();
        tarjeta.Cargar(5000m);
        var colectivo = new Colectivo("132");

        var boleto = colectivo.PagarCon(tarjeta);

        Assert.AreEqual(1580m, boleto.Monto);
        Assert.AreEqual("132", boleto.LineaColectivo);
        Assert.AreEqual("TarjetaComun", boleto.TipoTarjeta);
        Assert.AreEqual(5000m - 1580m, boleto.SaldoRestante);
        Assert.IsTrue(boleto.IdTarjeta > 0);
        Assert.IsTrue(boleto.EsValido);
    }

    [Test]
    public void ObtenerInformacionCompleta_DeberiaIncluirMontoRecargaSaldoNegativo()
    {
        var fecha = new DateTime(2025, 11, 3, 12, 0, 0);
        var boleto = new Boleto(
            monto: 70m,
            lineaColectivo: "153",
            fechaHora: fecha,
            esValido: true,
            tipoTarjeta: "SUBE",
            saldoRestante: 120m,
            idTarjeta: 4,
            montoRecarga: 25m
        );

        var resultado = boleto.ObtenerInformacionCompleta();

        StringAssert.Contains("Recarga Saldo Negativo: $25", resultado);
    }

    [Test]
    public void ObtenerInformacionCompleta_DeberiaIncluirTotalAbonado()
    {
        var fecha = new DateTime(2025, 11, 3, 11, 0, 0);
        var boleto = new Boleto(
            monto: 80m,
            lineaColectivo: "101",
            fechaHora: fecha,
            esValido: true,
            tipoTarjeta: "Común",
            saldoRestante: 300m,
            idTarjeta: 3,
            totalAbonado: 50m
        );

        var resultado = boleto.ObtenerInformacionCompleta();

        StringAssert.Contains("Total Abonado: $50", resultado);
    }


    
    [Test]
    public void ToString_DeberiaRetornarCadenaEsperada_CuandoNoEsTrasbordo()
    {
        var fecha = new DateTime(2025, 11, 3, 14, 30, 0);
        var boleto = new Boleto(
        monto: 120.50m,
        lineaColectivo: "143",
        fechaHora: fecha,
        esValido: true,
        tipoTarjeta: "SUBE",
        saldoRestante: 250.75m,
        idTarjeta: 12345,
        esTrasbordo: false
        );

        var resultado = boleto.ToString();

        StringAssert.Contains("Boleto - Línea: 143", resultado);
        StringAssert.IsMatch(@"Monto: \$120[.,]50", resultado);
        StringAssert.Contains("Fecha: 03/11/2025 14:30", resultado);
        StringAssert.Contains("Válido: True", resultado);
        StringAssert.Contains("Tipo: SUBE", resultado);
        StringAssert.IsMatch(@"Saldo: \$250[.,]75", resultado);
        StringAssert.Contains("ID: 12345", resultado);
        StringAssert.DoesNotContain("TRASBORDO", resultado);
    }

}
