using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TarkovPriceViewer
{
    public class TarkovMarketApi
    {
        private static readonly String apiKey = ""; // !!! replace this by your api key
        private static readonly HttpClient Client = InitializeNewApiClient(apiKey);
        private static readonly JsonSerializerOptions BsgJsonOptions = new JsonSerializerOptions() { PropertyNamingPolicy = new BsgMarketItem.NamingPolicy(), Converters = { new DictionaryStringObjectJsonConverter() } };

        private static HttpClient InitializeNewApiClient(String apiKey)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            return client;
        }

        static async Task FetchAndPrintAllBsgItemStats()
        {
            var allItems = await FetchAllBsgItems();
        }

        private static async Task<BsgMarketItem[]> FetchAllBsgItems()
            => (await JsonSerializer.DeserializeAsync<Dictionary<string, BsgMarketItem>>(await FetchStream(new Uri(MarketApiEndpoint, "bsg/items/all")), BsgJsonOptions))
                ?.Values.ToArray() ?? throw new Exception("Could not obtain items from BSG API");

        private static async Task<Stream> FetchStream(Uri uri)
        {
            var response = await Client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Could not fetch data from \"{uri}\". Reason: {response.StatusCode}");

            return await response.Content.ReadAsStreamAsync();
        }

        /*public record BsgMarketItem
        {
            public string Id { get; init; }
            public string Name { get; init; }
            public string Parent { get; init; }
            public string Type { get; init; }
            public Dictionary<string, object> Props { get; init; }
            public string Proto { get; init; }

            public class NamingPolicy : JsonNamingPolicy
            {
                public override string ConvertName(string name) => $"_{name.ToLower()}";
            }
        }*/
    }

    public class DictionaryStringObjectJsonConverter : JsonConverter<Dictionary<string, object>>
    {
        public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");

            var dictionary = new Dictionary<string, object>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return dictionary;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("JsonTokenType was not PropertyName");

                var propertyName = reader.GetString();

                if (string.IsNullOrWhiteSpace(propertyName))
                    throw new JsonException("Failed to get property name");

                reader.Read();

                dictionary.Add(propertyName, ExtractValue(ref reader, options));
            }

            return dictionary;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, options);

        private object ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.TryGetDateTime(out var date) ? date : reader.GetString(),
                JsonTokenType.False => false,
                JsonTokenType.True => true,
                JsonTokenType.Null => null,
                JsonTokenType.Number => reader.TryGetInt64(out var result) ? result : reader.GetDecimal(),
                JsonTokenType.StartObject => Read(ref reader, null, options),
                JsonTokenType.StartArray => ExtractList(ref reader),
                _ => throw new JsonException($"'{reader.TokenType}' is not supported")
            };

            List<object> ExtractList(ref Utf8JsonReader r)
            {
                var list = new List<object>();

                while (r.Read() && r.TokenType != JsonTokenType.EndArray)
                    list.Add(ExtractValue(ref r, options));

                return list;
            }
        }
    }
}
