using RecipeAppBackend.Data;
using RecipeAppBackend.Models;
using System.Diagnostics.Metrics;
using System.IO;

namespace RecipeAppBackend
{
    public class Seed
    {
        private readonly DataContext dataContext;
        public Seed(DataContext context)
        {
            this.dataContext = context;
        }
        public void SeedDataContext()
        {
            if (!dataContext.Images.Any())
            {
                string imagePath1 = @"C:\Temp\images\image1.jpg";
                string imagePath2 = @"C:\Temp\images\image2.jpg";
                string imagePath3 = @"C:\Temp\images\image3.jpg";
                string imagePath4 = @"C:\Temp\images\image4.jpg";
                string imagePath5 = @"C:\Temp\images\image5.jpg";

                byte[] imageData1 = File.ReadAllBytes(imagePath1);
                byte[] imageData2 = File.ReadAllBytes(imagePath2);
                byte[] imageData3 = File.ReadAllBytes(imagePath3);
                byte[] imageData4 = File.ReadAllBytes(imagePath4);
                byte[] imageData5 = File.ReadAllBytes(imagePath5);

                var images = new List<Image>()
                {
                    new Image()
                    {
                        ImageData = imageData1,
                        Recipe = new Recipe
                        {
                            Id = 1,
                            Name = "test",
                            Instructions = "",
                            Visibility = true,
                            User = new User
                            {
                                Id = 1,
                                Name = "test",
                                Admin = true,
                                Username = "test",
                                Password = "test",
                                Email = "test"
                            }
                        }
                    },
                    new Image()
                    {
                        ImageData = imageData2,
                        Recipe = new Recipe
                        {
                            Id = 2,
                            Name = "test",
                            Instructions = "",
                            Visibility = true,
                            User = new User
                            {
                                Id = 2,
                                Name = "test",
                                Admin = true,
                                Username = "test",
                                Password = "test",
                                Email = "test"
                            }
                        }
                    },
                    new Image()
                    {
                        ImageData = imageData3,
                        Recipe = new Recipe
                        {
                            Id = 3,
                            Name = "test",
                            Instructions = "",
                            Visibility = true,
                            User = new User
                            {
                                Id = 3,
                                Name = "test",
                                Admin = true,
                                Username = "test",
                                Password = "test",
                                Email = "test"
                            }
                        }
                    },
                    new Image()
                    {
                        ImageData = imageData4,
                        Recipe = new Recipe
                        {
                            Id = 4,
                            Name = "test",
                            Instructions = "",
                            Visibility = true,
                            User = new User
                            {
                                Id = 4,
                                Name = "test",
                                Admin = true,
                                Username = "test",
                                Password = "test",
                                Email = "test"
                            }
                        }
                    },
                    new Image()
                    {
                        ImageData = imageData5,
                        Recipe = new Recipe
                        {
                            Id = 5,
                            Name = "test",
                            Instructions = "",
                            Visibility = true,
                            User = new User
                            {
                                Id = 5,
                                Name = "test",
                                Admin = true,
                                Username = "test",
                                Password = "test",
                                Email = "test"
                            }
                        }
                    }
                };
                dataContext.Images.AddRange(images);
                dataContext.SaveChanges();
            }
        }
    }
}
