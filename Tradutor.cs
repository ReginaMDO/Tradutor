using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    // Credenciais
    private static readonly string subscriptionKey = "CHAVE";
    private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";
    private static readonly string region = "brazilsouth";

    static async Task Main(string[] args)
    {
        // URL do texto
        string url = "https://exemplo.com/texto";
        string texto = await ObterTextoDeUrl(url);
        
        // Tradução
        string textoTraduzido = await TraduzirTexto(texto, "pt");
        Console.WriteLine("Texto Traduzido: ");
        Console.WriteLine(textoTraduzido);
    }

    // Método para obter o texto de uma URL
    static async Task<string> ObterTextoDeUrl(string url)
    {
        using HttpClient client = new HttpClient();
        try
        {
            return await client.GetStringAsync(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter texto da URL: {ex.Message}");
            return string.Empty;
        }
    }

    // Método para traduzir o texto
    static async Task<string> TraduzirTexto(string texto, string idiomaDestino)
    {
        using HttpClient client = new HttpClient();
        
        // Configura as headers para a requisição
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", region);

        // Monta a URL para a API de tradução
        string url = $"{endpoint}translate?api-version=3.0&to={idiomaDestino}";

        // Cria o conteúdo da requisição em formato JSON
        var requestBody = new object[] { new { Text = texto } };
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            // Envia o pedido à API
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            // Processa a resposta JSON
            string responseBody = await response.Content.ReadAsStringAsync();
            var resultado = JsonSerializer.Deserialize<dynamic>(responseBody);

            // Retorna o texto traduzido
            return resultado[0]["translations"][0]["text"];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao traduzir texto: {ex.Message}");
            return string.Empty;
        }
    }
}
