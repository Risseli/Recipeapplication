import React from "react";
import { render, screen, waitFor } from "@testing-library/react";
import { Recipes } from "./Recipes";
import { useAuth } from "../Authentication";
import { recipesList, userList } from "../TestData";

// Mocking useAuth hook
jest.mock("../Authentication", () => ({
  useAuth: userList[0],
}));

describe("Recipes component", () => {
  test("renders loading message while fetching recipes", async () => {
    // Simulate that user is not logged in
    useAuth.mockReturnValue({ user: null });

    render(<Recipes />);

    expect(screen.getByText("Loading recipes..")).toBeInTheDocument();

    /* await waitFor(() =>
      expect(screen.queryByText("Loading recipes..")).not.toBeInTheDocument()
    ); */
  });

  test("renders recipes after loading for logged out user", async () => {
    // Simulate that user is not logged in
    useAuth.mockReturnValue({ user: null });

    render(<Recipes />);

    await waitFor(() =>
      expect(screen.queryByText("Loading recipes..")).not.toBeInTheDocument()
    );

    recipesList.forEach((recipe) => {
      if (recipe.visibility) {
        expect(screen.getByText(recipe.name)).toBeInTheDocument();
      } else {
        expect(screen.queryByText(recipe.name)).not.toBeInTheDocument();
      }
    });
  });

  test("renders all recipes after loading for logged in user", async () => {
    // Simulate that user is logged in with bearer token
    useAuth.mockReturnValue({ user: { id: 1, token: "mockBearerToken" } });

    render(<Recipes />);

    await waitFor(() =>
      expect(screen.queryByText("Loading recipes..")).not.toBeInTheDocument()
    );

    recipesList.forEach((recipe) => {
      expect(screen.getByText(recipe.name)).toBeInTheDocument();
    });
  });

  // Add more tests as needed...
});
