using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http; 

namespace MauiAppTempoAgora.Services
{
    public class DataService 
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            string chave = "6135072afe7f6cec1537d5cb08a5a1a2";
            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                         $"q={cidade}&units=metric&appid={chave}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage resp = await client.GetAsync(url);

                    if (resp.IsSuccessStatusCode)
                    {
                        string json = await resp.Content.ReadAsStringAsync();
                        var rascunho = JObject.Parse(json);

                        DateTime time = new();
                        DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                        DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                        t = new()
                        {
                            lat = (double)rascunho["coord"]["lat"],
                            lon = (double)rascunho["coord"]["lon"],
                            description = (string)rascunho["weather"][0]["description"],
                            main = (string)rascunho["weather"][0]["main"],
                            temp_min = (double)rascunho["main"]["temp_min"],
                            temp_max = (double)rascunho["main"]["temp_max"],
                            speed = (double)rascunho["wind"]["speed"],
                            visibility = (int)rascunho["visibility"],
                            sunrise = sunrise.ToString(),
                            sunset = sunset.ToString(),
                        };

                 
                        string descricaoClima = $"Clima: {t.description}, " +
                                                $"Temperatura: {t.temp_min}°C a {t.temp_max}°C, " +
                                                $"Vento: {t.speed} m/s, " +
                                                $"Visibilidade: {t.visibility} metros.";

                        Console.WriteLine(descricaoClima);
                    }
                    else
                    {
                        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            Console.WriteLine("Cidade não encontrada. Por favor, verifique o nome.");
                        }
                        else
                        {
                            Console.WriteLine($"Erro ao buscar os dados. Status: {resp.StatusCode}");
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                  
                    Console.WriteLine("Erro de conexão. Verifique sua conexão com a internet.");
                }
                catch (Exception ex)
                {
                  
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                }
            }

            return t;
        }
    }
}
