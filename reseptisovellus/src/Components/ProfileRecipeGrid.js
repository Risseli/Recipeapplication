import React from "react";
import { Link } from "react-router-dom";
import "./ProfileRecipeGrid.css";
import { useAuth } from "./Authentication";



const ProfileRecipeGrid = ({ recipes }) => {
  const { user:authUser} = useAuth();

const handleDeleteRecipe = async (recipeId) => {
  
 
  if (window.confirm(`Are you sure you want to delete this recipe?`)) {
    try {
      // Delete the recipe on the server
      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Recipe/${recipeId}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${authUser.token}`,
        },
      });

      if (response.ok) {
        console.log("Recipe deleted successfully.");

        // Display success message
        alert("Recipe deleted successfully.");

        // Reload the profile page
      //  window.location.reload();
      } else {
        console.error("Error deleting recipe:", response);
      }
    } catch (error) {
      console.error("Error deleting recipe:", error);
    }
  }
};


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
                        {!recipe.images || recipe.images.length === 0 && (
               <img src="default_pic.jpg" alt="default picture" className="profile-recipe-default"/>
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
                <div className="profile-recipe-actions">
                  <Link to={`/edit-recipe/${recipe.id}`} className="edit-button">
                    Edit
                  </Link>
                  <button className="delete-button" onClick={() => handleDeleteRecipe(recipe.id)}>
                    Delete
                  </button>
                </div>
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