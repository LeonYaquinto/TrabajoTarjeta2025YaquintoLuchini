using NUnit.Framework;
using TarjetaSube;
using System;
using System.Linq;
using System.Reflection;

namespace TarjetaSube.Tests
{
    public class TarjetaDummy : Tarjeta
    {
        public override decimal CalcularMontoPasaje(decimal tarifaBase) => tarifaBase;
        public override bool PuedePagar(decimal tarifaBase) => saldo >= tarifaBase;
        public void SetSaldo(decimal valor) => saldo = valor;
        public void SetSaldoPendiente(decimal valor) => saldoPendienteAcreditacion = valor;



        public void LimpiarViajes()
        {
            var campoHistorial = typeof(Tarjeta)
                .GetField("historialViajes", BindingFlags.NonPublic | BindingFlags.Instance);
            ((System.Collections.Generic.List<DateTime>)campoHistorial.GetValue(this)).Clear();

            var campoMensual = typeof(Tarjeta)
                .GetField("historialViajesMensual", BindingFlags.NonPublic | BindingFlags.Instance);
            ((System.Collections.Generic.List<DateTime>)campoHistorial.GetValue(this)).Clear();
        }


        public void RegistrarViajeManual(DateTime fecha)
        {
            var campoHistorial = typeof(Tarjeta)
                .GetField("historialViajes", BindingFlags.NonPublic | BindingFlags.Instance);
            var lista = (System.Collections.Generic.List<DateTime>)campoHistorial.GetValue(this);
            lista.Add(fecha);

            var campoMensual = typeof(Tarjeta)
                .GetField("historialViajesMensual", BindingFlags.NonPublic | BindingFlags.Instance);
            var listaMensual = (System.Collections.Generic.List<DateTime>)campoMensual.GetValue(this);
            listaMensual.Add(fecha);
        }
    }

    [TestFixture]
    public class CargaEndeudadoTest
    {


        [Test]
        public void Cargar_DeudaCompletaPeroExcedeLimite_SaldoAlLimiteYPendienteCorrecto()
        {

            var tarjeta = new TarjetaDummy();
            tarjeta.SetSaldo(-1000m); 
            decimal montoCarga = 60000m; 

            bool resultado = tarjeta.Cargar(montoCarga);

            Assert.IsTrue(resultado, "La carga debería ser exitosa.");
            Assert.AreEqual(56000m, tarjeta.Saldo, "El saldo no debería superar el límite establecido (56000).");
            decimal excedenteEsperado = 60000m - 1000m - 56000m;
            Assert.AreEqual(excedenteEsperado, tarjeta.SaldoPendienteAcreditacion, "El saldo pendiente debe coincidir con el excedente sobre el límite.");
        }



        [Test]
        public void CantidadViajesHoy_SinViajes_RetornaCero()
        {
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();

            int resultado = tarjeta.CantidadViajesHoy();

            Assert.AreEqual(0, resultado);
        }

        [Test]
        public void Cargar_DeudaCompletaYSaldoDentroDelLimite_ActualizaSaldoCorrectamente()
        {
            var tarjeta = new TarjetaDummy();
            tarjeta.SetSaldo(-500m);
            decimal montoCarga = 2000m;

            bool resultado = tarjeta.Cargar(montoCarga);

            Assert.IsTrue(resultado, "La carga debería ser exitosa.");
            Assert.AreEqual(1500m, tarjeta.Saldo, "El saldo debería reflejar el monto restante después de pagar la deuda.");
            Assert.AreEqual(0m, tarjeta.SaldoPendienteAcreditacion, "No debería haber saldo pendiente de acreditación.");
        }

        [Test]
        public void CantidadViajesHoy_ConViajeHoy_RetornaUno()
        {
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();
            tarjeta.RegistrarViajeManual(DateTime.Now);

            int resultado = tarjeta.CantidadViajesHoy();

            Assert.AreEqual(1, resultado);
        }


        [Test]
        public void PuedeViajarGratuito_MenosDeDosViajes_RetornaTrue()
        {
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();
            tarjeta.RegistrarViajeManual(DateTime.Now);

            bool resultado = tarjeta.PuedeViajarGratuito();

            Assert.IsTrue(resultado);
        }

