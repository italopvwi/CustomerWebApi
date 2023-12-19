using System.Text;
using Newtonsoft.Json;
using Serilog;

namespace Customer.Tester;

public class CustomerTester
{
    private readonly string _uri;

    private readonly string[] _firstNames = new[]
    {
        "Leia",
        "Sadie",
        "Jose",
        "Sara",
        "Frank",
        "Dewey",
        "Tomas",
        "Joel",
        "Lukas",
        "Carlos"
    };

    private readonly string[] _lastNames = new[]
    {
        "Liberty",
        "Ray",
        "Harrison",
        "Ronan",
        "Drew",
        "Powell",
        "Larsen",
        "Chan",
        "Anderson",
        "Lane"
    };

    private readonly HttpClient _httpClient;

    private readonly Random _random = new();

    public CustomerTester(string uri)
    {
        _uri = uri;
        _httpClient = new()
        {
            BaseAddress = new Uri(uri)
        };
    }

    private int lastId = 1;

    private async Task<bool> PostCustomers()
    {
        try
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(
                    JsonConvert.SerializeObject(
                        GetTwoCustomers(),
                        Formatting.None),
                    Encoding.UTF8,
                    "application/json"),
                RequestUri = new Uri($"{_uri}/customers")
            };

            var response = await _httpClient.SendAsync(httpRequestMessage);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            Log.Error("Error occurred while trying to send new Customers: {responseBody}", responseBody);
            return false;

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while trying to send new Customers");
            return false;
        }
    }

    public Task StartAsync()
    {
        return Task.WhenAll(StartParallelPosting(), StartParallelGetting());
    }

    private Task StartParallelPosting()
    {
        return Parallel.ForEachAsync(
            Enumerable.Range(0, 200), 
            new ParallelOptions{MaxDegreeOfParallelism = 10},
            async (i, token) =>
            {
                await PostCustomers();
            });
    }

    private Task StartParallelGetting()
    {
        return Parallel.ForEachAsync(
            Enumerable.Range(0, 300),
            new ParallelOptions { MaxDegreeOfParallelism = 10 },
            async (i, token) =>
            {
                await GetCustomers();
            });
    }

    private async Task<IEnumerable<Core.Models.Customer>> GetCustomers()
    {
        try
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_uri}/customers")
            };

            var response = await _httpClient.SendAsync(httpRequestMessage);
            var responseBody = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Core.Models.Customer[]>(responseBody);
            }

            
            Log.Error("Error occurred while trying to get Customers: {responseBody}", responseBody);
            return Enumerable.Empty<Core.Models.Customer>();

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while trying to get Customers");
            return Enumerable.Empty<Core.Models.Customer>();
        }
    }

    private Core.Models.Customer[] GetTwoCustomers()
    {
        return new[]
        {
            new Core.Models.Customer
            {
                id = Interlocked.Add(ref lastId, 1),
                lastName = _lastNames[_random.Next(0, _lastNames.Length - 1)],
                firstName = _firstNames[_random.Next(0, _firstNames.Length - 1)],
                age = _random.Next(10, 90)
            },
            new Core.Models.Customer
            {
                id = Interlocked.Add(ref lastId, 1),
                lastName = _lastNames[_random.Next(0, _lastNames.Length - 1)],
                firstName = _firstNames[_random.Next(0, _firstNames.Length - 1)],
                age = _random.Next(10, 90)
            }
        };
    }
}