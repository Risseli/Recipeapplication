import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import './Profile.css';
import ProfileRecipeGrid from "../../Components/ProfileRecipeGrid";
import FavoritesRecipeGrid from "../../Components/FavoritesRecipeGrid";
import { Link } from 'react-router-dom';
import { useAuth } from "../Authentication";

const Profile = () => {
  const { user:authUser} = useAuth();
  const [user, setUser] = useState(null);
  const [editMode, setEditMode] = useState(false);
  const [editedUser, setEditedUser] = useState({});
  const [selectedOption, setSelectedOption] = useState("ownRecipes");
  const navigate = useNavigate();
  const [recipes, setRecipes] = useState([]); 
  const [recipeNames, setRecipeNames] = useState({});
  const [adminMode, setAdminMode] = useState(false);  
  const [users, setUsers] = useState([]); 


    // Uudet tilat arvostelun muokkausta varten
    const [isEditing, setEditing] = useState(false);
    const [editedRating, setEditedRating] = useState(0);
    const [editedComment, setEditedComment] = useState("");
    const [editingReviewId, setEditingReviewId] = useState(null);


      // Uusi funktio avaa arvostelun muokkauslomakkeen
  const handleEditReviewClick = (reviewId) => {
    const reviewToEdit = recipes.find((recipe) => recipe.id === reviewId);
    if (reviewToEdit) {
      setEditing(true);
      setEditingReviewId(reviewId);
      setEditedRating(reviewToEdit.rating);
      setEditedComment(reviewToEdit.comment);
    }
  };

  const handleSaveEdit = async () => {
    try {
      console.log('Start handleSaveEdit');
  
      const requestBody = {
        id: editingReviewId,
        rating: editedRating !== undefined ? editedRating : null,
        comment: editedComment !== undefined ? editedComment : null,
      };
  
      console.log('Request Body:', requestBody);
  
      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Review/${editingReviewId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json', 
          Authorization: `Bearer ${authUser.token}`,
        },
        body: JSON.stringify(requestBody),
      });
  
      console.log('Response:', response);
  
      let data;
  
      if (response.ok) {
        data = await response.json();
        console.log('Review updated successfully.');
        setEditing(false);
        setRecipes((prevRecipes) => {
          return prevRecipes.map((recipe) =>
            recipe.id === editingReviewId
              ? { ...recipe, rating: editedRating !== undefined ? editedRating : recipe.rating, comment: editedComment !== undefined ? editedComment : recipe.comment }
              : recipe
          );
        });
        alert('Review updated successfully.');
      } else {
        data = await response.json();
        console.error('Error updating review:', data);
        alert('Error updating review. Please try again.');
      }
    } catch (error) {
      console.error('Error in handleSaveEdit:', error);
      alert('Error updating review. Please try again.');
    }
  };
  
  


  useEffect(() => {
    const checkUserStatus = async () => {
      try {
        // Tarkista, onko käyttäjä kirjautunut sisään
        if (!authUser || !authUser.token) {
          navigate("/login");
          return;
        }
  
        // Hae käyttäjän tiedot
        const response = await fetch(`https://recipeappapi.azurewebsites.net/api/user/${authUser.userId}`, {
          method: 'GET',
          headers: {
            Authorization: `Bearer ${authUser.token}`,
          },
        });
  
        const data = await response.json();
  
        if (response.ok) {
          setUser(data);



        // Tarkista, onko käyttäjä admin
        if (data.admin) {
          setAdminMode(true);

          // Hae käyttäjien lista tarvittaessa
          const usersResponse = await fetch("https://recipeappapi.azurewebsites.net/api/User", {
            method: "GET",
            headers: {
              Authorization: `Bearer ${authUser.token}`,
            },
          });
          const usersData = await usersResponse.json();
          setUsers(usersData);
        }

        // Lataa käyttäjän reseptit
        loadRecipes("ownRecipes", data.id);
      } else {
        // Käyttäjä ei ole kirjautunut sisään, ohjaa kirjautumissivulle
        navigate("/login");
      }
    } catch (error) {
      console.error("Error checking user status:", error);
    }
  };

  checkUserStatus();
}, [authUser, navigate]);





 const handleOptionChange = (event) => {
  setSelectedOption(event.target.value);
  // Pass the correct user id based on edit mode
  const userId = editMode ? editedUser.id : user.id;
  loadRecipes(event.target.value, userId);
};


