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
        const response = await fetch(`https://recipeappapi.azurewebsites.net/api/recipe/${id}`, {
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
  }, [id, authUser.token]);

  const handleEditRecipe = async () => {
    try {
      setLoading(true);

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
        alert('Recipe edited successfully!');
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



  // ingredients
  const handleAddIngredient = () => {
    setRecipeData({
      ...recipeData,
      ingredients: [...recipeData.ingredients, { name: '', amount: '', unit: '' }],
    });
  };

  const handleIngredientChange = (index, field, value) => {
    const updatedIngredients = [...recipeData.ingredients];
    updatedIngredients[index][field] = value;
    setRecipeData({ ...recipeData, ingredients: updatedIngredients });
  };

  const handleSaveIngredient = async () => {
    try {
      setLoading(true);
      console.log("Saving ingredient changes...");
  
      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Ingredient`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ recipeId: id, ingredients: recipeData.ingredients }),
      });
  
      if (response.ok) {
        console.log('Ingredient changes saved successfully!');
        alert('Ingredient changes saved successfully!');
        window.location.reload();
      } else {
        console.error('Failed to save ingredient changes.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while saving ingredient changes.');
    } finally {
      setLoading(false);
    }
  };

  const handleRemoveIngredient = async (ingredient) => {
    try {
      setLoading(true);
  
      if (ingredient.recipeId === recipeData.recipeId) {
        const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Ingredient/${ingredient.id}`, {
          method: 'DELETE',
          headers: {
            Authorization: `Bearer ${authUser.token}`,
            'Accept': 'application/json',
          },
        });
  
        if (response.ok) {
          const updatedIngredients = recipeData.ingredients.filter(item => item.id !== ingredient.id);
          setRecipeData({ ...recipeData, ingredients: updatedIngredients });
          alert('Ingredient removed successfully!');
        } else {
          console.error('Failed to remove ingredient.');
        }
      } else {
        console.error('Recipe ID mismatch. Unable to remove ingredient.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while removing ingredient.');
    } finally {
      setLoading(false);
    }
  };
  


  
  //images
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

  const handleSaveImages = async () => {
    try {
      setLoading(true);
      console.log("Saving image changes...");
  
      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Image`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ recipeId: id, images: recipeData.images }),
      });
  
      if (response.ok) {
        console.log('Image changes saved successfully!');
        alert('Image changes saved successfully!');
        window.location.reload();
      } else {
        console.error('Failed to save image changes.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while saving image changes.');
    } finally {
      setLoading(false);
    }
  };


  const handleRemoveImg = async (index) => {
    try {
      setLoading(true);
      console.log("Removing image...");
  
      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Image/${recipeData.images[index].id}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
        },
      });
  
      if (response.ok) {
        console.log('Image removed successfully!');
        alert('Image removed successfully!');
        const updatedImages = [...recipeData.images];
        updatedImages.splice(index, 1);
        setRecipeData({ ...recipeData, images: updatedImages });
        window.location.reload();
      } else {
        console.error('Failed to remove image.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while removing image.');
    } finally {
      setLoading(false);
    }
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

  const handleRemoveKeyword = async (keyword) => {
    try {
      setLoading(true);

      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Recipe/${id}/Keywords?keyword=${keyword.word}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
        },
      });

      if (response.ok) {
        alert('Keyword changes saved successfully!');
       const updatedKeywords = [...recipeData.keywords];
       setRecipeData({ ...recipeData, keywords: updatedKeywords });
       window.location.reload();

      } else {
        console.error('Failed to remove keyword.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while removing keyword.');
    } finally {
      setLoading(false);
    }
  };

  const handleSaveKeyword = async (keyword) => {
    try {
      setLoading(true);

      const response = await fetch(`https://recipeappapi.azurewebsites.net/api/Recipe/${id}/Keywords?keyword=${keyword.word}`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
        },
      });
  
      if (response.ok) {
        alert('Keyword changes saved successfully!');
      } else {
        console.error('Failed to save keyword changes.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while saving keyword changes.');
    } finally {
      setLoading(false);
    }
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
        <br />
        <button className="edit-button" type="button" onClick={handleEditRecipe}>
          Save Changes
        </button>
        <br />
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
          value={ingredient.name}
          onChange={(e) => handleIngredientChange(index, 'name', e.target.value)}
        />
      </label>
      <label>
        Amount:
        <br />
        <input
          type="text"
          value={ingredient.amount}
          onChange={(e) => handleIngredientChange(index, 'amount', e.target.value)}
        />
      </label>
      <label>
        Unit:
        <br />
        <input
          type="text"
          value={ingredient.unit}
          onChange={(e) => handleIngredientChange(index, 'unit', e.target.value)}
        />
      </label>
      <button className="remove-button" type="button" onClick={() => handleRemoveIngredient(ingredient, index)}>
        Remove ingredient
      </button>
      <button className="save-button" type="button" onClick={() => handleSaveIngredient(index)}>
        Save ingredient changes
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
            <button className="remove-button" type="button" onClick={() => handleRemoveKeyword(keyword)}>
      Remove keyword
    </button>
            <button
      className="save-button"
      type="button"
      onClick={() => handleSaveKeyword(keyword)}>Save keyword changes</button>
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
      <button className="save-button" type="button" onClick={handleSaveImages}>
          Save image changes
        </button>
    </div>
  ))}
<br />
        {loading && <p>Loading...</p>}
        {error && <p style={{ color: 'red' }}>{error}</p>}
      </form>
    </div>
  );
};

export { EditRecipe };
