import React, { useState, useEffect } from 'react';
import { useAuth } from '../Authentication';
import { useParams } from 'react-router-dom';
import './EditRecipe.css';

const EditRecipe = () => {
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

  const handleIngredientChange = (index, e) => {
    const updatedIngredients = [...recipeData.ingredients];
    updatedIngredients[index][e.target.name] = e.target.value;
    setRecipeData({ ...recipeData, ingredients: updatedIngredients });
  };

  const handleRemoveIngredient = (index) => {
    const updatedIngredients = [...recipeData.ingredients];
    updatedIngredients.splice(index, 1);
    setRecipeData({ ...recipeData, ingredients: updatedIngredients });
  };

  const handleAddIngredient = () => {
    setRecipeData({
      ...recipeData,
      ingredients: [...recipeData.ingredients, { name: '', amount: '', unit: '' }],
    });
  };

  const handleKeywordChange = (index, e) => {
    const updatedKeywords = [...recipeData.keywords];
    updatedKeywords[index][e.target.name] = e.target.value;
    setRecipeData({ ...recipeData, keywords: updatedKeywords });
  };

  const handleRemoveKeyword = (index) => {
    const updatedKeywords = [...recipeData.keywords];
    updatedKeywords.splice(index, 1);
    setRecipeData({ ...recipeData, keywords: updatedKeywords });
  };

  const handleAddKeyword = () => {
    setRecipeData({
      ...recipeData,
      keywords: [...recipeData.keywords, { word: '' }],
    });
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    const reader = new FileReader();

    reader.onloadend = () => {
      const newImage = {
        name: file.name,
        imageData: reader.result,
      };
      setRecipeData({ ...recipeData, images: [...recipeData.images, newImage] });
    };

    if (file) {
      reader.readAsDataURL(file);
    }
  };

  const handleRemoveImg = (index) => {
    const updatedImages = [...recipeData.images];
    updatedImages.splice(index, 1);
    setRecipeData({ ...recipeData, images: updatedImages });
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
        <div className="ingredient-section">
        <h2>Ingredients</h2>
        {recipeData.ingredients.map((ingredient, index) => (
          <div key={index}>
            <label>
              Name:
              <br />
              <input
                type="text"
                name="name"
                value={ingredient.name}
                onChange={(e) => handleIngredientChange(index, e)}
              />
            </label>
            <label>
              Amount:
              <br />
              <input
                type="text"
                name="amount"
                value={ingredient.amount}
                onChange={(e) => handleIngredientChange(index, e)}
              />
            </label>
            <label>
              Unit:
              <br />
              <input
                type="text"
                name="unit"
                value={ingredient.unit}
                onChange={(e) => handleIngredientChange(index, e)}
              />
            </label>
            <button className="remove-button" onClick={() => handleRemoveIngredient(index)}>
              Remove ingredient
            </button>
          </div>
        ))}
        <button className="add-button" type="button" onClick={handleAddIngredient}>
          Add Ingredient
        </button>
        </div>
        <h2>Keywords</h2>
        {recipeData.keywords.map((keyword, index) => (
          <div key={index}>
            <label>
              Keyword:
              <input
                type="text"
                name="word"
                value={keyword.word}
                onChange={(e) => handleKeywordChange(index, e)}
              />
            </label>
            <br />
            <button className="remove-button" onClick={() => handleRemoveKeyword(index)}>
              Remove keyword
            </button>
          </div>
        ))}
        <button className="add-button" type="button" onClick={handleAddKeyword}>
          Add Keyword
        </button>
        <h2>Images</h2>
        Select Image:
  <label>
    <input type="file" accept="image/*" onChange={handleImageChange} />
  </label>
  <br />
  {recipeData.images.map((image, index) => (
    <div key={index}>
      <p>{image.name}</p>
      <img
        src={`data:image/jpeg;base64,${image.imageData}`}
        alt={`Preview of ${image.name}`}
        style={{ maxWidth: '200px', maxHeight: '200px' }}
      />
      <br />
      <button className="remove-button" onClick={() => handleRemoveImg(index)}>Remove image</button>
    </div>
  ))}
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
