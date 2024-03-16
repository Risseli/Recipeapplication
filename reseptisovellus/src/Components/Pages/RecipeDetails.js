import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import "./RecipeDetails.css";
import { StarRating } from "../StarRating";
import { useAuth } from "../Authentication";
import { Link } from "react-router-dom";
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
import { IoMdHeart, IoMdHeartEmpty } from "react-icons/io";

const RecipeDetails = () => {
  const [recipe, setRecipe] = useState(null); // recipe info
  const [user, setUser] = useState(null); // user info
  const { id } = useParams(); // Get the recipe id from the URL
  const [printing, setPrinting] = useState(false); // print recipe
  const { user: authUser } = useAuth(); // Get the user from the context
  const [comment, setComment] = useState(""); // review comment
  const [visibility, setVisibility] = useState(false); // show/hide add review form
  const [rating, setRating] = useState(null); // sent as props to StarRating component in reviews
  const [color, setColor] = useState(null); // sent as props to StarRating component in reviews
  const [isFavorite, setIsFavorite] = useState(false); // check if recipe is favourite

  console.log("rating:", rating, "color:", color);

  // get recipe details from certain recipe and all users
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

  useEffect(() => {
    fetchRecipeDetails();
  }, [id]);

  const checkFavorite = async () => {
    if (authUser) {
      try {
        const response = await fetch(
          `https://recipeappapi.azurewebsites.net/api/User/${authUser.userId}/Favorites`
        );
        if (response.ok) {
          const data = await response.json();
          if (data.find((f) => f.id === parseInt(id))) {
            console.log("Recipe is favourite");
            setIsFavorite(true);
          } else {
            console.log("Recipe is not favourite");
            setIsFavorite(false);
          }
          console.log("Check favorites status: ", data);
        } else {
          console.log("Recipe is not favourite");
          console.error("Error fetching favourites:", response.status);
        }
      } catch (error) {
        console.error("Error fetching favourites:", error);
        console.log("Hello from checkfavorite catch block!");
      }
    }
  };

  // set recipe as favourite
  const addFavorite = async () => {
    try {
      const response = await fetch(
        `https://recipeappapi.azurewebsites.net/api/User/${authUser.userId}/Favorites?recipeId=${id}`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${authUser.token}`,
            "Content-Type": "application/json",
          },
        }
      );

      if (response.ok) {
        // Fetch updated recipe details after setting favourite
        const updatedRecipeResponse = await fetch(
          `https://recipeappapi.azurewebsites.net/api/recipe/${id}`
        );
        const updatedRecipeData = await updatedRecipeResponse.json();

        // Update recipe state with the updated recipe data
        setRecipe(updatedRecipeData);
        setIsFavorite(true);

        console.log("Recipe's favourites updated:", updatedRecipeData);
      } else {
        console.error("Setting favourite failed: ", response.status);
      }
    } catch (error) {
      console.error("Error setting favourite:", error);
    }
  };

  const removeFavorite = async () => {
    try {
      const response = await fetch(
        `https://recipeappapi.azurewebsites.net/api/User/${authUser.userId}/Favorites?recipeId=${id}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${authUser.token}`,
          },
        }
      );

      if (response.ok) {
        // Fetch updated recipe details after removing favourite
        const updatedRecipeResponse = await fetch(
          `https://recipeappapi.azurewebsites.net/api/recipe/${id}`
        );

        const updatedRecipeData = await updatedRecipeResponse.json();
        setIsFavorite(false);

        // Update recipe state with the updated recipe data
        setRecipe(updatedRecipeData);

        console.log("Recipe's favourites updated:", updatedRecipeData);
      } else {
        console.error("Removing favourite failed: ", response.status);
      }
    } catch (error) {
      console.error("Error removing favourite:", error);
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

  // toggle visibility of add review form, click to show/hide, resets comment and rating
  const toggleVisibility = () => {
    setVisibility(!visibility);
    setComment("");
    setRating(null);
  };

  // add new review to the recipe
  const addReview = async () => {
    setVisibility(false);
    try {
      const response = await fetch(
        `https://recipeappapi.azurewebsites.net/api/review/`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${authUser.token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            recipeId: id,
            userId: authUser.userId,
            comment: comment,
            rating: rating,
          }),
        }
      );
      // reset comment and rating after adding review
      setComment("");
      setRating(null);

      if (response.ok) {
        await response.json();

        console.log("Review sesponse ok!");
      } else {
        console.error("Lisää arvostelu epäonnistui:", response.status);
      }
      // error not fixed have to do this way, if fixed move fetchRecipeDetails to try response.ok
    } catch (error) {
      console.error("Virhe arvostelun lisäämisessä:", error);
    }
    fetchRecipeDetails();
  };

  const currentPage = `https://recipeappgl.azurewebsites.net/`;

  return (
    <div className="recipe-details-container">
      {recipe ? (
        <>
          <h1>
            {recipe.name} from {userInfo()}
          </h1>

          <div className="recipe-detail-sharing">
            <EmailShareButton
              url={currentPage}
              subject="Check out this awesome recipe!"
              body={`Here is a delicious recipe: ${recipe.name} \n\n ${recipe.instructions} \n\n`}
            >
              <EmailIcon size={45} round={false} borderRadius={10} />
            </EmailShareButton>
            <FacebookShareButton url={currentPage} hashtag="#reseptisovellus">
              <FacebookIcon size={45} round={false} borderRadius={10} />
            </FacebookShareButton>
            <LinkedinShareButton
              url={"https://recipeappgl.azurewebsites.net/"}
              title="Reseptisovellus"
              summary="Browse add and share recepies!"
              // source="Reseptisovellus Group L"
            >
              <LinkedinIcon size={45} round={false} borderRadius={10} />
            </LinkedinShareButton>
            <WhatsappShareButton url={currentPage}>
              <WhatsappIcon size={45} round={false} borderRadius={10} />
            </WhatsappShareButton>

            {authUser ? (
              <>
                <button
                  className="recipe-detail-favorite"
                  onClick={async () => {
                    await checkFavorite();
                    if (isFavorite) {
                      await removeFavorite();
                    } else {
                      await addFavorite();
                    }
                  }}
                >
                  {isFavorite ? (
                    <>
                      <span>Remove from Favorites</span>{" "}
                      <IoMdHeart size={60} color="#172554" />
                    </>
                  ) : (
                    <>
                      <span>Add to Favorites</span>{" "}
                      <IoMdHeartEmpty size={50} color="#172554" />
                    </>
                  )}
                </button>
              </>
            ) : null}
          </div>

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
            <h3>Reviews</h3> {/* if logged show add review */}
            {authUser ? (
              <a
                href="#"
                style={{
                  paddingLeft: "75%",
                  fontSize: "20px",
                  textDecoration: "none",
                }}
                // prevent screen moving up when clicked preventDefault action
                onClick={(e) => {
                  e.preventDefault();
                  toggleVisibility();
                }}
              >
                Add new review +
              </a>
            ) : (
              // if not logged in, show link to login and redirect to login page
              <Link
                to="/login"
                style={{
                  paddingLeft: "75%",
                  fontSize: "20px",
                  textDecoration: "none",
                }}
              >
                Login to add review
              </Link>
            )}
            {visibility ? (
              <div className="recipe-detail-newReview">
                <textarea
                  style={{ resize: "vertical" }}
                  autoFocus={true}
                  maxLength={200}
                  rows={10}
                  cols={98}
                  value={comment}
                  onChange={(e) => setComment(e.target.value)}
                  placeholder="Write your review here"
                ></textarea>
                <br />
                {/* Add star rating to review, rating and color sent as props to StarRating component */}
                <StarRating
                  rating={rating}
                  setRating={setRating}
                  color={color}
                  setColor={setColor}
                />
                <br />
                <br />
                <button onClick={addReview}>Add review</button>
              </div>
            ) : null}
            {/* map reviews backwards, newest comment first */}
            {recipe.reviews.slice().reverse().map((review, index) => (
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
