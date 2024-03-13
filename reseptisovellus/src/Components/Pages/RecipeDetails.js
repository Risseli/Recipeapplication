import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import "./RecipeDetails.css";
import { useAuth } from "../Authentication";
import {
  EmailShareButton,
  FacebookShareButton,
  LinkedinShareButton,
  WhatsappShareButton,
  EmailIcon,
  FacebookIcon,
  LinkedinIcon,
  WhatsappIcon,
} from "react-share";

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
        `https://recipeappapi.azurewebsites.net/api/User/${authUser.userId}/Favorites?recipeId=${id}`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${authUser.token}`,
            "Content-Type": "application/json",
          },
          // body: JSON.stringify({ favorite: true }), // Voit lähettää lisätietoja täällä
        }
      );
      const responseData = await response.text(); // Haetaan vastauksen tekstimuotoinen sisältö
      const trimmedData = responseData.trim(); // Poistetaan ylimääräiset merkit
      const updatedUser = JSON.parse(trimmedData); // Parsitaan JSON-muotoon
      console.log("Response data:", updatedUser);

      if (response.ok) {
        setUser(updatedUser);
        console.log("Käyttäjän suosikit päivitetty:", updatedUser);
      } else {
        console.error(
          "Suosikin lisääminen epäonnistui: ",
          response.status,
          updatedUser
        );
      }
    } catch (error) {
      console.error("Virhe suosikin lisäämisessä:", error);
    }
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

  const currentPage = `https://recipeappgl.azurewebsites.net/recipe/${id}`;

  return (
    <div className="recipe-details-container">
      {recipe ? (
        <>
          <h1>
            {recipe.name} from {userInfo()}
          </h1>
          {authUser ? (
            <div className="recipe-detail-sharing">
              <EmailShareButton
                subject="Check out this awesome recipe!"
                body={`Here is a delicious recipe: ${recipe.name} \n\n ${recipe.instructions}`}
              >
                <EmailIcon size={45} round={false} borderRadius={10} />
              </EmailShareButton>
              <FacebookShareButton url={currentPage} hashtag="#reseptisovellus">
                <FacebookIcon size={45} round={false} borderRadius={10} />
              </FacebookShareButton>
              <LinkedinShareButton
                url={currentPage}
                title="Reseptisovellus"
                summary="Browse add and share recepies!"
                source="Reseptisovellus Group L"
              >
                <LinkedinIcon size={45} round={false} borderRadius={10} />
              </LinkedinShareButton>
              <WhatsappShareButton url={currentPage}>
                <WhatsappIcon size={45} round={false} borderRadius={10} />
              </WhatsappShareButton>
              <a
                className="recipe-detail-links"
                href="#"
                onClick={setFavourite}
              >
                Set Favourite {recipe.favoriteCount}
              </a>
            </div>
          ) : null}
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
