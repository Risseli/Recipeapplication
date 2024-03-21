import React, { useEffect, useState } from "react";
import RecipeGrid from "../../Components/recipeGrid"; // This is used in Home and Recipes Pages

export const Recipes = () => {
  const [recipes, setRecipes] = useState([]);
  const [originalRecipes, setOriginalRecipes] = useState([]); // Alkuperäiset reseptit tallennetaan tähän
  const [loading, setLoading] = useState(true);
  const [ingredient, setIngredient] = useState("");
  const [noResults, setNoResults] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const response = await fetch(
        //"https://recipeappapi.azurewebsites.net/api/recipe/"
        "https://localhost:7005/api/recipe/"
      );
      const data = await response.json();
      setRecipes(data);
      setOriginalRecipes(data); // Tallenna alkuperäiset reseptit
      setLoading(false);
    } catch (error) {
      console.error("Error fetching data:", error);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleSubmit = (e) => {
    e.preventDefault();
    setLoading(true);
    findRecipes();
  };

  const findRecipes = () => {
    if (!ingredient) {
      setRecipes(originalRecipes); // Palauta alkuperäiset reseptit
      setLoading(false);
      return;
    }

    const filteredRecipes = originalRecipes.filter((recipe) => {
      return recipe.ingredients.some((ingredientObj) => {
        return ingredientObj.name
          .toLowerCase()
          .includes(ingredient.toLowerCase());
      });
    });
    if (filteredRecipes.length === 0) {
      setNoResults(true); // Aseta tila hakutulosten puuttumiselle
    } else {
      setNoResults(false);
    }
    console.log("Filtered Recipes:", filteredRecipes);
    setRecipes(filteredRecipes);
    setLoading(false);
  };

  return (
    <div style={{ textAlign: "center" }}>
      <h1>Welcome to the recipe app</h1>
      <p>
        Here you can browse through our recipes and find something to cook for
        dinner tonight!
      </p>
      <br/>
      <br/>

      <form onSubmit={handleSubmit}>
        <label></label>
        <input style={{ borderRadius: "4px"}}
          type="text"
          placeholder="Search with ingredient.."
          value={ingredient}
          onChange={(e) => setIngredient(e.target.value)}
        ></input>
        <button type="submit" style={{marginLeft : "10px"}}>Search</button>
      </form>

      {loading ? (
        <p style={{ fontSize: "48px" }}>Loading recipes..</p>
      ) : noResults ? (
        <p style={{ fontSize: "24px" }}>
          Sorry, couldn't find recipes containing that ingredient. Try something
          else.
        </p>
      ) : (
        <RecipeGrid recipes={recipes} />
      )}
    </div>
  );
};
