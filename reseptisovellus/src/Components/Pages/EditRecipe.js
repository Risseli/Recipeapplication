import React, { useState, useEffect } from 'react';
import { useAuth } from '../Authentication';
import { useNavigate } from 'react-router-dom'; 
import { useParams } from 'react-router-dom';
import './EditRecipe.css';

const EditRecipe = ({ match }) => {
  const navigate = useNavigate(); 
  const { user: authUser } = useAuth();
  const [recipeData, setRecipeData] = useState({
    name: '',
    instructions: '',
    visibility: false,
    userId: authUser.id,
    ingredients: [],
    keywords: [],
    images: [],
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [selectedImages, setSelectedImages] = useState([]);

  const { recipeId } = useParams();

  useEffect(() => {
    const fetchRecipeData = async () => {
      try {
        setLoading(true);
  
        const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Recipe/${recipeId}`, {
          method: 'GET',
          headers: {
            Authorization: `Bearer ${authUser.token}`,
            'Accept': 'application/json',
          },
        });
  
        if (response.ok) {
          const data = await response.json();
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
  }, [recipeId, authUser.token]);
  


  const handleEditRecipe = async () => {
    try {
      setLoading(true);
  
      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Recipe/${recipeId}`, {
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


  const handleImageChange = (e) => {
    const file = e.target.files[0];
    const reader = new FileReader();

    reader.onloadend = () => {
      const newImage = {
        name: file.name,
        base64Data: reader.result,
      };
      setSelectedImages([...selectedImages, newImage]);
    };

    if (file) {
      reader.readAsDataURL(file);
    }
  };



  // komponentit kuvien lisäykseen ja poistoon
 /* const handleAddImage = async () => {
    try {
      setLoading(true);

      const response = await fetch('https://recipeappapi.azurewebsites.net/api/Image', {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          recipeId: recipeData.id,
          imageData: 'base64EncodedImageData', // Replace with the actual base64-encoded image data
        }),
      });

      if (response.ok) {
        console.log('Image added successfully!');
        // Perform necessary actions after adding the image
      } else {
        console.error('Failed to add image.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while adding the image.');
    } finally {
      setLoading(false);
    }
  };*/

 /* const handleDeleteImage = async (imageId) => {
    try {
      setLoading(true);

      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Image/${imageId}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
        },
      });

      if (response.ok) {
        console.log('Image deleted successfully!');
        // Perform necessary actions after deleting the image
      } else {
        console.error('Failed to delete image.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while deleting the image.');
    } finally {
      setLoading(false);
    }
  };*/





// komponentit joilla voi lisätä ja poistaa ainesosia
/*const handleAddIngredient = async () => {
    try {
      setLoading(true);

      const response = await fetch('https://recipeappapi.azurewebsites.net/api/Ingredient', {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          recipeId: recipeData.id,
          name: 'New Ingredient', // Replace with the actual ingredient name
          amount: 0, // Replace with the actual amount
          unit: 'grams', // Replace with the actual unit
        }),
      });

      if (response.ok) {
        console.log('Ingredient added successfully!');
        // Perform necessary actions after adding the ingredient
      } else {
        console.error('Failed to add ingredient.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while adding the ingredient.');
    } finally {
      setLoading(false);
    }
  };*/

 /* const handleDeleteIngredient = async (ingId) => {
    try {
      setLoading(true);
  
      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Ingredient/${ingId}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
        },
      });
  
      if (response.ok) {
        console.log('Ingredient deleted successfully!');
        // Perform necessary actions after deleting the ingredient
      } else {
        console.error('Failed to delete ingredient.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while deleting the ingredient.');
    } finally {
      setLoading(false);
    }
  };*/




  // komponentit millä lisätään reseptille avainsanoja ja poistetaan
  /*const handleAddKeyword = async (keyword) => {
    try {
      setLoading(true);

      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Recipe/${match.params.recipeId}/Keywords?keyword=${keyword}`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
        },
      });

      if (response.ok) {
        console.log('Keyword added successfully!');
        // Perform necessary actions after adding the keyword
      } else {
        console.error('Failed to add keyword.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while adding the keyword.');
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteKeyword = async (keyword) => {
    try {
      setLoading(true);

      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Recipe/${match.params.recipeId}/Keywords?keyword=${keyword}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
        },
      });

      if (response.ok) {
        console.log('Keyword deleted successfully!');
        // Perform necessary actions after deleting the keyword
      } else if (response.status === 422) {
        console.warn('Recipe does not have the specified keyword.');
      } else {
        console.error('Failed to delete keyword.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while deleting the keyword.');
    } finally {
      setLoading(false);
    }
  };*/

  





const handleAddIngredient = () => {
    setRecipeData({
      ...recipeData,
      ingredients: [...recipeData.ingredients, { name: '', amount: '', unit: '' }],
    });
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






// adding keywords and deleting keywords
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

  const handleNameInputChange = (e) => {
    setRecipeData({ ...recipeData, name: e.target.value });
  };

  const handleInstructionsInputChange = (e) => {
    setRecipeData({ ...recipeData, instructions: e.target.value });
  };

  const handleVisibilityChange = () => {
    setRecipeData({ ...recipeData, visibility: !recipeData.visibility });
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

  const handleRemoveImg = (index) => {
    const updatedImages = [...selectedImages];
    updatedImages.splice(index, 1);
    setSelectedImages(updatedImages);
  };

  return (
    <div className="container">
      <h1>Edit Recipe</h1>
      <form className="edit-recipe-section">
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
            onChange={handleVisibilityChange}
          />
        </label>
        {/* Ingredients Section */}
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
        <br />
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
              {/* Images Section */}
      <h2>Images</h2>
      <label>
        Select Image:
        <input
  type="file"
  accept="image/*"
  onChange={(e) => handleImageChange(e.target.files[0])}
/>
      </label>
      <br />

      {/* Display selected images */}
      {selectedImages.map((image, index) => (
        <div key={index}>
          <p>{image.name}</p>
          <img
            src={image.base64Data}
            alt={`Preview of ${image.name}`}
            style={{ maxWidth: '200px', maxHeight: '200px' }}
          />
          <br />
          <button
            className="remove-button"
            onClick={() => handleRemoveImg(index)}
          >
            Remove image
          </button>
        </div>
      ))}
      <br />
      <br />

        <br />
        <button className="edit-button" type="button" onClick={handleEditRecipe}>
          Save Changes
        </button>
        <button className="reset-button" type="button" onClick={handleReset}>
          Reset
        </button>
        {loading && <p>Loading...</p>}
        {error && <p style={{ color: 'red' }}>{error}</p>}
      </form>
    </div>
  );
};

export { EditRecipe };
