using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Recipes.Client.Core.ViewModels;

public class RecipesOverviewViewModel : ObservableObject
{
    private const int NumberOfDemoRecipes = 20;
    private const int DeselectDelayInMilliseconds = 300;

    private static readonly string[] DemoTitles =
    {
        "Caesar Salad", "Spaghetti Carbonara", "Chicken Curry", "Rösti", "Fondue",
        "Pancakes", "Tomato Soup", "Risotto", "Falafel", "Apple Pie"
    };

    private RecipeListItemViewModel? _selectedRecipe;
    private string _newRecipeTitle = "";
    private string _statusMessage = "";

    public ObservableCollection<RecipeListItemViewModel> Recipes { get; } = new();

    public RecipeListItemViewModel? SelectedRecipe
    {
        get => _selectedRecipe;
        set => SetProperty(ref _selectedRecipe, value);
    }

    public string NewRecipeTitle
    {
        get => _newRecipeTitle;
        set
        {
            if (SetProperty(ref _newRecipeTitle, value))
                AddRecipeCommand.NotifyCanExecuteChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public IAsyncRelayCommand NavigateToSelectedDetailCommand { get; }
    public IRelayCommand AddRecipeCommand { get; }
    public IRelayCommand<RecipeListItemViewModel> RemoveRecipeCommand { get; }

    public RecipesOverviewViewModel()
    {
        NavigateToSelectedDetailCommand = new AsyncRelayCommand(NavigateToSelectedDetail);
        AddRecipeCommand = new RelayCommand(AddRecipe, CanAddRecipe);
        RemoveRecipeCommand = new RelayCommand<RecipeListItemViewModel>(RemoveRecipe);

        LoadDemoRecipes();
        UpdateStatusMessage();
    }

    private async Task NavigateToSelectedDetail()
    {
        if (SelectedRecipe is null)
            return;

        StatusMessage = $"You picked \"{SelectedRecipe.Title}\"";

        await Task.Delay(DeselectDelayInMilliseconds);
        SelectedRecipe = null;
    }

    private bool CanAddRecipe() => !string.IsNullOrWhiteSpace(NewRecipeTitle);

    private void AddRecipe()
    {
        var title = NewRecipeTitle.Trim();
        if (Recipes.Any(recipe => recipe.Title == title))
            return;

        Recipes.Add(new RecipeListItemViewModel(Guid.NewGuid().ToString(), title, isFavorite: false));
        NewRecipeTitle = "";
        UpdateStatusMessage();
    }

    private void RemoveRecipe(RecipeListItemViewModel? recipe)
    {
        if (recipe is null)
            return;

        Recipes.Remove(recipe);
        UpdateStatusMessage();
    }

    private void LoadDemoRecipes()
    {
        for (var i = 0; i < NumberOfDemoRecipes; i++)
        {
            var number = i + 1;
            var title = $"{DemoTitles[i % DemoTitles.Length]} #{number}";
            var isFavorite = i % 3 == 0;

            Recipes.Add(new RecipeListItemViewModel($"recipe-{number}", title, isFavorite));
        }
    }

    private void UpdateStatusMessage()
        => StatusMessage = $"{Recipes.Count} recipes loaded";
}
