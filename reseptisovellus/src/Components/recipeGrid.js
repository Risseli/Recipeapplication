import { React, useState } from "react";
import "./RecipeGrid.css";
import { Link } from "react-router-dom";

const RecipeGrid = ({ recipes }) => {
  const [loading, setLoading] = useState(true); // loading state for images
  //const [images, setImages] = useState([]); // real images from recepy images
  return (
    <div className="recipe-grid">
      {recipes.map((recipe) => (
        <div key={recipe.id} className="recipe-grid-item">
          {/* create every recipe as a link */}
          <Link to={`/recipe/${recipe.id}`} className="recipe-grid-link">
            <div className="recipe-grid-image">
              {recipe.images.length > 0 ? ( // Tarkista, onko kuvia
                <img
                  src={`data:image/jpeg;base64,${recipe.images[0].imageData}`} // Näytä vain ensimmäinen kuva
                  alt={`Image for ${recipe.name}`}
                />
              ) : (
                <img src="default_pic.jpg" alt="default picture" />
              )}
            </div>
            <div className="recipe-grid-details">
              <h3 className="recipe-grid-name">{recipe.name}</h3>
              {/* <p className="recipe-instructions">{recipe.instructions}</p> */}
              <div>
                <ul>
                  {recipe.reviews.length > 0 ? (
                    <li className="recipe-grid-rating">
                      <p>
                        Average Rating: {"⭐".repeat(Math.round(recipe.rating))}{" "}
                        <br />
                      </p>
                    </li>
                  ) : (
                    <li className="recipe-grid-rating">
                      <p>No ratings yet</p>
                    </li>
                  )}
                </ul>
              </div>
            </div>
          </Link>
        </div>
      ))}
    </div>
  );
};

export default RecipeGrid;
