using NUnit.Framework;
using System;

namespace TarjetaTests
{
    [TestFixture]
    public class SaldoNegativoTests
    {
        private const decimal TARIFA_BASICA = 1580m;
        private const decimal LIMITE_SALDO_NEGATIVO = -1200m;

        [Test]
        public void Test_PuedeQuedarConSaldoNegativoHasta1200()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            
            // Act - Gastar todo y dejar saldo en -1200 exacto
            tarjeta.Descontar(3200m); // 2000 - 3200 = -1200

            // Assert
            Assert.AreEqual(-1200m, tarjeta.Saldo, "Debe permitir saldo negativo hasta -1200");
        }

        [Test]
        public void Test_NoPermiteSaldoMenorAMenos1200()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            
            // Act - Intentar gastar más del límite negativo
            bool resultado = tarjeta.Descontar(3201m); // 2000 - 3201 = -1201 (no permitido)

            // Assert
            Assert.IsFalse(resultado, "No debe permitir saldo menor a -1200");
            Assert.AreEqual(2000m, tarjeta.Saldo, "El saldo no debe cambiar");
        }

        [Test]
        public void Test_ViajePlusConSaldoCero()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();
            // No cargar nada, saldo = 0

            // Act
            bool resultado = tarjeta.PagarPasaje(); // 0 - 1580 = -1580, pero solo permite -1200

            // Assert
            Assert.IsFalse(resultado, "No debe permitir un viaje que deje el saldo por debajo de -1200");
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void Test_ViajePlusConSaldoInsuficientePeroPermitido()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1500m); // Saldo: 500

            // Act
            bool resultado = tarjeta.PagarPasaje(); // 500 - 1580 = -1080 (permitido)

            // Assert
            Assert.IsTrue(resultado, "Debe permitir el viaje que deja saldo en -1080");
            Assert.AreEqual(-1080m, tarjeta.Saldo);
        }

        [Test]
        public void Test_AlCargarConSaldoNegativo_DescuentaLaDeuda()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(2500m); // Saldo: -500

            // Act
            tarjeta.Cargar(3000m); // Debe pagar los 500 de deuda y quedar con 2500

            // Assert
            Assert.AreEqual(2500m, tarjeta.Saldo, "Debe descontar la deuda y dejar el resto en saldo");
        }

        [Test]
        public void Test_AlCargarConSaldoNegativo_CargaNoCubreLaDeuda()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(3000m); // Saldo: -1000

            // Act
            tarjeta.Cargar(2000m); // Solo cubre 2000 de los 1000 de deuda

            // Assert
            Assert.AreEqual(1000m, tarjeta.Saldo, "Debe quedar con el saldo positivo después de pagar parte de la deuda");
        }

        [Test]
        public void Test_AlCargarConSaldoNegativo_CargaCubreExactoLaDeuda()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(3200m); // Saldo: -1200

            // Act
            tarjeta.Cargar(2000m); // Cubre exactamente 1200 de deuda y quedan 800

            // Assert
            Assert.AreEqual(800m, tarjeta.Saldo, "Debe quedar con el saldo positivo exacto");
        }

        [Test]
        public void Test_MultiplesViajesPlus()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            
            // Act - Primer viaje normal
            tarjeta.PagarPasaje(); // 2000 - 1580 = 420
            Assert.AreEqual(420m, tarjeta.Saldo);

            // Segundo viaje con saldo negativo
            bool resultado = tarjeta.PagarPasaje(); // 420 - 1580 = -1160 (permitido)
            
            // Assert
            Assert.IsTrue(resultado, "Debe permitir segundo viaje plus");
            Assert.AreEqual(-1160m, tarjeta.Saldo);
        }

        [Test]
        public void Test_NoPermiteDosViajesPlus()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            tarjeta.PagarPasaje(); // 420 restante
            tarjeta.PagarPasaje(); // -1160

            // Act - Intentar un tercer viaje plus
            bool resultado = tarjeta.PagarPasaje(); // -1160 - 1580 = -2740 (no permitido)

            // Assert
            Assert.IsFalse(resultado, "No debe permitir que el saldo baje de -1200");
            Assert.AreEqual(-1160m, tarjeta.Saldo);
        }

        [Test]
        public void Test_RecargaDespuesDeViajePlus_RestauraraSaldo()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            tarjeta.PagarPasaje(); // 420
            tarjeta.PagarPasaje(); // -1160

            // Act
            tarjeta.Cargar(5000m); // Paga 1160 de deuda, quedan 3840

            // Assert
            Assert.AreEqual(3840m, tarjeta.Saldo);
            Assert.AreEqual(0m, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Test_DescontarDejandoSaldoExactoEn1200Negativo()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();

            // Act
            bool resultado = tarjeta.Descontar(1200m); // 0 - 1200 = -1200 (límite exacto)

            // Assert
            Assert.IsTrue(resultado, "Debe permitir llegar exactamente a -1200");
            Assert.AreEqual(-1200m, tarjeta.Saldo);
        }

        [Test]
        public void Test_DescontarDejandoSaldoMenorA1200Negativo()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();

            // Act
            bool resultado = tarjeta.Descontar(1200.01m); // 0 - 1200.01 = -1200.01 (no permitido)

            // Assert
            Assert.IsFalse(resultado, "No debe permitir superar el límite de -1200");
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void Test_CargaConSaldoNegativoYExcedente()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(2500m); // -500

            // Act - Cargar más del límite
            tarjeta.Cargar(30000m); // Paga 500 deuda, quedan 29500, pero límite es 56000
            tarjeta.Cargar(30000m); // Esto genera pendiente

            // Assert
            Assert.AreEqual(56000m, tarjeta.Saldo);
            Assert.AreEqual(3500m, tarjeta.SaldoPendiente); // 29500 + 30000 = 59500, excede por 3500
        }

        [Test]
        public void Test_ViajesPlusDesdeDistintosSaldosIniciales()
        {
            // Test 1: Desde 100
            Tarjeta tarjeta1 = new Tarjeta();
            tarjeta1.Cargar(2000m);
            tarjeta1.Descontar(1900m); // Saldo: 100
            bool resultado1 = tarjeta1.PagarPasaje(); // 100 - 1580 = -1480, no permitido
            Assert.IsFalse(resultado1);

            // Test 2: Desde 381
            Tarjeta tarjeta2 = new Tarjeta();
            tarjeta2.Cargar(2000m);
            tarjeta2.Descontar(1619m); // Saldo: 381
            bool resultado2 = tarjeta2.PagarPasaje(); // 381 - 1580 = -1199, permitido
            Assert.IsTrue(resultado2);
            Assert.AreEqual(-1199m, tarjeta2.Saldo);
        }

        [Test]
        public void Test_IntegracionCompleta_SaldoNegativoYRecarga()
        {
            // Arrange
            Tarjeta tarjeta = new Tarjeta();
            
            // Cargar inicialmente
            tarjeta.Cargar(5000m);
            Assert.AreEqual(5000m, tarjeta.Saldo);

            // Varios viajes normales
            tarjeta.PagarPasaje(); // 3420
            tarjeta.PagarPasaje(); // 1840
            Assert.AreEqual(1840m, tarjeta.Saldo);

            // Viaje que lleva a negativo
            bool resultado = tarjeta.PagarPasaje(); // 1840 - 1580 = 260
            Assert.IsTrue(resultado);
            Assert.AreEqual(260m, tarjeta.Saldo);

            // Otro viaje a negativo
            resultado = tarjeta.PagarPasaje(); // 260 - 1580 = -1320, pero límite es -1200
            Assert.IsFalse(resultado, "No debe permitir superar el límite negativo");
            Assert.AreEqual(260m, tarjeta.Saldo);

            // Descontar exacto para llegar a -1200
            tarjeta.Descontar(1460m); // 260 - 1460 = -1200
            Assert.AreEqual(-1200m, tarjeta.Saldo);

            // Recargar y verificar que se descuenta la deuda
            tarjeta.Cargar(10000m); // Paga 1200 deuda, quedan 8800
            Assert.AreEqual(8800m, tarjeta.Saldo);
            Assert.AreEqual(0m, tarjeta.SaldoPendiente);
        }
    }
}
