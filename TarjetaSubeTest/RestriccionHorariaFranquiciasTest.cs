using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class RestriccionHorariaFranquiciasTest
    {
        // NOTA: Estos tests dependen de la hora actual del sistema
        // En un entorno de producción, se debería usar inyección de dependencias
        // para mockear el tiempo

        [Test]
        public void Test_MedioBoleto_PuedePagarEnHorarioPermitido()
        {
            // Este test solo funciona si se ejecuta L-V de 6-22hs
            DateTime ahora = DateTime.Now;
            
            if (ahora.DayOfWeek >= DayOfWeek.Monday && ahora.DayOfWeek <= DayOfWeek.Friday &&
                ahora.Hour >= 6 && ahora.Hour < 22)
            {
                TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
                tarjeta.Cargar(5000);
                Colectivo colectivo = new Colectivo("120", "Rosario Bus");

                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, "Debería poder pagar en horario permitido");
            }
            else
            {
                Assert.Ignore("Test solo válido en horario L-V 6-22hs");
            }
        }

        [Test]
        public void Test_FranquiciaCompleta_PuedePagarEnHorarioPermitido()
        {
            DateTime ahora = DateTime.Now;
            
            if (ahora.DayOfWeek >= DayOfWeek.Monday && ahora.DayOfWeek <= DayOfWeek.Friday &&
                ahora.Hour >= 6 && ahora.Hour < 22)
            {
                TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta();
                tarjeta.Cargar(5000);
                Colectivo colectivo = new Colectivo("120", "Rosario Bus");

                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNotNull(boleto, "Debería poder pagar en horario permitido");
            }
            else
            {
                Assert.Ignore("Test solo válido en horario L-V 6-22hs");
            }
        }

        [Test]
        public void Test_MedioBoleto_NoPuedePagarFueraDeHorario()
        {
            DateTime ahora = DateTime.Now;
            
            // Solo ejecutar si estamos fuera del horario permitido
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
                tarjeta.Cargar(5000);
                Colectivo colectivo = new Colectivo("120", "Rosario Bus");

                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNull(boleto, "No debería poder pagar fuera del horario permitido");
            }
            else
            {
                Assert.Ignore("Test solo válido fuera del horario L-V 6-22hs");
            }
        }

        [Test]
        public void Test_FranquiciaCompleta_NoPuedePagarFueraDeHorario()
        {
            DateTime ahora = DateTime.Now;
            
            if (ahora.DayOfWeek == DayOfWeek.Saturday || ahora.DayOfWeek == DayOfWeek.Sunday ||
                ahora.Hour < 6 || ahora.Hour >= 22)
            {
                TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta();
                tarjeta.Cargar(5000);
                Colectivo colectivo = new Colectivo("120", "Rosario Bus");

                Boleto boleto = colectivo.PagarCon(tarjeta);
                Assert.IsNull(boleto, "No debería poder pagar fuera del horario permitido");
            }
            else
            {
                Assert.Ignore("Test solo válido fuera del horario L-V 6-22hs");
            }
        }

        [Test]
        public void Test_TarjetaNormal_PuedePagarEnCualquierHorario()
        {
            // Las tarjetas normales no tienen restricción horaria
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("120", "Rosario Bus");

            Boleto boleto = colectivo.PagarCon(tarjeta);
            Assert.IsNotNull(boleto, "Tarjeta normal debería poder pagar en cualquier horario");
        }

        [Test]
        public void Test_PuedePagarEnHorario_MedioBoleto_LunesAViernes()
        {
            TarjetaMedioBoleto tarjeta = new TarjetaMedioBoleto();
            
            // Este método verifica el horario actual
            DateTime ahora = DateTime.Now;
            bool puedeEnHorarioActual = tarjeta.PuedePagarEnHorario();
            
            bool enHorarioPermitido = (ahora.DayOfWeek >= DayOfWeek.Monday && 
                                       ahora.DayOfWeek <= DayOfWeek.Friday &&
                                       ahora.Hour >= 6 && ahora.Hour < 22);
            
            Assert.AreEqual(enHorarioPermitido, puedeEnHorarioActual);
        }

        [Test]
        public void Test_PuedePagarEnHorario_FranquiciaCompleta_LunesAViernes()
        {
            TarjetaFranquiciaCompleta tarjeta = new TarjetaFranquiciaCompleta();
            
            DateTime ahora = DateTime.Now;
            bool puedeEnHorarioActual = tarjeta.PuedePagarEnHorario();
            
            bool enHorarioPermitido = (ahora.DayOfWeek >= DayOfWeek.Monday && 
                                       ahora.DayOfWeek <= DayOfWeek.Friday &&
                                       ahora.Hour >= 6 && ahora.Hour < 22);
            
            Assert.AreEqual(enHorarioPermitido, puedeEnHorarioActual);
        }

        [Test]
        public void Test_PuedePagarEnHorario_TarjetaNormal_Siempre()
        {
            Tarjeta tarjeta = new Tarjeta();
            
            // Tarjeta normal siempre puede pagar
            bool puede = tarjeta.PuedePagarEnHorario();
            
            Assert.IsTrue(puede, "Tarjeta normal siempre debería poder pagar");
        }
    }
}