const loadRecipes = async (option, id) => {
  try {
    let recipesData = [];

    // Aseta lataustila fetch-pyyntöjä tehtäessä
    setRecipes([]);
    setRecipeNames({}); // Tyhjennä reseptinimet aina kun ladat reseptejä

    if (!id) {
      console.error("User ID is undefined.");
      return; // Poistu funktiosta, jos ID on määrittelemätön
    }

    let apiUrl;
    if (option === "ownRecipes") {
      // Hae käyttäjän omat reseptit
      apiUrl = `https://recipeappapi.azurewebsites.net/api/User/${id}/Recipes`;
    } else if (option === "favorites") {
      // Hae käyttäjän suosikkireseptit
      apiUrl = `https://recipeappapi.azurewebsites.net/api/User/${id}/Favorites`;
    } else if (option === "reviews") {
      // Hae käyttäjän arvostelut
      apiUrl = `https://recipeappapi.azurewebsites.net/api/User/${id}/Reviews`;
    }

    const response = await fetch(apiUrl, {
      method: 'GET',
      headers: {
        Authorization: `Bearer ${authUser.token}`, // Lisää token otsikkoon
      },
    });
    const responseData = await response.json();

    if (response.ok) {
      recipesData = responseData;
      console.log(`${option} Data:`, recipesData);

 // Hae reseptinimet käyttämällä reseptin id:tä
 if (option === "reviews") {
 const namesPromises = recipesData.map(async (recipe) => {
  const nameResponse = await fetch(`https://recipeappapi.azurewebsites.net/api/Recipe/${recipe.recipeId}`, {
    method:'GET',
  });
  const nameData = await nameResponse.json();
  return { id: recipe.id, name: nameData.name };
});


      const resolvedNames = await Promise.all(namesPromises);
      const recipeNameMap = resolvedNames.reduce((acc, item) => {
        acc[item.id] = item.name;
        return acc;
      }, {});

      setRecipeNames(recipeNameMap);
    }
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
          Authorization: `Bearer ${authUser.token}`, // Lisää token otsikkoon
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
          headers: {
            Authorization: `Bearer ${authUser.token}`, // Lisää token otsikkoon
          },
        });
  
        if (response.ok) {
          console.log("User deleted successfully.");
          // Näytä ilmoitus onnistuneesta poistosta
          alert('User deleted successfully.');
          navigate("/"); // Palaa etusivulle
        } else {
          console.error("Error deleting user:", response);
          // Näytä ilmoitus epäonnistuneesta poistosta
          alert('Error deleting user. Please try again.');
        }
      } catch (error) {
        console.error("Error deleting user:", error);
        // Näytä ilmoitus epäonnistuneesta poistosta
        alert('Error deleting user. Please try again.');
      }
    }
  };
  
  



  const deleteReview = async (reviewId) => {
    if (window.confirm(`Are you sure you want to delete this review?`)) {
      try {
        const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Review/${reviewId}`, {
          method: 'DELETE',
          headers: {
            Authorization: `Bearer ${authUser.token}`, // Lisää token otsikkoon
          },
        });
  
        if (response.ok) {
          // Update the state by removing the deleted review
          setRecipes((prevRecipes) => prevRecipes.filter(recipe => recipe.id !== reviewId));
          console.log('Review deleted successfully.');
          // Display a success message
          alert('Review deleted successfully.');
        } else {
          console.error('Error deleting review:', response);
          // Display an error message
          alert('Error deleting review. Please try again.');
        }
      } catch (error) {
        console.error('Error deleting review:', error);
        // Display an error message
        alert('Error deleting review. Please try again.');
      }
    }
  };
  
  
  
  



  const handleAdminCheckboxChange = (userId) => {
    const updatedUsers = users.map((user) =>
      user.id === userId ? { ...user, admin: !user.admin } : user
    );
  
    setUsers(updatedUsers);
  };
  
  const handleSaveAdminChanges = async () => {
    try {
      // Create an array of promises for each user update
      const updatePromises = users.map(async (user) => {
        await fetch(`https://recipeappapi.azurewebsites.net/api/User/${user.id}`, {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${authUser.token}`, // Lisää token otsikkoon
          },
          body: JSON.stringify({ admin: user.admin }),
        });
      });
  
      // Wait for all updates to complete
      await Promise.all(updatePromises);
  
      console.log("Admin changes saved successfully.");
  
      // Notify the user upon successful save
      alert("Admin changes saved successfully.");
    } catch (error) {
      console.error("Error saving admin changes:", error);
      // Voit lisätä tähän lisää virheenkäsittelyä tai ilmoituksia tarvittaessa
    }
  };
  


  const handleDeleteUser = async (userId) => {
    if (window.confirm(`Are you sure you want to delete this user?`)) {
      try {
        // Delete the user on the server
        const response = await fetch(`https://recipeappapi.azurewebsites.net/api/User/${userId}`, {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${authUser.token}`, // Lisää token otsikkoon
          },
        });
  
        if (response.ok) {
          // Update the users list in the state
          setUsers(users.filter((user) => user.id !== userId));
          console.log("User deleted successfully.");
        } else {
          console.error("Error deleting user:", response);
        }
      } catch (error) {
        console.error("Error deleting user:", error);
      }
    }
  };
  
  

  return (
    <div className="container">
      <h1>Profile</h1>
      {user ? (
        <>
          {!editMode && !adminMode && (
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
              <Link to="/add-recipe" className="add-recipe-button">
    Add new Recipe
</Link>
            </div>
          )}
          {editMode && !adminMode && (
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
{!editMode && adminMode && (
  <div className="admin-section">
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
              <Link to="/add-recipe" className="add-recipe-button">
    Add new Recipe
</Link>
            </div>
    <h2>Admin Mode</h2>
    <p>User Management:</p>
    {users.map((adminUser) => (
      <div key={adminUser.id} className="admin-user">
        <span>{adminUser.name}</span>
        <input
          type="checkbox"
          checked={adminUser.admin}
          onChange={() => handleAdminCheckboxChange(adminUser.id)}
        />
        {/* Delete button remains for individual deletion */}
        <button onClick={() => handleDeleteUser(adminUser.id)}>Delete</button>
      </div>
    ))}

    {/* Save button to save admin changes */}
    <button className="save-button" onClick={handleSaveAdminChanges}>
      Save Admin Changes
    </button>
  </div>
)}
{editMode && adminMode && (
  <div className="admin-section">
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
    <h2>Admin Mode</h2>
    <p>User Management:</p>
    {users.map((adminUser) => (
      <div key={adminUser.id} className="admin-user">
        <span>{adminUser.name}</span>
        <input
          type="checkbox"
          checked={adminUser.admin}
          onChange={() => handleAdminCheckboxChange(adminUser.id)}
        />
        {/* Delete button remains for individual deletion */}
        <button onClick={() => handleDeleteUser(adminUser.id)}>Delete</button>
      </div>
    ))}

    {/* Save button to save admin changes */}
    <button className="save-button" onClick={handleSaveAdminChanges}>
      Save Admin Changes
    </button>
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
  {selectedOption === "ownRecipes" ? (
    // Renderöi reseptit normaalisti, kun valittu option on own recipes
    <ProfileRecipeGrid recipes={recipes} />
  ) : (
    selectedOption === "favorites" ? (
      // Renderöi reseptit normaalisti, kun valittu option on favorites
      <FavoritesRecipeGrid recipes={recipes} />
    ) : (
      <div className="reviews-list">
      {recipes.length > 0 ? (
        <ul>
          {recipes.map((recipe) => (
            <li key={recipe.id}>
              <p>Recipe: {recipeNames[recipe.id]}</p>
              <p>
                Rating: {Array.from({ length: recipe.rating }, (_, index) => (
                  <span key={index} className="star-icon">
                    ⭐
                  </span>
                ))}
              </p>
              <p>Comment: {recipe.comment}</p>
                <button onClick={() => handleEditReviewClick(recipe.id)}>Edit</button>
                <button onClick={() => deleteReview(recipe.id)}>Delete</button>
              </li>
            ))}

         {/* Muokkauslomake */}
         {isEditing && (
  <div className="edit-form">
    <h2>Edit Review:</h2>
    <label>
      Rating:
      <input
        type="number"
        value={editedRating}
        onChange={(e) => setEditedRating(e.target.value)}
      />
    </label>
    <label>
      <br />
      Comment:
      <textarea
        value={editedComment}
        onChange={(e) => setEditedComment(e.target.value)}
      />
    </label>
    <br />
    <button onClick={handleSaveEdit}>Save</button>
    <button onClick={() => setEditing(false)}>Cancel</button>
  </div>
)}

          </ul>
        ) : (
          <p>This User has no reviews.</p>
        )}
      </div>
    )
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