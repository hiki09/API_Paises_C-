using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json.Serialization;

class Program
{
    private static readonly string apiUrl = "https://restcountries.com/v3.1/all";

    private static async Task<string[]> FetchCountriesAsync()
    {
        using HttpClient client = new HttpClient();

        var response = await client.GetStringAsync(apiUrl);

        var countries = JsonSerializer.Deserialize<Country[]>(response);

        if (countries == null || countries.Length == 0)
        {
            throw new Exception("No se encontraron países en la respuesta.");
        }

        Console.WriteLine($"Cantidad de países deserializados: {countries.Length}");

        // Imprimir los primeros 5 países para revisar su estructura
        for (int i = 0; i < 5 && i < countries.Length; i++)
        {
            Console.WriteLine($"País {i + 1}: {JsonSerializer.Serialize(countries[i])}");
        }

        // Filtrar y extraer los nombres de los países
        var countryNames = countries
            .Where(country => country?.Name?.Common != null)
            .Select(country => country!.Name!.Common!)
            .ToArray();

        if (countryNames.Length == 0)
        {
            Console.WriteLine("No se encontraron nombres de países.");
        }

        return countryNames;
    }



    static async Task Main(string[] args)
    {
        try
        {
            // Consumir la API y obtener la lista de paises
            var countryNames = await FetchCountriesAsync();

            // Ordenar los paises por alfabeto
            var sortedCountries = countryNames.OrderBy(c => c).ToList();

            // Mostrar los países antes de guardarlos
            sortedCountries.ForEach(c => Console.WriteLine(c));

            // Obtener la ruta del escritorio 
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Crear el nombre del archivo con la fecha actual
            var fileName = $"{DateTime.Now:dd-MM-yyyy}_paises.txt";
            var filePath = Path.Combine(desktopPath, fileName);

            // Guardar la lista de paises en el archivo
            await File.WriteAllLinesAsync(filePath, sortedCountries);

            Console.WriteLine($"Archivo guardado exitosamente en: {filePath}");
        }
        catch (HttpRequestException e)
        {
            // Manejar errores de conexion en la API
            Console.WriteLine("Error al conectar con la API: " + e.Message);
        }
        catch (Exception e)
        {
            // Manejar cualquier otro error
            Console.WriteLine("Ha ocurrido un error: " + e.Message);
        }
    }




}

// Clase para representar el nombre del pais en el JSON
public class Country
{
    [JsonPropertyName("name")]
    public Name? Name { get; set; } //el signo ? indica que puede ser null
}

public class Name
{
    [JsonPropertyName("common")]
    public string? Common { get; set; } //el signo ?  permite nulls
}
