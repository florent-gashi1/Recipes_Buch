using CommunityToolkit.Mvvm.ComponentModel;

namespace Recipes.Client.Core.ViewModels;

// BindableLayout: one item of the Ingredients list
public class RecipeIngredientViewModel : ObservableObject
{
    private readonly double baseAmount;
    private readonly int baseServings;

    public string IngredientName { get; }
    public string Measurement { get; }

    // DataTemplate: bound OneWay, changes with the servings
    private double _displayAmount;
    public double DisplayAmount
    {
        get => _displayAmount;
        private set => SetProperty(ref _displayAmount, value);
    }

    public RecipeIngredientViewModel(string ingredientName, double amount, string measurement, int baseServings = 4)
    {
        IngredientName = ingredientName;
        Measurement = measurement;
        baseAmount = amount;
        this.baseServings = baseServings;
        DisplayAmount = amount;
    }

    public void UpdateServings(int servings)
    {
        var factor = servings / (double)baseServings;
        DisplayAmount = factor * baseAmount;
    }
}
