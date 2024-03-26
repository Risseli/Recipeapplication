const recipesList = [
  {
    id: 1,
    name: "Chocolate cake",
    images: [{ imageData: "base64" }],
    reviews: [{ rating: 3 }],
    rating: 3,
    visibility: true,
    keyword: [{ id: 1, word: "chocolate" }],
  },
  {
    id: 2,
    name: "Pasta Carbonara",
    images: [{ imageData: "base64" }],
    reviews: [{ rating: 4 }],
    rating: 4,
    visibility: false,
    keyword: [{ id: 2, word: "carbonara" }],
  },
  {
    id: 3,
    name: "Chicken Curry",
    images: [{ imageData: "base64" }],
    reviews: [{ rating: 5 }],
    rating: 5,
    visibility: true,
    keyword: [{ id: 3, word: "curry" }],
  },
  {
    id: 4,
    name: "Vegetable Stir-Fry",
    images: [{ imageData: "base64" }],
    reviews: [{ rating: 4.5 }],
    rating: 4.5,
    visibility: true,
    keyword: [{ id: 4, word: "vegetable" }],
  },
  {
    id: 5,
    name: "Salmon with Lemon Sauce",
    images: [{ imageData: "base64" }],
    reviews: [{ rating: 4 }],
    rating: 4,
    visibility: true,
    keyword: [{ id: 5, word: "salmon" }],
  },
  {
    id: 6,
    name: "Spagetti Bolognese",
    images: [{ imageData: "base64" }],
    reviews: [{ rating: 2 }],
    rating: 2,
    visibility: false,
    keyword: [{ id: 6, word: "bolognese" }],
  },
];

const userList = [
  {
    id: 2,
    admin: true,
    name: "Admin User",
    email: "admin@example.com",
  },
  {
    id: 3,
    admin: false,
    name: "normal_user",
    email: "user@example.com",
  },
  {
    id: 4,
    admin: false,
    name: "Chef User22",
    email: "chef@example.com",
  },
  {
    id: 5,
    admin: false,
    name: "Foodie User2",
    email: "foodie@example.com",
  },
];

export { recipesList, userList };
