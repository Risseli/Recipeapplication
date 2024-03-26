import React from "react";
import { render, screen } from "@testing-library/react";
import { Home } from "./Home";
import { useAuth } from "../Authentication"; // Tuodaan useAuth käyttöön

// Luodaan mock-versio useAuth-koukusta
jest.mock("../Authentication", () => ({
  useAuth: jest.fn(),
}));

test("renders Home component", () => {
  // Simuloidaan tilanne, jossa käyttäjä ei ole kirjautunut sisään
  const authUser = null;

  // Palautetaan simuloidut kirjautumistiedot
  useAuth.mockReturnValue({ authUser });

  render(<Home />);

  // Tarkistetaan, että Home-komponentti renderöidään oikein
  expect(screen.getByText("Welcome to the recipe app")).toBeInTheDocument();
  expect(
    screen.getByText(
      "Here you can browse through our recipes and find something to cook for dinner tonight!"
    )
  ).toBeInTheDocument();
});
