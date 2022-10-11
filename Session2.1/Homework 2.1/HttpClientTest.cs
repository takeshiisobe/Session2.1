using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: Parallelize(Workers = 10, Scope = ExecutionScope.MethodLevel)]
namespace Homework_2._1
{
    [TestClass]
    public class HttpClientTest
    {
        private static HttpClient httpClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetEndpoint = "pet";

        private static string GetURL(string enpoint) => $"{BaseURL}{enpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        [TestInitialize]
        public void TestInitialize()
        {
            httpClient = new HttpClient();
        }

        [TestCleanup]
        public async Task TestCleanUp()
        {
            foreach (var data in cleanUpList)
            {
               var httpResponse = await httpClient.DeleteAsync(GetURL($"{PetEndpoint}/{data.Id}"));
            }
        }

        [TestMethod]
        public async Task PutMethod()
        {
            #region create data

            // Create Json Object
            PetModel petData = new PetModel()
            {
                Id = 998,
                Category = new Category { Id = 9999, Name = "Homework" },
                Name = "Takeshi",
                PhotoUrls = new string[] { "https://www.rd.com/wp-content/uploads/2021/01/GettyImages-588935825.jpg" },
                Tags = new Category[] { new Category { Id = 9999, Name = "Update" } },
                Status = "available"
            };

            // Serialize Content
            var request = JsonConvert.SerializeObject(petData);
            var postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Post Request
            await httpClient.PutAsync(GetURL(PetEndpoint), postRequest);

            #endregion

            #region get data

            // Send Request
            var httpResponse = await httpClient.GetAsync(GetURI($"{PetEndpoint}/{petData.Id}"));

            // Get Content
            var httpResponseMessage = httpResponse.Content;

            // Get Status Code
            var statusCode = httpResponse.StatusCode;

            // Deserialize Content
            var listpetData = JsonConvert.DeserializeObject<PetModel>(httpResponseMessage.ReadAsStringAsync().Result);

            #endregion

            #region cleanupdata

            // Add data to cleanup list
            cleanUpList.Add(listpetData);

            #endregion

            #region assertion

            // Assertion
            Assert.AreEqual(HttpStatusCode.OK, statusCode, "Status code is equal to 200");
            Assert.IsTrue(listpetData.Id == petData.Id);
            Assert.AreEqual(listpetData.Name, petData.Name);

    
            #endregion
        }
    }

}
