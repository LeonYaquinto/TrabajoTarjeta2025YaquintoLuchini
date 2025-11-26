using System;

public class Boleto
{
    private decimal montoPagado;
    private string lineaColectivo;
    private string empresa;
    private decimal saldoRestante;
    private DateTime fechaHora;
    private string tipoTarjeta;
    private int idTarjeta;
    private bool esTransbordo;

    public Boleto(decimal montoPagado, string lineaColectivo, string empresa, decimal saldoRestante, string tipoTarjeta, int idTarjeta)
    {
        this.montoPagado = montoPagado;
        this.lineaColectivo = lineaColectivo;
        this.empresa = empresa;
        this.saldoRestante = saldoRestante;
        this.fechaHora = DateTime.Now;
        this.tipoTarjeta = tipoTarjeta;
        this.idTarjeta = idTarjeta;
        this.esTransbordo = false;
    }

    public Boleto(decimal montoPagado, string lineaColectivo, string empresa, decimal saldoRestante, string tipoTarjeta, int idTarjeta, bool esTransbordo)
    {
        this.montoPagado = montoPagado;
        this.lineaColectivo = lineaColectivo;
        this.empresa = empresa;
        this.saldoRestante = saldoRestante;
        this.fechaHora = DateTime.Now;
        this.tipoTarjeta = tipoTarjeta;
        this.idTarjeta = idTarjeta;
        this.esTransbordo = esTransbordo;
    }

    public decimal MontoPagado
    {
        get { return montoPagado; }
    }

    public string LineaColectivo
    {
        get { return lineaColectivo; }
    }

    public string Empresa
    {
        get { return empresa; }
    }

    public decimal SaldoRestante
    {
        get { return saldoRestante; }
    }

    public DateTime FechaHora
    {
        get { return fechaHora; }
    }

    public DateTime Fecha
    {
        get { return fechaHora.Date; }
    }

    public string TipoTarjeta
    {
        get { return tipoTarjeta; }
    }

    public decimal TotalAbonado
    {
        get { return montoPagado; }
    }

    public decimal Saldo
    {
        get { return saldoRestante; }
    }

    public int IdTarjeta
    {
        get { return idTarjeta; }
    }

    public bool EsTransbordo
    {
        get { return esTransbordo; }
    }

    public override string ToString()
    {
        string trasbordo = esTransbordo ? " (TRASBORDO)" : "";
        return $"Boleto{trasbordo} - Fecha: {fechaHora:dd/MM/yyyy HH:mm}\n" +
               $"Línea: {lineaColectivo} - Empresa: {empresa}\n" +
               $"Tipo de tarjeta: {tipoTarjeta} (ID: {idTarjeta})\n" +
               $"Total abonado: ${montoPagado}\n" +
               $"Saldo restante: ${saldoRestante}";
    }
}
