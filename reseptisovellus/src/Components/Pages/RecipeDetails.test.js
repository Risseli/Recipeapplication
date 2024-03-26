import React from "react";
import { render, waitFor, screen } from "@testing-library/react";
import { RecipeDetails } from "./RecipeDetails";
import { useAuth } from "../Authentication";
import { BrowserRouter as Router } from "react-router-dom";

// Mocking useAuth hook
jest.mock("../Authentication", () => ({
  useAuth: jest.fn(function () {
    return {
      user: [
        {
          id: 1,
          username: "admin_user",
          password: "admin_password",
          email: "admin@gmail.com",
          admin: true,
        },
        // Muita käyttäjiä tarvittaessa
      ],
    };
  }),
}));

// Mockataan fetch-kutsu
global.fetch = jest.fn();

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useParams: () => ({
    id: "1", // Korvaa tämä id:llä, jolla haluat testata reseptin tietoja
  }),
}));

describe("RecipeDetails component", () => {
  test("renders recipe details", async () => {
    const recipeData = {
      id: 1,
      name: "Test Recipe",
      ingredients: ["Ingredient 1", "Ingredient 2"],
      instructions: "Test instructions",
      favorite: false,
      reviews: [],
      rating: 0,
      visibility: true,
      keyword: [{ id: 1, word: "test" }],
      userId: 1,
    };

    const userData = [
      { id: 1, username: "Test User", admin: true, token: "test_token" },
    ];

    const authUser = {
      id: 1,
      name: "admin_user",
      password: "admin_password",
      email: "admin@gmail.com",
      admin: true,
      token: "test_token",
    };

    // Mockataan useAuth-hookin palauttama arvo
    useAuth.mockReturnValue({ authUser });

    // Mockataan fetch-kutsut ja niiden vastaukset
    global.fetch = jest
      .fn()
      .mockResolvedValueOnce({
        ok: true,
        json: async () => recipeData,
      })
      .mockResolvedValueOnce({
        ok: true,
        json: async () => userData,
      })
      .mockResolvedValueOnce(
        authUser.admin === true
          ? {
              ok: true,
              json: async () => authUser,
            }
          : {
              ok: false,
            }
      );

    // console.log("user", user);
    // Renderöi komponentti
    render(
      <Router>
        <RecipeDetails />
      </Router>
    );

    // Odotetaan, että reseptin tiedot ladataan ja näytetään otsikot
    await waitFor(() => {
      expect(screen.getByText("Ingredients")).toBeInTheDocument();
      expect(screen.getByText("Instructions")).toBeInTheDocument();
      expect(screen.getByText("Reviews")).toBeInTheDocument();
      // expect(screen.getByText("Ingredient 1")).toBeInTheDocument();
      // expect(screen.getByText("Test instructions")).toBeInTheDocument();
      // Lisää tarvittaessa muita testejä reseptin tiedoille
    });
  });
});
