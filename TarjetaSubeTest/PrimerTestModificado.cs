using System;
using NUnit.Framework;
using TarjetaSube;

[TestFixture]
public class PrimerTestModificado
{

    
    [Test]
    public void Boleto_Crear_ConParametrosCorrectos()
    {
        var boleto = new Boleto(1580m, "132", DateTime.Now, true, "TarjetaComun", 1000m, 1);

        Assert.AreEqual(1580m, boleto.Monto);
        Assert.AreEqual("132", boleto.LineaColectivo);
        Assert.AreEqual("TarjetaComun", boleto.TipoTarjeta);
    }

    [Test]
    public void TarjetaComun_Cargar_5000_Saldo5000()
    {
        var tarjeta = new TarjetaComun();
        bool resultado = tarjeta.Cargar(5000m);

        Assert.IsTrue(resultado);
        Assert.AreEqual(5000m, tarjeta.Saldo);
    }

    [Test]
    public void Colectivo_Crear_Linea132()
    {
        var colectivo = new Colectivo("132");
        Assert.IsNotNull(colectivo);
    }

    [Test]
    public void TarjetaComun_Crear_SaldoInicialCero()
    {
        var tarjeta = new TarjetaComun();
        Assert.AreEqual(0m, tarjeta.Saldo);
    }

}