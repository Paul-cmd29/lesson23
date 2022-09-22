using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HttpClientSample
{
    public class Product
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Category { get; set; }
    }

    class Program
    {
            static HttpClient client = new HttpClient();
            static void Main(string[] args)
            {
                RunAsync().GetAwaiter().GetResult();
            }

        

            static void ShowProduct(Product product)
            {
                Console.WriteLine($"Name: {product.Name}\tPrice: " +
                    $"{product.Price}\tCategory: {product.Category}");
            }

            static async Task<Uri> CreateProductAsync(Product product)
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(
                    "api/products", product);
                response.EnsureSuccessStatusCode();

                
                return response.Headers.Location;
            }

            static async Task<Product> GetProductAsync(string path)
            {
                Product product = null;
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    product = await response.Content.ReadAsAsync<Product>();
                }
                return product;
            }

            static async Task<Product> UpdateProductAsync(Product product)
            {
                HttpResponseMessage response = await client.PutAsJsonAsync(
                    $"api/products/{product.Id}", product);
                response.EnsureSuccessStatusCode();

               
                product = await response.Content.ReadAsAsync<Product>();
                return product;
            }

            static async Task<HttpStatusCode> DeleteProductAsync(string id)
            {
                HttpResponseMessage response = await client.DeleteAsync(
                    $"api/products/{id}");
                return response.StatusCode;
            }

           

            static async Task RunAsync()
            {
               
                client.BaseAddress = new Uri("http://localhost:64195/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    
                    Product product = new Product
                    {
                        Name = "Gizmo",
                        Price = 100,
                        Category = "Widgets"
                    };

                    var url = await CreateProductAsync(product);
                    Console.WriteLine($"Created at {url}");

                   
                    product = await GetProductAsync(url.PathAndQuery);
                    ShowProduct(product);

                    
                    Console.WriteLine("Updating price...");
                    product.Price = 80;
                    await UpdateProductAsync(product);

                    
                    product = await GetProductAsync(url.PathAndQuery);
                    ShowProduct(product);

                   
                    var statusCode = await DeleteProductAsync(product.Id);
                    Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.ReadLine();
            }
        
    }
}

