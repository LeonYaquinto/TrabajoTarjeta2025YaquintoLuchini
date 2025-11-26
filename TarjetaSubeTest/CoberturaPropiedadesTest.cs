using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class CoberturaPropiedadesTest
    {
        [Test]
        public void Test_Tarjeta_PropiedadViajesEnMes()
        {
            Tarjeta tarjeta = new Tarjeta();
            Assert.AreEqual(0, tarjeta.ViajesEnMes);
        }

        [Test]
        public void Test_Tarjeta_PropiedadId()
        {
            Tarjeta tarjeta1 = new Tarjeta();
            Tarjeta tarjeta2 = new Tarjeta();
            
            Assert.Greater(tarjeta2.Id, tarjeta1.Id, "Los IDs deben ser únicos y crecientes");
        }

        [Test]
        public void Test_Tarjeta_PropiedadEsTransbordo_Inicial()
        {
            Tarjeta tarjeta = new Tarjeta();
            Assert.IsFalse(tarjeta.EsTransbordo);
        }

        [Test]
        public void Test_Tarjeta_PropiedadUltimaTarifaCobrada_Inicial()
        {
            Tarjeta tarjeta = new Tarjeta();
            Assert.AreEqual(0m, tarjeta.UltimaTarifaCobrada);
        }

        [Test]
        public void Test_Colectivo_PropiedadLinea()
        {
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");
            Assert.AreEqual("120", colectivo.Linea);
        }

        [Test]
        public void Test_Colectivo_PropiedadEmpresa()
        {
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");
            Assert.AreEqual("Rosario Bus", colectivo.Empresa);
        }

        [Test]
        public void Test_Colectivo_PropiedadEsInterurbano_Urbano()
        {
            Colectivo urbano = new Colectivo("120", "Rosario Bus");
            Assert.IsFalse(urbano.EsInterurbano);
        }

        [Test]
        public void Test_Colectivo_PropiedadEsInterurbano_Interurbano()
        {
            Colectivo interurbano = new Colectivo("500", "Gálvez", true);
            Assert.IsTrue(interurbano.EsInterurbano);
        }

        [Test]
        public void Test_Boleto_PropiedadMontoPagado()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(1580m, boleto.MontoPagado);
        }

        [Test]
        public void Test_Boleto_PropiedadLineaColectivo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("143", "Semtur");
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual("143", boleto.LineaColectivo);
        }

        [Test]
        public void Test_Boleto_PropiedadEmpresa()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("120", "Las Delicias");
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual("Las Delicias", boleto.Empresa);
        }

        [Test]
        public void Test_Boleto_PropiedadSaldoRestante()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(3420m, boleto.SaldoRestante);
        }

        [Test]
        public void Test_Boleto_PropiedadSaldo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(3420m, boleto.Saldo);
        }

        [Test]
        public void Test_Boleto_PropiedadFechaHora()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");
            
            DateTime antes = DateTime.Now;
            Boleto boleto = colectivo.PagarCon(tarjeta);
            DateTime despues = DateTime.Now;
            
            Assert.GreaterOrEqual(boleto.FechaHora, antes);
            Assert.LessOrEqual(boleto.FechaHora, despues);
        }

        [Test]
        public void Test_Boleto_PropiedadFecha()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(DateTime.Now.Date, boleto.Fecha);
        }

        [Test]
        public void Test_Boleto_PropiedadTipoTarjeta_Normal()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual("Normal", boleto.TipoTarjeta);
        }

        [Test]
        public void Test_Boleto_PropiedadTipoTarjeta_MedioBoleto()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            tarjeta.Cargar(5000);
            
            DateTime ahora = DateTime.Now;
            if (ahora.DayOfWeek >= DayOfWeek.Monday && ahora.DayOfWeek <= DayOfWeek.Friday &&
                ahora.Hour >= 6 && ahora.Hour < 22)
            {
                Colectivo colectivo = new Colectivo("120", "Rosario Bus");
                Boleto boleto = colectivo.PagarCon(tarjeta);
                
                Assert.AreEqual("Medio Boleto", boleto.TipoTarjeta);
            }
            else
            {
                Assert.Ignore("Test solo válido en horario L-V 6-22hs");
            }
        }

        [Test]
        public void Test_Boleto_PropiedadTotalAbonado()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(1580m, boleto.TotalAbonado);
        }

        [Test]
        public void Test_Boleto_PropiedadIdTarjeta()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
        }

        [Test]
        public void Test_Boleto_PropiedadEsTransbordo_NoTransbordo()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsFalse(boleto.EsTransbordo);
        }

        [Test]
        public void Test_Boleto_ToString_ContieneInformacion()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            string texto = boleto.ToString();
            
            Assert.IsTrue(texto.Contains("Boleto"));
            Assert.IsTrue(texto.Contains("120"));
            Assert.IsTrue(texto.Contains("Rosario Bus"));
            Assert.IsTrue(texto.Contains("Normal"));
            Assert.IsTrue(texto.Contains("1580"));
        }

        [Test]
        public void Test_Boleto_ToString_ConTransbordo()
        {
            DateTime ahora = DateTime.Now;
            
            if (ahora.DayOfWeek != DayOfWeek.Sunday && ahora.Hour >= 7 && ahora.Hour < 22)
            {
                Tarjeta tarjeta = new Tarjeta();
                tarjeta.Cargar(5000);
                
                Colectivo colectivo1 = new Colectivo("120", "Rosario Bus");
                Colectivo colectivo2 = new Colectivo("115", "Rosario Bus");
                
                colectivo1.PagarCon(tarjeta);
                Boleto boleto2 = colectivo2.PagarCon(tarjeta);
                
                string texto = boleto2.ToString();
                
                Assert.IsTrue(texto.Contains("TRASBORDO"));
            }
            else
            {
                Assert.Ignore("Test solo válido en horario de trasbordo (L-S 7-22hs)");
            }
        }

        [Test]
        public void Test_TarjetaMedioBoleto_ObtenerTipo()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            Assert.AreEqual("Medio Boleto", tarjeta.ObtenerTipo());
        }

        [Test]
        public void Test_TarjetaFranquiciaCompleta_ObtenerTipo()
        {
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta();
            Assert.AreEqual("Franquicia Completa", tarjeta.ObtenerTipo());
        }

        [Test]
        public void Test_Tarjeta_ObtenerTipo()
        {
            Tarjeta tarjeta = new Tarjeta();
            Assert.AreEqual("Normal", tarjeta.ObtenerTipo());
        }

        [Test]
        public void Test_Tarjeta_PuedePagarEnHorario_Siempre()
        {
            Tarjeta tarjeta = new Tarjeta();
            Assert.IsTrue(tarjeta.PuedePagarEnHorario());
        }

        [Test]
        public void Test_Tarjeta_PuedeUsarSaldoNegativo_TarjetaNormal()
        {
            Tarjeta tarjeta = new Tarjeta();
            Assert.IsTrue(tarjeta.PuedeUsarSaldoNegativo(500m));
        }

        [Test]
        public void Test_Tarjeta_PuedeUsarSaldoNegativo_ExcedeLimite()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(2000);
            Assert.IsFalse(tarjeta.PuedeUsarSaldoNegativo(5000m));
        }

        [Test]
        public void Test_Tarjeta_ObtenerTarifaInterurbana()
        {
            Tarjeta tarjeta = new Tarjeta();
            Assert.AreEqual(3000m, tarjeta.ObtenerTarifaInterurbana());
        }

        [Test]
        public void Test_MedioBoleto_ObtenerTarifaInterurbana_PrimerosViajes()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            Assert.AreEqual(1500m, tarjeta.ObtenerTarifaInterurbana());
        }

        [Test]
        public void Test_FranquiciaCompleta_ObtenerTarifaInterurbana_PrimerosViajes()
        {
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta();
            Assert.AreEqual(0m, tarjeta.ObtenerTarifaInterurbana());
        }
    }
}
