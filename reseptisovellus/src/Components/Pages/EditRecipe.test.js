import React from 'react';
import { render, fireEvent, waitFor, screen } from '@testing-library/react';
import '@testing-library/jest-dom/extend-expect';
import { MemoryRouter, Route } from 'react-router-dom';
import { EditRecipe } from './EditRecipe';

describe('EditRecipe component', () => {
  // Mocking fetch requests
  global.fetch = jest.fn(() =>
    Promise.resolve({
      json: () => Promise.resolve({ 
        name: 'Test Recipe',
        instructions: 'Test instructions',
        visibility: true,
        userId: 123,
        ingredients: [{ name: 'Ingredient 1', amount: '1', unit: 'unit' }],
        keywords: [{ word: 'Keyword' }],
        images: [{ imageData: 'base64image' }]
      }),
      ok: true,
    })
  );

  beforeEach(() => {
    fetch.mockClear();
  });

  test('renders EditRecipe component', async () => {
    render(
      <MemoryRouter initialEntries={['/edit/1']}>
        <Route path="/edit/:id">
          <EditRecipe />
        </Route>
      </MemoryRouter>
    );

    // Loading state
    expect(screen.getByText('Loading...')).toBeInTheDocument();

    // Wait for data to be loaded
    await waitFor(() => {
      expect(screen.getByLabelText('Name:')).toBeInTheDocument();
    });

    // Check if data is loaded correctly
    expect(screen.getByLabelText('Name:')).toHaveValue('Test Recipe');
    expect(screen.getByLabelText('Instructions:')).toHaveValue('Test instructions');
    expect(screen.getByLabelText('Recipe is visible to everyone:')).toBeChecked();
    expect(screen.getByLabelText('Amount:')).toHaveValue('1');
    expect(screen.getByLabelText('Unit:')).toHaveValue('unit');
    expect(screen.getByLabelText('Keyword:')).toHaveValue('Keyword');
    expect(screen.getByAltText('Preview of Test Recipe')).toBeInTheDocument();
  });

  test('handles changes and saves', async () => {
    render(
      <MemoryRouter initialEntries={['/edit/1']}>
        <Route path="/edit/:id">
          <EditRecipe />
        </Route>
      </MemoryRouter>
    );

    await waitFor(() => {
      expect(screen.getByLabelText('Name:')).toBeInTheDocument();
    });

    // Change name
    fireEvent.change(screen.getByLabelText('Name:'), { target: { value: 'New Name' } });

    // Save changes
    fireEvent.click(screen.getByText('Save Changes'));

    // Wait for save action
    await waitFor(() => {
      expect(screen.getByText('Recipe edited successfully!')).toBeInTheDocument();
    });

    // Ensure fetch is called with proper data
    expect(fetch).toHaveBeenCalledWith('https://recipeappapi.azurewebsites.net/api/Recipe/1', expect.any(Object));
    expect(fetch).toHaveBeenCalledWith('https://recipeappapi.azurewebsites.net/api/Ingredient', expect.any(Object));
    expect(fetch).toHaveBeenCalledWith('https://recipeappapi.azurewebsites.net/api/Recipe/1/Keywords?keyword=Keyword', expect.any(Object));
    expect(fetch).toHaveBeenCalledWith('https://recipeappapi.azurewebsites.net/api/Image', expect.any(Object));
  });
});
