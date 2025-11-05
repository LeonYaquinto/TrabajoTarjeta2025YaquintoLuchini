using NUnit.Framework;
using TarjetaSube;


[TestFixture]
public class ColectivoInterurbanoTest
{


    [Test]
    public void ColectivoInterurbano_MedioBoleto_Tarifa1500()
    {
        var colectivo = new Colectivo("33", true);
        var tarjeta = new MedioBoletoEstudiantil();
        tarjeta.Cargar(5000m);

        var boleto = colectivo.PagarCon(tarjeta);

        if (boleto.EsValido)
        {
            Assert.AreEqual(1500m, boleto.Monto);
        }
    }
    
    [Test]
    public void ColectivoInterurbano_Tarifa3000()
    {
        var colectivo = new Colectivo("33", true); 

        var tarjeta = new TarjetaComun();
        tarjeta.Cargar(5000m);

        var boleto = colectivo.PagarCon(tarjeta);

        Assert.IsTrue(boleto.EsValido);
        Assert.AreEqual(3000m, boleto.Monto);
    }
}