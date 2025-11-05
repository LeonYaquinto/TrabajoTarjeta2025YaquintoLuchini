using NUnit.Framework;
using System;
using System.Threading;
using TarjetaSube;

[TestFixture]
public class MedioBoletoLimitesTest
{


    
    [Test]
    public void MedioBoleto_VerificacionRapida()
    {
        var medioBoleto = new MedioBoletoEstudiantil();
        medioBoleto.Cargar(5000m);

        Assert.AreEqual(790m, medioBoleto.CalcularMontoPasaje(1580m));

        Assert.IsTrue(medioBoleto.PuedePagar(1580m));
    }

    [Test]
    public void MedioBoleto_NoPermiteMasDeDosViajesPorDia()
    {
        DateTime now = new DateTime(2025, 1, 1, 10, 0, 0);
        var medioBoleto = new MedioBoletoEstudiantil(() => now);
        medioBoleto.Cargar(10000m);
        var colectivo = new Colectivo("132");

        var boleto1 = colectivo.PagarCon(medioBoleto);
        Assert.IsTrue(boleto1.EsValido);
        Assert.AreEqual(790m, boleto1.Monto);

        now = now.AddSeconds(6);

        var boleto2 = colectivo.PagarCon(medioBoleto);
        Assert.IsTrue(boleto2.EsValido);
        Assert.AreEqual(790m, boleto2.Monto);

        now = now.AddSeconds(6);

        var boleto3 = colectivo.PagarCon(medioBoleto);
        Assert.IsFalse(boleto3.EsValido, "Tercer viaje debe ser bloqueado - límite de 2 viajes por día");
    }

    [Test]
    public void MedioBoleto_ComportamientoCorrecto()
    {
        DateTime now = new DateTime(2025, 1, 1, 10, 0, 0);
        var medioBoleto = new MedioBoletoEstudiantil(() => now);
        medioBoleto.Cargar(5000m);
        var colectivo = new Colectivo("132");

        var boleto1 = colectivo.PagarCon(medioBoleto);
        Assert.IsTrue(boleto1.EsValido);
        Assert.AreEqual(790m, boleto1.Monto);
        Assert.AreEqual(4210m, medioBoleto.Saldo);

        var boleto2Inmediato = colectivo.PagarCon(medioBoleto);
        Assert.IsFalse(boleto2Inmediato.EsValido, "Segundo viaje inmediato debe fallar");
        Assert.AreEqual(4210m, medioBoleto.Saldo, "Saldo no debe cambiar");

        now = now.AddSeconds(6);

        var boleto2Espera = colectivo.PagarCon(medioBoleto);
        Assert.IsTrue(boleto2Espera.EsValido, "Segundo viaje después de espera debe ser válido");
        Assert.AreEqual(790m, boleto2Espera.Monto, "Segundo viaje debe ser medio boleto");
        Assert.AreEqual(3420m, medioBoleto.Saldo);

        now = now.AddSeconds(6);

        var boleto3 = colectivo.PagarCon(medioBoleto);
        Assert.IsFalse(boleto3.EsValido, "Tercer viaje debe ser bloqueado - límite de 2 viajes por día");
    }


    [Test]
    public void MedioBoleto_NoPermiteDosViajesEnMenosDe5Segundos()
    {
        DateTime now = new DateTime(2025, 1, 1, 10, 0, 0);
        var medioBoleto = new MedioBoletoEstudiantil(() => now);
        medioBoleto.Cargar(5000m);
        var colectivo = new Colectivo("132");

        Console.WriteLine("=== PRIMER VIAJE ===");
        var boleto1 = colectivo.PagarCon(medioBoleto);
        medioBoleto.DebugInfo();

        Console.WriteLine("=== SEGUNDO VIAJE INMEDIATO ===");
        var boleto2 = colectivo.PagarCon(medioBoleto);
        medioBoleto.DebugInfo();

        Console.WriteLine($"Boleto1 válido: {boleto1.EsValido}");
        Console.WriteLine($"Boleto2 válido: {boleto2.EsValido}");

        Assert.IsTrue(boleto1.EsValido);
        Assert.IsFalse(boleto2.EsValido, "No debe permitir segundo viaje en menos de 5 segundos");
    }

}