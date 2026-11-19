using CommunityToolkit.Mvvm.ComponentModel;

namespace Recipes.Client.Core.ViewModels;

public class IngredientsListViewModel : ObservableObject
{
    private int _numberOfServings = 4;

    public int NumberOfServings
    {
        get => _numberOfServings;
        set
        {
            if (SetProperty(ref _numberOfServings, value))
            {
                // Collection: update every item when the servings change
                Ingredients.ForEach(i => i.UpdateServings(value));
            }
        }
    }

    // BindableLayout: ItemsSource for the ingredients list
    public List<RecipeIngredientViewModel> Ingredients { get; } = new()
    {
        new("Romaine lettuce", 1, "head"),
        new("Parmesan", 50, "g"),
        new("Croutons", 100, "g"),
        new("Olive oil", 4, "tbsp"),
        new("Lemon juice", 2, "tbsp"),
        new("Garlic", 1, "clove"),
        new("Anchovy fillets", 4, "pcs"),
    };
}
