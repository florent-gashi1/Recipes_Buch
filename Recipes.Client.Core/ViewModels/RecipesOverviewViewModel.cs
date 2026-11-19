using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Recipes.Client.Core.ViewModels;

public class RecipeListItemViewModel
{
    public string Id { get; }
    public string Title { get; }
    public bool IsFavorite { get; }

    public RecipeListItemViewModel(string id, string title, bool isFavorite)
    {
        Id = id;
        Title = title;
        IsFavorite = isFavorite;
    }
}

public class RecipesOverviewViewModel : ObservableObject
{
    private const int NumberOfRecipes = 20;

    private static readonly string[] DemoTitles =
    {
        "Caesar Salad", "Spaghetti Carbonara", "Chicken Curry", "Rösti", "Fondue",
        "Pancakes", "Tomato Soup", "Risotto", "Falafel", "Apple Pie"
    };


    public ObservableCollection<RecipeListItemViewModel> Recipes { get; } = new();


    private RecipeListItemViewModel? _selectedRecipe;

    public RecipeListItemViewModel? SelectedRecipe
    {
        get => _selectedRecipe;
        set => SetProperty(ref _selectedRecipe, value);
    }

    private string _statusMessage = "";

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    private string _newRecipeTitle = "";

    public string NewRecipeTitle
    {
        get => _newRecipeTitle;
        set
        {
            if (SetProperty(ref _newRecipeTitle, value))
                AddRecipeCommand.NotifyCanExecuteChanged();
        }
    }

    public IAsyncRelayCommand NavigateToSelectedDetailCommand { get; }
    public IRelayCommand AddRecipeCommand { get; }
    public IRelayCommand<RecipeListItemViewModel> RemoveRecipeCommand { get; }

    public RecipesOverviewViewModel()
    {
        AddRecipes();
        UpdateStatus();

        NavigateToSelectedDetailCommand = new AsyncRelayCommand(NavigateToSelectedDetail);
        AddRecipeCommand = new RelayCommand(AddRecipe, () => !string.IsNullOrWhiteSpace(NewRecipeTitle));
        RemoveRecipeCommand = new RelayCommand<RecipeListItemViewModel>(RemoveRecipe);
    }

    private void AddRecipe()
    {
        var title = NewRecipeTitle.Trim();
        if (Recipes.Any(r => r.Title == title))
            return;

        Recipes.Add(new RecipeListItemViewModel(id: Guid.NewGuid().ToString(), title: title, isFavorite: false));
        NewRecipeTitle = "";
        UpdateStatus();
    }

    private void RemoveRecipe(RecipeListItemViewModel? recipe)
    {
        if (recipe is null)
            return;

        Recipes.Remove(recipe);
        UpdateStatus();
    }

    private async Task NavigateToSelectedDetail()
    {
        if (SelectedRecipe is null)
            return;

        StatusMessage = $"You picked \"{SelectedRecipe.Title}\"";

        await Task.Delay(300);


        SelectedRecipe = null;
    }

    private void AddRecipes()
    {
        for (var i = 0; i < NumberOfRecipes; i++)
        {
            var title = $"{DemoTitles[i % DemoTitles.Length]} #{i + 1}";
            Recipes.Add(new RecipeListItemViewModel(id: $"recipe-{i + 1}", title: title, isFavorite: i % 3 == 0));
        }
    }

    private void UpdateStatus()
        => StatusMessage = $"{Recipes.Count} recipes loaded";
}