using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Prueba del Sistema SUBE ===");

        var tarjeta = new Tarjeta();
        Console.WriteLine($"Saldo inicial: ${tarjeta.Saldo}");

        if (tarjeta.Cargar(5000))
        {
            Console.WriteLine($"Carga exitosa. Saldo: ${tarjeta.Saldo}");
        }

        var colectivo = new Colectivo("132", "Rosario Bus");
        var boleto = colectivo.PagarCon(tarjeta);

        if (boleto != null)
        {
            Console.WriteLine($"Viaje exitoso!");
            Console.WriteLine($"Monto: ${boleto.MontoPagado}");
            Console.WriteLine($"Línea: {boleto.LineaColectivo}");
            Console.WriteLine($"Saldo restante: ${boleto.SaldoRestante}");
        }
        else
        {
            Console.WriteLine("Error en el pago del viaje");
        }

        Console.WriteLine("Prueba completada.");
    }
}
