import React, { useEffect, useState } from "react";
import RecipeGrid from "../../Components/recipeGrid";
import { useAuth } from "../Authentication";

export const Home = () => {
  const [recipes, setRecipes] = useState([]);
  const [loading, setLoading] = useState(true); // loading state for images
  const { user: authUser } = useAuth();

  // fetch data from database, all queries can be done in one fetch. This is done when site loads or is refreshed
  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      try {
        const response = await fetch(
          "https://localhost:7005/api/recipe/"//"https://recipeappapi.azurewebsites.net/api/recipe/"
        );
        const data = await response.json();
        const filteredData = data.filter(recipe => recipe.visibility === true); // Only show visible recipes
        
        if (authUser){
          setRecipes(data);
          setLoading(false);
          console.log("logged user: Show all recipes", data);
        
        }
        else {
          setRecipes(filteredData);
          setLoading(false);  
          console.log("not logged user filtered recipes", filteredData);
        }
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    };
    fetchData();
  }, []);

  // Arrange recipes by rating
  const sortedRecipes = recipes.sort((a, b) => b.rating - a.rating);

  // Take 5 best recipes
  const topFiveRecipes = sortedRecipes.slice(0, 5);

  return (
    <div style={{ textAlign: "center" }}>
      <h1>Welcome to the recipe app</h1>
      <p>
        Here you can browse through our recipes and find something to cook for
        dinner tonight!
      </p>
      {loading ? (
        <p style={{ fontSize: "48px" }}>Loading recipes..</p>
      ) : (
        <RecipeGrid recipes={topFiveRecipes} />
      )}
    </div>
  );
};
