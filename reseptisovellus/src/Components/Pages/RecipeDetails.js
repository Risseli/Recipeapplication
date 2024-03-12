import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import "./RecipeDetails.css";
import { useAuth } from "../Authentication";

const RecipeDetails = () => {
  const [recipe, setRecipe] = useState(null);
  const [user, setUser] = useState(null);
  const { id } = useParams();
  const [printing, setPrinting] = useState(false);
  const { user: authUser } = useAuth();

  useEffect(() => {
    const fetchRecipeDetails = async () => {
      try {
        const response = await fetch(
          `https://recipeappapi.azurewebsites.net/api/recipe/${id}`
        );
        const response2 = await fetch(
          `https://recipeappapi.azurewebsites.net/api/user`
        );

        const data = await response.json(); // recipe
        const data2 = await response2.json(); // user
        setRecipe(data);
        setUser(data2);
        console.log(data);
        console.log("User: ", data2);
      } catch (error) {
        console.error("Error fetching recipe details:", error);
      }
    };
    fetchRecipeDetails();
  }, [id]);

  const likeRecipe = async () => {
    //something to like the recipe put method to database
    return;
  };

  // set recipe as favourite
  const setFavourite = async () => {
    try {
      const response = await fetch(
        `https://recipeappapi.azurewebsites.net/api/User/${authUser.userId}/Favorites`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${authUser.token}`,
            "Content-Type": "application/json",
          },
          // body: JSON.stringify({ favorite: true }), // Voit lähettää lisätietoja täällä
        }
      );

      if (response.ok) {
        const updatedUser = await response.json();
        setUser(updatedUser);
        console.log("Käyttäjän suosikit päivitetty:", updatedUser);
      } else {
        console.error(
          "Suosikin lisääminen epäonnistui: ",
          response,
          authUser.userId
        );
      }
    } catch (error) {
      console.error("Virhe suosikin lisäämisessä:", error);
    }
  };

  // open email client with recipe details
  const shareRecipe = () => {
    //something to share the recipe
    const emailSubject = encodeURIComponent("Tarkista tämä resepti!");
    const emailBody = encodeURIComponent(
      `Tässä on herkullinen resepti: ${recipe.name} \n\n ${recipe.instructions}`
    );

    window.open(`mailto:?subject=${emailSubject}&body=${emailBody}`);
    return;
  };

  // create a function to print the recipe with button click
  const printRecipe = () => {
    setPrinting(true);
    window.print();
    setPrinting(false);
  };

  // extract user info using fetch users and fetch recepies
  const userInfo = () => {
    const userMatch = user.find((u) => u.id === recipe.userId);
    if (userMatch) {
      console.log("User info:", userMatch.name);
      return userMatch.name;
    }
    return null; // Jos vastaavaa käyttäjää ei löydy
  };

  //extract person name who created review based on reviews.userid and user.id
  const reviewUser = (review) => {
    const userMatch = user.find((u) => u.id === review.userId);
    if (userMatch) {
      return userMatch.name;
    }
    return null; // Jos vastaavaa käyttäjää ei löydy
  };

  return (
    <div className="recipe-details-container">
      {recipe ? (
        <>
          <h1>
            {recipe.name} from {userInfo()}
          </h1>
          <div className="recipe-detail-image">
            {recipe.images.length > 0 ? (
              <RecipeSlider images={recipe.images} />
            ) : (
              <img src="/default_pic.jpg" alt="default picture" />
            )}
          </div>

          <p>
            Keywords:{" "}
            {recipe.keywords.map((keyword) => keyword.word).join(", ")}
          </p>

          <div className="recipe-detail-actions">
            <a className="recipe-detail-links" href="#" onClick={setFavourite}>
              Set Favourite {recipe.favoriteCount}
            </a>
            <a className="recipe-detail-links" href="#" onClick={shareRecipe}>
              Share
            </a>
            <a
              className="recipe-detail-links"
              href="#"
              onClick={printRecipe}
              disabled={printing} // prevent multiple clicks
            >
              Print
            </a>
          </div>
          <div className="recipe-detail-ingredients">
            <h3>Ingredients</h3>
            <ul>
              {recipe.ingredients.map((ingredient, index) => (
                <li key={index}>
                  {ingredient.amount} {ingredient.unit} {ingredient.name}
                </li>
              ))}
            </ul>
          </div>
          <div className="recipe-detail-instructions">
            <br />
            <h3>Instructions</h3>
            <p>{recipe.instructions}</p>
          </div>
          <div className="recipe-detail-reviews">
            <h3>Reviews</h3>
            {/* map through reviews */}
            {recipe.reviews.map((review, index) => (
              <div key={index} className="recipe-detail-item">
                <p>
                  <strong>{reviewUser(review)}:</strong> "{review.comment}"
                </p>
                <p>Rating: {review.rating} / 5</p>
              </div>
            ))}
          </div>
          {/* create bar for like, setFavourite, share, print and share recipe*/}
        </>
      ) : (
        <p style={{ textAlign: "center", fontSize: "48px" }}>Loading...</p>
      )}
    </div>
  );
};

const RecipeSlider = ({ images }) => {
  const [currentImageIndex, setCurrentImageIndex] = useState(0);

  const nextImage = () => {
    setCurrentImageIndex((prevIndex) =>
      prevIndex === images.length - 1 ? 0 : prevIndex + 1
    );
  };

  const prevImage = () => {
    setCurrentImageIndex((prevIndex) =>
      prevIndex === 0 ? images.length - 1 : prevIndex - 1
    );
  };

  return (
    <div className="recipe-slider-container">
      <button className="slider-button-left" onClick={prevImage}>
        &#10094;
      </button>
      <div className="recipe-slider-image">
        <img
          src={`data:image/jpeg;base64,${images[currentImageIndex].imageData}`}
          alt={`Image ${currentImageIndex + 1}`}
        />
      </div>
      <button className="slider-button-right" onClick={nextImage}>
        &#10095;
      </button>
    </div>
  );
};

export { RecipeDetails };
