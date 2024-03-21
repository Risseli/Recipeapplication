import React, { useState } from 'react';
import { useAuth } from '../Authentication';
import './AddRecipe.css';

const AddRecipe = () => {
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
  const [recipeImages, setRecipeImages] = useState([]);

  const [selectedImages, setSelectedImages] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleFormSubmit = async (e) => {
    e.preventDefault();
    await handleAddRecipe();
  };

  const handleAddRecipe = async () => {
    try {
      setLoading(true);

      const response = await fetch('https://localhost:7005/api/Recipe', { // https://recipeappapi.azurewebsites.net/api/Recipe
        method: 'POST',
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({...recipeData, images: recipeImages}),
      });

      if (response.ok) {
        console.log('Recipe added successfully!');
        alert('Recipe added successfully!');
        handleReset();
        window.location.href = '/profile'; 
      } else {
        console.error('Failed to add recipe.');
      }
    } catch (error) {
      console.error('An error occurred:', error);
      setError('An error occurred while adding the recipe.');
    } finally {
      setLoading(false);
    }
  };

  const handleNameInputChange = (e) => {
    setRecipeData({ ...recipeData, name: e.target.value });
  };

  const handleInstructionsInputChange = (e) => {
    setRecipeData({ ...recipeData, instructions: e.target.value });
  };

  const handleIngredientChange = (index, e) => {
    const updatedIngredients = [...recipeData.ingredients];
    // Convert commas to periods in the amount field
    if (e.target.name === 'amount') {
      const newValue = e.target.value.replace(',', '.');
      updatedIngredients[index][e.target.name] = newValue;
    } else {
      updatedIngredients[index][e.target.name] = e.target.value;
    }
    setRecipeData({ ...recipeData, ingredients: updatedIngredients });
  };
  
  const handleImageChange = (e) => {
    const file = e.target.files[0];
    const reader = new FileReader();

    reader.onloadend = () => {
      const newImage = {
        name: file.name,
        imageData: reader.result,
      };
      setSelectedImages([...selectedImages, newImage]);
      setRecipeImages([...recipeImages, { imageData: newImage.imageData.split(",")[1] }])
    };

    if (file) {
      reader.readAsDataURL(file);
    }
  };

  const handleAddIngredient = () => {
    setRecipeData({
      ...recipeData,
      ingredients: [...recipeData.ingredients, { name: '', amount: '', unit: '' }],
    });
  };

  const handleAddKeyword = () => {
    setRecipeData({
      ...recipeData,
      keywords: [...recipeData.keywords, { word: '' }],
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

  const handleRemoveImg = (e) => {
    e.preventDefault();
    const updatedSelectedImages = [...selectedImages];
    updatedSelectedImages.pop();
    setSelectedImages(updatedSelectedImages);
  };

  const handleRemoveIngredient = (index) => {
    const updatedIngredients = [...recipeData.ingredients];
    updatedIngredients.splice(index, 1);
    setRecipeData({ ...recipeData, ingredients: updatedIngredients });
  };


  const handleReset = () => {
    setRecipeData({
      name: '',
      instructions: '',
      visibility: false,
      userId: authUser ? authUser.userId : 0,
      ingredients: [],
      keywords: [],
      images: [],
    });
    setSelectedImages([]);
    setError('');
  };

  return (
    <div className="add-recipe-container">
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
          Recipe is visible to everyone: 
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
        {/* Ingredients */}
        <div className="ingredient-section">
        <h2>Ingredients</h2>
        {recipeData.ingredients.map((ingredient, index) => (
          <div key={index}>
            <label>
              Name:
              <br/>
              <input
                type="text"
                name="name"
                value={ingredient.name}
                onChange={(e) => handleIngredientChange(index, e)}
              />
            </label>
            <label>
              Amount:
              <br/>
              <input
                type="text"
                name="amount"
                value={ingredient.amount}
                onChange={(e) => handleIngredientChange(index, e)}
              />
            </label>
            <label>
              Unit:
              <br/>
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
        {/* Keywords */}
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
        <label>
          Select Image:
          <input type="file" accept="image/*" onChange={handleImageChange} />
        </label>
        <br />
        {/* Display selected images */}
        {selectedImages.map((image, index) => (
          <div key={index}>
            <p>{image.name}</p>
            <img
              src={image.imageData}
              alt={`Preview of ${image.name}`}
              style={{ maxWidth: '200px', maxHeight: '200px' }}
            />
            <br/>
            <button className="remove-button" onClick={(e) => handleRemoveImg(e)}>Remove image</button>
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
