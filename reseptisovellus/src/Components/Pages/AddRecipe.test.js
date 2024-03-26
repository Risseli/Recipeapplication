import React from 'react';
import { render, fireEvent, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';
import { AddRecipe } from './AddRecipe';
import { AuthContext } from '../Authentication'; // Import AuthContext

describe('AddRecipe Component', () => {
  // Helper function to fill the form fields
  const fillForm = (getByLabelText, getByTestId, name, instructions) => {
    fireEvent.change(getByLabelText('Name:'), { target: { value: name } });
    fireEvent.change(getByLabelText('Instructions:'), { target: { value: instructions } });
    // Fill in one ingredient
    fireEvent.change(getByTestId('ingredient-name-input-0'), { target: { value: 'Ingredient1' } });
    fireEvent.change(getByTestId('ingredient-amount-input-0'), { target: { value: '1.5' } });
    fireEvent.change(getByTestId('ingredient-unit-input-0'), { target: { value: 'kg' } });
  };

  test('renders without crashing', () => {
    const mockUser = {
      userId: 1,
      token: 'mockToken',
    };

    render(
      <AuthContext.Provider value={{ user: mockUser }}>
        <AddRecipe />
      </AuthContext.Provider>
    );
  });

  test('submit form with valid data', async () => {
    const mockUser = {
      userId: 1,
      token: 'mockToken',
    };
  
    const { getByText, getByTestId, getByLabelText } = render(
      <AuthContext.Provider value={{ user: mockUser }}>
        <AddRecipe />
      </AuthContext.Provider>
    );
  
    fillForm(getByLabelText, getByTestId, 'Test Recipe', 'Test instructions');
  
    fireEvent.click(getByText('Add Recipe'));
  
    // Expect loading message
    expect(getByText('Loading...')).toBeInTheDocument();
  
    // Wait for API call to resolve
    await waitFor(() => {
      expect(getByText('Recipe added successfully!')).toBeInTheDocument();
    });
  });
  
  test('reset form', async () => {
    const mockUser = {
      userId: 1,
      token: 'mockToken',
    };
  
    const { getByText, getByTestId, getByLabelText } = render(
      <AuthContext.Provider value={{ user: mockUser }}>
        <AddRecipe />
      </AuthContext.Provider>
    );
  
    fillForm(getByLabelText, getByTestId, 'Test Recipe', 'Test instructions');
  
    fireEvent.click(getByText('Reset'));
  
    // Expect form fields to be empty
    expect(getByTestId('name-input').value).toBe('');
    expect(getByTestId('instructions-textarea').value).toBe('');
    expect(getByTestId('ingredient-name-input-0').value).toBe('');
    expect(getByTestId('ingredient-amount-input-0').value).toBe('');
    expect(getByTestId('ingredient-unit-input-0').value).toBe('');
  });
  
  test('handle form submission failure', async () => {
    // Mock fetch to simulate failure
    jest.spyOn(global, 'fetch').mockResolvedValueOnce({
      ok: false,
      json: async () => ({ message: 'Failed to add recipe.' }),
    });
  
    const mockUser = {
      userId: 1,
      token: 'mockToken',
    };
  
    const { getByText, getByTestId, getByLabelText } = render(
      <AuthContext.Provider value={{ user: mockUser }}>
        <AddRecipe />
      </AuthContext.Provider>
    );
  
    fillForm(getByLabelText, getByTestId, 'Test Recipe', 'Test instructions');
  
    fireEvent.click(getByText('Add Recipe'));
  
    // Expect loading message
    expect(getByText('Loading...')).toBeInTheDocument();
  
    // Wait for API call to resolve
    await waitFor(() => {
      expect(getByText('An error occurred while adding the recipe.')).toBeInTheDocument();
    });
  });
});
  