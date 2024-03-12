import React, { useState } from 'react';
import './AddRecipe.css';
import { useAuth } from '../Authentication';

const AddRecipe = () => {
  const { user: authUser } = useAuth();
  const [recipeData, setRecipeData] = useState({
    name: '',
    instructions: '',
    visibility: false,
    userId: authUser ? authUser.userId : 0,
    ingredients: [],
    keywords: [],
    images: [],
  });

  const [selectedImages, setSelectedImages] = useState([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleNameInputChange = (e) => {
    const { name, value } = e.target;
    setRecipeData({ ...recipeData, [name]: value });
  };

  const handleInstructionsInputChange = (e) => {
    const { name, value } = e.target;
    setRecipeData({ ...recipeData, instructions: value });
  };

  const handleIngredientChange = (index, e) => {
    const updatedIngredients = [...recipeData.ingredients];
    updatedIngredients[index][e.target.name] = e.target.value;
    setRecipeData({ ...recipeData, ingredients: updatedIngredients });
  };

  const handleKeywordChange = (index, e) => {
    const updatedKeywords = [...recipeData.keywords];
    updatedKeywords[index][e.target.name] = e.target.value;
    setRecipeData({ ...recipeData, keywords: updatedKeywords });
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

    const reader = new FileReader();
    reader.onload = (event) => {
      const base64Data = event.target.result;

      setSelectedImages([...selectedImages, { name: newSelectedImages[0].name, base64Data }]);
      setError('');
    };

    reader.readAsDataURL(newSelectedImages[0]);
  };

  const handleAddIngredient = () => {
    setRecipeData({
      ...recipeData,
      ingredients: [...recipeData.ingredients, { name: '', amount: 0, unit: 'string' }],
    });
  };

  const handleAddKeyword = () => {
    setRecipeData({ ...recipeData, keywords: [...recipeData.keywords, { word: 'string' }] });
  };

  const handleFormSubmit = async (e) => {
    e.preventDefault();

    // Form validation
    if (!recipeData.name || !recipeData.instructions || recipeData.ingredients.length === 0) {
      setError('Please fill in all required fields.');
      return;
    }

    setLoading(true);

    try {
      const formData = new FormData();
    
      // Append recipe data as a JSON string
      formData.append('recipeData', JSON.stringify(recipeData));
      
      // Append selected images
      selectedImages.forEach((image, index) => {
        formData.append(`images[${index}]`, image);
      });
      
      try {
        const response = await fetch('https://recipeappapi.azurewebsites.net/api/Recipe', {
          method: 'POST',
          headers: {
            Authorization: `Bearer ${authUser.token}`,
            'Accept': 'application/json', 
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ recipeData, images: selectedImages }),
        });
    
        console.log(response)

        if (!response.ok) {
          throw new Error('Failed to create recipe');
        }
    
        const createdRecipe = await response.json();
    
        if (!createdRecipe.id) {
          throw new Error('Recipe ID is missing or invalid.');
        }
    
        console.log('Recipe added successfully');
        handleReset();
      } catch (error) {
        console.error('Error:', error.message);
        setError('Failed to add recipe. Please try again.');
      } finally {
        setLoading(false);
      }
    } catch (outerError) {
      console.error('Outer Error:', outerError.message);
      setError('Outer error occurred. Please try again.');
    } finally {
      setLoading(false);
    }
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
        {recipeData.ingredients.map((ingredient, index) => (
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