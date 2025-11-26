using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class BoletoUsoFrecuenteTest
    {
        private const decimal TARIFA_BASICA = 1580m;

        [Test]
        public void Test_PrimerosViajesDelMes_TarifaNormal()
        {
            // Viajes 1 al 29: Tarifa normal
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000); // Total: 60000 (suficiente para 29 viajes)
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");

            // Hacer 29 viajes
            for (int i = 0; i < 29; i++)
            {
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"El viaje {i + 1} debería ser exitoso");
                Assert.AreEqual(TARIFA_BASICA, boleto.TotalAbonado, $"El viaje {i + 1} debería cobrar tarifa normal");
            }

            Assert.AreEqual(29, tarjeta.ViajesEnMes);
        }

        [Test]
        public void Test_Viaje30_Aplica20PorCientoDescuento()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");

            // Hacer 29 viajes con tarifa normal
            for (int i = 0; i < 29; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 30: debería tener 20% descuento
            Boleto boleto30 = colectivo.PagarCon(tarjeta);
            decimal tarifaEsperada = TARIFA_BASICA * 0.8m; // 1264

            Assert.IsNotNull(boleto30);
            Assert.AreEqual(tarifaEsperada, boleto30.TotalAbonado, "El viaje 30 debería tener 20% de descuento");
            Assert.AreEqual(30, tarjeta.ViajesEnMes);
        }

        [Test]
        public void Test_Viajes30al59_Aplican20PorCientoDescuento()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");

            decimal tarifaDescuento20 = TARIFA_BASICA * 0.8m;

            // Hacer 29 viajes
            for (int i = 0; i < 29; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viajes 30 al 59: 20% descuento
            for (int i = 30; i <= 59; i++)
            {
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"El viaje {i} debería ser exitoso");
                Assert.AreEqual(tarifaDescuento20, boleto.TotalAbonado, $"El viaje {i} debería tener 20% de descuento");
            }

            Assert.AreEqual(59, tarjeta.ViajesEnMes);
        }

        [Test]
        public void Test_Viaje60_Aplica25PorCientoDescuento()
        {
            Tarjeta tarjeta = new Tarjeta();
            for (int i = 0; i < 10; i++)
            {
                tarjeta.Cargar(30000);
            }
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");

            // Hacer 59 viajes
            for (int i = 0; i < 59; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 60: debería tener 25% descuento
            Boleto boleto60 = colectivo.PagarCon(tarjeta);
            decimal tarifaEsperada = TARIFA_BASICA * 0.75m; // 1185

            Assert.IsNotNull(boleto60);
            Assert.AreEqual(tarifaEsperada, boleto60.TotalAbonado, "El viaje 60 debería tener 25% de descuento");
            Assert.AreEqual(60, tarjeta.ViajesEnMes);
        }

        [Test]
        public void Test_Viajes60al79_Aplican25PorCientoDescuento()
        {
            Tarjeta tarjeta = new Tarjeta();
            for (int i = 0; i < 15; i++)
            {
                tarjeta.Cargar(30000);
            }
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");

            decimal tarifaDescuento25 = TARIFA_BASICA * 0.75m;

            // Hacer 59 viajes
            for (int i = 0; i < 59; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viajes 60 al 79: 25% descuento
            for (int i = 60; i <= 79; i++)
            {
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"El viaje {i} debería ser exitoso");
                Assert.AreEqual(tarifaDescuento25, boleto.TotalAbonado, $"El viaje {i} debería tener 25% de descuento");
            }

            Assert.AreEqual(79, tarjeta.ViajesEnMes);
        }

        [Test]
        public void Test_Viaje80_Aplica25PorCientoDescuento()
        {
            Tarjeta tarjeta = new Tarjeta();
            for (int i = 0; i < 15; i++)
            {
                tarjeta.Cargar(30000);
            }
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");

            // Hacer 79 viajes
            for (int i = 0; i < 79; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 80: último con 25% descuento
            Boleto boleto80 = colectivo.PagarCon(tarjeta);
            decimal tarifaEsperada = TARIFA_BASICA * 0.75m;

            Assert.IsNotNull(boleto80);
            Assert.AreEqual(tarifaEsperada, boleto80.TotalAbonado, "El viaje 80 debería tener 25% de descuento");
            Assert.AreEqual(80, tarjeta.ViajesEnMes);
        }

        [Test]
        public void Test_Viaje81_VuelveTarifaNormal()
        {
            Tarjeta tarjeta = new Tarjeta();
            for (int i = 0; i < 20; i++)
            {
                tarjeta.Cargar(30000);
            }
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");

            // Hacer 80 viajes
            for (int i = 0; i < 80; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viaje 81: vuelve a tarifa normal
            Boleto boleto81 = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto81);
            Assert.AreEqual(TARIFA_BASICA, boleto81.TotalAbonado, "El viaje 81 debería volver a tarifa normal");
            Assert.AreEqual(81, tarjeta.ViajesEnMes);
        }

        [Test]
        public void Test_Viaje81EnAdelante_TarifaNormal()
        {
            Tarjeta tarjeta = new Tarjeta();
            for (int i = 0; i < 25; i++)
            {
                tarjeta.Cargar(30000);
            }
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");

            // Hacer 80 viajes
            for (int i = 0; i < 80; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Viajes 81 al 90: tarifa normal
            for (int i = 81; i <= 90; i++)
            {
                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, $"El viaje {i} debería ser exitoso");
                Assert.AreEqual(TARIFA_BASICA, boleto.TotalAbonado, $"El viaje {i} debería tener tarifa normal");
            }

            Assert.AreEqual(90, tarjeta.ViajesEnMes);
        }

        [Test]
        public void Test_MedioBoletoNoAplicaBoletoUsoFrecuente()
        {
            // El boleto de uso frecuente SOLO aplica a tarjetas normales
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");

            decimal tarifaMedioBoleto = TARIFA_BASICA * 0.5m;

            // Hacer viajes - todos deberían ser medio boleto (primeros 2 del día)
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(tarifaMedioBoleto, boleto1.TotalAbonado);
        }

        [Test]
        public void Test_FranquiciaNoAplicaBoletoUsoFrecuente()
        {
            // El boleto de uso frecuente SOLO aplica a tarjetas normales
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta();
            tarjeta.Cargar(30000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");

            // Hacer viajes - primeros 2 deberían ser gratuitos
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(0m, boleto1.TotalAbonado);
        }
    }
}
