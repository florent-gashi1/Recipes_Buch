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
