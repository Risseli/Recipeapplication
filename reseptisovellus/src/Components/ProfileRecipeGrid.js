import React from "react";
import { Link } from "react-router-dom";
import "./ProfileRecipeGrid.css";

const ProfileRecipeGrid = ({ recipes }) => {
  return (
    <div className="profile-recipe-grid">
      {recipes.map((recipe) => (
        <div key={recipe.id} className="profile-recipe-item">
          <Link to={`/recipe/${recipe.id}`} className="recipe-link">
            {recipe.images && recipe.images.length > 0 && (
              <img
                src={`data:image/jpeg;base64,${recipe.images[0].imageData}`}
                alt={`Image for ${recipe.name}`}
                className="profile-recipe-image"
              />
            )}
            <div className="profile-recipe-details">
              <h3 className="profile-recipe-name">{recipe.name}</h3>
              <div className="profile-reviews">
                {recipe.reviews && recipe.reviews.length > 0 && (
                  <div className="profile-recipe-rating">
                    <p>
                      Rating: {"⭐".repeat(recipe.reviews[0].rating)} <br />
                      Review: {truncateText(recipe.reviews[0].comment, 50)}
                    </p>
                  </div>
                )}
              </div>
            </div>
          </Link>
        </div>
      ))}
    </div>
  );
};

// Apufunktio teksti katkaisemiseen
const truncateText = (text, maxLength) => {
  if (text.length <= maxLength) {
    return text;
  } else {
    return text.substring(0, maxLength) + "...";
  }
};

export default ProfileRecipeGrid;
