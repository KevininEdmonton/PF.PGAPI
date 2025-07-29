using KS.Library.Interface.PFAPI.Domain;
using PFAPI.SupportModels;
using System.Text.Json;

namespace PFAPI.utility
{
    public static class DataHelper
    {
        public static List<KTopicModel> LoadKTopicsFromJson(string fileName = "testdata.json")
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", fileName);
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Seed file not found at: {filePath}");

                string jsonString = File.ReadAllText(filePath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<KTopicModel>? topics = JsonSerializer.Deserialize<List<KTopicModel>>(jsonString, options);

                return topics ?? new List<KTopicModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading KTopic data: {ex.Message}");
                return new List<KTopicModel>();
            }
        }

        public static List<HousewPrice> GetHouseList(string fileName = "housetestdata.json")
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", fileName);
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Seed file not found at: {filePath}");

                string jsonString = File.ReadAllText(filePath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<HousewPrice>? datalist = JsonSerializer.Deserialize<List<HousewPrice>>(jsonString, options);

                return datalist ?? new List<HousewPrice>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading KTopic data: {ex.Message}");
                return new List<HousewPrice>();
            }
        }
    }

}
