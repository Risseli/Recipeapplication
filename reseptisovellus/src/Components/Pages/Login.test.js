import React from 'react';
import {  render } from '@testing-library/react';
import { Login } from './Login';
import { screen,waitFor,getByText } from '@testing-library/react';

// Mocking the useAuth hook
jest.mock('../Authentication', () => ({
  useAuth: () => ({}),
}));

// Mocking the useNavigate hook
jest.mock('react-router-dom', () => ({
  useNavigate: () => jest.fn(),
}));


describe('Login component', () => {
  it('renders login form correctly', () => {
    const { getAllByPlaceholderText, getByText,getAllByText } = render(<Login />);
    const usernameInputs = getAllByPlaceholderText('Username');
    const passwordInputs = getAllByPlaceholderText('Password');
    const emailInputs = getAllByPlaceholderText('Email');
    const nameInputs = getAllByPlaceholderText('Name');


    expect(emailInputs.length).toBeGreaterThan(0);
    expect(nameInputs.length).toBeGreaterThan(0);
    expect(usernameInputs.length).toBe(2); 
    expect(passwordInputs.length).toBe(2); 

  
  });
  describe('Buttons', () => {
    it('checks if buttons are used', () => {
      const { getAllByPlaceholderText, getByText,getAllByText } = render(<Login />);

      const loginButtons = getAllByText('Login');
      const recoverPasswordText = getByText(/Click here to recover it/i);
      const registerButtons = getAllByText('Register');
      

      expect(loginButtons.length).toBeGreaterThan(1)
      expect(recoverPasswordText).toBeInTheDocument();
      expect(registerButtons.length).toBeGreaterThanOrEqual(1)
    });
  });
});