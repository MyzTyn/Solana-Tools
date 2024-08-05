using Solnet.Wallet;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace TokenPriceTracker
{
    class Program
    {
        /// <summary>
        /// Represents the price data model from the Jupiter Price API.
        /// </summary>
        public struct PriceModel
        {
            // Token A
            [JsonPropertyName("id")]
            [JsonConverter(typeof(PublicKeyJsonConverter))]
            public PublicKey ID { get; set; }

            [JsonPropertyName("mintSymbol")]
            public string MintSymbol { get; set; }

            // Token B
            [JsonPropertyName("vsToken")]
            [JsonConverter(typeof(PublicKeyJsonConverter))]
            public PublicKey VsToken { get; set; }

            [JsonPropertyName("vsTokenSymbol")]
            public string VsTokenSymbol { get; set; }

            // Price
            [JsonPropertyName("price")]
            public decimal Price { get; set; }
        }

        /// <summary>
        /// Custom JSON converter for Solnet.Wallet.PublicKey.
        /// </summary>
        public class PublicKeyJsonConverter : JsonConverter<PublicKey>
        {
            public override PublicKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var stringValue = reader.GetString();
                return new PublicKey(stringValue);
            }

            public override void Write(Utf8JsonWriter writer, PublicKey value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.Key);
            }
        }
        
        /// <summary>
        /// The Basic Main Program for Token Price Tracker
        /// </summary>
        /// <param name="args"></param>
        private static async Task Main(string[] args)
        {
            // Get the Token Address (Wrapped Sol)
            PublicKey tokenAddress = new PublicKey("So11111111111111111111111111111111111111112");

            // Signal to cancel the process
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // Start the Token Price Tracker
            TokenPriceMonitor(tokenAddress, cancellationTokenSource.Token);

            // Wait for a key press to exit
            Console.ReadKey();

            // Cancel the Token Price Tracker
            cancellationTokenSource.Cancel();
        }
        
        /// <summary>
        /// The simple Token Price Monitor to keep track of the token's price
        /// </summary>
        /// <param name="tokenAddress"></param>
        /// <param name="cancellationToken"></param>
        private static async Task TokenPriceMonitor(PublicKey tokenAddress, CancellationToken cancellationToken)
        {
            // Setup the Http Client
            HttpClient client = new HttpClient();

            // Use Jupiter API (Price API) Get method
            // Link: https://docs.jup.ag/docs/apis/price-api
            // Example: https://price.jup.ag/v6/price?ids={token}
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://price.jup.ag/v6/price?ids={tokenAddress}"),
                Headers =
                {
                    { "accept", "application/json" },
                },
            };

            // Do the first fetch
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();

                JsonNode? jsonNode = JsonNode.Parse(body);

                // Skip if the jsonNode is null
                if (jsonNode != null)
                {
                    // Parse the data
                    PriceModel data = jsonNode["data"].AsObject().First().Value.Deserialize<PriceModel>();

                    Console.Clear();
                    Console.WriteLine($"{data.MintSymbol} to {data.VsTokenSymbol}: {data.Price}");
                }
            }

            // Refresh the Token Price every 30 seconds
            PeriodicTimer timer = new PeriodicTimer(new TimeSpan(0, 0, 30));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    string body = await response.Content.ReadAsStringAsync();

                    JsonNode? jsonNode = JsonNode.Parse(body);

                    // Skip if the jsonNode is null
                    if (jsonNode == null)
                        continue;

                    // Parse the data
                    PriceModel data = jsonNode["data"].AsObject().First().Value.Deserialize<PriceModel>();

                    Console.Clear();
                    Console.WriteLine($"{data.MintSymbol} to {data.VsTokenSymbol}: {data.Price}");
                }
            }
        }
    }
}