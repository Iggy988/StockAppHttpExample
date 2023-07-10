namespace StockAppHttpExample.ServiceContracts;

public interface IFinnhubService
{
    Task<Dictionary<string, object>?> GetStockPriceQuote(string stocSymbol);
}
