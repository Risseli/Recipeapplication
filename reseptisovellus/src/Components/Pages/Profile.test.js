import React from 'react';
import { render, screen, waitFor, fireEvent } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { Profile } from './Profile';
import { AuthProvider } from '../Authentication';

// Mocking useNavigate hook
jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: jest.fn(),
}));

// Mocking useAuth hook
jest.mock('../Authentication', () => ({
  useAuth: jest.fn(),
}));

// Mocking fetch requests
global.fetch = jest.fn();

describe('Profile component', () => {
  beforeEach(() => {
    // Reset mocks and clear any leftover state
    jest.clearAllMocks();
    fetch.mockClear();
  });

  it('renders loading state initially', () => {
    // Mock useAuth to return null initially
    AuthProvider.mockReturnValue({ user: null });

    render(
      <MemoryRouter>
        <Profile />
      </MemoryRouter>
    );

    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('renders profile information when user data is loaded', async () => {
    // Mock useAuth to return user data
    AuthProvider.mockReturnValue({
      user: { id: 1, name: 'Test User', email: 'test@example.com', admin: false },
    });

    render(
      <MemoryRouter>
        <Profile />
      </MemoryRouter>
    );

    // Wait for profile information to be rendered
    await waitFor(() => {
      expect(screen.getByText('Profile')).toBeInTheDocument();
      expect(screen.getByText('Name: Test User')).toBeInTheDocument();
      expect(screen.getByText('Email: test@example.com')).toBeInTheDocument();
      expect(screen.getByText('Admin: No')).toBeInTheDocument();
    });
  });

  
});
