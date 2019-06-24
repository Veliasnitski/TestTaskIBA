using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebClient.Repositories;
using WebClient.Models;

namespace WebClient.Repositories
{
    internal class WebAPIRepository<T> : IRepository<T>
    {
        //send to appsetting.json
        private string localHostAPI =  "https://localhost:5001/api/";

        private string url;
        HttpClient client;
        public WebAPIRepository(string urlApi)
        {
            UrlAPI = urlApi;
            client = new HttpClient();
        }
        public string UrlAPI
        {
            get
            {
                return url;
            }
            set
            {
                url = localHostAPI + value;
            }
        }

        public async Task AddAsync(T item)
        {
            var tmpj = JsonConvert.SerializeObject(item);
            var content = new StringContent(tmpj.ToString(), Encoding.UTF8, "application/json");
            await client.PostAsync(UrlAPI, content);
        }

        public async Task DeleteAsync(int id)
        {
            await client.DeleteAsync(UrlAPI + "/" + id.ToString());
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            HttpResponseMessage response = await client.GetAsync(UrlAPI);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody == "") return default;
            List<T> deserializedList = JsonConvert.DeserializeObject<List<T>>(responseBody).ToList();
            return deserializedList;
        }

        public async Task<T> GetAsync(string path)
        {
            string fullPath = (UrlAPI + "/" + path).TrimEnd('/');
            HttpResponseMessage response = await client.GetAsync(fullPath);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody == "") return default;
            T deserializedItem = JsonConvert.DeserializeObject<T>(responseBody);

            return deserializedItem;
        }

        public async Task UpdateAsync(T item)
        {
            var tmpj = JsonConvert.SerializeObject(item);
            var content = new StringContent(tmpj.ToString(), Encoding.UTF8, "application/json");
            await client.PutAsync(UrlAPI, content);
        }

    }
}
