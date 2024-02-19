import React, { useEffect, useState } from "react";
import RecipeGrid from "../../Components/recipeGrid"; // import works like this

export const Home = () => {
  const [recipes, setRecipes] = useState([]);
  const [reviews, setReviews] = useState([]);
  const [testimages, setTestImages] = useState([]);
  const [loading, setLoading] = useState(true); // loading state for images

  // test set until we have images working properly, images in projects public folder
  useEffect(() => {
    setTestImages([
      "image1.jpg",
      "image2.jpg",
      "image3.jpg",
      "image4.jpg",
      "image5.jpg",
    ]);
  }, []);

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

  return (
    <div style={{ textAlign: "center" }}>
      <h1>Welcome to the recipe app</h1>
      <p>
        Here you can browse through our recipes and find something to cook for
        dinner tonight!
      </p>

      {console.log("Home", reviews)}

      {/* Render your recipes here */}
      { loading ? <p style={{fontSize : "48px"}}>Loading recepies..</p> :
      <RecipeGrid recipes={recipes} />
      };
  
    </div>
  );
};
