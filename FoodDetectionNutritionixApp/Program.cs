using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string appId = "bb68607b";
        string appKey = "1f7e3fb199c3ab88c05cce0003c77ba6";

        Boolean quit = false;

        while (!quit)
        {
            Console.Write("Enter the ingredients detected by the model (or type quit to quit): ");
            string query = Console.ReadLine();

            if(query == "quit")
            {
                quit = true;
            }

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://trackapi.nutritionix.com/v2/");
            client.DefaultRequestHeaders.Add("x-app-id", appId);
            client.DefaultRequestHeaders.Add("x-app-key", appKey);

            var payload = new Dictionary<string, string>
            {
                { "query", query }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);

            var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("natural/nutrients", content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                var json = JsonDocument.Parse(responseBody);

                float totalCal = 0;
                float totalPro = 0;
                float totalCarb = 0;
                float totalSug = 0;
                float totalFat = 0;
                float totalFibre = 0;
                float totalSod = 0;

                var foodArray = json.RootElement.GetProperty("foods").EnumerateArray();
                foreach (var food in foodArray)
                {
                    var foodName = food.GetProperty("food_name").GetRawText();
                    var nfCalories = food.GetProperty("nf_calories").GetRawText();
                    var nfProtein = food.GetProperty("nf_protein").GetRawText();
                    var nfCarbohydrates = food.GetProperty("nf_total_carbohydrate").GetRawText();
                    var nfSugars = food.GetProperty("nf_sugars").GetRawText();
                    var nfFat = food.GetProperty("nf_total_fat").GetRawText();
                    var nfFibre = food.GetProperty("nf_dietary_fiber").GetRawText();
                    var nfSodium = food.GetProperty("nf_sodium").GetRawText();
                    var servingUnit = food.GetProperty("serving_unit").GetRawText();
                    var servingQuantity = food.GetProperty("serving_qty").GetRawText();

                    Console.WriteLine($"Food: {foodName}");
                    Console.WriteLine($"Calories: {nfCalories}kcal");
                    Console.WriteLine($"Protein: {nfProtein}g");
                    Console.WriteLine($"Carbohydrates: {nfCarbohydrates}g");
                    Console.WriteLine($"of which Sugars: {nfSugars}g");
                    Console.WriteLine($"Fat: {nfFat}g");
                    Console.WriteLine($"Fibre: {nfFibre}g");
                    Console.WriteLine($"Sodium: {nfSodium}mg");
                    Console.WriteLine($"Serving unit: {servingUnit}");
                    Console.WriteLine($"Quantity: {servingQuantity}");
                    Console.WriteLine();

                    if (nfCalories != "null")
                    {
                        totalCal = totalCal + float.Parse(nfCalories);
                    }
                    if (nfProtein != "null")
                    {
                        totalPro = totalPro + float.Parse(nfProtein);
                    }
                    if (nfCarbohydrates != "null")
                    {
                        totalCarb = totalCarb + float.Parse(nfCarbohydrates);
                    }
                    if (nfSugars != "null")
                    {
                        totalSug = totalSug + float.Parse(nfSugars);
                    }
                    if (nfFat != "null")
                    {
                        totalFat = totalFat + float.Parse(nfFat);
                    }
                    if (nfFibre != "null")
                    {
                        totalFibre = totalFibre + float.Parse(nfFibre);
                    }
                    if (nfSodium != "null")
                    {
                        totalSod = totalSod + float.Parse(nfSodium);
                    }
                }

                Console.WriteLine("Total nutrtional data for entire plate:");
                Console.WriteLine($"Calories: {totalCal}kcal");
                Console.WriteLine($"Protein: {totalPro}g");
                Console.WriteLine($"Carbohydrates: {totalCarb}g");
                Console.WriteLine($"of which Sugars: {totalSug}g");
                Console.WriteLine($"Fat: {totalFat}g");
                Console.WriteLine($"Fibre: {totalFibre}g");
                Console.WriteLine($"Sodium: {totalSod}mg");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }

            client.Dispose();

        }
    }
}
