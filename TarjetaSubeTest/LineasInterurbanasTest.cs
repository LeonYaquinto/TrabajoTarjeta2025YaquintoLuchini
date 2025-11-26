using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class LineasInterurbanasTest
    {
        private const decimal TARIFA_BASICA = 1580m;
        private const decimal TARIFA_INTERURBANA = 3000m;

        [Test]
        public void Test_ColectivoInterurbano_CobraTarifaInterurbana()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivoInterurbano = new Colectivo("500", "Gálvez", true);

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_INTERURBANA, boleto.TotalAbonado);
            Assert.AreEqual(5000m - TARIFA_INTERURBANA, tarjeta.Saldo);
        }

        [Test]
        public void Test_ColectivoUrbano_CobraTarifaBasica()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivoUrbano = new Colectivo("120", "Rosario Bus", false);

            Boleto boleto = colectivoUrbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_BASICA, boleto.TotalAbonado);
            Assert.AreEqual(5000m - TARIFA_BASICA, tarjeta.Saldo);
        }

        [Test]
        public void Test_ColectivoInterurbano_SinSaldoSuficiente_PermiteSaldoNegativo()
        {
            // MODIFICADO: Ahora permite saldo negativo hasta -1200
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000); // 2000 - 3000 = -1000 (dentro del límite)
            Colectivo colectivoInterurbano = new Colectivo("500", "Gálvez", true);

            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto, "Debe permitir saldo negativo hasta -1200");
            Assert.AreEqual(-1000m, tarjeta.Saldo);
        }

        [Test]
        public void Test_MedioBoleto_EnInterurbano_AplicaDescuento()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(5000);
            
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek >= DayOfWeek.Monday && ahora.DayOfWeek <= DayOfWeek.Friday &&
                ahora.Hour >= 6 && ahora.Hour < 22)
            {
                Colectivo colectivoInterurbano = new Colectivo("500", "Gálvez", true);

                Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

                Assert.IsNotNull(boleto);
                decimal tarifaEsperada = TARIFA_INTERURBANA * 0.5m; // 1500
                Assert.AreEqual(tarifaEsperada, boleto.TotalAbonado);
            }
            else
            {
                Assert.Ignore("Test solo válido en horario L-V 6-22hs");
            }
        }

        [Test]
        public void Test_FranquiciaCompleta_EnInterurbano_PrimerosViajesGratuitos()
        {
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta();
            tarjeta.Cargar(10000);
            
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek >= DayOfWeek.Monday && ahora.DayOfWeek <= DayOfWeek.Friday &&
                ahora.Hour >= 6 && ahora.Hour < 22)
            {
                Colectivo colectivoInterurbano = new Colectivo("500", "Gálvez", true);

                // Primer viaje: gratuito
                Boleto boleto1 = colectivoInterurbano.PagarCon(tarjeta);
                Assert.IsNotNull(boleto1);
                Assert.AreEqual(0m, boleto1.TotalAbonado);

                // Segundo viaje: gratuito
                Boleto boleto2 = colectivoInterurbano.PagarCon(tarjeta);
                Assert.IsNotNull(boleto2);
                Assert.AreEqual(0m, boleto2.TotalAbonado);

                // Tercer viaje: debe cobrar tarifa interurbana completa
                Boleto boleto3 = colectivoInterurbano.PagarCon(tarjeta);
                Assert.IsNotNull(boleto3);
                Assert.AreEqual(TARIFA_INTERURBANA, boleto3.TotalAbonado);
            }
            else
            {
                Assert.Ignore("Test solo válido en horario L-V 6-22hs");
            }
        }

        [Test]
        public void Test_ColectivoInterurbano_ConSaldoPendiente()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Saldo: 56000, Pendiente: 4000
            
            decimal saldoInicial = tarjeta.Saldo;
            decimal pendienteInicial = tarjeta.SaldoPendiente;
            
            Colectivo colectivoInterurbano = new Colectivo("500", "Gálvez", true);
            Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_INTERURBANA, boleto.TotalAbonado);
            
            // Verifica que se acreditó saldo pendiente
            Assert.AreEqual(56000m, tarjeta.Saldo);
            Assert.Less(tarjeta.SaldoPendiente, pendienteInicial);
        }

        [Test]
        public void Test_ColectivoInterurbano_PropiedadEsInterurbano()
        {
            Colectivo urbano = new Colectivo("120", "Rosario Bus", false);
            Colectivo interurbano = new Colectivo("500", "Gálvez", true);

            Assert.IsFalse(urbano.EsInterurbano);
            Assert.IsTrue(interurbano.EsInterurbano);
        }

        [Test]
        public void Test_MultiplesViajesInterurbanos()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            Colectivo colectivoInterurbano = new Colectivo("500", "Gálvez", true);

            // Realizar 5 viajes interurbanos
            for (int i = 0; i < 5; i++)
            {
                Boleto boleto = colectivoInterurbano.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"Viaje {i + 1} debería ser exitoso");
                Assert.AreEqual(TARIFA_INTERURBANA, boleto.TotalAbonado);
            }

            decimal saldoEsperado = 30000m - (TARIFA_INTERURBANA * 5);
            Assert.AreEqual(saldoEsperado, tarjeta.Saldo);
        }

        [Test]
        public void Test_BoletoEstudiantil_EnInterurbano()
        {
            // TarjetaBoletoGratuitoEstudiantil hereda de FranquiciaCompleta
            TarjetaBoletoGratuitoEstudiantil tarjeta = new TarjetaBoletoGratuitoEstudiantil();
            tarjeta.Cargar(10000);
            
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek >= DayOfWeek.Monday && ahora.DayOfWeek <= DayOfWeek.Friday &&
                ahora.Hour >= 6 && ahora.Hour < 22)
            {
                Colectivo colectivoInterurbano = new Colectivo("500", "Gálvez", true);

                // Primeros 2 viajes: gratuitos
                Boleto boleto1 = colectivoInterurbano.PagarCon(tarjeta);
                Boleto boleto2 = colectivoInterurbano.PagarCon(tarjeta);
                
                Assert.AreEqual(0m, boleto1.TotalAbonado);
                Assert.AreEqual(0m, boleto2.TotalAbonado);

                // Tercer viaje: cobra tarifa interurbana
                Boleto boleto3 = colectivoInterurbano.PagarCon(tarjeta);
                Assert.AreEqual(TARIFA_INTERURBANA, boleto3.TotalAbonado);
            }
            else
            {
                Assert.Ignore("Test solo válido en horario L-V 6-22hs");
            }
        }
    }
}
