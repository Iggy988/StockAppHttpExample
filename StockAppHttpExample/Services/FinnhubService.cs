using StockAppHttpExample.ServiceContracts;
using System.Text.Json;
namespace StockAppHttpExample.Services;

public class FinnhubService : IFinnhubService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public FinnhubService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    

    public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
    {
        //moramo staviti u using da se zatvori konekcija nakon koristenja
        using (HttpClient httpClient = _httpClientFactory.CreateClient())
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
            {
                // nije dozvoljen string, moramo prebaciti u Uri
                //stavili smo token u secrets
                RequestUri = new Uri($"https://finnhub.io/api/v1/quote?symbol={stockSymbol}&token={_configuration["FinnhubToken"]}"),
                //drugi element je mrthod (get, post, put..)
                Method = HttpMethod.Get
            };

            //da posaljemo/primimo message
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            // primamo stram data
            Stream stream = httpResponseMessage.Content.ReadAsStream();
            //da citamo stram koristimo StreamReader
            StreamReader streamReader = new StreamReader(stream);
            //moramo konvertovati stream u string
            string response = streamReader.ReadToEnd();

            //convert from json to c# object(dictionary)
            Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

            if (responseDictionary == null)
            {
                throw new InvalidOperationException("No response from finhub server");
            }

            if (responseDictionary.ContainsKey("error"))
            {
                throw new InvalidOperationException(Convert.ToString(responseDictionary["error"]));
            }
            return responseDictionary;
        }
    }
}
