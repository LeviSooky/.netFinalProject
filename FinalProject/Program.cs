using System.Text.Encodings.Web;
using System.Text.Json;using FinalProject;

Console.WriteLine("Ez a program egy txt file sorait olvassa ki, a megadott prefix alapján kiszűri azokat és rendezi az eredményt!");
Console.WriteLine("Kérlek add meg a file elérési útvonalát amiből be szeretnéd olvasni a sorokat!");
var fileName = Console.ReadLine();
Console.WriteLine("add meg a prefixet, ami alapján a hős nevét szürjük!");
var prefix = Console.ReadLine();
Console.WriteLine("add meg a minimum kacsaerőt, ami alapján szürjük a hősöket, egész integer szám!");
var minDuckPowerInput = Console.ReadLine();
int minDuckPower;
try
{
    minDuckPower = int.Parse(minDuckPowerInput);
}
catch (Exception ex) when (ex is FormatException or ArgumentNullException or OverflowException)
{
    Console.WriteLine($"Hibás kascsaerő input: ${minDuckPowerInput} hiba: ${ex.Message}");
    return;
}

var heroes = new List<Hero>();
using (var file = File.OpenRead(fileName))
{
    using (var reader = new StreamReader(file))
    {
        try
        {
            while (await reader.ReadLineAsync() is { } line)
            {
                var values = line.Split(";");
                if (values.Length != 4)
                {
                    Console.Error.WriteLine($"Hibás adat, a sorban több/kevesebb érték van a szükségesnél: ${line}");
                    return;
                }
                heroes.Add(Parse(values));
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Hiba történt: ${ex.Message}");
        }
    }
}

var sortedHeroes = heroes.FindAll(hero => hero.HeroName.StartsWith(prefix))
    .FindAll(hero => hero.DuckStrength >= minDuckPower)
    .OrderBy(hero => -hero.DuckStrength);


var result =  JsonSerializer.Serialize(sortedHeroes, new JsonSerializerOptions()
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
});

Console.WriteLine(result);

static Hero Parse(string[] values)
{
    return new Hero
    {
        HeroName = values[0],
        RealName = values[1],
        SuperPower = values[2],
        DuckStrength = int.Parse(values[3])
    };
}
