import { React, useState } from "react";
//import "./StarRating.css";
import { FaStar } from "react-icons/fa";

export default function StarRating({rating, setRating, rateColor, setColor}) {
  //const [rating, setRating] = useState(null);
  //const [rateColor, setColor] = useState(null);
  return (
    <>
      {[...Array(5)].map((star, index) => {
        const currentRate = index + 1;

        return (
          <>
            <label className="star-rating">
              <input
                style={{ display: "none" }}
                type="radio"
                name="rate"
                value={currentRate}
                onClick={() => setRating(currentRate)}
              />

              <FaStar
                className="star"
                size={30}
                color={currentRate <= (rateColor || rating) ? "orange" : "grey"}
              />
            </label>
            
          </>
        );
      })}
    </>
  );
}

export { StarRating };
