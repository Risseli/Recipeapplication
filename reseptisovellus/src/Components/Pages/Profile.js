import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

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
        // Simuloitu kirjautumistietojen hakeminen JSON-tiedostosta. Haetaanko tietokannasta vai tarkistetaanko vaan onko user vai ei??
        const response = await fetch("http://localhost:3004/user?userID=4");
        const data = await response.json();

        console.log(data); // Tarkistetaan saadaanko data

        if (data.length > 0) {
          setUser(data[0]); // Otetaan ensimmäinen käyttäjä, koska tulos on yksi käyttäjä
          loadRecipes("ownRecipes"); // Lataa reseptit oletuksena "ownRecipes" -vaihtoehdon mukaan
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
      username: user.username,
      name: user.name,
      email: user.email,
      password: user.password, // tarvitaanko??
    });
  };

  const handleSaveClick = async () => {
    try {
      // Lähetä päivitetyt tiedot palvelimelle
      const response = await fetch(`http://localhost:3004/user/${user.userID}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(editedUser),
      });

      if (response.ok) {
        // Päivitä käyttäjätila paikallisesti
        setUser(editedUser);
        setEditMode(false);
      } else {
        console.error("Error updating user data");
      }
    } catch (error) {
      console.error("Error saving user data:", error);
    }
  };

  const handleCancelClick = () => {
    setEditMode(false);
  };


  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
    loadRecipes(event.target.value); // Lataa reseptit valinnan mukaan
  };



  const loadRecipes = async (option) => {
    try {
      let recipesData = [];

  
      if (option === "ownRecipes") {
        // Hae käyttäjän omat reseptit
        const response = await fetch(`http://localhost:3004/recipe?userID=${user.userID}`);
        recipesData = await response.json();
      } else if (option === "favorites") {
        // Hae käyttäjän suosikkireseptit
        //const response = await fetch(`http://localhost:3004/favorites?userID=${user.userID}`);
        //const favoritesData = await response.json();
  
        // Hae suosikkireseptien tiedot "recipe" taulusta, ei palauta nimeä?? nyt kovakoodattu hakemaan reseptillä 5 niin nimi toimii...
//for (const favorite of favoritesData) {
  const response = await fetch(`http://localhost:3004/recipe?recipeID=5`);
  recipesData = await response.json();
  //recipesData.push(recipeData);
  //    }
      }
  
      setRecipes(recipesData);
    } catch (error) {
      console.error("Error loading recipes:", error);
    }
  };


  if (!user) {
    navigate("/login");
    return null;
  }

  return (
    <div>
      <h1>Profile</h1>
      {!editMode && (
        <div>
          <p>
            <strong>Username:</strong> {user.username}
          </p>
          <p>
            <strong>Name:</strong> {user.name}
          </p>
          <p>
            <strong>E-mail:</strong> {user.email}
          </p>
          <p>
            <strong>Password:</strong> {user.password}
          </p>
          <button onClick={handleEditClick}>Edit</button>
        </div>
      )}
      {editMode && (
        <div>
          <p>
            <strong>Username:</strong> {editedUser.username}
          </p>
          <p>
            <strong>Name:</strong>{" "}
            <input
              type="text"
              value={editedUser.name}
              onChange={(e) =>
                setEditedUser({ ...editedUser, name: e.target.value })
              }
            />
          </p>
          <p>
            <strong>Email:</strong>{" "}
            <input
              type="email"
              value={editedUser.email}
              onChange={(e) =>
                setEditedUser({ ...editedUser, email: e.target.value })
              }
            />
          </p>
          <p>
            <strong>Password:</strong>{" "}
            <input
              type="password"
              value={editedUser.password}
              onChange={(e) =>
                setEditedUser({ ...editedUser, password: e.target.value })
              }
            />
          </p>
          <button onClick={handleSaveClick}>Save</button>
          <button onClick={handleCancelClick}>Cancel</button>
        </div>
      )}
      <div>
        <label>
          <strong>View:</strong>
          <select value={selectedOption} onChange={handleOptionChange}>
            <option value="ownRecipes">Own Recipes</option>
            <option value="favorites">Favorites</option>
          </select>
        </label>
      </div>
      <div>
        <h2>Recipes:</h2>
        <ul>
          {recipes.map((recipe) => (
            <li key={recipe.recipeID}>{recipe.name}</li>
          ))}
        </ul>
      </div>
    </div>
  );
};

export { Profile };