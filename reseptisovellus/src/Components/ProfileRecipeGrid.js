import React from "react";
import "./ProfileRecipeGrid.css";

const ProfileRecipeGrid = ({ recipes }) => {
  return (
    <div className="profile-recipe-grid">
      {recipes.map((recipe) => (
        <div key={recipe.id} className="profile-recipe-item">
          {recipe.images && recipe.images.map((image, index) => (
            <img
              key={index}
              src={`data:image/jpeg;base64,${image.imageData}`}
              alt={`Image for ${recipe.name}`}
              className="profile-recipe-image"
            />
          ))}
          <div className="profile-recipe-details">
            <h3 className="profile-recipe-name">{recipe.name}</h3>
            <p className="profile-recipe-instructions">{recipe.instructions}</p>
            <div className="profile-reviews">
              {recipe.reviews && recipe.reviews.map((review) => (
                <div key={review.id} className="profile-recipe-rating">
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

export default ProfileRecipeGrid;