        [Test]
        public void PuedeViajarGratuito_DosViajesHoy_RetornaFalse()
        {
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();
            tarjeta.RegistrarViajeManual(DateTime.Now);
            tarjeta.RegistrarViajeManual(DateTime.Now.AddMinutes(-1));

            bool resultado = tarjeta.PuedeViajarGratuito();

            Assert.IsFalse(resultado);
        }

        [Test]
        public void PuedeViajarMedioBoleto_ViajeReciente_RetornaFalse()
        {
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();
            tarjeta.RegistrarViajeManual(DateTime.Now.AddSeconds(-2));

            bool resultado = tarjeta.PuedeViajarMedioBoleto();

            Assert.IsFalse(resultado);
        }

        [Test]
        public void PuedeViajarMedioBoleto_MasDeCincoSegundosYMenosDeDosViajes_RetornaTrue()
        {
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();
            tarjeta.RegistrarViajeManual(DateTime.Now.AddSeconds(-10)); 

            bool resultado = tarjeta.PuedeViajarMedioBoleto();

            Assert.IsTrue(resultado);
        }

        [Test]
        public void PuedeViajarMedioBoleto_DosViajesHoy_RetornaFalse()
        {
            DateTime now = new DateTime(2025, 1, 1, 10, 0, 0);
            var tarjeta = new MedioBoletoEstudiantil(() => now);

            tarjeta.Cargar(5000m);
            var colectivo = new Colectivo("132");

            var boleto1 = colectivo.PagarCon(tarjeta);
            now = now.AddSeconds(6);
            var boleto2 = colectivo.PagarCon(tarjeta);

            now = now.AddSeconds(6);
            var boleto3 = colectivo.PagarCon(tarjeta);

            Assert.IsFalse(boleto3.EsValido, "No debe permitir más de dos viajes por día con medio boleto");
        }


        [Test]
        public void Cargar_MontoInsuficienteParaCubrirDeuda_ReduceDeudaParcialmente()
        {
            var tarjeta = new TarjetaDummy();
            tarjeta.SetSaldo(-1000m); 
            decimal montoCarga = 500m; 


            bool resultado = tarjeta.Cargar(montoCarga);


            Assert.IsTrue(resultado, "La carga debería ser exitosa incluso si no cubre toda la deuda.");
            Assert.AreEqual(-500m, tarjeta.Saldo, "El saldo debería reflejar la deuda restante (-500).");
            Assert.AreEqual(0m, tarjeta.SaldoPendienteAcreditacion, "No debería haber saldo pendiente de acreditación.");
        }
        
        [Test]
        public void AcreditarCarga_EspacioInsuficiente_AcreditaHastaElLimiteYDejaPendiente()
        {

            var tarjeta = new TarjetaDummy();
            tarjeta.SetSaldo(55000m);
            tarjeta.SetSaldoPendiente(10000m);         


            tarjeta.AcreditarCarga();


            Assert.AreEqual(56000m, tarjeta.Saldo, "Debería haberse acreditado solo hasta el límite de saldo.");
            Assert.AreEqual(9000m, tarjeta.SaldoPendienteAcreditacion, "Debería quedar pendiente el excedente.");
        }



        [Test]
        public void AcreditarCarga_EspacioSuficiente_AcreditaTodoElSaldoPendiente()
        {
            var tarjeta = new TarjetaDummy();
            tarjeta.SetSaldo(10000m);
            tarjeta.SetSaldoPendiente(5000m);

            tarjeta.AcreditarCarga();

            Assert.AreEqual(15000m, tarjeta.Saldo, "Debería haberse acreditado todo el saldo pendiente.");
            Assert.AreEqual(0m, tarjeta.SaldoPendienteAcreditacion, "No debería quedar saldo pendiente.");
        }
        

        
        [Test]
        public void PuedeViajarMedioBoleto_SinViajes_RetornaTrue()
        {
            var tarjeta = new TarjetaDummy();
            tarjeta.LimpiarViajes();

            bool resultado = tarjeta.PuedeViajarMedioBoleto();

            Assert.IsTrue(resultado);
        }


    }
}
