using NUnit.Framework;
using System;
using TarjetaSube;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class BoletoGratuitoTestLimites
    {

        [Test]
        public void BoletoGratuito_TercerViajeDelDia_TarifaCompleta()
        {
            DateTime now = new DateTime(2025, 1, 1, 10, 0, 0);
            var tarjetaGratuita = new BoletoGratuitoEstudiantil(() => now);
            tarjetaGratuita.Cargar(5000m);
            var colectivo = new Colectivo("132");

            var boleto1 = colectivo.PagarCon(tarjetaGratuita);
            Assert.IsTrue(boleto1.EsValido);
            Assert.AreEqual(0m, boleto1.Monto);

            now = now.AddSeconds(6);

            var boleto2 = colectivo.PagarCon(tarjetaGratuita);
            Assert.IsTrue(boleto2.EsValido);
            Assert.AreEqual(0m, boleto2.Monto);

            now = now.AddSeconds(6);

            var boleto3 = colectivo.PagarCon(tarjetaGratuita);
            Assert.IsTrue(boleto3.EsValido);
            Assert.AreEqual(1580m, boleto3.Monto, "Tercer viaje debe ser tarifa completa");
            Assert.AreEqual(5000m - 1580m, tarjetaGratuita.Saldo);
        }
        



        
        [Test]
        public void BoletoGratuito_PermiteDosViajesGratuitosPorDia()
        {
            DateTime now = new DateTime(2025, 1, 1, 10, 0, 0);
            var tarjetaGratuita = new BoletoGratuitoEstudiantil(() => now);
            var colectivo = new Colectivo("132");

            var boleto1 = colectivo.PagarCon(tarjetaGratuita);
            Assert.IsTrue(boleto1.EsValido);
            Assert.AreEqual(0m, boleto1.Monto);

            now = now.AddSeconds(6);

            var boleto2 = colectivo.PagarCon(tarjetaGratuita);
            Assert.IsTrue(boleto2.EsValido);
            Assert.AreEqual(0m, boleto2.Monto);
        }


    }
}