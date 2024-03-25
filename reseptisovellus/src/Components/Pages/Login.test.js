import React from 'react';
import { render, fireEvent, waitFor } from '@testing-library/react';
import { Login } from './Login';

jest.mock('react-router-dom', () => ({
  useNavigate: jest.fn(),
}));

jest.mock('../Authentication', () => ({
  useAuth: jest.fn(() => ({
    login: jest.fn(),
  })),
}));

describe('Login Component', () => {
  test('renders login form', () => {
    const { getByText, getByPlaceholderText } = render(<Login />);

    expect(getByText('Login')).toBeInTheDocument();
    expect(getByPlaceholderText('Username')).toBeInTheDocument();
    expect(getByPlaceholderText('Password')).toBeInTheDocument();
    expect(getByText('Forgot your password?')).toBeInTheDocument();
    expect(getByText('Register')).toBeInTheDocument();
  });

  test('displays error message on invalid login', async () => {
    const { getByText, getByPlaceholderText } = render(<Login />);
    const usernameInput = getByPlaceholderText('Username');
    const passwordInput = getByPlaceholderText('Password');
    const loginButton = getByText('Login');

    fireEvent.change(usernameInput, { target: { value: 'invalidUser' } });
    fireEvent.change(passwordInput, { target: { value: 'invalidPassword' } });
    fireEvent.click(loginButton);

    await waitFor(() => {
      expect(getByText('Wrong Username or Password')).toBeInTheDocument();
    });
  });

  test('displays error message on invalid registration', async () => {
    const { getByText, getByPlaceholderText } = render(<Login />);
    const emailInput = getByPlaceholderText('Email');
    const usernameInput = getByPlaceholderText('Username');
    const passwordInput = getByPlaceholderText('Password');
    const nameInput = getByPlaceholderText('Name');
    const registerButton = getByText('Register');

    fireEvent.change(emailInput, { target: { value: 'invalidemail' } });
    fireEvent.change(usernameInput, { target: { value: 'invalidUser' } });
    fireEvent.change(passwordInput, { target: { value: 'invalidPassword' } });
    fireEvent.change(nameInput, { target: { value: 'Invalid Name' } });
    fireEvent.click(registerButton);

    await waitFor(() => {
      expect(getByText('Please enter a valid email address.')).toBeInTheDocument();
    });
  });


});
