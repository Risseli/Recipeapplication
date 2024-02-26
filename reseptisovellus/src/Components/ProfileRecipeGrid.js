import React from "react";
import { Link } from "react-router-dom";
import "./ProfileRecipeGrid.css";

const ProfileRecipeGrid = ({ recipes }) => {
  return (
    <div className="profile-recipe-grid">
      {recipes.map((recipe) => (
        <div key={recipe.id} className="profile-recipe-item">
          {recipe.images && recipe.images.length > 0 && (
            <Link to={`/recipe/${recipe.id}`} className="recipe-link">
              <img
                src={`data:image/jpeg;base64,${recipe.images[0].imageData}`}
                alt={`Image for ${recipe.name}`}
                className="profile-recipe-image"
              />
            </Link>
          )}
          <div className="profile-recipe-details">
            <h3 className="profile-recipe-name">{recipe.name}</h3>
            <div className="profile-reviews">
              {recipe.reviews && recipe.reviews.length > 0 && (
                <div className="profile-recipe-rating">
                  <p>
                    Rating: {"⭐".repeat(recipe.reviews[0].rating)} <br />
                    Review: {recipe.reviews[0].comment}
                  </p>
                </div>
              )}
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};

export default ProfileRecipeGrid;