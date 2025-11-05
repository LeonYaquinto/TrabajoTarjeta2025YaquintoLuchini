using System;

public class Boleto
{
    public decimal Monto { get; }
    public string LineaColectivo { get; }
    public DateTime FechaHora { get; }
    public bool EsValido { get; }
    public string TipoTarjeta { get; }
    public decimal SaldoRestante { get; }
    public int IdTarjeta { get; }
    public decimal TotalAbonado { get; }
    public decimal MontoRecargaSaldoNegativo { get; }
    public bool EsTrasbordo { get; }
    public int? IdBoletoOrigenTrasbordo { get; }

    public Boleto(decimal monto, string lineaColectivo, DateTime fechaHora, bool esValido,
                 string tipoTarjeta, decimal saldoRestante, int idTarjeta,
                 decimal totalAbonado = 0, decimal montoRecarga = 0,
                 bool esTrasbordo = false, int? idBoletoOrigenTrasbordo = null)
    {
        Monto = monto;
        LineaColectivo = lineaColectivo;
        FechaHora = fechaHora;
        EsValido = esValido;
        TipoTarjeta = tipoTarjeta;
        SaldoRestante = saldoRestante;
        IdTarjeta = idTarjeta;
        TotalAbonado = totalAbonado;
        MontoRecargaSaldoNegativo = montoRecarga;
        EsTrasbordo = esTrasbordo;
        IdBoletoOrigenTrasbordo = idBoletoOrigenTrasbordo;
    }

    public override string ToString()
    {
        string infoBase = $"Boleto - Línea: {LineaColectivo}, Monto: ${Monto}, " +
               $"Fecha: {FechaHora:dd/MM/yyyy HH:mm}, Válido: {EsValido}, " +
               $"Tipo: {TipoTarjeta}, Saldo: ${SaldoRestante}, ID: {IdTarjeta}";

        if (EsTrasbordo)
            infoBase += ", TRASBORDO";

        return infoBase;
    }

    public string ObtenerInformacionCompleta()
    {
        string info = $"=== BOLETO DETALLADO ===\n" +
                     $"Línea: {LineaColectivo}\n" +
                     $"Fecha: {FechaHora:dd/MM/yyyy HH:mm}\n" +
                     $"Tipo Tarjeta: {TipoTarjeta}\n" +
                     $"ID Tarjeta: {IdTarjeta}\n" +
                     $"Monto Viaje: ${Monto}\n" +
                     $"Saldo Restante: ${SaldoRestante}\n" +
                     $"Estado: {(EsValido ? "VÁLIDO" : "INVÁLIDO")}";

        if (EsTrasbordo)
        {
            info += $"\nTipo: TRASBORDO";
            if (IdBoletoOrigenTrasbordo.HasValue)
                info += $"\nID Boleto Origen: {IdBoletoOrigenTrasbordo.Value}";
        }

        if (TotalAbonado > 0)
            info += $"\nTotal Abonado: ${TotalAbonado}";

        if (MontoRecargaSaldoNegativo > 0)
            info += $"\nRecarga Saldo Negativo: ${MontoRecargaSaldoNegativo}";

        return info;
    }
}