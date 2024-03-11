import React from "react";
import { Link } from "react-router-dom";
import "./FavoritesRecipeGrid.css";





const FavoritesRecipeGrid = ({ recipes}) => {
  return (
    <div className="favorite-recipe-grid">
      {recipes.map((recipe) => (
        <div key={recipe.id} className="favorite-recipe-item">
          <Link to={`/recipe/${recipe.id}`} className="recipe-link">
          {recipe.images && recipe.images.length > 0 && (
              <img
                src={`data:image/jpeg;base64,${recipe.images[0].imageData}`}
                alt={`Image for ${recipe.name}`}
                className="favorite-recipe-image"
              />
            )}
                        {!recipe.images || recipe.images.length === 0 && (
               <img src="default_pic.jpg" alt="default picture" className="favorite-recipe-default"/>
            )}
            <div className="favorite-recipe-details">
              <h3 className="favorite-recipe-name">{recipe.name}</h3>
              <div className="favorite-reviews">
                {recipe.reviews && recipe.reviews.length > 0 && (
                  <div className="favorite-recipe-rating">
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

export default FavoritesRecipeGrid;