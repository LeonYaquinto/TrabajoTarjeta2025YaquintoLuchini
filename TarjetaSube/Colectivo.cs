public class Colectivo
{
    private string linea;
    private string empresa;

    public Colectivo(string linea, string empresa)
    {
        this.linea = linea;
        this.empresa = empresa;
    }

    public Boleto PagarCon(Tarjeta tarjeta)
    {
        if (tarjeta == null)
            return null;

        decimal tarifaAPagar = tarjeta.ObtenerTarifa();

        if (tarifaAPagar == 0)
        {
            if (tarjeta.PagarPasaje())
            {
                return new Boleto(tarifaAPagar, linea, empresa, tarjeta.Saldo, tarjeta.ObtenerTipo(), tarjeta.Id);
            }
            return null;
        }

        if (tarjeta.Saldo < tarifaAPagar)
            return null;

        if (tarjeta.PagarPasaje())
        {
            return new Boleto(tarifaAPagar, linea, empresa, tarjeta.Saldo, tarjeta.ObtenerTipo(), tarjeta.Id);
        }

        return null;
    }

    public string Linea
    {
        get { return linea; }
    }

    public string Empresa
    {
        get { return empresa; }
    }
}