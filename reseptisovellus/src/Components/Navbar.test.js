import React from "react";
import { render, screen } from "@testing-library/react";
import { BrowserRouter } from "react-router-dom";
import { Navbar } from "./Navbar";
import { useAuth } from "./Authentication";

jest.mock("./Authentication", () => ({
    useAuth: jest.fn(),
  }));

test("renders all links in Navbar", () => {
    const authUser = {
        id: 1,
        username: "admin_user",
        password: "admin_password",
        email: "admin@gmail.com",
        admin: true,
      };
      useAuth.mockReturnValue({ authUser });

  render(
    <BrowserRouter>
      <Navbar />
    </BrowserRouter>
  );

  const links = screen.getAllByRole("link");
  expect(links).toHaveLength(5); // Olettaen, että Navbarissa on 5 linkkiä

  // Tarkistetaan, että jokainen linkki on renderöity
  links.forEach((link) => {
    expect(link).toBeInTheDocument();
  });
});


