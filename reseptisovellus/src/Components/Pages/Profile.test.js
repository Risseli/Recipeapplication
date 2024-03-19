import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { Profile } from './Profile';
import { AuthContext } from '../Authentication'; // Assuming you have an AuthContext for authentication

// Mocking the AuthContext
const mockAuthContextValue = {
  user: {
    id: 1,
    name: 'Test User',
    email: 'test@example.com',
    admin: false,
    token: 'mockToken',
  },
};

describe('Profile component', () => {
  test('renders profile details correctly when user is not in edit mode', () => {
    render(
      <BrowserRouter>
        <AuthContext.Provider value={mockAuthContextValue}>
          <Profile />
        </AuthContext.Provider>
      </BrowserRouter>
    );

    // Assert that profile details are rendered correctly
    expect(screen.getByText('Name: Test User')).toBeInTheDocument();
    expect(screen.getByText('Email: test@example.com')).toBeInTheDocument();
    expect(screen.getByText('Admin: No')).toBeInTheDocument();
    expect(screen.getByText('Edit')).toBeInTheDocument();
    expect(screen.getByText('Delete')).toBeInTheDocument();
  });

  test('renders profile details correctly when user is in edit mode', async () => {
    render(
      <BrowserRouter>
        <AuthContext.Provider value={mockAuthContextValue}>
          <Profile />
        </AuthContext.Provider>
      </BrowserRouter>
    );

    // Click the edit button
    fireEvent.click(screen.getByText('Edit'));

    // Assert that input fields are rendered correctly for editing
    await waitFor(() => {
      expect(screen.getByDisplayValue('Test User')).toBeInTheDocument();
      expect(screen.getByDisplayValue('test@example.com')).toBeInTheDocument();
      expect(screen.getByText('Admin: No')).toBeInTheDocument(); // Admin field should not be editable
    });
  });

  test('saves changes successfully', async () => {
    render(
      <BrowserRouter>
        <AuthContext.Provider value={mockAuthContextValue}>
          <Profile />
        </AuthContext.Provider>
      </BrowserRouter>
    );

    // Click the edit button
    fireEvent.click(screen.getByText('Edit'));

    // Change user name
    fireEvent.change(screen.getByDisplayValue('Test User'), { target: { value: 'Updated User' } });

    // Click the save button
    fireEvent.click(screen.getByText('Save'));

    // Assert that changes are saved successfully
    await waitFor(() => {
      expect(screen.getByText('Name: Updated User')).toBeInTheDocument();
    });
  });

  test('deletes the user successfully', async () => {
    render(
      <BrowserRouter>
        <AuthContext.Provider value={mockAuthContextValue}>
          <Profile />
        </AuthContext.Provider>
      </BrowserRouter>
    );

    // Click the delete button
    fireEvent.click(screen.getByText('Delete'));

    // Confirm deletion
    fireEvent.click(screen.getByText('OK'));

    // Assert that user is deleted successfully
    await waitFor(() => {
      expect(screen.queryByText('Name: Test User')).not.toBeInTheDocument();
    });
  });

  // Add more test cases to cover other functionalities of the Profile component
});
