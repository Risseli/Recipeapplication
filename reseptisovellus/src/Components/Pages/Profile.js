import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import './Profile.css';

const Profile = () => {
  const [user, setUser] = useState(null);
  const [editMode, setEditMode] = useState(false);
  const [editedUser, setEditedUser] = useState({});
  const [selectedOption, setSelectedOption] = useState("ownRecipes");
  const [recipes, setRecipes] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    const checkUserStatus = async () => {
      try {
        const response = await fetch("https://recipeappapi.azurewebsites.net/api/user/5"); // testataan käyttäjällä id:4
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

  const handleEditClick = () => {
    setEditMode(true);
    setEditedUser({
      name: user.name,
      email: user.email,
      admin: user.admin,
    });
  };


  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
    loadRecipes(event.target.value);
  };


  const loadRecipes = async (option, id) => {
    try {
      let recipesData = [];
  
      if (option === "ownRecipes") {
        const response = await fetch(`https://recipeappapi.azurewebsites.net/api/recipe`);
      recipesData = await response.json();


      const userRecipes = recipesData.filter(recipe => recipe.userId === id);
      setRecipes(userRecipes);

      } else if (option === "favorites") {
        // Käytä paikallista json-serveriä hakemaan favorites-tiedot
        // const response = await fetch(`http://localhost:3004/favorites?userId=${userId}`);
        // const favoritesData = await response.json();
  
        // Hae suosikkireseptien tiedot "recipe" taulusta
        // for (const favorite of favoritesData) {
          // const recipeResponse = await fetch(`http://localhost:3004/recipe?recipeID=${favorite.recipeId}`);
          // const recipeData = await recipeResponse.json();
          // recipesData.push(recipeData);

          const response = await fetch(`http://localhost:3004/recipe?recipeID=5`);
          recipesData = await response.json();
          setRecipes(recipesData);

        //}
      }
  

    } catch (error) {
      console.error("Error loading recipes:", error);
    }
  };

  const handleSaveClick = async () => {
    try {
      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/user/${user.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(editedUser),
      });

      if (response.ok) {
        setUser(editedUser);
        setEditMode(false);
      } else {
        console.error("Error updating user data");
      }
    } catch (error) {
      console.error("Error saving user data:", error);
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
            </div>
          )}
          {editMode && (
            <div className="profile-section">
              <p>
                <strong>Name:</strong> {editedUser.name}
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
              </select>
            </label>
          </div>
          <div className="recipe-list">
            <h2>Recipes:</h2>
            <ul>
  {recipes.map((recipe) => (
    <li key={recipe.id}>{recipe.name}</li>
  ))}
</ul>
          </div>
        </>
      ) : (
        // Render a loading state or redirect to login if the user is not available
        <p>Loading...</p>
      )}
    </div>
  );

      };


export { Profile };