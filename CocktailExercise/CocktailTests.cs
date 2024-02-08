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
    public class CocktailTests
    {
        private RestClient client;
        private readonly string baseUrl = "https://www.thecocktaildb.com/api/json/v1/1/";

        [TestInitialize]
        public void Setup()
        {
            client = new RestClient(baseUrl);
        }

        /// <summary>
        /// Verify that searching for an existing cocktail returns expected fields
        /// </summary>

        [TestMethod]
        public void SearchCocktails_ExistingCocktail_ReturnsExpectedFields()
        {
            // Arrange
            var request = new RestRequest("search.php");
            request.AddParameter("s", "margarita");

            //API Call
            var response = client.Get(request);

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsNotNull(response.Content);

            // Parse JSON response
            JObject jsonResponse = JObject.Parse(response.Content);

            // Assert expected fields
            JArray drinksArray = (JArray)jsonResponse["drinks"];
            Assert.IsNotNull(drinksArray);

            foreach (var drink in drinksArray)
            {
                //Ensure that all the Required fields are not null
                Assert.IsNotNull(drink["strDrink"]);
                Assert.IsNotNull(drink["strTags"]);
                Assert.IsNotNull(drink["strCategory"]);
                Assert.IsNotNull(drink["strAlcoholic"]);
                Assert.IsNotNull(drink["strGlass"]);

                // Ensure strInstructions exists
                Assert.IsNotNull(drink["strInstructions"]);

                Assert.IsNotNull(drink["strIngredient1"]);
                Assert.IsNotNull(drink["strMeasure1"]);

                Assert.IsNotNull(drink["strCreativeCommonsConfirmed"]);
                Assert.IsNotNull(drink["dateModified"]);
            }
        }

        /// <summary>
        /// Verify that searching for a non-existing cocktail returns null.
        /// </summary>
        [TestMethod]
        public void SearchCocktails_NonExistingCocktail_ReturnsNull()
        {
            // Arrange
            var request = new RestRequest("search.php");
            request.AddParameter("s", "testingcocktail");

            //API Call
            var response = client.Get(request);

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.AreEqual("{\"drinks\":null}", response.Content);
        }

        /// <summary>
        ///  Verify that searching for cocktails is case-insensitive.
        /// </summary>
        [TestMethod]
        public void SearchCocktails_CaseInsensitive_ReturnsExpectedResults()
        {
            // Arrange
            var request = new RestRequest("search.php");
            request.AddParameter("s", "MargArIta");

            //API Call
            var response = client.Get(request);

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsNotNull(response.Content);

            // Parse JSON response
            JObject jsonResponse = JObject.Parse(response.Content);

            // Assert expected fields
            JArray drinksArray = (JArray)jsonResponse["drinks"];
            Assert.IsNotNull(drinksArray);
        }

        /// <summary>
        /// Verify that searching for cocktails with a partial name returns expected results, e.g. marg (for margarita)
        /// </summary>
        [TestMethod]
        public void SearchCocktails_PartialName_ReturnsExpectedResults()
        {
            // Arrange
            var partialCocktailName = "margar"; // Partial name of the cocktail
            var request = new RestRequest("search.php");
            request.AddParameter("s", partialCocktailName);

            //API Call
            var response = client.Get(request);

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsNotNull(response.Content);

            // Parse JSON response
            JObject jsonResponse = JObject.Parse(response.Content);

            // Assert expected fields
            JArray drinksArray = (JArray)jsonResponse["drinks"];
            Assert.IsNotNull(drinksArray);

            // Ensure at least one drink is returned
            Assert.IsTrue(drinksArray.Count > 0);

            foreach (var drink in drinksArray)
            {
                // Assert that the name contains the partial name provided
                string strDrink = drink["strDrink"].ToString().ToLower(); // Convert to lower case for case-insensitive comparison
                Assert.IsTrue(strDrink.Contains(partialCocktailName.ToLower()));
            }
        }

        /// <summary>
        /// Verify that searching for cocktails with special characters in the name returns expected results. e.g. Piña Colada
        /// </summary>

        [TestMethod]
        public void SearchCocktails_WithSpecialCharacters_ReturnsExpectedResults()
        {
            // Arrange
            var specialCharacterName = "Piña Colada"; // Cocktail name with a special character
            var request = new RestRequest("search.php");
            request.AddParameter("s", specialCharacterName);

            //API Call
            var response = client.Get(request);

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsNotNull(response.Content);

            // Parse JSON response
            JObject jsonResponse = JObject.Parse(response.Content);

            // Assert expected fields
            JArray drinksArray = (JArray)jsonResponse["drinks"];
            Assert.IsNotNull(drinksArray);

            // Ensure at least one drink is returned
            Assert.IsTrue(drinksArray.Count > 0);
        }
    }
}
