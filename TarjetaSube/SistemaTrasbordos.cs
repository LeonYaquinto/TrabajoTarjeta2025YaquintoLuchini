using System;
using System.Collections.Generic;
using System.Linq;

public static class SistemaTrasbordos
{
    private static Dictionary<int, List<Boleto>> boletosPorTarjeta = new Dictionary<int, List<Boleto>>();

    public static Boleto? ObtenerBoletoOrigenTrasbordo(int idTarjeta, string lineaColectivo)
    {
        if (!boletosPorTarjeta.ContainsKey(idTarjeta))
            return null;

        DateTime ahora = DateTime.Now;
        return boletosPorTarjeta[idTarjeta]
            .Where(b => b.EsValido && !b.EsTrasbordo &&
                   (ahora - b.FechaHora).TotalMinutes <= 60 &&
                   b.LineaColectivo != lineaColectivo)
            .OrderByDescending(b => b.FechaHora)
            .FirstOrDefault();
    }

    public static void RegistrarBoleto(Boleto boleto)
    {
        if (!boletosPorTarjeta.ContainsKey(boleto.IdTarjeta))
        {
            boletosPorTarjeta[boleto.IdTarjeta] = new List<Boleto>();
        }

        boletosPorTarjeta[boleto.IdTarjeta].Add(boleto);

        var ahora = DateTime.Now;
        boletosPorTarjeta[boleto.IdTarjeta].RemoveAll(b => (ahora - b.FechaHora).TotalHours > 2);
    }

    public static bool PuedeRealizarTrasbordo(int idTarjeta, string lineaColectivo)
    {
        if (!boletosPorTarjeta.ContainsKey(idTarjeta))
            return false;

        DateTime ahora = DateTime.Now;
        var boletosRecientes = boletosPorTarjeta[idTarjeta]
            .Where(b => b.EsValido && !b.EsTrasbordo &&
                   (ahora - b.FechaHora).TotalMinutes <= 60 &&
                   b.LineaColectivo != lineaColectivo)
            .ToList();


        if (ahora.DayOfWeek == DayOfWeek.Sunday)
            return false;

        int hora = ahora.Hour;
        if (hora < 7 || hora >= 22)
            return false;

        return boletosRecientes.Any();
    }




}