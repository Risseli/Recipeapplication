import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import "./RecipeDetails.css";
import { StarRating } from "../StarRating";
import { useAuth } from "../Authentication";
import { NavLink } from "react-router-dom";

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
import { CiEdit } from "react-icons/ci";

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
  const [isFavoriteLoading, setIsFavoriteLoading] = useState(false); // loading state for checking favorite
  const [thisUser, setThisUser] = useState(null); // user info
  const [reviewId, setReviewId] = useState(null); // review id

  console.log("rating:", rating, "color:", color);

  // get recipe details from certain recipe and all users
  const fetchRecipeDetails = async () => {
    try {
      const response = await fetch(
        //`https://recipeappapi.azurewebsites.net/api/recipe/${id}`
        `https://localhost:7005/api/recipe/${id}`
      );
      const response2 = await fetch(
        // `https://recipeappapi.azurewebsites.net/api/user`
        `https://localhost:7005/api/user`
      );
      if (authUser) {
        const response3 = await fetch(
          // `https://recipeappapi.azurewebsites.net/api/User/${authUser.userId}`
          `https://localhost:7005/api/User/${authUser.userId}`
        );

        const data3 = await response3.json(); // this user
        setThisUser(data3);
        console.log("This user: ", data3);
      }

      const data = await response.json(); // recipe
      const data2 = await response2.json(); // user
      // const data3 = await response3.json(); // this user
      const firstReview = data.reviews[0]; // Oletetaan, että arvostelut ovat saatavilla ja että ensimmäinen arvostelu on riittävä
      const reviewId = firstReview ? firstReview.id : null;

      setReviewId(reviewId);
      setRecipe(data);
      setUser(data2);
      // setThisUser(data3);

      console.log(data);
      console.log("User: ", data2);
      //console.log("This user: ", data3);
    } catch (error) {
      console.error("Error fetching recipe details:", error);
    }
  };

  useEffect(() => {
    fetchRecipeDetails();
  }, [id]);

  useEffect(() => {
    if (authUser) {
      checkFavorite();
    }
  }, [authUser]);

  const checkFavorite = async () => {
    if (authUser) {
      setIsFavoriteLoading(true);
      try {
        const response = await fetch(
          // `https://recipeappapi.azurewebsites.net/api/User/${authUser.userId}/Favorites`
          `https://localhost:7005/api/User/${authUser.userId}/Favorites`
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
      } finally {
        setIsFavoriteLoading(false);
      }
    }
  };

  // set recipe as favourite
  const addFavorite = async () => {
    try {
      const response = await fetch(
        //`https://recipeappapi.azurewebsites.net/api/User/${authUser.userId}/Favorites?recipeId=${id}`,
        `https://localhost:7005/api/User/${authUser.userId}/Favorites?recipeId=${id}`,

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
          // `https://recipeappapi.azurewebsites.net/api/recipe/${id}`
          `https://localhost:7005/api/recipe/${id}`
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
        // `https://recipeappapi.azurewebsites.net/api/User/${authUser.userId}/Favorites?recipeId=${id}`,
        `https://localhost:7005/api/User/${authUser.userId}/Favorites?recipeId=${id}`,

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
          // `https://recipeappapi.azurewebsites.net/api/recipe/${id}`
          `https://localhost:7005/api/recipe/${id}`
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

  // find out if logged user is recipe creator, if it is show edit button
  const isCreator = () => {
    if (authUser && recipe.userId === authUser.userId) {
      return true;
    }
    return false;
  };

  // function to show if user is admin or not using authUser and id
  console.log("is admin or not: ");

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
        // `https://recipeappapi.azurewebsites.net/api/review/`,
        `https://localhost:7005/api/review/`,
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

        console.log("Review response ok!");
      } else {
        console.error("Lisää arvostelu epäonnistui:", response.status);
      }
      // error not fixed have to do this way, if fixed move fetchRecipeDetails to try response.ok
    } catch (error) {
      console.error("Virhe arvostelun lisäämisessä:", error);
    }
    fetchRecipeDetails();
  };

  const deleteReview = async () => {
    try {
      const response = await fetch(
        // `https://recipeappapi.azurewebsites.net/api/review/${id}`,
        `https://localhost:7005/api/review/${reviewId}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${authUser.token}`,
          },
        }
      );

      if (response.ok) {
        console.log("Review deleted successfully.");
        alert("Review deleted successfully.");
        window.location.reload();
      } else {
        console.error("Error deleting review:", response);
      }
    } catch (error) {
      console.error("Error deleting review:", error);
    }
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

            {authUser && !isFavoriteLoading && (
              <button
                style={{ marginLeft: "60%" }}
                onClick={() => {
                  if (isFavorite) {
                    removeFavorite();
                  } else {
                    addFavorite();
                  }
                }}
              >
                {isFavorite ? (
                  <IoMdHeart size={45} color="#172554" />
                ) : (
                  <IoMdHeartEmpty size={45} color="#172554" />
                )}
              </button>
            )}
            {thisUser && thisUser.admin ? (
              <NavLink to={`/edit-recipe/${recipe.id}`}>
                <CiEdit size={45} />
              </NavLink>
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
              <NavLink
                to="/login"
                style={{
                  paddingLeft: "70%",
                  fontSize: "20px",
                  textDecoration: "none",
                }}
              >
                Login to add review
              </NavLink>
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
            <br />
            {/* map reviews backwards, newest comment first */}
            {recipe.reviews
              .slice()
              .reverse()
              .map((review, index) => (
                <div key={index} className="recipe-detail-item">
                  <p>
                    <strong>{reviewUser(review)}:</strong> "{review.comment}"
                  </p>
                  <p>Rating: {review.rating} / 5</p>
                  {((authUser && authUser.userId === review.userId) ||
                    (authUser && thisUser && thisUser.admin)) && (
                    <button
                      style={{ marginTop: "10px" }}
                      onClick={deleteReview}
                    >
                      Remove review
                    </button>
                  )}
                </div>
              ))}
            {/* if logged user is admin or recipe owner show edit button */}
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
