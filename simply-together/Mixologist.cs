using System;
using System.Reflection;
using Newtonsoft.Json;
using RestSharp;
using simply_together.Models;
using simply_together;


namespace simply_together
{
    public class Mixologist
    {
     
        /// <summary>
        /// Method <c>GetDrinkTypes</c> calls the external drinks API and makes a list of drink categories.
        /// It then passes the list to TableVisualisation class to be displayed in the console.
        /// </summary>
        internal void GetDrinkTypes()
        {
            var client = new RestClient("http://www.thecocktaildb.com/api/json/v1/1");
            var request = new RestRequest("list.php?c=list");
            var response = client.ExecuteAsync(request);

            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string rawResponse = response.Result.Content;
                var serialize = JsonConvert.DeserializeObject<DrinkTypes>(rawResponse);

            List<DrinkType> returnedList = serialize.DrinkTypesList;

            TableVisualisation.DisplayTable(returnedList, "Drinks Menu");

                
            }
        }

        /// <summary>
        /// Method <c>GetDrinksByType</c> calls the external drinks API and makes a list of drinks in the chosen category.
        /// It then passes the list to TableVisualisation class to be displayed in the console.
        /// </summary>
        internal void GetDrinksByType(string? drinkType)
        {
            var client = new RestClient("http://www.thecocktaildb.com/api/json/v1/1/");
            var request = new RestRequest($"filter.php?c={System.Web.HttpUtility.UrlEncode(drinkType)}");
            var response = client.ExecuteAsync(request);

            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string rawResponse = response.Result.Content;
                var serialize = JsonConvert.DeserializeObject<Drinks>(rawResponse);
                List<Drink> returnedList = serialize.DrinksList;

                TableVisualisation.DisplayTable(returnedList, "Drinks Selection");
                
            }
        }

        /// <summary>
        /// Method <c>GetDrinke</c> calls the external drinks API and gets the recipe for the selected drink. 
        /// It formats the data into a more user friendly style.
        /// It calls the UserInput method for the user to choose the date idea.
        /// </summary>
        public void GetDrink(string? drink)
        {
           UserInput userInput = new();
          

            var client = new RestClient("http://www.thecocktaildb.com/api/json/v1/1/");
            var request = new RestRequest($"lookup.php?i={drink}");
            var response = client.ExecuteAsync(request);

            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                
                string rawResponse = response.Result.Content;
                
                var serialize = JsonConvert.DeserializeObject<DrinkRecipeObject>(rawResponse);
                List<DrinkRecipe> returnedList = serialize.DrinkRecipeList;
                DrinkRecipe drinkRecipe = returnedList[0];
                List<object> prepList = new();
                string formattedName = "";

                foreach (PropertyInfo prop in drinkRecipe.GetType().GetProperties())
                {
                    if (prop.Name.Contains ("str"))
                    {
                        formattedName = prop.Name.Substring(3);
                    }
                    if (!string.IsNullOrEmpty(prop.GetValue(drinkRecipe)?.ToString()))
                    {
                        prepList.Add(new
                        {
                            Key = formattedName,
                            Value = prop.GetValue(drinkRecipe)
                        });
                    }
                }

                TableVisualisation.DisplayTable(prepList, drinkRecipe.strDrink);
                
                
            }
            
        
            userInput.GetActivityInput();
        }


    }
}
