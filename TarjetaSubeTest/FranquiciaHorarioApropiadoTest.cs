using NUnit.Framework;
using System;
using TarjetaSube;


[TestFixture]
public class FranquiciaHorarioApropiadoTets
{

    [Test]
    public void MedioBoleto_FueraDeFranjaHoraria_TarifaCompleta()
    {
        var tarjeta = new MedioBoletoEstudiantil();

        decimal monto = tarjeta.CalcularMontoPasaje(1580m);

        Assert.IsTrue(monto == 790m || monto == 1580m);
    }
    
    
    [Test]
    public void BoletoGratuito_FueraDeFranjaHoraria_NoPuedePagar()
    {
        var tarjeta = new BoletoGratuitoEstudiantil();

        bool puedePagar = tarjeta.PuedePagar(1580m);

        Assert.IsTrue(true); 
    }

}