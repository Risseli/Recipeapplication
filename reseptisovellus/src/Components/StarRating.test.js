import React from "react";
import { render, fireEvent } from "@testing-library/react";
import { StarRating } from "./StarRating";

describe("StarRating component", () => {
  test("renders star icons", () => {
    const { getAllByTestId } = render(<StarRating rating={3} />);
    const stars = getAllByTestId("star");

    // Tarkistetaan, että tähti-ikoneita on 5
    expect(stars.length).toBe(5);
  });

  test("changes rating on click", () => {
    let rating = 3;
    const setRating = jest.fn();

    const { getAllByTestId } = render(
      <StarRating rating={rating} setRating={setRating} />
    );
    const stars = getAllByTestId("star");

    // Klikataan kolmatta tähteä
    fireEvent.click(stars[2]);

    // Tarkistetaan, että setRating-funktio kutsutaan oikein
    expect(setRating).toHaveBeenCalledWith(3);
  });
});
