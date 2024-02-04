import React, { useEffect, useState } from "react";
import RecipeGrid from "../../Components/recipeGrid"; // import works like this

export const Home = () => {
  const [recipes, setRecipes] = useState([]);
  const [reviews, setReviews] = useState([]);
  const [images, setImages] = useState([]); // not in use yet
  const [testimages, setTestImages] = useState([]);

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
      try {
        const response1 = await fetch("http://localhost:3004/recipe");
        const response2 = await fetch("http://localhost:3004/review");
        const response3 = await fetch("http://localhost:3004/image");
        const data1 = await response1.json(); // recipes
        const data2 = await response2.json(); // reviews
        const data3 = await response3.json(); // images
        setRecipes(data1);
        setReviews(data2);
        setImages(data3);
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
      {/* Render your recipes here */}
      <RecipeGrid recipes={recipes} reviews={reviews} images={testimages} />
    </div>
  );
};
