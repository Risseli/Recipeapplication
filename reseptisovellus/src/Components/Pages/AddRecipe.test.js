import React from 'react';
import { render, fireEvent, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom/extend-expect';
import { AddRecipe } from './AddRecipe';

describe('AddRecipe Component', () => {
  // Mocking the useAuth hook
  jest.mock('../Authentication', () => ({
    useAuth: jest.fn(() => ({
      user: {
        userId: 1,
        token: 'mockToken',
      },
    })),
  }));

  // Helper function to fill the form fields
  const fillForm = (getByLabelText, getByTestId, name, instructions) => {
    fireEvent.change(getByLabelText('Name:'), { target: { value: name } });
    fireEvent.change(getByLabelText('Instructions:'), { target: { value: instructions } });
    // Fill in one ingredient
    fireEvent.change(getByTestId('ingredient-name-input-0'), { target: { value: 'Ingredient 1' } });
    fireEvent.change(getByTestId('ingredient-amount-input-0'), { target: { value: '1.5' } });
    fireEvent.change(getByTestId('ingredient-unit-input-0'), { target: { value: 'kg' } });
  };

  test('renders without crashing', () => {
    render(<AddRecipe />);
  });

  test('submit form with valid data', async () => {
    const { getByText, getByLabelText, getByTestId } = render(<AddRecipe />);

    fillForm(getByLabelText, getByTestId, 'Test Recipe', 'Test instructions');

    fireEvent.click(getByText('Add Recipe'));

    // Expect loading message
    expect(getByText('Loading...')).toBeInTheDocument();

    // Wait for API call to resolve
    await waitFor(() => {
      expect(getByText('Recipe added successfully!')).toBeInTheDocument();
    });
  });

  test('handle form submission failure', async () => {
    // Mock fetch to simulate failure
    jest.spyOn(global, 'fetch').mockResolvedValueOnce({
      ok: false,
      json: async () => ({ message: 'Failed to add recipe.' }),
    });

    const { getByText, getByLabelText, getByTestId } = render(<AddRecipe />);

    fillForm(getByLabelText, getByTestId, 'Test Recipe', 'Test instructions');

    fireEvent.click(getByText('Add Recipe'));

    // Expect loading message
    expect(getByText('Loading...')).toBeInTheDocument();

    // Wait for API call to resolve
    await waitFor(() => {
      expect(getByText('An error occurred while adding the recipe.')).toBeInTheDocument();
    });
  });

  test('reset form', async () => {
    const { getByText, getByLabelText, getByTestId } = render(<AddRecipe />);

    fillForm(getByLabelText, getByTestId, 'Test Recipe', 'Test instructions');

    fireEvent.click(getByText('Reset'));

    // Expect form fields to be empty
    expect(getByLabelText('Name:').value).toBe('');
    expect(getByLabelText('Instructions:').value).toBe('');
    expect(getByTestId('ingredient-name-input-0').value).toBe('');
    expect(getByTestId('ingredient-amount-input-0').value).toBe('');
    expect(getByTestId('ingredient-unit-input-0').value).toBe('');
  });
});
