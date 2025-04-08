using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Memory_Game.Model;

namespace Memory_Game.Services
{
    public class CategoryService
    {
        private readonly string _categoriesDirectory;
        private readonly string _categoriesFilePath;
        private readonly string _imagesBasePath;

        public CategoryService()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MemoryGame");

            _categoriesDirectory = Path.Combine(appDataPath, "Categories");
            _categoriesFilePath = Path.Combine(appDataPath, "categories.json");

            // Get the application's base directory
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Directory.GetParent(currentDir).Parent.Parent.Parent.Parent.FullName;
            _imagesBasePath = Path.Combine(projectRoot, "Images");

            // Create directories if they don't exist
            Directory.CreateDirectory(appDataPath);
            Directory.CreateDirectory(_categoriesDirectory);
            Directory.CreateDirectory(_imagesBasePath);
        }

        public List<Category> LoadCategories()
        {
            try
            {
                if (File.Exists(_categoriesFilePath))
                {
                    string json = File.ReadAllText(_categoriesFilePath);
                    var categories = JsonSerializer.Deserialize<List<Category>>(json);
                    return categories ?? new List<Category>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading categories: {ex.Message}");
            }

            return new List<Category>();
        }

        public void SaveCategories(List<Category> categories)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(categories, options);
                File.WriteAllText(_categoriesFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving categories: {ex.Message}");
            }
        }

        public List<string> GetCategoryImages(string categoryName)
        {
            try
            {
                string categoryPath = Path.Combine(_imagesBasePath, categoryName);
                if (Directory.Exists(categoryPath))
                {
                    return Directory.GetFiles(categoryPath, "*.png")
                        .Concat(Directory.GetFiles(categoryPath, "*.jpg"))
                        .Concat(Directory.GetFiles(categoryPath, "*.jpeg"))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting category images: {ex.Message}");
            }

            return new List<string>();
        }

        public bool AddCategory(string categoryName, List<string> imagePaths)
        {
            try
            {
                var categories = LoadCategories();
                if (categories.Any(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }

                string categoryPath = Path.Combine(_imagesBasePath, categoryName);
                Directory.CreateDirectory(categoryPath);

                foreach (string imagePath in imagePaths)
                {
                    string fileName = Path.GetFileName(imagePath);
                    string destinationPath = Path.Combine(categoryPath, fileName);
                    File.Copy(imagePath, destinationPath, true);
                }

                categories.Add(new Category { Name = categoryName });
                SaveCategories(categories);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding category: {ex.Message}");
                return false;
            }
        }

        public bool DeleteCategory(string categoryName)
        {
            try
            {
                var categories = LoadCategories();
                var category = categories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

                if (category != null)
                {
                    // Delete from both locations
                    string appDataCategoryPath = Path.Combine(_categoriesDirectory, categoryName);
                    string projectCategoryPath = Path.Combine(_imagesBasePath, categoryName);

                    if (Directory.Exists(appDataCategoryPath))
                    {
                        Directory.Delete(appDataCategoryPath, true);
                    }

                    if (Directory.Exists(projectCategoryPath))
                    {
                        Directory.Delete(projectCategoryPath, true);
                    }

                    categories.Remove(category);
                    SaveCategories(categories);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting category: {ex.Message}");
                return false;
            }
        }

        public void InitializeDefaultCategories()
        {
            var categories = LoadCategories();
            if (categories.Count == 0)
            {
                // Add default categories if none exist
                var defaultCategories = new List<Category>
                {
                    new Category { Name = "Animals" },
                    new Category { Name = "Food" },
                    new Category { Name = "Sports" }
                };

                SaveCategories(defaultCategories);

                // Create directories for default categories
                foreach (var category in defaultCategories)
                {
                    string categoryPath = Path.Combine(_imagesBasePath, category.Name);
                    Directory.CreateDirectory(categoryPath);
                }
            }
        }
    }
}