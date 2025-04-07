using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Memory_Game.Common;
using Memory_Game.Model;
using Memory_Game.Services;
using Microsoft.Win32;

namespace Memory_Game.ViewModel
{
    public class CategoryViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly CategoryService _categoryService;
        private ObservableCollection<Category> _categories;
        private Category _selectedCategory;
        private string _newCategoryName;
        private List<string> _selectedImages;

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    OnPropertyChanged();
                }
            }
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanDeleteCategory));
                }
            }
        }

        public string NewCategoryName
        {
            get => _newCategoryName;
            set
            {
                if (_newCategoryName != value)
                {
                    _newCategoryName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanAddCategory));
                }
            }
        }

        public List<string> SelectedImages
        {
            get => _selectedImages;
            set
            {
                if (_selectedImages != value)
                {
                    _selectedImages = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanAddCategory));
                }
            }
        }

        public bool CanAddCategory => !string.IsNullOrWhiteSpace(NewCategoryName) &&
                                    SelectedImages != null &&
                                    SelectedImages.Count > 0;

        public bool CanDeleteCategory => SelectedCategory != null;

        public ICommand AddCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand SelectImagesCommand { get; }
        public ICommand SelectCategoryCommand { get; }

        public CategoryViewModel()
        {
            _categoryService = new CategoryService();
            _categoryService.InitializeDefaultCategories();

            AddCategoryCommand = new RelayCommand(
                execute: (object param) => ExecuteAddCategory(),
                canExecute: (object param) => CanAddCategory
            );

            DeleteCategoryCommand = new RelayCommand(
                execute: (object param) => ExecuteDeleteCategory(),
                canExecute: (object param) => CanDeleteCategory
            );

            SelectImagesCommand = new RelayCommand(
                execute: (object param) => ExecuteSelectImages(),
                canExecute: (object param) => true
            );

            SelectCategoryCommand = new RelayCommand<Category>(
                execute: (Category category) => ExecuteSelectCategory(category),
                canExecute: (Category category) => category != null
            );

            Categories = new ObservableCollection<Category>();
            LoadCategories();
        }

        private void ExecuteAddCategory()
        {
            try
            {
                if (_categoryService.AddCategory(NewCategoryName, SelectedImages))
                {
                    Categories.Add(new Category { Name = NewCategoryName });
                    NewCategoryName = string.Empty;
                    SelectedImages = null;
                }
                else
                {
                    MessageBox.Show("A category with this name already exists.", "Add Category Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding category: {ex.Message}", "Add Category Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteDeleteCategory()
        {
            try
            {
                if (SelectedCategory != null)
                {
                    var result = MessageBox.Show(
                        $"Are you sure you want to delete category {SelectedCategory.Name}?",
                        "Confirm Delete",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (_categoryService.DeleteCategory(SelectedCategory.Name))
                        {
                            Categories.Remove(SelectedCategory);
                            SelectedCategory = null;
                        }
                        else
                        {
                            MessageBox.Show("Error deleting category.", "Delete Category Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting category: {ex.Message}", "Delete Category Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSelectImages()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                Multiselect = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (dialog.ShowDialog() == true)
            {
                SelectedImages = dialog.FileNames.ToList();
            }
        }

        private void ExecuteSelectCategory(Category category)
        {
            SelectedCategory = category;
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _categoryService.LoadCategories();
                Categories = new ObservableCollection<Category>(categories);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Load Categories Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Categories = new ObservableCollection<Category>();
            }
        }
    }
}