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




  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
    loadRecipes(event.target.value, user.id);
  };


  const loadRecipes = async (option, id) => {
    try {
      let recipesData = [];
  
      // Set a loading state while fetching recipes
      setRecipes([]);
  
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
  
      const response = await fetch(apiUrl);
      const responseData = await response.json();
  
      if (response.ok) {
        recipesData = responseData;
        console.log(`${option} Data:`, recipesData);
      } else {
        console.error(`Error fetching ${option} recipes:`, responseData);
      }
  
      setRecipes(recipesData);
      console.log("Recipes State:", recipes);
  
    } catch (error) {
      console.error("Error loading recipes:", error);
    }
  };
  
  
  const handleEditClick = () => {
    console.log("User state:", user); // Lisää tämä rivi
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
  
      if (response.ok) {
        console.log("User data saved successfully.");
        setUser(editedUser);
        setEditMode(false);
      } else {
        console.error("Error updating user data", response);
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
              </select>
            </label>
          </div>
          <div className="recipe-list">
          {Array.isArray(recipes) ? (
  <ul>
    {recipes.map((recipe) => (
      <li key={`${recipe.userId}-${recipe.id}`}>{recipe.name}</li>
    ))}
  </ul>
) : (
  <p>No recipes available</p>
)}
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