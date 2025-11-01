using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class TarjetaTest
    {
        private Tarjeta tarjeta;

        [SetUp]
        public void Setup()
        {
            tarjeta = new Tarjeta();
        }

        [Test]
        public void TestSaldoInicial()
        {
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestObtenerTarifaBasica()
        {
            Assert.AreEqual(1580m, tarjeta.ObtenerTarifa());
        }

        [Test]
        public void TestCargarMontoValido2000()
        {
            bool resultado = tarjeta.Cargar(2000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(2000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoValido3000()
        {
            bool resultado = tarjeta.Cargar(3000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(3000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoValido4000()
        {
            bool resultado = tarjeta.Cargar(4000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(4000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoValido5000()
        {
            bool resultado = tarjeta.Cargar(5000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(5000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoValido8000()
        {
            bool resultado = tarjeta.Cargar(8000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(8000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoValido10000()
        {
            bool resultado = tarjeta.Cargar(10000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(10000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoValido15000()
        {
            bool resultado = tarjeta.Cargar(15000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(15000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoValido20000()
        {
            bool resultado = tarjeta.Cargar(20000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(20000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoValido25000()
        {
            bool resultado = tarjeta.Cargar(25000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(25000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoValido30000()
        {
            bool resultado = tarjeta.Cargar(30000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(30000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarTodosLosMontosValidos()
        {
            decimal[] montosValidos = { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };
            
            foreach (var monto in montosValidos)
            {
                var tarjetaTemp = new Tarjeta();
                bool resultado = tarjetaTemp.Cargar(monto);
                Assert.IsTrue(resultado, $"Debería aceptar el monto {monto}");
                Assert.AreEqual(monto, tarjetaTemp.Saldo);
            }
        }

        [Test]
        public void TestCargarMontoInvalido1000()
        {
            bool resultado = tarjeta.Cargar(1000);
            Assert.IsFalse(resultado);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoInvalido500()
        {
            bool resultado = tarjeta.Cargar(500);
            Assert.IsFalse(resultado);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoInvalido7000()
        {
            bool resultado = tarjeta.Cargar(7000);
            Assert.IsFalse(resultado);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoInvalido50000()
        {
            bool resultado = tarjeta.Cargar(50000);
            Assert.IsFalse(resultado);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarSuperandoLimite()
        {
            tarjeta.Cargar(30000);
            tarjeta.Cargar(10000); // Total: 40000
            
            bool resultado = tarjeta.Cargar(2000); // Intentar superar el límite
            Assert.IsFalse(resultado);
            Assert.AreEqual(40000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarHastaElLimiteExacto()
        {
            tarjeta.Cargar(30000);
            bool resultado = tarjeta.Cargar(10000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(40000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarVariasVeces()
        {
            tarjeta.Cargar(5000);
            Assert.AreEqual(5000m, tarjeta.Saldo);
            
            tarjeta.Cargar(8000);
            Assert.AreEqual(13000m, tarjeta.Saldo);
            
            tarjeta.Cargar(10000);
            Assert.AreEqual(23000m, tarjeta.Saldo);
        }

        [Test]
        public void TestDescontarConSaldoSuficiente()
        {
            tarjeta.Cargar(5000);
            bool resultado = tarjeta.Descontar(2000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(3000m, tarjeta.Saldo);
        }

        [Test]
        public void TestDescontarConSaldoInsuficiente()
        {
            tarjeta.Cargar(2000);
            bool resultado = tarjeta.Descontar(3000);
            Assert.IsFalse(resultado);
            Assert.AreEqual(2000m, tarjeta.Saldo);
        }

        [Test]
        public void TestDescontarSaldoCompleto()
        {
            tarjeta.Cargar(2000);
            bool resultado = tarjeta.Descontar(2000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestDescontarConSaldoCero()
        {
            bool resultado = tarjeta.Descontar(1000);
            Assert.IsFalse(resultado);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestDescontarVariasVeces()
        {
            tarjeta.Cargar(10000);
            
            tarjeta.Descontar(1000);
            Assert.AreEqual(9000m, tarjeta.Saldo);
            
            tarjeta.Descontar(2000);
            Assert.AreEqual(7000m, tarjeta.Saldo);
            
            tarjeta.Descontar(3000);
            Assert.AreEqual(4000m, tarjeta.Saldo);
        }

        [Test]
        public void TestPagarPasajeConSaldoSuficiente()
        {
            tarjeta.Cargar(3000);
            bool resultado = tarjeta.PagarPasaje();
            Assert.IsTrue(resultado);
            Assert.AreEqual(1420m, tarjeta.Saldo); // 3000 - 1580
        }

        [Test]
        public void TestPagarPasajeSinSaldoSuficiente()
        {
            tarjeta.Cargar(2000);
            tarjeta.Descontar(1000); // Queda 1000
            bool resultado = tarjeta.PagarPasaje();
            Assert.IsFalse(resultado);
            Assert.AreEqual(1000m, tarjeta.Saldo);
        }

        [Test]
        public void TestPagarPasajeConSaldoExacto()
        {
            tarjeta.Cargar(2000);
            tarjeta.Descontar(420); // Queda exactamente 1580
            bool resultado = tarjeta.PagarPasaje();
            Assert.IsTrue(resultado);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestPagarPasajeSinSaldo()
        {
            bool resultado = tarjeta.PagarPasaje();
            Assert.IsFalse(resultado);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestPagarMultiplesPasajes()
        {
            tarjeta.Cargar(10000);
            
            // Primer pasaje: 10000 - 1580 = 8420
            Assert.IsTrue(tarjeta.PagarPasaje());
            Assert.AreEqual(8420m, tarjeta.Saldo);
            
            // Segundo pasaje: 8420 - 1580 = 6840
            Assert.IsTrue(tarjeta.PagarPasaje());
            Assert.AreEqual(6840m, tarjeta.Saldo);
            
            // Tercer pasaje: 6840 - 1580 = 5260
            Assert.IsTrue(tarjeta.PagarPasaje());
            Assert.AreEqual(5260m, tarjeta.Saldo);
        }

        [Test]
        public void TestPagarPasajesHastaQuedarSinSaldo()
        {
            tarjeta.Cargar(5000);
            
            // Primer pasaje: 5000 - 1580 = 3420
            Assert.IsTrue(tarjeta.PagarPasaje());
            
            // Segundo pasaje: 3420 - 1580 = 1840
            Assert.IsTrue(tarjeta.PagarPasaje());
            
            // Tercer pasaje: 1840 - 1580 = 260
            Assert.IsTrue(tarjeta.PagarPasaje());
            
            // Cuarto pasaje: 260 < 1580, no alcanza
            Assert.IsFalse(tarjeta.PagarPasaje());
            Assert.AreEqual(260m, tarjeta.Saldo);
        }

        [Test]
        public void TestIntegracionCargarDescontarPagar()
        {
            // Cargar
            tarjeta.Cargar(5000);
            Assert.AreEqual(5000m, tarjeta.Saldo);
            
            // Descontar
            tarjeta.Descontar(1000);
            Assert.AreEqual(4000m, tarjeta.Saldo);
            
            // Pagar pasaje
            tarjeta.PagarPasaje();
            Assert.AreEqual(2420m, tarjeta.Saldo);
            
            // Cargar más
            tarjeta.Cargar(3000);
            Assert.AreEqual(5420m, tarjeta.Saldo);
            
            // Pagar otro pasaje
            tarjeta.PagarPasaje();
            Assert.AreEqual(3840m, tarjeta.Saldo);
        }

        [Test]
        public void TestPropertySaldoGetterFunciona()
        {
            Assert.AreEqual(0m, tarjeta.Saldo);
            
            tarjeta.Cargar(8000);
            Assert.AreEqual(8000m, tarjeta.Saldo);
            
            tarjeta.Descontar(3000);
            Assert.AreEqual(5000m, tarjeta.Saldo);
        }

        [Test]
        public void TestLimiteSaldoEs40000()
        {
            tarjeta.Cargar(20000);
            tarjeta.Cargar(20000);
            Assert.AreEqual(40000m, tarjeta.Saldo);
            
            // Intentar agregar más
            bool resultado = tarjeta.Cargar(2000);
            Assert.IsFalse(resultado);
            Assert.AreEqual(40000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCombinacionesDiferentesMontosValidos()
        {
            tarjeta.Cargar(2000);
            tarjeta.Cargar(3000);
            tarjeta.Cargar(5000);
            Assert.AreEqual(10000m, tarjeta.Saldo);
        }
    }
}