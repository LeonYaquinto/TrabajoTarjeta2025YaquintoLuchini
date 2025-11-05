using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TarjetaSube.Tests
{
    [TestFixture]
    public class CoberturaCompletaTests
    {
        #region Tests para Tarjeta (Clase Base)

        [Test]
        public void Tarjeta_Constructor_DeberiaInicializarValoresCorrectos()
        {
            // Arrange & Act
            var tarjeta = new TarjetaDummy();

            // Assert
            Assert.AreEqual(0m, tarjeta.Saldo);
            Assert.AreEqual(0m, tarjeta.SaldoPendienteAcreditacion);
            Assert.Greater(tarjeta.Id, 0);
        }

        [Test]
        public void Tarjeta_Cargar_MontoAceptado_DeberiaActualizarSaldo()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();

            // Act
            bool resultado = tarjeta.Cargar(2000m);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(2000m, tarjeta.Saldo);
        }

        [Test]
        public void Tarjeta_Cargar_MontoNoAceptado_DeberiaRetornarFalse()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();

            // Act
            bool resultado = tarjeta.Cargar(100m);

            // Assert
            Assert.IsFalse(resultado);
            Assert.AreEqual(0m, tarjeta.Saldo);
        }

        [Test]
        public void Tarjeta_Cargar_ConDeuda_PagaDeudaCompleta()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.SetSaldo(-500m);

            // Act
            bool resultado = tarjeta.Cargar(2000m);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(1500m, tarjeta.Saldo); // 2000 - 500 = 1500
        }

        [Test]
        public void Tarjeta_Cargar_ConDeuda_PagaDeudaParcial()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.SetSaldo(-1000m);

            // Act
            bool resultado = tarjeta.Cargar(500m);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(-500m, tarjeta.Saldo); // -1000 + 500 = -500
        }

        [Test]
        public void Tarjeta_Cargar_ExcedeLimite_DeberiaGuardarExcedente()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.SetSaldo(55000m);

            // Act
            bool resultado = tarjeta.Cargar(2000m);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(56000m, tarjeta.Saldo); // Límite máximo
            Assert.AreEqual(1000m, tarjeta.SaldoPendienteAcreditacion); // 57000 - 56000 = 1000
        }

        [Test]
        public void Tarjeta_Descontar_SaldoSuficiente_DeberiaActualizarSaldo()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.SetSaldo(2000m);

            // Act
            bool resultado = tarjeta.Descontar(1000m);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(1000m, tarjeta.Saldo);
        }

        [Test]
        public void Tarjeta_Descontar_SaldoInsuficiente_DeberiaRetornarFalse()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.SetSaldo(100m);

            // Act
            bool resultado = tarjeta.Descontar(1500m); // Excede límite de -1200

            // Assert
            Assert.IsFalse(resultado);
            Assert.AreEqual(100m, tarjeta.Saldo); // Saldo no cambia
        }

        [Test]
        public void Tarjeta_Descontar_DentroLimiteNegativo_DeberiaPermitir()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.SetSaldo(100m);

            // Act
            bool resultado = tarjeta.Descontar(1100m); // -1000 dentro del límite

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(-1000m, tarjeta.Saldo);
        }

        [Test]
        public void Tarjeta_AcreditarCarga_EspacioSuficiente_DeberiaAcreditarTodo()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.SetSaldo(10000m);
            tarjeta.SetSaldoPendiente(5000m);

            // Act
            tarjeta.AcreditarCarga();

            // Assert
            Assert.AreEqual(15000m, tarjeta.Saldo);
            Assert.AreEqual(0m, tarjeta.SaldoPendienteAcreditacion);
        }

        [Test]
        public void Tarjeta_AcreditarCarga_EspacioInsuficiente_DeberiaAcreditarParcial()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.SetSaldo(55000m);
            tarjeta.SetSaldoPendiente(2000m);

            // Act
            tarjeta.AcreditarCarga();

            // Assert
            Assert.AreEqual(56000m, tarjeta.Saldo); // Límite máximo
            Assert.AreEqual(1000m, tarjeta.SaldoPendienteAcreditacion); // 2000 - 1000 = 1000
        }

        [Test]
        public void Tarjeta_CantidadViajesHoy_SinViajes_DeberiaRetornarCero()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();

            // Act
            int resultado = tarjeta.CantidadViajesHoy();

            // Assert
            Assert.AreEqual(0, resultado);
        }

        [Test]
        public void Tarjeta_CantidadViajesHoy_ConViajes_DeberiaContarCorrectamente()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();
            tarjeta.RegistrarViajeManual(DateTime.Now);
            tarjeta.RegistrarViajeManual(DateTime.Now.AddHours(-1));

            // Act
            int resultado = tarjeta.CantidadViajesHoy();

            // Assert
            Assert.AreEqual(2, resultado);
        }

        [Test]
        public void Tarjeta_CantidadViajesEsteMes_DeberiaContarCorrectamente()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();
            tarjeta.RegistrarViajeManual(DateTime.Now);
            tarjeta.RegistrarViajeManual(DateTime.Now.AddDays(-1));

            // Act
            int resultado = tarjeta.CantidadViajesEsteMes();

            // Assert
            Assert.AreEqual(2, resultado);
        }

        [Test]
        public void Tarjeta_PuedeViajarMedioBoleto_SinViajes_DeberiaRetornarTrue()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();

            // Act
            bool resultado = tarjeta.PuedeViajarMedioBoleto();

            // Assert
            Assert.IsTrue(resultado);
        }

        [Test]
        public void Tarjeta_PuedeViajarMedioBoleto_ViajeReciente_DeberiaRetornarFalse()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();
            tarjeta.RegistrarViajeManual(DateTime.Now.AddSeconds(-2));

            // Act
            bool resultado = tarjeta.PuedeViajarMedioBoleto();

            // Assert
            Assert.IsFalse(resultado);
        }

        [Test]
        public void Tarjeta_PuedeViajarMedioBoleto_DosViajesHoy_DeberiaRetornarFalse()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();
            tarjeta.RegistrarViajeManual(DateTime.Now);
            tarjeta.RegistrarViajeManual(DateTime.Now.AddMinutes(-10));

            // Act
            bool resultado = tarjeta.PuedeViajarMedioBoleto();

            // Assert
            Assert.IsFalse(resultado);
        }

        [Test]
        public void Tarjeta_PuedeViajarGratuito_MenosDeDosViajes_DeberiaRetornarTrue()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();
            tarjeta.RegistrarViajeManual(DateTime.Now);

            // Act
            bool resultado = tarjeta.PuedeViajarGratuito();

            // Assert
            Assert.IsTrue(resultado);
        }

        [Test]
        public void Tarjeta_PuedeViajarGratuito_DosViajesHoy_DeberiaRetornarFalse()
        {
            // Arrange
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();
            tarjeta.RegistrarViajeManual(DateTime.Now);
            tarjeta.RegistrarViajeManual(DateTime.Now.AddMinutes(-5));

            // Act
            bool resultado = tarjeta.PuedeViajarGratuito();

            // Assert
            Assert.IsFalse(resultado);
        }

        #endregion

        #region Tests para TarjetaComun

        [Test]
        public void TarjetaComun_CalcularMontoPasaje_MenosDe30Viajes_DeberiaRetornarTarifaCompleta()
        {
            // Arrange
            var tarjeta = new TarjetaComun();
            
            // Act
            decimal resultado = tarjeta.CalcularMontoPasaje(1580m);

            // Assert
            Assert.AreEqual(1580m, resultado);
        }

        [Test]
        public void TarjetaComun_CalcularMontoPasaje_Entre30y59Viajes_DeberiaAplicar20Porciento()
        {
            // Arrange
            var tarjeta = new TarjetaComun();
            // Simular 35 viajes este mes
            for (int i = 0; i < 35; i++)
            {
                tarjeta.RegistrarViajeParaTest();
            }

            // Act
            decimal resultado = tarjeta.CalcularMontoPasaje(1000m);

            // Assert
            Assert.AreEqual(800m, resultado); // 1000 - 20% = 800
        }

        [Test]
        public void TarjetaComun_CalcularMontoPasaje_MasDe60Viajes_DeberiaAplicar25Porciento()
        {
            // Arrange
            var tarjeta = new TarjetaComun();
            // Simular 65 viajes este mes
            for (int i = 0; i < 65; i++)
            {
                tarjeta.RegistrarViajeParaTest();
            }

            // Act
            decimal resultado = tarjeta.CalcularMontoPasaje(1000m);

            // Assert
            Assert.AreEqual(750m, resultado); // 1000 - 25% = 750
        }

        [Test]
        public void TarjetaComun_PuedePagar_SaldoSuficiente_DeberiaRetornarTrue()
        {
            // Arrange
            var tarjeta = new TarjetaComun();
            tarjeta.Cargar(2000m);

            // Act
            bool resultado = tarjeta.PuedePagar(1580m);

            // Assert
            Assert.IsTrue(resultado);
        }

        [Test]
        public void TarjetaComun_PuedePagar_SaldoInsuficiente_DeberiaRetornarFalse()
        {
            // Arrange
            var tarjeta = new TarjetaComun();
            tarjeta.Cargar(100m);

            // Act
            bool resultado = tarjeta.PuedePagar(1580m);

            // Assert
            Assert.IsFalse(resultado);
        }

        #endregion

        #region Tests para MedioBoletoEstudiantil

        [Test]
        public void MedioBoletoEstudiantil_CalcularMontoPasaje_PrimerosDosViajes_DeberiaAplicarMedioBoleto()
        {
            // Arrange
            DateTime tiempoFijo = new DateTime(2024, 1, 1, 10, 0, 0);
            var tarjeta = new MedioBoletoEstudiantil(() => tiempoFijo);

            // Act
            decimal resultado = tarjeta.CalcularMontoPasaje(1580m);

            // Assert
            Assert.AreEqual(790m, resultado); // 1580 / 2 = 790
        }

        [Test]
        public void MedioBoletoEstudiantil_CalcularMontoPasaje_TercerViaje_DeberiaCobrarCompleto()
        {
            // Arrange
            DateTime tiempoFijo = new DateTime(2024, 1, 1, 10, 0, 0);
            var tarjeta = new MedioBoletoEstudiantil(() => tiempoFijo);
            tarjeta.Cargar(5000m);
            var colectivo = new Colectivo("132");

            // Primeros dos viajes
            colectivo.PagarCon(tarjeta);
            tiempoFijo = tiempoFijo.AddSeconds(6);
            colectivo.PagarCon(tarjeta);

            // Act - tercer viaje
            tiempoFijo = tiempoFijo.AddSeconds(6);
            decimal resultado = tarjeta.CalcularMontoPasaje(1580m);

            // Assert
            Assert.AreEqual(1580m, resultado); // Tarifa completa
        }

        [Test]
        public void MedioBoletoEstudiantil_PuedePagar_MenosDe5Segundos_DeberiaRetornarFalse()
        {
            // Arrange
            DateTime tiempoInicial = new DateTime(2024, 1, 1, 10, 0, 0);
            var tarjeta = new MedioBoletoEstudiantil(() => tiempoInicial);
            tarjeta.Cargar(2000m);
            var colectivo = new Colectivo("132");

            // Primer viaje
            colectivo.PagarCon(tarjeta);

            // Act - segundo viaje inmediato
            bool resultado = tarjeta.PuedePagar(1580m);

            // Assert
            Assert.IsFalse(resultado);
        }

        [Test]
        public void MedioBoletoEstudiantil_PuedePagar_MasDe5Segundos_DeberiaRetornarTrue()
        {
            // Arrange
            DateTime tiempoInicial = new DateTime(2024, 1, 1, 10, 0, 0);
            var tarjeta = new MedioBoletoEstudiantil(() => tiempoInicial);
            tarjeta.Cargar(2000m);
            var colectivo = new Colectivo("132");

            // Primer viaje
            colectivo.PagarCon(tarjeta);

            // Avanzar tiempo
            tiempoInicial = tiempoInicial.AddSeconds(6);
            var tarjetaNueva = new MedioBoletoEstudiantil(() => tiempoInicial);
            tarjetaNueva.Cargar(2000m);

            // Act
            bool resultado = tarjetaNueva.PuedePagar(1580m);

            // Assert
            Assert.IsTrue(resultado);
        }

        #endregion

        #region Tests para BoletoGratuitoEstudiantil

        [Test]
        public void BoletoGratuitoEstudiantil_CalcularMontoPasaje_PrimerosDosViajes_DeberiaSerGratuito()
        {
            // Arrange
            DateTime tiempoFijo = new DateTime(2024, 1, 1, 10, 0, 0);
            var tarjeta = new BoletoGratuitoEstudiantil(() => tiempoFijo);

            // Act
            decimal resultado = tarjeta.CalcularMontoPasaje(1580m);

            // Assert
            Assert.AreEqual(0m, resultado);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_CalcularMontoPasaje_TercerViaje_DeberiaCobrarCompleto()
        {
            // Arrange
            DateTime tiempoFijo = new DateTime(2024, 1, 1, 10, 0, 0);
            var tarjeta = new BoletoGratuitoEstudiantil(() => tiempoFijo);
            tarjeta.Cargar(5000m);
            var colectivo = new Colectivo("132");

            // Primeros dos viajes gratuitos
            colectivo.PagarCon(tarjeta);
            tiempoFijo = tiempoFijo.AddSeconds(6);
            colectivo.PagarCon(tarjeta);

            // Act - tercer viaje
            tiempoFijo = tiempoFijo.AddSeconds(6);
            decimal resultado = tarjeta.CalcularMontoPasaje(1580m);

            // Assert
            Assert.AreEqual(1580m, resultado); // Tarifa completa
        }

        [Test]
        public void BoletoGratuitoEstudiantil_Descontar_ViajeGratuito_NoDeberiaDescontarSaldo()
        {
            // Arrange
            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Cargar(2000m);
            decimal saldoInicial = tarjeta.Saldo;

            // Act
            bool resultado = tarjeta.Descontar(0m);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_Descontar_ViajeNoGratuito_DeberiaDescontarSaldo()
        {
            // Arrange
            DateTime tiempoFijo = new DateTime(2024, 1, 1, 10, 0, 0);
            var tarjeta = new BoletoGratuitoEstudiantil(() => tiempoFijo);
            tarjeta.Cargar(2000m);
            
            // Simular tercer viaje (no gratuito)
            for (int i = 0; i < 2; i++)
            {
                tarjeta.Descontar(0m);
                tiempoFijo = tiempoFijo.AddSeconds(6);
            }

            // Act
            bool resultado = tarjeta.Descontar(1580m);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(420m, tarjeta.Saldo); // 2000 - 1580 = 420
        }

        #endregion

        #region Tests para FranquiciaCompleta

        [Test]
        public void FranquiciaCompleta_CalcularMontoPasaje_SiempreDeberiaSerCero()
        {
            // Arrange
            var tarjeta = new FranquiciaCompleta();

            // Act
            decimal resultado = tarjeta.CalcularMontoPasaje(1580m);

            // Assert
            Assert.AreEqual(0m, resultado);
        }

        [Test]
        public void FranquiciaCompleta_PuedePagar_SiempreDeberiaRetornarTrue()
        {
            // Arrange
            var tarjeta = new FranquiciaCompleta();

            // Act
            bool resultado = tarjeta.PuedePagar(1580m);

            // Assert
            Assert.IsTrue(resultado);
        }

        [Test]
        public void FranquiciaCompleta_Descontar_NoDeberiaDescontarSaldo()
        {
            // Arrange
            var tarjeta = new FranquiciaCompleta();
            tarjeta.Cargar(2000m);
            decimal saldoInicial = tarjeta.Saldo;

            // Act
            bool resultado = tarjeta.Descontar(1580m);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
        }

        #endregion

        #region Tests para Colectivo

        [Test]
        public void Colectivo_PagarCon_TarjetaComun_SaldoSuficiente_DeberiaGenerarBoletoValido()
        {
            // Arrange
            var colectivo = new Colectivo("132");
            var tarjeta = new TarjetaComun();
            tarjeta.Cargar(2000m);

            // Act
            var boleto = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.IsTrue(boleto.EsValido);
            Assert.AreEqual(1580m, boleto.Monto);
        }

        [Test]
        public void Colectivo_PagarCon_TarjetaComun_SaldoInsuficiente_DeberiaGenerarBoletoInvalido()
        {
            // Arrange
            var colectivo = new Colectivo("132");
            var tarjeta = new TarjetaComun();
            tarjeta.Cargar(100m);

            // Act
            var boleto = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.IsFalse(boleto.EsValido);
            Assert.AreEqual(1580m, boleto.Monto);
        }

        [Test]
        public void Colectivo_PagarCon_MedioBoleto_DeberiaAplicarDescuento()
        {
            // Arrange
            var colectivo = new Colectivo("132");
            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Cargar(2000m);

            // Act
            var boleto = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.IsTrue(boleto.EsValido);
            Assert.AreEqual(790m, boleto.Monto); // 1580 / 2 = 790
        }

        [Test]
        public void Colectivo_PagarCon_BoletoGratuito_DeberiaSerGratuito()
        {
            // Arrange
            var colectivo = new Colectivo("132");
            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Cargar(2000m);

            // Act
            var boleto = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.IsTrue(boleto.EsValido);
            Assert.AreEqual(0m, boleto.Monto);
        }

        [Test]
        public void Colectivo_PagarCon_FranquiciaCompleta_DeberiaSerGratuito()
        {
            // Arrange
            var colectivo = new Colectivo("132");
            var tarjeta = new FranquiciaCompleta();

            // Act
            var boleto = colectivo.PagarCon(tarjeta);

            // Assert
            Assert.IsTrue(boleto.EsValido);
            Assert.AreEqual(0m, boleto.Monto);
        }

        [Test]
        public void Colectivo_ObtenerTarifa_Interurbano_DeberiaRetornarTarifaMayor()
        {
            // Arrange
            var colectivoInterurbano = new Colectivo("500", true);

            // Act
            decimal tarifa = colectivoInterurbano.ObtenerTarifa();

            // Assert
            Assert.AreEqual(3000m, tarifa);
        }

        [Test]
        public void Colectivo_ObtenerTarifa_Urbano_DeberiaRetornarTarifaBasica()
        {
            // Arrange
            var colectivoUrbano = new Colectivo("132");

            // Act
            decimal tarifa = colectivoUrbano.ObtenerTarifa();

            // Assert
            Assert.AreEqual(1580m, tarifa);
        }

        #endregion

        #region Tests para Boleto

        [Test]
        public void Boleto_Constructor_DeberiaInicializarPropiedadesCorrectamente()
        {
            // Arrange
            decimal monto = 1580m;
            string linea = "132";
            DateTime fecha = DateTime.Now;
            bool esValido = true;
            string tipoTarjeta = "TarjetaComun";
            decimal saldoRestante = 420m;
            int idTarjeta = 1;

            // Act
            var boleto = new Boleto(monto, linea, fecha, esValido, tipoTarjeta, saldoRestante, idTarjeta);

            // Assert
            Assert.AreEqual(monto, boleto.Monto);
            Assert.AreEqual(linea, boleto.LineaColectivo);
            Assert.AreEqual(fecha, boleto.FechaHora);
            Assert.AreEqual(esValido, boleto.EsValido);
            Assert.AreEqual(tipoTarjeta, boleto.TipoTarjeta);
            Assert.AreEqual(saldoRestante, boleto.SaldoRestante);
            Assert.AreEqual(idTarjeta, boleto.IdTarjeta);
        }

        [Test]
        public void Boleto_ToString_DeberiaIncluirInformacionBasica()
        {
            // Arrange
            var boleto = new Boleto(1580m, "132", DateTime.Now, true, "TarjetaComun", 420m, 1);

            // Act
            string resultado = boleto.ToString();

            // Assert
            Assert.IsTrue(resultado.Contains("Boleto"));
            Assert.IsTrue(resultado.Contains("132"));
            Assert.IsTrue(resultado.Contains("1580"));
        }

        [Test]
        public void Boleto_ObtenerInformacionCompleta_DeberiaIncluirTodosLosDetalles()
        {
            // Arrange
            var boleto = new Boleto(1580m, "132", DateTime.Now, true, "TarjetaComun", 420m, 1);

            // Act
            string resultado = boleto.ObtenerInformacionCompleta();

            // Assert
            Assert.IsTrue(resultado.Contains("BOLETO DETALLADO"));
            Assert.IsTrue(resultado.Contains("132"));
            Assert.IsTrue(resultado.Contains("1580"));
            Assert.IsTrue(resultado.Contains("420"));
            Assert.IsTrue(resultado.Contains("VÁLIDO"));
        }

        [Test]
        public void Boleto_ConTrasbordo_DeberiaMarcarComoTrasbordo()
        {
            // Arrange
            var boleto = new Boleto(0m, "143", DateTime.Now, true, "TarjetaComun", 100m, 1, 
                esTrasbordo: true, idBoletoOrigenTrasbordo: 2);

            // Assert
            Assert.IsTrue(boleto.EsTrasbordo);
            Assert.AreEqual(2, boleto.IdBoletoOrigenTrasbordo);
        }

        #endregion

        #region Tests para SistemaTrasbordos

        [Test]
        public void SistemaTrasbordos_RegistrarBoleto_DeberiaAgregarBoletoCorrectamente()
        {
            // Arrange
            var boleto = new Boleto(1580m, "132", DateTime.Now, true, "TarjetaComun", 420m, 1);
            
            // Limpiar datos previos
            var field = typeof(SistemaTrasbordos).GetField("boletosPorTarjeta", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var diccionario = (Dictionary<int, List<Boleto>>)field.GetValue(null);
            diccionario.Clear();

            // Act
            SistemaTrasbordos.RegistrarBoleto(boleto);

            // Assert
            var boletoObtenido = SistemaTrasbordos.ObtenerBoletoOrigenTrasbordo(1, "143");
            Assert.IsNotNull(boletoObtenido);
            Assert.AreEqual("132", boletoObtenido.LineaColectivo);
        }

        [Test]
        public void SistemaTrasbordos_ObtenerBoletoOrigenTrasbordo_SinBoletos_DeberiaRetornarNull()
        {
            // Arrange
            // Limpiar datos previos
            var field = typeof(SistemaTrasbordos).GetField("boletosPorTarjeta", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var diccionario = (Dictionary<int, List<Boleto>>)field.GetValue(null);
            diccionario.Clear();

            // Act
            var resultado = SistemaTrasbordos.ObtenerBoletoOrigenTrasbordo(999, "132");

            // Assert
            Assert.IsNull(resultado);
        }

        #endregion

        #region Clases auxiliares para testing

        // Clase dummy para testing de Tarjeta
        public class TarjetaDummy : Tarjeta
        {
            public override decimal CalcularMontoPasaje(decimal tarifaBase) => tarifaBase;
            public override bool PuedePagar(decimal tarifaBase) => saldo >= tarifaBase;
            public void SetSaldo(decimal valor) => saldo = valor;
            public void SetSaldoPendiente(decimal valor) => saldoPendienteAcreditacion = valor;

            public void RegistrarViajeManual(DateTime fecha)
            {
                var campoHistorial = typeof(Tarjeta)
                    .GetField("historialViajes", BindingFlags.NonPublic | BindingFlags.Instance);
                var lista = (List<DateTime>)campoHistorial.GetValue(this);
                lista.Add(fecha);

                var campoMensual = typeof(Tarjeta)
                    .GetField("historialViajesMensual", BindingFlags.NonPublic | BindingFlags.Instance);
                var listaMensual = (List<DateTime>)campoMensual.GetValue(this);
                listaMensual.Add(fecha);
            }

            public void LimpiarViajes()
            {
                var campoHistorial = typeof(Tarjeta)
                    .GetField("historialViajes", BindingFlags.NonPublic | BindingFlags.Instance);
                ((List<DateTime>)campoHistorial.GetValue(this)).Clear();

                var campoMensual = typeof(Tarjeta)
                    .GetField("historialViajesMensual", BindingFlags.NonPublic | BindingFlags.Instance);
                ((List<DateTime>)campoHistorial.GetValue(this)).Clear();
            }
        }

        // Contexto para modificar DateTime.Now en tests
        public class DateTimeContext : IDisposable
        {
            private readonly System.Threading.ThreadLocal<DateTime> _threadLocal;

            public DateTimeContext(DateTime newNow)
            {
                _threadLocal = new System.Threading.ThreadLocal<DateTime>(() => newNow);
                // Aquí se implementaría la lógica para modificar DateTime.Now
                // En un entorno real usaríamos una librería como SystemTime
            }

            public void Dispose()
            {
                _threadLocal?.Dispose();
            }
        }

        #endregion
    }
}