import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import './Profile.css';
import ProfileRecipeGrid from "../../Components/ProfileRecipeGrid";

const Profile = () => {
  const [user, setUser] = useState(null);
  const [editMode, setEditMode] = useState(false);
  const [editedUser, setEditedUser] = useState({});
  const [selectedOption, setSelectedOption] = useState("ownRecipes");
  const navigate = useNavigate();
  const [recipes, setRecipes] = useState([]); 
  const [recipeNames, setRecipeNames] = useState({});






  useEffect(() => {
    const checkUserStatus = async () => {
      try {
        const response = await fetch("https://recipeappapi.azurewebsites.net/api/user/4"); // testataan käyttäjällä id:5
        const data = await response.json();
  
        if (data) {
          setUser(data);
          loadRecipes("ownRecipes", data.id);
        } else {
          navigate("/login");
        }
      } catch (error) {
        console.error("Error checking user status:", error);
      }
    };
  
    checkUserStatus();
  }, [navigate]);




  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
    loadRecipes(event.target.value, user.id);
  };


  const loadRecipes = async (option, id) => {
    try {
      let recipesData = [];
  
      // Set a loading state while fetching recipes
      setRecipes([]);
      setRecipeNames({}); // Tyhjennä reseptinimet joka kerta kun ladat reseptejä
  
      if (!id) {
        console.error("User ID is undefined.");
        return; // Exit the function if ID is undefined
      }
  
      let apiUrl;
      if (option === "ownRecipes") {
        // Fetch user's own recipes
        apiUrl = `https://recipeappapi.azurewebsites.net/api/User/${id}/Recipes`;
      } else if (option === "favorites") {
        // Fetch user's favorite recipes
        apiUrl = `https://recipeappapi.azurewebsites.net/api/User/${id}/Favorites`;
      }
      else if (option === "reviews") {
        // Fetch user's reviews
        apiUrl = `https://recipeappapi.azurewebsites.net/api/User/${id}/Reviews`;
      }
  
      const response = await fetch(apiUrl);
      const responseData = await response.json();
  
      if (response.ok) {
        recipesData = responseData;
        console.log(`${option} Data:`, recipesData);



                // Hae reseptinimet käyttämällä reseptin id:tä
                const namesPromises = recipesData.map(async (recipe) => {
                  const nameResponse = await fetch(`https://recipeappapi.azurewebsites.net/api/Recipe/${recipe.recipeId}`);
                  const nameData = await nameResponse.json();
                  return { id: recipe.id, name: nameData.name };
                });
        
                const resolvedNames = await Promise.all(namesPromises);
                const recipeNameMap = resolvedNames.reduce((acc, item) => {
                  acc[item.id] = item.name;
                  return acc;
                }, {});
        
                setRecipeNames(recipeNameMap);




      } else {
        console.error(`Error fetching ${option} recipes:`, responseData);
      }
  
      setRecipes(recipesData);
      console.log("Recipes State:", recipes);
      console.log("Recipe Names State:", recipeNames);
  
    } catch (error) {
      console.error("Error loading recipes:", error);
    }
  };




  
  const handleEditClick = () => {
    console.log("User state:", user); 
    setEditMode(true);
    setEditedUser({
      id: user.id, // Tarkista, että user.id on määritelty oikein
      name: user.name,
      email: user.email,
      admin: user.admin,
    });
  };




  const handleSaveClick = async () => {
    try {
      // Poista userId pyynnön rungosta, koska se on jo polussa
      const { userId, ...userWithoutId } = editedUser;
  
      console.log("Saving user data...", userWithoutId);
  
      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/user/${editedUser.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(userWithoutId), // Käytä userWithoutId:tä pyynnön rungossa
      });
  
      console.log("Response:", response);
      const data = await response.json();
  
      if (response.ok) {
        console.log("User data saved successfully.");
        setUser(editedUser);
        setEditMode(false);
      } else {
        console.error("Error updating user data", data['']['errors'][0]['errorMessage']);
      }
    } catch (error) {
      console.error("Error saving user data:", error);
    }
  };
  


  const handleDeleteClick = async () => {
    if (window.confirm(`Are you sure you want to delete profile "${user.name}"?`)) {
      try {
        const response = await fetch(`https://recipeappapi.azurewebsites.net/api/user/${user.id}`, {
          method: "DELETE",
        });
  
        if (response.ok) {
          console.log("User deleted successfully.");
          navigate("/"); // Palaa etusivulle
        } else {
          console.error("Error deleting user:", response);
        }
      } catch (error) {
        console.error("Error deleting user:", error);
      }
    }
  };



  const deleteReview = async (reviewId) => {
    try {
      // Tee API-kutsu poistaaksesi arvostelun
      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Review/${reviewId}`, {
        method: 'DELETE',
      });

      if (response.ok) {
        // Päivitä tila poistetulla arvostelulla
        setRecipes(recipes.filter(recipe => recipe.id !== reviewId));
        console.log('Review deleted successfully.');
      } else {
        console.error('Error deleting review:', response);
      }
    } catch (error) {
      console.error('Error deleting review:', error);
    }
  };
  
  


  

  return (
    <div className="container">
      <h1>Profile</h1>
      {user ? (
        <>
          {!editMode && (
            <div className="profile-section">
              <p>
                <strong>Name:</strong> {user.name}
              </p>
              <p>
                <strong>Email:</strong> {user.email}
              </p>
              <p>
                <strong>Admin:</strong> {user.admin ? "Yes" : "No"}
              </p>
              <button className="edit-button" onClick={handleEditClick}>Edit</button>
              <button className="delete-button" onClick={handleDeleteClick}>Delete</button>
            </div>
          )}
          {editMode && (
            <div className="profile-section">
<p>
  <strong>Name:</strong>{" "}
  <input
    type="text"
    value={editedUser.name}
    onChange={(e) => setEditedUser({ ...editedUser, name: e.target.value })}
  />
</p>
              <p>
                <strong>Email:</strong>{" "}
                <input
                  type="email"
                  value={editedUser.email}
                  onChange={(e) => setEditedUser({ ...editedUser, email: e.target.value })}
                />
              </p>
              <p>
                <strong>Admin:</strong> {editedUser.admin ? "Yes" : "No"}
              </p>
              {/* Admin information is not editable, so no input field */}
              <button className="save-button" onClick={handleSaveClick}>Save</button>
              <button className="cancel-button" onClick={() => setEditMode(false)}>Cancel</button>
            </div>
          )}
          <div className="view-select">
            <label>
              <strong>View: </strong>
              <select value={selectedOption} onChange={handleOptionChange}>
                <option value="ownRecipes">Own Recipes</option>
                <option value="favorites">Favorites</option>
                <option value="reviews">Reviews</option>
              </select>
            </label>
            </div>
            <div className="recipe-list">
        {selectedOption !== "reviews" ? (
          // Renderöi reseptit normaalisti, kun valittu option ei ole "reviews"
          <ProfileRecipeGrid recipes={recipes} />
          ) : (
            <div className="reviews-list">
              {recipes.length > 0 ? (
                <ul>
                  {recipes.map(recipe => (
                    <li key={recipe.id}>
                      <p>Recipe: {recipeNames[recipe.id]}</p>
                      <p>
                        Rating: {Array.from({ length: recipe.rating }, (_, index) => (
                          <span key={index} className="star-icon">⭐</span>
                        ))}
                      </p>
                      <p>Comment: {recipe.comment}</p>
                      <button onClick={() => deleteReview(recipe.id)}>Delete</button>
                    </li>
                  ))}
                </ul>
              ) : (
                <p>This User has no reviews.</p>
              )}
            </div>
          )}
        </div>
      </>
    ) : (
      <p>Loading...</p>
    )}
  </div>
);
};


export { Profile };