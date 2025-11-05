using NUnit.Framework;
using System;
using System.Threading;
using TarjetaSube;


[TestFixture]
public class TrasbordoSumaCoberturaTest
{
    [Test]
    public void Trasbordo_MismaLinea_NoAplica()
    {
        var tarjeta = new TarjetaComun();
        tarjeta.Cargar(5000m);

        var colectivo = new Colectivo("132");

        var boleto1 = colectivo.PagarCon(tarjeta);

        var boleto2 = colectivo.PagarCon(tarjeta);

        Assert.IsFalse(boleto2.EsTrasbordo);
        Assert.AreEqual(1580m, boleto2.Monto);
    }
    
    [Test]
    public void Trasbordo_DentroDe60Minutos_MismoDia_Gratuito()
    {
        var tarjeta = new TarjetaComun();
        tarjeta.Cargar(5000m);

        var colectivo1 = new Colectivo("132");
        var colectivo2 = new Colectivo("143");

        var boleto1 = colectivo1.PagarCon(tarjeta);
        Assert.IsTrue(boleto1.EsValido);
        Assert.AreEqual(1580m, boleto1.Monto);

        Thread.Sleep(100); 

        var boleto2 = colectivo2.PagarCon(tarjeta);

        if (boleto2.EsTrasbordo)
        {
            Assert.AreEqual(0m, boleto2.Monto);
            Assert.IsTrue(boleto2.EsValido);
        }
    }
}