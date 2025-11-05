using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace TarjetaSube
{
    public abstract class Tarjeta
    {
        protected decimal saldo;
        protected decimal saldoPendienteAcreditacion;
        private const decimal LIMITE_SALDO = 56000m;
        public const decimal SALDO_NEGATIVO_MAXIMO = -1200m;
        protected static readonly decimal[] CARGAS_ACEPTADAS =
            { 2000m, 3000m, 4000m, 5000m, 8000m, 10000m, 15000m, 20000m, 25000m, 30000m, 60000m, 500m };

        public decimal Saldo => saldo;
        public decimal SaldoPendienteAcreditacion => saldoPendienteAcreditacion;
        public int Id { get; protected set; }
        private static int proximoId = 1;

        private List<DateTime> historialViajes;
        private List<DateTime> historialViajesMensual;
        protected Tarjeta()
        {
            saldo = 0m;
            saldoPendienteAcreditacion = 0m;
            Id = proximoId++;
            historialViajes = new List<DateTime>();
            historialViajesMensual = new List<DateTime>();
        }

        public virtual bool Cargar(decimal monto)
        {
            if (!CARGAS_ACEPTADAS.Contains(monto))
                return false;

            if (saldo < 0)
            {
                decimal deuda = Math.Abs(saldo);
                decimal montoRestante = monto - deuda;

                if (montoRestante >= 0)
                {
                    saldo = 0m; 


                    decimal espacioDisponible = LIMITE_SALDO - saldo;

                    if (montoRestante <= espacioDisponible)
                    {
                        saldo += montoRestante;
                    }
                    else
                    {
                        saldo = LIMITE_SALDO;
                        saldoPendienteAcreditacion += (montoRestante - espacioDisponible);
                    }
                    return true;
                }
                else
                {

                    saldo += monto;
                    return true;
                }
            }
            else
            {
                decimal espacioDisponible = LIMITE_SALDO - saldo;

                if (monto <= espacioDisponible)
                {
                    saldo += monto;
                }
                else
                {
                    saldo = LIMITE_SALDO;
                    saldoPendienteAcreditacion += (monto - espacioDisponible);
                }
                return true;
            }
        }

        

        public virtual bool Descontar(decimal monto)
        {
            
            if (saldo - monto < SALDO_NEGATIVO_MAXIMO)
                return false;

            saldo -= monto;
            RegistrarViaje();
            AcreditarCarga(); 
            return true;
        }

        public bool PuedeViajarGratuito()
        {

            bool resultado = CantidadViajesHoy() < 2;

            return resultado;
        }

        protected void RegistrarViaje()
        {
            DateTime ahora = DateTime.Now;
            historialViajes.Add(ahora);
            historialViajesMensual.Add(ahora);
        }

        public int CantidadViajesHoy()
        {
            DateTime hoy = DateTime.Today;
            return historialViajes.Count(v => v.Date == hoy);
        }

        public bool PuedeViajarMedioBoleto()
        {
            if (historialViajes.Count == 0) return true;

            DateTime ultimoViaje = historialViajes.Last();
            TimeSpan tiempoDesdeUltimoViaje = DateTime.Now - ultimoViaje;


            if (tiempoDesdeUltimoViaje.TotalSeconds < 5)  
                return false;


            if (CantidadViajesHoy() >= 2)
                return false;

            return true;
        }

        public virtual void AcreditarCarga()
        {
            if (saldoPendienteAcreditacion > 0)
            {
                decimal espacioDisponible = LIMITE_SALDO - saldo;
                decimal montoAAcreditar = Math.Min(saldoPendienteAcreditacion, espacioDisponible);

                saldo += montoAAcreditar;
                saldoPendienteAcreditacion -= montoAAcreditar;
            }
        }

        
        public int CantidadViajesEsteMes()
        {
            DateTime ahora = DateTime.Now;
            DateTime primerDiaMes = new DateTime(ahora.Year, ahora.Month, 1);
            return historialViajesMensual.Count(v => v >= primerDiaMes && v <= ahora);
        }
        public void RegistrarViajeParaTest()
        {
            RegistrarViaje();
        }


        public abstract decimal CalcularMontoPasaje(decimal tarifaBase);
        public abstract bool PuedePagar(decimal tarifaBase);


        [ExcludeFromCodeCoverage]
        public static void Main(string[] args)
        {
            Console.WriteLine("Sistema de Tarjeta SUBE - Modo compilación");
            var colectivo = new Colectivo("132");
            var franquicia = new FranquiciaCompleta();
            Console.WriteLine("Sistema compilado correctamente");
        }
   
    }
}