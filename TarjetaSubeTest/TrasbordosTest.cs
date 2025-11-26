using NUnit.Framework;
using System;
using System.Threading;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class TrasbordosTest
    {
        [Test]
        public void Test_PrimerViaje_NoEsTransbordo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.IsFalse(boleto.EsTransbordo);
            Assert.Greater(boleto.TotalAbonado, 0);
        }

        [Test]
        public void Test_Trasbordo_EntreLineasDiferentes_Dentro1Hora_Gratuito()
        {
            DateTime ahora = DateTime.Now;
            
            // Solo ejecutar si estamos en horario de trasbordo (L-S 7-22)
            if (ahora.DayOfWeek != DayOfWeek.Sunday && ahora.Hour >= 7 && ahora.Hour < 22)
            {
                Tarjeta tarjeta = new Tarjeta();
                tarjeta.Cargar(5000);
                
                Colectivo colectivo1 = new Colectivo("120", "Rosario Bus");
                Colectivo colectivo2 = new Colectivo("115", "Rosario Bus");

                // Primer viaje
                Boleto boleto1 = colectivo1.PagarCon(tarjeta);
                Assert.IsNotNull(boleto1);
                Assert.IsFalse(boleto1.EsTransbordo);
                decimal saldoDespuesPrimerViaje = tarjeta.Saldo;

                // Segundo viaje en otra línea (trasbordo)
                Boleto boleto2 = colectivo2.PagarCon(tarjeta);
                Assert.IsNotNull(boleto2);
                Assert.IsTrue(boleto2.EsTransbordo, "Debería ser trasbordo");
                Assert.AreEqual(0m, boleto2.TotalAbonado, "Trasbordo debería ser gratuito");
                Assert.AreEqual(saldoDespuesPrimerViaje, tarjeta.Saldo, "El saldo no debería cambiar");
            }
            else
            {
                Assert.Ignore("Test solo válido en horario de trasbordo (L-S 7-22hs)");
            }
        }

        [Test]
        public void Test_Trasbordo_MismaLinea_NoEsGratuito()
        {
            DateTime ahora = DateTime.Now;
            
            if (ahora.DayOfWeek != DayOfWeek.Sunday && ahora.Hour >= 7 && ahora.Hour < 22)
            {
                Tarjeta tarjeta = new Tarjeta();
                tarjeta.Cargar(10000);
                
                Colectivo colectivo = new Colectivo("120", "Rosario Bus");

                // Primer viaje
                Boleto boleto1 = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto1);
                decimal saldoDespuesPrimerViaje = tarjeta.Saldo;

                // Segundo viaje en la misma línea (NO es trasbordo)
                Boleto boleto2 = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto2);
                Assert.IsFalse(boleto2.EsTransbordo, "No debería ser trasbordo en la misma línea");
                Assert.Greater(boleto2.TotalAbonado, 0, "Debería cobrar tarifa normal");
                Assert.Less(tarjeta.Saldo, saldoDespuesPrimerViaje, "El saldo debería disminuir");
            }
            else
            {
                Assert.Ignore("Test solo válido en horario de trasbordo (L-S 7-22hs)");
            }
        }

        [Test]
        public void Test_Trasbordo_Domingo_NoPermitido()
        {
            DateTime ahora = DateTime.Now;
            
            if (ahora.DayOfWeek == DayOfWeek.Sunday)
            {
                Tarjeta tarjeta = new Tarjeta();
                tarjeta.Cargar(10000);
                
                Colectivo colectivo1 = new Colectivo("120", "Rosario Bus");
                Colectivo colectivo2 = new Colectivo("115", "Rosario Bus");

                // Primer viaje
                Boleto boleto1 = colectivo1.PagarCon(tarjeta);
                Assert.IsNotNull(boleto1);
                decimal saldoDespuesPrimerViaje = tarjeta.Saldo;

                // Segundo viaje - NO debería ser trasbordo los domingos
                Boleto boleto2 = colectivo2.PagarCon(tarjeta);
                Assert.IsNotNull(boleto2);
                Assert.IsFalse(boleto2.EsTransbordo, "No debería haber trasbordo los domingos");
                Assert.Greater(boleto2.TotalAbonado, 0);
                Assert.Less(tarjeta.Saldo, saldoDespuesPrimerViaje);
            }
            else
            {
                Assert.Ignore("Test solo válido los domingos");
            }
        }

        [Test]
        public void Test_Trasbordo_FueraDeHorario_NoPermitido()
        {
            DateTime ahora = DateTime.Now;
            
            // Solo si estamos fuera del horario 7-22
            if (ahora.DayOfWeek != DayOfWeek.Sunday && (ahora.Hour < 7 || ahora.Hour >= 22))
            {
                Tarjeta tarjeta = new Tarjeta();
                tarjeta.Cargar(10000);
                
                Colectivo colectivo1 = new Colectivo("120", "Rosario Bus");
                Colectivo colectivo2 = new Colectivo("115", "Rosario Bus");

                // Primer viaje
                Boleto boleto1 = colectivo1.PagarCon(tarjeta);
                Assert.IsNotNull(boleto1);
                decimal saldoDespuesPrimerViaje = tarjeta.Saldo;

                // Segundo viaje fuera de horario - NO debería ser trasbordo
                Boleto boleto2 = colectivo2.PagarCon(tarjeta);
                Assert.IsNotNull(boleto2);
                Assert.IsFalse(boleto2.EsTransbordo, "No debería haber trasbordo fuera del horario 7-22");
                Assert.Greater(boleto2.TotalAbonado, 0);
                Assert.Less(tarjeta.Saldo, saldoDespuesPrimerViaje);
            }
            else
            {
                Assert.Ignore("Test solo válido fuera del horario 7-22hs");
            }
        }

        [Test]
        public void Test_MultiplesTrasbordos_Dentro1Hora_Gratuitos()
        {
            DateTime ahora = DateTime.Now;
            
            if (ahora.DayOfWeek != DayOfWeek.Sunday && ahora.Hour >= 7 && ahora.Hour < 22)
            {
                Tarjeta tarjeta = new Tarjeta();
                tarjeta.Cargar(10000);
                
                Colectivo colectivo1 = new Colectivo("120", "Rosario Bus");
                Colectivo colectivo2 = new Colectivo("115", "Rosario Bus");
                Colectivo colectivo3 = new Colectivo("102", "Rosario Bus");

                // Primer viaje
                Boleto boleto1 = colectivo1.PagarCon(tarjeta);
                decimal saldoDespuesPrimerViaje = tarjeta.Saldo;

                // Segundo viaje (trasbordo)
                Boleto boleto2 = colectivo2.PagarCon(tarjeta);
                Assert.IsTrue(boleto2.EsTransbordo);
                Assert.AreEqual(0m, boleto2.TotalAbonado);

                // Tercer viaje (otro trasbordo)
                Boleto boleto3 = colectivo3.PagarCon(tarjeta);
                Assert.IsTrue(boleto3.EsTransbordo);
                Assert.AreEqual(0m, boleto3.TotalAbonado);

                Assert.AreEqual(saldoDespuesPrimerViaje, tarjeta.Saldo, "Solo debería haber descontado el primer viaje");
            }
            else
            {
                Assert.Ignore("Test solo válido en horario de trasbordo (L-S 7-22hs)");
            }
        }

        [Test]
        public void Test_MedioBoleto_PuedeHacerTrasbordo()
        {
            DateTime ahora = DateTime.Now;
            
            if (ahora.DayOfWeek >= DayOfWeek.Monday && ahora.DayOfWeek <= DayOfWeek.Friday &&
                ahora.Hour >= 7 && ahora.Hour < 22)
            {
                TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
                tarjeta.Cargar(10000);
                
                Colectivo colectivo1 = new Colectivo("120", "Rosario Bus");
                Colectivo colectivo2 = new Colectivo("115", "Rosario Bus");

                // Primer viaje con medio boleto
                Boleto boleto1 = colectivo1.PagarCon(tarjeta);
                Assert.IsNotNull(boleto1);
                decimal saldoDespuesPrimerViaje = tarjeta.Saldo;

                // Segundo viaje (trasbordo gratuito)
                Boleto boleto2 = colectivo2.PagarCon(tarjeta);
                Assert.IsNotNull(boleto2);
                Assert.IsTrue(boleto2.EsTransbordo);
                Assert.AreEqual(0m, boleto2.TotalAbonado);
                Assert.AreEqual(saldoDespuesPrimerViaje, tarjeta.Saldo);
            }
            else
            {
                Assert.Ignore("Test solo válido en horario L-V 7-22hs");
            }
        }

        [Test]
        public void Test_FranquiciaCompleta_PuedeHacerTrasbordo()
        {
            DateTime ahora = DateTime.Now;
            
            if (ahora.DayOfWeek >= DayOfWeek.Monday && ahora.DayOfWeek <= DayOfWeek.Friday &&
                ahora.Hour >= 7 && ahora.Hour < 22)
            {
                TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta();
                tarjeta.Cargar(10000);
                
                Colectivo colectivo1 = new Colectivo("120", "Rosario Bus");
                Colectivo colectivo2 = new Colectivo("115", "Rosario Bus");

                // Primer viaje gratuito (franquicia)
                Boleto boleto1 = colectivo1.PagarCon(tarjeta);
                Assert.IsNotNull(boleto1);
                Assert.AreEqual(0m, boleto1.TotalAbonado);
                decimal saldoInicial = tarjeta.Saldo;

                // Segundo viaje (trasbordo)
                Boleto boleto2 = colectivo2.PagarCon(tarjeta);
                Assert.IsNotNull(boleto2);
                Assert.IsTrue(boleto2.EsTransbordo);
                Assert.AreEqual(0m, boleto2.TotalAbonado);
                Assert.AreEqual(saldoInicial, tarjeta.Saldo);
            }
            else
            {
                Assert.Ignore("Test solo válido en horario L-V 7-22hs");
            }
        }

        [Test]
        public void Test_Trasbordo_EnInterurbano_Gratuito()
        {
            DateTime ahora = DateTime.Now;
            
            if (ahora.DayOfWeek != DayOfWeek.Sunday && ahora.Hour >= 7 && ahora.Hour < 22)
            {
                Tarjeta tarjeta = new Tarjeta();
                tarjeta.Cargar(10000);
                
                Colectivo colectivoUrbano = new Colectivo("120", "Rosario Bus", false);
                Colectivo colectivoInterurbano = new Colectivo("500", "Gálvez", true);

                // Primer viaje urbano
                Boleto boleto1 = colectivoUrbano.PagarCon(tarjeta);
                decimal saldoDespuesPrimerViaje = tarjeta.Saldo;

                // Segundo viaje interurbano (trasbordo)
                Boleto boleto2 = colectivoInterurbano.PagarCon(tarjeta);
                Assert.IsNotNull(boleto2);
                Assert.IsTrue(boleto2.EsTransbordo);
                Assert.AreEqual(0m, boleto2.TotalAbonado, "Trasbordo debería ser gratuito incluso en interurbano");
                Assert.AreEqual(saldoDespuesPrimerViaje, tarjeta.Saldo);
            }
            else
            {
                Assert.Ignore("Test solo válido en horario de trasbordo (L-S 7-22hs)");
            }
        }

        [Test]
        public void Test_BoletoMuestraTransbordo()
        {
            DateTime ahora = DateTime.Now;
            
            if (ahora.DayOfWeek != DayOfWeek.Sunday && ahora.Hour >= 7 && ahora.Hour < 22)
            {
                Tarjeta tarjeta = new Tarjeta();
                tarjeta.Cargar(10000);
                
                Colectivo colectivo1 = new Colectivo("120", "Rosario Bus");
                Colectivo colectivo2 = new Colectivo("115", "Rosario Bus");

                Boleto boleto1 = colectivo1.PagarCon(tarjeta);
                Boleto boleto2 = colectivo2.PagarCon(tarjeta);

                string textoBoleto2 = boleto2.ToString();
                Assert.IsTrue(textoBoleto2.Contains("TRASBORDO"), "El boleto debería indicar que es trasbordo");
            }
            else
            {
                Assert.Ignore("Test solo válido en horario de trasbordo (L-S 7-22hs)");
            }
        }
    }
}
