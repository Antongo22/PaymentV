using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaymentV.SBP_API
{
    public class PaymentApiService
    {
        private static PaymentApiService _instance;
        private readonly HttpClient _httpClient;

        private PaymentApiService()
        {
            _httpClient = new HttpClient();
            DotNetEnv.Env.Load();
            string secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");
        }

        public static PaymentApiService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PaymentApiService();
                }
                return _instance;
            }
        }

        public async Task<string> GetOrderDataAsync(string orderId)
        {
            try
            {
                string apiUrl = $"https://pay-test.raif.ru/api/payment/v1/orders/{orderId}";


                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                JObject jsonResponse = JObject.Parse(responseBody);

                string orderQR = jsonResponse?["qr"]?["id"]?.ToString();

                apiUrl = $"https://pay-test.raif.ru/api/sbp/v2/qrs/{orderQR}";

                response = await _httpClient.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                responseBody = await response.Content.ReadAsStringAsync();

                jsonResponse = JObject.Parse(responseBody);

                string orderStatus = jsonResponse["qrStatus"].ToString();

                return orderStatus;
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
