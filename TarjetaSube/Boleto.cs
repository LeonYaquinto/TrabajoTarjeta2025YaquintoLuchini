using System;
public class Boleto
{
    private decimal montoPagado;
    private string lineaColectivo;
    private string empresa;
    private decimal saldoRestante;
    private DateTime fechaHora;

    public Boleto(decimal montoPagado, string lineaColectivo, string empresa, decimal saldoRestante)
    {
        this.montoPagado = montoPagado;
        this.lineaColectivo = lineaColectivo;
        this.empresa = empresa;
        this.saldoRestante = saldoRestante;
        this.fechaHora = DateTime.Now;
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
}