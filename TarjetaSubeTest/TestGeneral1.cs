using NUnit.Framework;
using TarjetaSube;

[TestFixture]
public class TestGeneral1
{
    private TarjetaComun t;
    private Colectivo colectivo;

    [SetUp]
    public void Setup()
    {
        t = new TarjetaComun();
        colectivo = new Colectivo("132");
    }

    [Test]
    public void CargaTest()
    {
        t.Cargar(1000m); 

        var boleto = colectivo.PagarCon(t);

        Assert.IsNotNull(boleto);
    }

    [Test]
    public void CargaTest_SaldoInsuficiente()
    {
        t.Cargar(100m); 


        var boleto = colectivo.PagarCon(t);

        Assert.AreEqual(0m, t.Saldo);
        Assert.IsFalse(boleto.EsValido); 
    }
}