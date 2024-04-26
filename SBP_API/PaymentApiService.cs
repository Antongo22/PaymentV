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
        private readonly HttpClient _httpClient;

        public PaymentApiService()
        {
            _httpClient = new HttpClient();
            // Загрузка переменных окружения из файла .env
            DotNetEnv.Env.Load();
            // Получение значения переменной окружения SECRET_KEY
            string secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");
            Console.WriteLine($"SECRET_KEY: {secretKey}");
            // Добавляем заголовок Authorization с помощью Bearer токена
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");
        }

        public async Task<string> GetOrderDataAsync(string orderId)
        {
            try
            {
                // Формируем URL для запроса, подставляя значение orderId
                string apiUrl = $"https://pay-test.raif.ru/api/payment/v1/orders/{orderId}";

                await Console.Out.WriteLineAsync(apiUrl);

                // Выполняем GET запрос и получаем ответ
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                // Проверяем успешность запроса
                response.EnsureSuccessStatusCode();

                // Читаем ответ в виде строки
                string responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseBody);

                // Возвращаем полученные данные
                return responseBody;
            }
            catch (HttpRequestException ex)
            {
                // Обрабатываем ошибки HTTP запроса
                Console.WriteLine($"HTTP Error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Обрабатываем другие исключения
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}
