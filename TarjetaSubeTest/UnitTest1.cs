using Xunit;

public class UnitTest1
{
    [Fact]
    public void Tarjeta_SeCreaConSaldoCero()
    {
        var tarjeta = new Tarjeta();
        
        Assert.Equal(0, tarjeta.Saldo);
    }
    
    [Fact]
    public void Tarjeta_CargaExitosa()
    {
        var tarjeta = new Tarjeta();
        
        bool resultado = tarjeta.Cargar(2000);
        
        Assert.True(resultado);
        Assert.Equal(2000, tarjeta.Saldo);
    }
    
    [Fact]
    public void Colectivo_PagarConTarjetaConSaldo_RetornaBoleto()
    {
        var tarjeta = new Tarjeta();
        tarjeta.Cargar(2000);
        var colectivo = new Colectivo("132", "Rosario Bus");
        
        var boleto = colectivo.PagarCon(tarjeta);
        
        Assert.NotNull(boleto);
        Assert.Equal(1580, boleto.MontoPagado);
    }
}