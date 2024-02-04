import React from "react";
import "./RecipeGrid.css";

// props recipes, reviews, images
const RecipeGrid = ({ recipes, reviews, images }) => {
  return (
    <div className="recipe-grid">
      {recipes.map((recipe, index) => (
        <div key={recipe.id} className="recipe-item">
          <img
            src={images[index]} // testing image with this, images should be in public folder
            alt={recipe.name}
            className="recipe-image"
            /* src={
              images.find((image) => image.imageID === recipe.ID).imagePath
            } */
          />
          <div className="recipe-details">
            <h3 className="recipe-name">{recipe.name}</h3>
            <p className="recipe-instructions">{recipe.instructions}</p>
            <div className="reviews">
              {reviews
                .filter((review) => review.recipeID == recipe.recipeID)
                .map((review) => (
                  <div key={review.id} className="recipe-rating">
                    <p>
                      Rating: {"⭐".repeat(review.rating)} <br />
                      Review: {review.comment}
                    </p>
                  </div>
                ))}
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};

export default RecipeGrid;
