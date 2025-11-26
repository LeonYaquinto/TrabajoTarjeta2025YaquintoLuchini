using System;

public class Colectivo
{
    private string linea;
    private string empresa;
    protected bool esInterurbano;
    private const decimal TARIFA_INTERURBANA = 3000m;

    public Colectivo(string linea, string empresa)
    {
        this.linea = linea;
        this.empresa = empresa;
        this.esInterurbano = false;
    }

    public Colectivo(string linea, string empresa, bool esInterurbano)
    {
        this.linea = linea;
        this.empresa = empresa;
        this.esInterurbano = esInterurbano;
    }

    public virtual Boleto PagarCon(Tarjeta tarjeta)
    {
        if (tarjeta == null)
            return null;

        // Verificar si la tarjeta puede pagar (considerando franquicias y horarios)
        if (!tarjeta.PuedePagarEnHorario())
            return null;

        // Pagar el pasaje (esto maneja trasbordos internamente)
        if (tarjeta.PagarPasaje(linea, esInterurbano))
        {
            // La tarifa cobrada está guardada en la tarjeta
            return new Boleto(tarjeta.UltimaTarifaCobrada, linea, empresa, tarjeta.Saldo, tarjeta.ObtenerTipo(), tarjeta.Id, tarjeta.EsTransbordo);
        }

        return null;
    }

    protected virtual decimal ObtenerTarifaColectivo(Tarjeta tarjeta)
    {
        if (esInterurbano)
        {
            return tarjeta.ObtenerTarifaInterurbana();
        }
        return tarjeta.ObtenerTarifa();
    }

    public string Linea
    {
        get { return linea; }
    }

    public string Empresa
    {
        get { return empresa; }
    }

    public bool EsInterurbano
    {
        get { return esInterurbano; }
    }
}
