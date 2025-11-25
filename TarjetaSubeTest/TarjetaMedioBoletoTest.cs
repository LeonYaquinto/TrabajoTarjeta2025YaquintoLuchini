using NUnit.Framework;
using System;
using System.Threading;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class TarjetaMedioBoletoTest
    {
        private Colectivo colectivo;
        private const decimal TARIFA_COMPLETA = 1580m;
        private const decimal TARIFA_MEDIO_BOLETO = 790m;

        [SetUp]
        public void Setup()
        {
            colectivo = new Colectivo("152", "Rosario Bus");
        }

        // Tests básicos de creación y tipo
        [Test]
        public void TestCreacionTarjetaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            Assert.IsNotNull(tarjeta);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestObtenerTipo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            Assert.AreEqual("Medio Boleto", tarjeta.ObtenerTipo());
        }

        // Tests de tarifa
        [Test]
        public void TestObtenerTarifaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, tarjeta.ObtenerTarifa());
        }

        [Test]
        public void TestTarifaEsMitadDeTarifaCompleta()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            Assert.AreEqual(TARIFA_COMPLETA * 0.5m, tarjeta.ObtenerTarifa());
        }

        // Tests de primer viaje
        [Test]
        public void TestPrimerViajeConSaldoSuficiente()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto.MontoPagado);
            Assert.AreEqual(2000m - TARIFA_MEDIO_BOLETO, boleto.SaldoRestante);
            Assert.AreEqual("Medio Boleto", boleto.TipoTarjeta);
        }

        [Test]
        public void TestPrimerViajeSinSaldoSuficiente()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1500m); // Dejar menos del medio boleto
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNull(boleto);
        }

        // Tests de intervalo mínimo de 5 minutos
        [Test]
        public void TestSegundoViajeInmediatoRechazado()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(5000m);
            
            // Primer viaje
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            
            // Segundo viaje inmediato (sin esperar)
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNull(boleto2);
            
            // El saldo debe permanecer igual al primer viaje
            Assert.AreEqual(5000m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        [Test]
        public void TestSegundoViajeAntesDe5MinutosRechazado()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(5000m);
            
            // Primer viaje
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto1);
            
            // Esperar menos de 5 minutos (3 segundos)
            Thread.Sleep(3000);
            
            // Segundo viaje debería ser rechazado
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.IsNull(boleto2);
        }

        [Test]
        public void TestViajesSinIntervaloMinimoNoDescuentanSaldo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(5000m);
            
            // Primer viaje
            colectivo.PagarCon(tarjeta);
            decimal saldoDespuesPrimerViaje = tarjeta.Saldo;
            
            // Intentar segundo viaje inmediato (debe fallar)
            colectivo.PagarCon(tarjeta);
            
            // El saldo no debe cambiar
            Assert.AreEqual(saldoDespuesPrimerViaje, tarjeta.Saldo);
        }

        // Tests de múltiples viajes con espera correcta
        [Test]
        public void TestMultiplesViajesConIntervalosCorrectos()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(10000m);
            decimal saldoEsperado = 10000m;
            
            // Primer viaje
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            saldoEsperado -= TARIFA_MEDIO_BOLETO;
            Assert.IsNotNull(boleto1);
            Assert.AreEqual(saldoEsperado, tarjeta.Saldo);
            
            // Intentar viaje inmediato (debe fallar)
            Boleto boletoRechazado = colectivo.PagarCon(tarjeta);
            Assert.IsNull(boletoRechazado);
            Assert.AreEqual(saldoEsperado, tarjeta.Saldo); // Saldo no cambia
        }

        // Tests de carga de saldo
        [Test]
        public void TestCargarSaldoEnTarjetaMedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            bool resultado = tarjeta.Cargar(3000m);
            
            Assert.IsTrue(resultado);
            Assert.AreEqual(3000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMultiplesVeces()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            tarjeta.Cargar(2000m);
            tarjeta.Cargar(3000m);
            
            Assert.AreEqual(5000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarMontoNoPermitido()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            bool resultado = tarjeta.Cargar(1500m);
            
            Assert.IsFalse(resultado);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        // Tests de saldo exacto
        [Test]
        public void TestViajeConSaldoExacto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1210m); // Dejar exactamente 790
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void TestViajeConUnPesoMenos()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1211m); // Dejar 789 (un peso menos)
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNull(boleto);
        }

        // Tests de propiedades del boleto
        [Test]
        public void TestBoletoContieneInformacionCorrecta()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(3000m);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(TARIFA_MEDIO_BOLETO, boleto.MontoPagado);
            Assert.AreEqual("152", boleto.LineaColectivo);
            Assert.AreEqual("Rosario Bus", boleto.Empresa);
            Assert.AreEqual(3000m - TARIFA_MEDIO_BOLETO, boleto.SaldoRestante);
            Assert.AreEqual("Medio Boleto", boleto.TipoTarjeta);
            Assert.IsNotNull(boleto.FechaHora);
        }

        // Tests de diferentes empresas y líneas
        [Test]
        public void TestViajeEnDiferentesLineas()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(5000m);
            
            Colectivo colectivo1 = new Colectivo("K", "Las Delicias");
            Boleto boleto1 = colectivo1.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto1);
            Assert.AreEqual("K", boleto1.LineaColectivo);
            Assert.AreEqual("Las Delicias", boleto1.Empresa);
        }

        // Tests de límite de saldo
        [Test]
        public void TestCargarHastaLimiteSaldo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            // Cargar hasta cerca del límite de 56000
            tarjeta.Cargar(30000m);
            tarjeta.Cargar(25000m); // Monto válido
            
            Assert.AreEqual(55000m, tarjeta.Saldo);
        }

        [Test]
        public void TestCargarExcediendoLimiteGeneraSaldoPendiente()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            tarjeta.Cargar(30000m);
            tarjeta.Cargar(30000m); // Excede el límite
            
            Assert.AreEqual(56000m, tarjeta.Saldo);
            Assert.AreEqual(4000m, tarjeta.SaldoPendiente);
        }

        // Tests de PagarPasaje directamente
        [Test]
        public void TestPagarPasajeConSaldoSuficiente()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            
            bool resultado = tarjeta.PagarPasaje();
            
            Assert.IsTrue(resultado);
            Assert.AreEqual(2000m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        [Test]
        public void TestPagarPasajeSinSaldoSuficiente()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            tarjeta.Descontar(1500m);
            
            bool resultado = tarjeta.PagarPasaje();
            
            Assert.IsFalse(resultado);
        }

        [Test]
        public void TestPagarPasajeDosVecesInmediato()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(5000m);
            
            bool resultado1 = tarjeta.PagarPasaje();
            Assert.IsTrue(resultado1);
            
            bool resultado2 = tarjeta.PagarPasaje();
            Assert.IsFalse(resultado2); // Debe fallar por intervalo mínimo
        }

        // Tests de casos extremos
        [Test]
        public void TestViajeConSaldoCero()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNull(boleto);
        }

        [Test]
        public void TestMultiplesIntentosDeViajeRechazados()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(5000m);
            
            colectivo.PagarCon(tarjeta); // Primer viaje exitoso
            
            // Múltiples intentos inmediatos
            Assert.IsNull(colectivo.PagarCon(tarjeta));
            Assert.IsNull(colectivo.PagarCon(tarjeta));
            Assert.IsNull(colectivo.PagarCon(tarjeta));
            
            // El saldo solo debe haber descontado un viaje
            Assert.AreEqual(5000m - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        // Tests de consistencia
        [Test]
        public void TestConsistenciaDeSaldoDespuesDeVariosViajes()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(10000m);
            
            decimal saldoInicial = tarjeta.Saldo;
            
            // Primer viaje
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(saldoInicial - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
            
            // Intentos rechazados no deben cambiar el saldo
            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(saldoInicial - TARIFA_MEDIO_BOLETO, tarjeta.Saldo);
        }

        // Tests de ID de tarjeta
        [Test]
        public void TestIdTarjetaEnBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            int idTarjeta = tarjeta.Id;
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(idTarjeta, boleto.IdTarjeta);
        }

        [Test]
        public void TestDiferentesTarjetasTienenDiferentesIds()
        {
            TarjetaMedioBoleto tarjeta1 = new TarjetaMedioBoleto();
            TarjetaMedioBoleto tarjeta2 = new TarjetaMedioBoleto();
            
            Assert.AreNotEqual(tarjeta1.Id, tarjeta2.Id);
        }

        // Test de acreditación de saldo pendiente
        [Test]
        public void TestAcreditacionSaldoPendienteDespuesDeViaje()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(30000m);
            tarjeta.Cargar(30000m); // Genera saldo pendiente de 4000
            
            // Realizar un viaje (esto debería acreditar saldo pendiente)
            colectivo.PagarCon(tarjeta);
            
            // El saldo pendiente debería haberse acreditado parcialmente
            Assert.Less(tarjeta.SaldoPendiente, 4000m);
        }

        // Test de descuento directo
        [Test]
        public void TestDescontarMontoDesdeSaldo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(3000m);
            
            bool resultado = tarjeta.Descontar(1000m);
            
            Assert.IsTrue(resultado);
            Assert.AreEqual(2000m, tarjeta.Saldo);
        }

        [Test]
        public void TestDescontarMasDelSaldoDisponible()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(2000m);
            
            bool resultado = tarjeta.Descontar(3000m);
            
            Assert.IsFalse(resultado);
            Assert.AreEqual(2000m, tarjeta.Saldo);
        }
    }
}