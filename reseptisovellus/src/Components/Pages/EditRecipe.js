import React, { useState, useEffect } from 'react';
import { useAuth } from '../Authentication';
import { useNavigate } from 'react-router-dom';
import { useParams } from 'react-router-dom';
import './EditRecipe.css';

const EditRecipe = () => {
  const navigate = useNavigate();
  const { user: authUser } = useAuth();
  const [recipeData, setRecipeData] = useState({
    name: '',
    instructions: '',
    visibility: false,
    userId: authUser.userId,
    ingredients: [],
    keywords: [],
    images: [],
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const { id } = useParams(); 

  useEffect(() => {
    const fetchRecipeData = async () => {
      try {
        setLoading(true);
        console.log("Fetching recipe data...");
        console.log(id);

        const response = await fetch(`https://recipeappapi.azurewebsites.net/api/recipe/${id}`, {
          method: 'GET',
          headers: {
            Authorization: `Bearer ${authUser.token}`,
            'Accept': 'application/json',
          },
        });

        if (response.ok) {
          const data = await response.json();
          console.log("Recipe data fetched:", data);
          setRecipeData(data);
        } else {
          console.error('Failed to fetch recipe.');
        }
      } catch (error) {
        console.error('Error occurred:', error);
        setError('Error occurred while fetching recipe.');
      } finally {
        setLoading(false);
      }
    };

    fetchRecipeData();
  }, [id, authUser.token]);

  const handleEditRecipe = async () => {
    try {
      setLoading(true);
      console.log("Editing recipe...");

      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Recipe/${id}`, {
        method: 'PUT',
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(recipeData),
      });

      if (response.ok) {
        console.log('Recipe edited successfully!');
        alert('Recipe edited successfully!');
        window.location.href = '/profile'; 
      } else {
        console.error('Failed to edit recipe.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while editing recipe.');
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setRecipeData({ ...recipeData, [name]: value });
  };

  console.log("Rendering EditRecipe component with recipe data:", recipeData);

  return (
    <div className="container">
      <h1>Edit Recipe</h1>
      <form className="edit-recipe-section">
        <label>
          Name:
          <input type="text" name="name" value={recipeData.name} onChange={handleInputChange} />
        </label>
        <br />
        <label>
          Instructions:
          <textarea
            name="instructions"
            value={recipeData.instructions}
            onChange={handleInputChange}
          />
        </label>
        <br />
        <label>
          Recipe is visible to everyone:
          <input
            type="checkbox"
            name="visibility"
            checked={recipeData.visibility}
            onChange={() => setRecipeData({ ...recipeData, visibility: !recipeData.visibility })}
          />
        </label>
        <br />
        <button className="edit-button" type="button" onClick={handleEditRecipe}>
          Save Changes
        </button>
        {loading && <p>Loading...</p>}
        {error && <p style={{ color: 'red' }}>{error}</p>}
      </form>
    </div>
  );
};

export { EditRecipe };
