import logo from "./logo.svg";
import "./App.css";
import { Navbar } from "./Components/Navbar";
import {
  Home,
  Recipes,
  About,
  Profile,
  Login,
  RecipeDetails,
  AddRecipe,
} from "./Components/Pages";
import { Route, Routes } from "react-router-dom";

function App() {
  return (
    <div>
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/recipes/" element={<Recipes />} />
        <Route path="/recipe/:id" element={<RecipeDetails />} />
        <Route path="/about" element={<About />} />
        <Route path="/profile" element={<Profile />} />
        <Route path="/login" element={<Login />} />
        <Route path="/add-recipe" element={<AddRecipe />} /> 
      </Routes>
    </div>
  );
}

export default App;
