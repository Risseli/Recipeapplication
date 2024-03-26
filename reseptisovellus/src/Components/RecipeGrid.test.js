import React from "react";
import { render, screen } from "@testing-library/react";
import RecipeGrid from "./recipeGrid";
import { useAuth } from "./Authentication"; // Tuodaan useAuth käyttöön
import { BrowserRouter } from "react-router-dom";
import { recipesList, userList } from "./TestData";

// Luodaan mock-versio useAuth-koukusta
jest.mock("./Authentication", () => ({
  useAuth: jest.fn(),
}));

describe("RecipeGrid component", () => {
  test("renders all recipes with correct details for logged in user", () => {
    // Simulate that user is logged in, not mandatory
    const authUser = {
      id: 1,
      username: "admin_user",
      password: "admin_password",
      email: "admin@gmail.com",
      admin: true,
    };
    useAuth.mockReturnValue({ authUser });

    console.log("logged user", authUser);
    render(
      <BrowserRouter>
        <RecipeGrid recipes={recipesList} />
      </BrowserRouter>
    );

    // All recipes should be rendered for user
    const recipeItems = screen.getAllByTestId("recipe-grid-item");
    expect(recipeItems).toHaveLength(6);

    recipesList.forEach((recipe, index) => {
      const recipeItem = recipeItems[index];
      expect(recipeItem).toBeInTheDocument();

      expect(screen.getByText(recipe.name)).toBeInTheDocument();

      expect(
        screen.getByAltText(`Image for ${recipe.name}`)
      ).toBeInTheDocument();
    });
  });
});
