import React, { useEffect, useState } from "react";
import RecipeGrid from "../../Components/recipeGrid";

export const Home = () => {
  const [recipes, setRecipes] = useState([]);
  const [loading, setLoading] = useState(true); // loading state for images

  // fetch data from database, all queries can be done in one fetch. This is done when site loads or is refreshed
  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      try {
        const response = await fetch(
          "https://recipeappapi.azurewebsites.net/api/recipe/"
        );
        const data = await response.json();
        setRecipes(data);
        setLoading(false);
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
