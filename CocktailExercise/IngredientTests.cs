using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CocktailExercise
{
    [TestClass]
    public class IngredientTests
    {
        private RestClient client;
        private readonly string baseUrl = "https://www.thecocktaildb.com/api/json/v1/1/";

        [TestInitialize]
        public void Setup()
        {
            client = new RestClient(baseUrl);
        }

        /// <summary>
        /// Verify that searching for an existing ingredient returns expected fields
        /// </summary>

        [TestMethod]
        public void SearchIngredients_ExistingIngredient_ReturnsExpectedFields()
        {
            // Arrange
            var request = new RestRequest("search.php");
            request.AddParameter("i", "vodka");

            //API Call
            var response = client.Get(request);

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsNotNull(response.Content);

            // Parse JSON response
            JObject jsonResponse = JObject.Parse(response.Content);

            // Assert expected fields
            JArray ingredientsArray = (JArray)jsonResponse["ingredients"];
            Assert.IsNotNull(ingredientsArray);

            foreach (var ingredient in ingredientsArray)
            {
                Assert.IsNotNull(ingredient["idIngredient"]);
                Assert.IsNotNull(ingredient["strIngredient"]);
                Assert.IsNotNull(ingredient["strDescription"]);
                Assert.IsNotNull(ingredient["strType"]);

                // Check Alcohol and ABV fields
                var isAlcoholic = (string)ingredient["strAlcohol"];
                var abv = (string)ingredient["strABV"];
                if (string.IsNullOrEmpty(isAlcoholic) || isAlcoholic.ToLower() == "no")
                {
                    // If non-alcoholic, Alcohol and ABV are null
                    Assert.IsNull(isAlcoholic);
                    Assert.IsNull(abv);
                }
                else
                {
                    // If alcoholic, ABV is not null
                    Assert.IsNotNull(abv);
                }
            }
        }

        /// <summary>
        /// Verify that searching for ingredients is case-insensitive.
        /// </summary>

        [TestMethod]
        public void SearchIngridients_CaseInsensitive_ReturnsExpectedResults()
        {
            // Arrange
            var request = new RestRequest("search.php");
            request.AddParameter("i", "vOdKA");

            //API Call
            var response = client.Get(request);

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsNotNull(response.Content);

            // Parse JSON response
            JObject jsonResponse = JObject.Parse(response.Content);

            // Assert expected fields
            JArray ingredientsArray = (JArray)jsonResponse["ingredients"];
            Assert.IsNotNull(ingredientsArray);
        }

        /// <summary>
        /// Verify that searching for ingredients with a partial name returns expected results, e.g. vod  (for vodka).
        /// </summary>
        [TestMethod]
        public void SearchIngredients_PartialName_ReturnsExpectedResults()
        {
            // Arrange
            var request = new RestRequest("search.php");
            request.AddParameter("i", "vod");

            //API Call
            var response = client.Get(request);

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsNotNull(response.Content);

            // Parse JSON response
            JObject jsonResponse = JObject.Parse(response.Content);

            // Assert expected fields
            JArray ingredientsArray = (JArray)jsonResponse["ingredients"];
            Assert.IsNotNull(ingredientsArray);
        }

        /// <summary>
        /// Verify that searching for a non-existing ingredient returns null.
        /// </summary>

        [TestMethod]
        public void SearchIngredients_NonExistingIngredient_ReturnsNull()
        {
            // Arrange
            var request = new RestRequest("search.php");
            request.AddParameter("i", "testingingredient");

            //API Call
            var response = client.Get(request);

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.AreEqual("{\"ingredients\":null}", response.Content);
        }
    }
}
