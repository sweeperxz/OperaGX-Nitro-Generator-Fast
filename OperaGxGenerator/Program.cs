/*******************************************************************/

/* Developer: sweeperxz                                            */

/* Copyright: sweeperxz                                            */

/* Description: OperaGX-Nitro-Generator-Fast                       */

/*******************************************************************/

using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace OperaGxGenerator;

/// <summary>
///     Main program class responsible for configuring services, managing HTTP client, and orchestrating request
///     iterations.
/// </summary>
internal abstract class Program
{
    /// <summary>
    ///     Gets the HttpClient instance used for making HTTP requests to the Discord API.
    /// </summary>
    public static HttpClient Client { get; } =
        CreateHttpClient(false); // !!!!!!!!!!!!!!Set to true if you want to use a proxy

    /// <summary>
    ///     Semaphore for controlling access to shared resources.
    /// </summary>
    public static readonly SemaphoreSlim Semaphore = new(1, 1);

    /// <summary>
    ///     Counter for tracking the number of processed requests.
    /// </summary>
    public static int Counter;

    /// <summary>
    ///     Entry point of the program, configures services, takes user input, and executes asynchronous request iterations.
    /// </summary>
    private static async Task Main()
    {
        await using var serviceProvider = ConfigureServices();
        serviceProvider.GetService<RequestPerformer>();

        Console.WriteLine("Enter the number of requests per iteration:");
        var countRequestPerIteration = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter the number of iterations:");
        var iterations = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter the delay between requests in milliseconds:");
        var delay = int.Parse(Console.ReadLine()!);

        while (iterations-- > 0)
        {
            var tasks = new List<Task>(); // Move inside the loop

            for (var i = 0; i < countRequestPerIteration; i++) tasks.Add(RequestPerformer.PerformRequestAsync());

            await Task.WhenAll(tasks).ConfigureAwait(false);
            await Task.Delay(delay).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Configures services using Microsoft.Extensions.DependencyInjection.
    /// </summary>
    /// <returns>The configured service provider.</returns>
    private static ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<RequestPerformer>()
            .BuildServiceProvider();
    }

    /// <summary>
    ///     Creates an HttpClient with optional proxy configuration.
    /// </summary>
    /// <param name="useProxy">Specifies whether to use a proxy for HTTP requests.</param>
    /// <returns>The configured HttpClient instance.</returns>
    private static HttpClient CreateHttpClient(bool useProxy)
    {
        var handler = new HttpClientHandler();
        if (useProxy)
            handler.Proxy = new WebProxy("address:port", false)
            {
                Credentials = new NetworkCredential("username", "password")
            };

        return new HttpClient(handler);
    }
}

/// <summary>
///     Class responsible for performing asynchronous HTTP requests to the Discord API.
/// </summary>
internal class RequestPerformer
{
    /// <summary>
    ///     Performs an asynchronous HTTP request to the Discord API, processes the response, and writes the result to a file.
    /// </summary>
    public static async Task PerformRequestAsync()
    {
        var request = CreateRequest();

        try
        {
            using var response = await Program.Client.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var tokenResult = await ParseResponseAsync(response).ConfigureAwait(false);

            var split =
                "https://discord.com/billing/partner-promotions/1180231712274387115https://discord.com/billing/partner-promotions/1180231712274387115/" +
                tokenResult.token;

            Interlocked.Increment(ref Program.Counter);

            await Program.Semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                WriteToFile(split);
            }
            finally
            {
                Program.Semaphore.Release();
            }

            Console.WriteLine($"Done: {Program.Counter}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    ///     Creates an HTTP request message with the necessary headers and content for the Discord API.
    /// </summary>
    /// <returns>The configured HttpRequestMessage instance.</returns>
    private static HttpRequestMessage CreateRequest()
    {
        return new HttpRequestMessage(HttpMethod.Post,
            "https://api.discord.gx.games/v1/direct-fulfillment")
        {
            Content = new StringContent(
                $"{{\"partnerUserId\":\"{Guid.NewGuid().ToString()}\"}}", null,
                "application/json"),
            Headers =
            {
                { "sec-ch-ua", "\"Opera GX\";v=\"105\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"" },
                {
                    "user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 OPR/105.0.0.0"
                }
            }
        };
    }

    /// <summary>
    ///     Parses the asynchronous HTTP response content into a DiscordResponse object.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    /// <returns>The deserialized DiscordResponse object.</returns>
    private static async Task<DiscordResponse?> ParseResponseAsync(HttpResponseMessage response)
    {
        await using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        return await JsonSerializer.DeserializeAsync<DiscordResponse>(stream).ConfigureAwait(false);
    }

    /// <summary>
    ///     Writes data to a file, appending the information along with the current counter value.
    /// </summary>
    /// <param name="data">The data to write to the file.</param>
    private static void WriteToFile(string data)
    {
        using var streamWriter = new StreamWriter("nitro.txt", true);
        streamWriter.WriteLine(data);
    }

    /// <summary>
    ///     Represents the response structure from the Discord API.
    /// </summary>
    public record DiscordResponse(string token);
}