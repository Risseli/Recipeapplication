import React from 'react';
import { render } from '@testing-library/react';
import '@testing-library/jest-dom';
import { MemoryRouter } from 'react-router-dom'; // Import MemoryRouter
import { Profile } from './Profile';
import { AuthContext } from '../Authentication';

describe('Profile Component', () => {
  test('renders without crashing', () => {
    const mockUser = {
      userId: 1,
      token: 'mockToken',
    };

    render(
      <MemoryRouter>
        <AuthContext.Provider value={{ user: mockUser }}>
          <Profile />
        </AuthContext.Provider>
      </MemoryRouter>
    );
  });
  
});
