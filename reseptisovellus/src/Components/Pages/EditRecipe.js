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
  const [recipeImages, setRecipeImages] = useState([]);
  const [selectedImages, setSelectedImages] = useState([]);
 

  useEffect(() => {
    const fetchRecipeData = async () => {
      try {
        setLoading(true);
        const response = await fetch(`https://localhost:7005/api/Recipe/${id}`, { // https://recipeappapi.azurewebsites.net/api/recipe/${id}
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

      const response = await fetch(`https://localhost:7005/api/Recipe/${id}`, { // https://recipeappapi.azurewebsites.net/api/Recipe/${id}
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
      ingredients: [...recipeData.ingredients, { name: '', amount: '', unit: '', new:true}],
    });
  };

  const handleIngredientChange = (index, field, value) => {
    const updatedIngredients = [...recipeData.ingredients];
    // Convert commas to periods in the amount field
    if (field === 'amount') {
      const newValue = value.replace(',', '.');
      updatedIngredients[index][field] = newValue;
    } else {
      updatedIngredients[index][field] = value;
    }
    setRecipeData({ ...recipeData, ingredients: updatedIngredients });
  };

  const handleSaveIngredient = async (ingredient) => {
    try {
      setLoading(true);
      console.log("Saving ingredient changes...");
  
      let url = 'https://localhost:7005/api/Ingredient'; // https://recipeappapi.azurewebsites.net/api/Ingredient
      let method = 'POST';
      let body = {
        recipeId: id,
        name: ingredient.name,
        amount: ingredient.amount,
        unit: ingredient.unit
      };
  
      // Jos ainesosa on uusi, vaihdetaan HTTP-metodi ja URL
      if (ingredient.new) {
        method = 'POST';
        body.id = 0; // Id-kenttä on joko tyhjä tai 0 uudelle ainesosalle
      } else {
        // Jos ainesosa on vanha, päivitetään sen tiedot PUT-pyynnöllä
        method = 'PUT';
        url += `/${ingredient.id}`;
        body.id = ingredient.id; // Id-kenttä pysyy samana vanhalle ainesosalle
      }
  
      const response = await fetch(url, {
        method: method,
        headers: {
          Authorization: `Bearer ${authUser.token}`,
          'Accept': 'application/json',
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(body),
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
  
      // Tarkistetaan, onko poistettava ainesosa uusi vai vanha
      if (!ingredient.new) {
        // Ainesosan poisto DELETE-pyynnöllä
        const response = await fetch(`https://localhost:7005/api/Ingredient/${ingredient.id}`, { // https://recipeappapi.azurewebsites.net/api/Ingredient/${ingredient.id}
          method: 'DELETE',
          headers: {
            Authorization: `Bearer ${authUser.token}`,
            'Accept': 'application/json',
          },
        });
  
        if (response.ok) {
          // Ainesosan poisto onnistui
          const updatedIngredients = recipeData.ingredients.filter(item => item.id !== ingredient.id);
          setRecipeData({ ...recipeData, ingredients: updatedIngredients });
          alert('Ingredient removed successfully!');
        } else {
          console.error('Failed to remove ingredient.');
        }
      } else {
        console.error('Cannot remove a new ingredient.');
      }
    } catch (error) {
      console.error('Error occurred:', error);
      setError('Error occurred while removing ingredient.');
    } finally {
      setLoading(false);
    }
  };
  
// keywords
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

      const response = await fetch(`https://localhost:7005/api/Recipe/${id}/Keywords?keyword=${keyword.word}`, { // https://recipeappapi.azurewebsites.net/api/Recipe/${id}/Keywords?keyword=${keyword.word}
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

      const response = await fetch(`https://localhost:7005/api/Recipe/${id}/Keywords?keyword=${keyword.word}`, { // https://recipeappapi.azurewebsites.net/api/Recipe/${id}/Keywords?keyword=${keyword.word}
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
  
  

//images
  const handleImageChange = (e) => {
    const file = e.target.files[0];
    const reader = new FileReader();
    
    reader.onloadend = () => {
      const newImage = {
        name: file.name,
        id: '0',
        recipeId: id,
        imageData: reader.result, // Tallennetaan Base64-muotoinen data
        new: true
      };
      setSelectedImages([...selectedImages, newImage]);
      // Lisätään uusi kuva recipeImages-taulukkoon
      setRecipeImages([...recipeImages, newImage]);
    };
    
    if (file) {
      reader.readAsDataURL(file);
    }
  };
  
  
  const handleSaveImages = async () => {
    try {
      setLoading(true);
      console.log("Saving image changes...");
  
      for (const image of recipeImages) {
        let url = 'https://localhost:7005/api/Image'; // https://recipeappapi.azurewebsites.net/api/Image
        let method = 'POST';
        let body = {
          id: image.id, // Otetaan kuvan id
          recipeId: image.recipeId, // Otetaan reseptin id
          imageData: image.imageData.substring(23) // Poistetaan "data:image/jpeg;base64,"
        };
  
        // Jos kuva on uusi, vaihdetaan HTTP-metodi ja URL
        if (image.new === true) {
          method = 'POST';
        } else {
          // Jos kuva on vanha, päivitetään sen tiedot PUT-pyynnöllä
          method = 'PUT';
          url += `/${image.id}`;
        }
  
        const response = await fetch(url, {
          method: method,
          headers: {
            Authorization: `Bearer ${authUser.token}`,
            'Accept': 'application/json',
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(body),
        });
  
        if (!response.ok) {
          console.error('Failed to save image changes.');
          return; // Jos jokin kuvaepäonnistuu, keskeytetään käsittely
        }
      }
  
      // Kaikki kuvat tallennettu onnistuneesti
      console.log('All image changes saved successfully!');
      alert('All image changes saved successfully!');
      window.location.reload();
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
  
      const response = await fetch(`https://localhost:7005/api/Image/${index.id}`, { // https://recipeappapi.azurewebsites.net/api/Image/${index.id}
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


  console.log("Rendering EditRecipe component with recipe data:", recipeData);

  return (
    <div className="edit-recipe-container">
      <h1>Edit Recipe</h1>
      <form className="edit-recipe-section"data-testid="edit-recipe-section">
        <label>
          Name:
          <input type="text" name="name" value={recipeData.name} onChange={handleInputChange}data-testid="recipe-name-input" />
        </label>
        <br />
        <label>
          Instructions:
          <textarea
            name="instructions"
            value={recipeData.instructions}
            onChange={handleInputChange}
            data-testid="recipe-instructions-textarea"
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
            data-testid="recipe-visibility-checkbox"
          />
        </label>
        <br />
        <br />
        <button className="edit-button" type="button" onClick={handleEditRecipe}data-testid="save-changes-button">
          Save Changes
        </button>
        <br />
        <br />
        <div className="ingredient-section"data-testid="ingredient-section">
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
          data-testid={`ingredient-name-input-${index}`}
        />
      </label>
      <label>
        Amount:
        <br />
        <input
          type="text"
          value={ingredient.amount}
          onChange={(e) => handleIngredientChange(index, 'amount', e.target.value)}
          data-testid={`ingredient-amount-input-${index}`}
        />
      </label>
      <label>
        Unit:
        <br />
        <input
          type="text"
          value={ingredient.unit}
          onChange={(e) => handleIngredientChange(index, 'unit', e.target.value)}
          data-testid={`ingredient-unit-input-${index}`}
        />
      </label>
      <button className="remove-button" type="button" onClick={() => handleRemoveIngredient(ingredient, index)}data-testid="remove-ingredient-button">
        Remove ingredient
      </button>
      <button className="save-button" type="button" onClick={() => handleSaveIngredient(ingredient, index)}data-testid="save-ingredient-button">
        Save ingredient changes
      </button>
    </div>
  ))}
  <button className="add-button" type="button" onClick={handleAddIngredient}data-testid="add-ingredient-button">
    Add Ingredient
  </button>
</div> 
        <div className="keywords-section"data-testid="keywords-section">
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
                data-testid="keyword-input"
              />
            </label>
            <br />
            <button className="remove-button" type="button" onClick={() => handleRemoveKeyword(keyword)}data-testid="remove-keyword-button">
      Remove keyword
    </button>
            <button
      className="save-button"
      type="button"
      onClick={() => handleSaveKeyword(keyword)}data-testid="save-keyword-button">Save keyword changes</button>
          </div>
        ))}
        <button className="add-button" type="button" onClick={handleAddKeyword}data-testid="add-keyword-button">
          Add Keyword
        </button>
        </div>
        <div className="images-section" data-testid="images-section">
  <h2>Images</h2>
  Select Image:
  <label>
    <input type="file" accept="image/*" onChange={handleImageChange} data-testid="image-upload-input" />
  </label>
  <br />
  {/* Display selected images */}
  {selectedImages.map((image, index) => (
    <div key={index} data-testid={`selected-image-${index}`}>
      <img
        src={image.imageData}
        alt={`Preview of ${image.name}`}
        style={{ maxWidth: '200px', maxHeight: '200px' }}
      />
      <br/>
      <button className="remove-button" onClick={(e) => handleRemoveImg(e)} data-testid={`remove-selected-image-${index}`}>
        Remove image
      </button>
      <button className="save-button" type="button" onClick={() => handleSaveImages(image, index)} data-testid={`save-selected-image-${index}`}>
        Save image changes
      </button>
    </div>
  ))}
  <br />
  <br />
  {/* Display images from recipe */}
  {recipeData.images.map((image, index) => (
    <div key={index} data-testid={`recipe-image-${index}`}>
      <img
        src={`data:image/jpeg;base64,${image.imageData}`}
        alt={`Preview of ${image.name}`}
        style={{ maxWidth: '200px', maxHeight: '200px' }}
      />
      <br />
      <button className="remove-button" type="button" onClick={() => handleRemoveImg(image, index)} data-testid={`remove-recipe-image-${index}`}>
        Remove image
      </button>
      <button className="save-button" type="button" onClick={() => handleSaveImages(image, index)} data-testid={`save-recipe-image-${index}`}>
        Save image changes
      </button>
    </div>
  ))}
  <br />
</div>
{loading && <p>Loading...</p> }
{error && <p style={{ color: 'red' }}>{error}</p>}

      </form>
    </div>
  );
};

export { EditRecipe };