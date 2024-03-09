import React, { useState } from 'react';
import './AddRecipe.css';
import { useAuth } from "../Authentication";

const AddRecipe = () => {

  const { user: authUser } = useAuth();
  const [recipeData, setRecipeData] = useState({
    name: '',
    instructions: '',
    visibility: false,
    userId: authUser ? authUser.userId : null,
  });

  const [ingredients, setIngredients] = useState([]);
  const [keywords, setKeywords] = useState([]);
  const [selectedImages, setSelectedImages] = useState([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false); 

  const handleNameInputChange = (e) => {
    const { name, value } = e.target;
    setRecipeData({ ...recipeData, [name]: value});
  };

  const handleInstructionsInputChange = (e) => {
    const { name, value } = e.target;
    setRecipeData({ ...recipeData, instructions: value });
  };


  const handleIngredientChange = (index, e) => {
    const updatedIngredients = [...ingredients];
    updatedIngredients[index][e.target.name] = e.target.value;
    setIngredients(updatedIngredients);
  };

  const handleKeywordChange = (index, e) => {
    const updatedKeywords = [...keywords];
    updatedKeywords[index][e.target.name] = e.target.value;
    setKeywords(updatedKeywords);
  };

  const handleImageChange = (e) => {
    const files = e.target.files;
    const newSelectedImages = [];
  
    for (let i = 0; i < files.length; i++) {
      const file = files[i];
  
      // File type validation
      if (!file.type.startsWith('image/')) {
        setError('Please select valid image files.');
        setSelectedImages([]);
        return;
      } else {
        newSelectedImages.push(file);
      }
    }
  
    setSelectedImages([...selectedImages, ...newSelectedImages]);
    setError('');
  };

  const handleAddIngredient = () => {
    setIngredients([...ingredients, { name: '', amount: '', unit: '' }]);
  };

  const handleAddKeyword = () => {
    setKeywords([...keywords, { word: '' }]);
  };

  const handleFormSubmit = async (e) => {
    e.preventDefault();

    // Form validation
    if (!recipeData.name || !recipeData.instructions) {
      setError('Please fill in all required fields.');
      return;
    }

     // Tarkista, että authUser ei ole falsy-arvo ennen kuin käytät authUser.userId
     const userId = authUser ? authUser.userId : null;
     setRecipeData((prevRecipeData) => ({ ...prevRecipeData, userId }));

    setLoading(true);

    try {
      // Create the recipe
      const response = await fetch('https://recipeappapi.azurewebsites.net/api/Recipe', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${authUser?.token}`,
        },
        body: JSON.stringify({
          ...recipeData,
          ingredients,
        }),
      });

      if (!response.ok) {
        throw new Error('Failed to create recipe');
      }

      const recipeId = await response.json();

      // Upload images
      if (selectedImages) {
        const imageData = new FormData();
        imageData.append('recipeId', recipeId);
        imageData.append('imageData', selectedImages);

        const imageResponse = await fetch('https://recipeappapi.azurewebsites.net/api/Image', {
          method: 'POST',
          body: imageData,
          headers: {
            Authorization: `Bearer ${authUser?.token}`,
          },
        });

        if (!imageResponse.ok) {
          throw new Error('Failed to upload image');
        }
      }

      // Add keywords to the recipe
      await Promise.all(
        keywords.map(async (keyword) => {
          const keywordResponse = await fetch(
            `https://recipeappapi.azurewebsites.net/api/Recipe/${recipeId}/Keywords?keyword=${encodeURIComponent(
              keyword.word.trim().toLowerCase()
            )}`,
            {
              method: 'POST',
              headers: {
                Authorization: `Bearer ${authUser?.token}`,
              },
            }
          );

          if (!keywordResponse.ok) {
            console.error(`Failed to add keyword: ${keyword.word}`);
          }
        })
      );

      // Handle success, e.g., redirect to the recipe list page
      console.log('Recipe added successfully');
      // Clear form fields
      handleReset();
      // Redirect or display success message
      window.location.href = '/profile';
    } catch (error) {
      console.error('Error:', error.message);
      setError('Failed to add recipe. Please try again.');
    } finally {
      setLoading(false);
    }
  };


  const handleReset = () => {
    // Reset all form fields
    setRecipeData({
      name: '',
      instructions: '',
      visibility: false,
      userId: 4,
    });
    setIngredients([]);
    setKeywords([]);
    setSelectedImages([]);
    setError('');
  };




  return (
    <div className="container">
      <h1>Add New Recipe</h1>
      <form className="add-recipe-section" onSubmit={handleFormSubmit}>
        {/* Recipe details */}
        <label>
          Name:
          <input type="text" name="name" value={recipeData.name} onChange={handleNameInputChange} />
        </label>
        <br />
        <label>
          Instructions:
          <textarea
            name="instructions"
            value={recipeData.instructions}
            onChange={handleInstructionsInputChange}
          />
        </label>
        <br />
        <label>
          Visibility:
          <input
            type="checkbox"
            name="visibility"
            checked={recipeData.visibility}
            onChange={() =>
              setRecipeData({
                ...recipeData,
                visibility: !recipeData.visibility,
              })
            }
          />
        </label>
        <br />

        {/* Ingredients */}
        <h2>Ingredients</h2>
        {ingredients.map((ingredient, index) => (
          <div key={index}>
            <label>
              Name:
              <input
                type="text"
                name="name"
                value={ingredient.name}
                onChange={(e) => handleIngredientChange(index, e)}
              />
            </label>
            <br />
            <label>
              Amount:
              <input
                type="text"
                name="amount"
                value={ingredient.amount}
                onChange={(e) => handleIngredientChange(index, e)}
              />
            </label>
            <br />
            <label>
              Unit:
              <input
                type="text"
                name="unit"
                value={ingredient.unit}
                onChange={(e) => handleIngredientChange(index, e)}
              />
            </label>
            <br />
          </div>
        ))}
        <button className="add-button" type="button" onClick={handleAddIngredient}>
          Add Ingredient
        </button>
        <br />

        {/* Keywords */}
        <h2>Keywords</h2>
        {keywords.map((keyword, index) => (
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
          </div>
        ))}
        <button className="add-button" type="button" onClick={handleAddKeyword}>
          Add Keyword
        </button>
        <h2>Images</h2>
        <label>
          Select Image:
          <input type="file" accept="image/*" onChange={handleImageChange} />
        </label>
        <br />
        {/* Display selected images */}
        {selectedImages.map((image, index) => (
          <div key={index}>
            <p>{image.name}</p>
          </div>
        ))}
        <br />
        <br />

        <button className="add-button" type="submit">
          Add Recipe
        </button>
        <button className="reset-button" onClick={handleReset}>
          Reset
        </button>
        {loading && <p>Loading...</p>}
        {error && <p style={{ color: 'red' }}>{error}</p>}
      </form>
    </div>
  );
};

export { AddRecipe };

