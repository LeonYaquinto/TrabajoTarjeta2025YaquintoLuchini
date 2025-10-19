public class Colectivo
{
    private string linea;
    private string empresa;
    private const decimal TARIFA_BASICA = 1580m;

    public Colectivo(string linea, string empresa)
    {
        this.linea = linea;
        this.empresa = empresa;
    }

    public Boleto PagarCon(Tarjeta tarjeta)
    {
        if (tarjeta == null)
            return null;

        if (tarjeta.Saldo < TARIFA_BASICA)
            return null;

        if (tarjeta.PagarPasaje())
        {
            return new Boleto(TARIFA_BASICA, linea, empresa, tarjeta.Saldo);
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