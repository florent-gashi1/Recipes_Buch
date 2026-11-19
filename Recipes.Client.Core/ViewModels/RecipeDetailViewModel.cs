using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Recipes.Client.Core.ViewModels;

public class RecipeDetailViewModel : ObservableObject
{
    public string Title { get; set; } = "Classic Caesar Salad";

    public IngredientsListViewModel IngredientsList { get; } = new();

    private bool _showAllergenInformation;

    public bool ShowAllergenInformation
    {
        get => _showAllergenInformation;
        set => SetProperty(ref _showAllergenInformation, value);
    }

    // DataTemplateSelector: mixed list of steps and notes
    public List<InstructionBaseViewModel> Instructions { get; } = new()
    {
        new InstructionViewModel(1, "Wash the lettuce, dry it well and tear it into bite-sized pieces."),
        new InstructionViewModel(2, "Mash the garlic and anchovies, then whisk in lemon juice and olive oil."),
        new NoteViewModel("Use cold lettuce straight from the fridge: it stays crunchy."),
        new InstructionViewModel(3, "Toss the lettuce with the dressing."),
        new InstructionViewModel(4, "Top with croutons and freshly grated Parmesan. Serve immediately."),
        new NoteViewModel("Homemade croutons: cube old bread, fry in olive oil until golden."),
    };

    // ObservableCollection: Add/Remove updates the UI
    public ObservableCollection<RecipeIngredientViewModel> ShoppingList { get; } = new();

    // RelayCommand<T>: the tapped item arrives as parameter
    public IRelayCommand<RecipeIngredientViewModel> AddToShoppingListCommand { get; }
    public IRelayCommand<RecipeIngredientViewModel> RemoveFromShoppingListCommand { get; }

    public RecipeDetailViewModel()
    {
        AddToShoppingListCommand = new RelayCommand<RecipeIngredientViewModel>(AddToShoppingList);
        RemoveFromShoppingListCommand = new RelayCommand<RecipeIngredientViewModel>(RemoveFromShoppingList);
    }

    private void AddToShoppingList(RecipeIngredientViewModel? ingredient)
    {
        if (ingredient is null || ShoppingList.Contains(ingredient))
            return;

        ShoppingList.Add(ingredient);
    }

    private void RemoveFromShoppingList(RecipeIngredientViewModel? ingredient)
    {
        if (ingredient is not null)
            ShoppingList.Remove(ingredient);
    }
}
