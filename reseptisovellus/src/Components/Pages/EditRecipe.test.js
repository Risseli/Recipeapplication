import React from 'react';
import { render, fireEvent, waitFor, screen } from '@testing-library/react';
import { EditRecipe } from './EditRecipe';
import { AuthContext } from '../Authentication';



describe('EditRecipe component', () => {
  test('renders EditRecipe component', async () => {
    const mockUser = {
      userId: 1,
      token: 'mockToken',
    };
    render(
      
        <AuthContext.Provider value={{ user: mockUser }}>
          <EditRecipe />
        </AuthContext.Provider>
     
    );

    // Check if the component renders
    expect(screen.getByText('Edit Recipe')).toBeInTheDocument();

    // Mocking API response
    global.fetch = jest.fn().mockResolvedValueOnce({
      ok: true,
      json: async () => ({
        name: 'Test Recipe',
        instructions: 'Test instructions',
        visibility: true,
        userId: 'user123',
        ingredients: [],
        keywords: [],
        images: [],
      }),
    });

    // Wait for the API call to resolve
    await waitFor(() => expect(screen.getByTestId('recipe-name-input')).toHaveValue('Test Recipe'));
    expect(screen.getByTestId('recipe-instructions-textarea')).toHaveValue('Test instructions');
    expect(screen.getByTestId('recipe-visibility-checkbox')).toBeChecked();
  });

  test('handles input change', async () => {
    const mockUser = {
      userId: 1,
      token: 'mockToken',
    };
    render(
      
        <AuthContext.Provider value={{ user: mockUser }}>
          <EditRecipe />
        </AuthContext.Provider>
     
    );

    // Mocking API response
    global.fetch = jest.fn().mockResolvedValueOnce({
      ok: true,
      json: async () => ({
        name: '',
        instructions: '',
        visibility: false,
        userId: 'user123',
        ingredients: [],
        keywords: [],
        images: [],
      }),
    });

    await waitFor(() => {
      fireEvent.change(screen.getByTestId('recipe-name-input'), { target: { value: 'New Test Recipe' } });
      fireEvent.change(screen.getByTestId('recipe-instructions-textarea'), { target: { value: 'New test instructions' } });
      fireEvent.click(screen.getByTestId('recipe-visibility-checkbox'));
    });

    expect(screen.getByTestId('recipe-name-input')).toHaveValue('New Test Recipe');
    expect(screen.getByTestId('recipe-instructions-textarea')).toHaveValue('New test instructions');
    expect(screen.getByTestId('recipe-visibility-checkbox')).toBeChecked();
  });

  test('handles adding and removing ingredients', async () => {
    const mockUser = {
      userId: 1,
      token: 'mockToken',
    };
    render(
      
        <AuthContext.Provider value={{ user: mockUser }}>
          <EditRecipe />
        </AuthContext.Provider>
      
    );

    // Mocking API response
    global.fetch = jest.fn().mockResolvedValueOnce({
      ok: true,
      json: async () => ({
        name: '',
        instructions: '',
        visibility: false,
        userId: 'user123',
        ingredients: [],
        keywords: [],
        images: [],
      }),
    });

    await waitFor(() => {
      fireEvent.click(screen.getByTestId('add-ingredient-button'));
      fireEvent.change(screen.getByTestId('ingredient-name-input-0'), { target: { value: 'Flour' } });
      fireEvent.change(screen.getByTestId('ingredient-amount-input-0'), { target: { value: '2' } });
      fireEvent.change(screen.getByTestId('ingredient-unit-input-0'), { target: { value: 'cups' } });
      fireEvent.click(screen.getByTestId('save-ingredient-button'));
    });

    expect(screen.getByTestId('ingredient-name-input-0')).toHaveValue('Flour');
    expect(screen.getByTestId('ingredient-amount-input-0')).toHaveValue('2');
    expect(screen.getByTestId('ingredient-unit-input-0')).toHaveValue('cups');

    await waitFor(() => fireEvent.click(screen.getByTestId('remove-ingredient-button')));
    expect(screen.queryByTestId('ingredient-name-input-0')).not.toBeInTheDocument();
  });

  test('handles adding and removing keywords', async () => {
    const mockUser = {
      userId: 1,
      token: 'mockToken',
    };
    render(
     
        <AuthContext.Provider value={{ user: mockUser }}>
          <EditRecipe />
        </AuthContext.Provider>
     
    );

    // Mocking API response
    global.fetch = jest.fn().mockResolvedValueOnce({
      ok: true,
      json: async () => ({
        name: '',
        instructions: '',
        visibility: false,
        userId: 'user123',
        ingredients: [],
        keywords: [],
        images: [],
      }),
    });

    await waitFor(() => {
      fireEvent.click(screen.getByTestId('add-keyword-button'));
      fireEvent.change(screen.getByTestId('keyword-input'), { target: { value: 'Healthy' } });
      fireEvent.click(screen.getByTestId('save-keyword-button'));
    });

    expect(screen.getByTestId('keyword-input')).toHaveValue('Healthy');

    await waitFor(() => fireEvent.click(screen.getByTestId('remove-keyword-button')));
    expect(screen.queryByTestId('keyword-input')).not.toBeInTheDocument();
  });

  test('handles image upload and removal', async () => {
    const mockUser = {
      userId: 1,
      token: 'mockToken',
    };
    render(
      
        <AuthContext.Provider value={{ user: mockUser }}>
          <EditRecipe />
        </AuthContext.Provider>
     
    );

    // Mocking API response
    global.fetch = jest.fn().mockResolvedValueOnce({
      ok: true,
      json: async () => ({
        name: '',
        instructions: '',
        visibility: false,
        userId: 'user123',
        ingredients: [],
        keywords: [],
        images: [],
      }),
    });

    const file = new File(['(⌐□_□)'], 'test.png', { type: 'image/png' });

    fireEvent.change(screen.getByTestId('image-upload-input'), { target: { files: [file] } });

    await waitFor(() => {
      expect(screen.getByTestId('selected-image-0')).toBeInTheDocument();
      fireEvent.click(screen.getByTestId('remove-selected-image-0'));
    });

    expect(screen.queryByTestId('selected-image-0')).not.toBeInTheDocument();
  });

  test('handles error scenarios', async () => {
    const mockUser = {
      userId: 1,
      token: 'mockToken',
    };
    render(
     
        <AuthContext.Provider value={{ user: mockUser }}>
          <EditRecipe />
        </AuthContext.Provider>
   
    );

    // Mocking API response to simulate error
    global.fetch = jest.fn().mockRejectedValueOnce(new Error('Failed to fetch recipe.'));

    await waitFor(() => expect(screen.getByText('Error occurred while fetching recipe.')).toBeInTheDocument());
  });
});
