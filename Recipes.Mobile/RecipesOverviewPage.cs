using Recipes.Client.Core.ViewModels;

namespace Recipes.Mobile;

public class RecipesOverviewPage : ContentPage
{
    // Konstruktor der Seite
    public RecipesOverviewPage()
    {
        Title = "Recipes";

        var layout = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star)
            }
        };
        layout.Add(CreateAddRecipeRow(), column: 0, row: 0);
        layout.Add(CreateStatusLabel(), column: 0, row: 1);
        layout.Add(CreateRecipeList(), column: 0, row: 2);

        Content = layout;
        BindingContext = new RecipesOverviewViewModel();
    }

    private static View CreateAddRecipeRow()
    {
        var titleEntry = new Entry { Placeholder = "New recipe" };
        titleEntry.SetBinding(Entry.TextProperty,
            nameof(RecipesOverviewViewModel.NewRecipeTitle), BindingMode.TwoWay);

        var addButton = new Button { Text = "Add" };
        addButton.SetBinding(Button.CommandProperty,
            nameof(RecipesOverviewViewModel.AddRecipeCommand));

        var row = new Grid
        {
            Padding = new Thickness(15, 5),
            ColumnSpacing = 10,
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };
        row.Add(titleEntry, column: 0);
        row.Add(addButton, column: 1);
        return row;
    }

    private static View CreateStatusLabel()
    {
        var statusLabel = new Label
        {
            Padding = new Thickness(15, 10),
            FontAttributes = FontAttributes.Italic,
            MaxLines = 1,
            LineBreakMode = LineBreakMode.TailTruncation
        };
        statusLabel.SetBinding(Label.TextProperty,
            nameof(RecipesOverviewViewModel.StatusMessage));
        return statusLabel;
    }

    private static View CreateRecipeList()
    {
        var recipeList = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            ItemTemplate = new DataTemplate(CreateRecipeRow),
            EmptyView = "No recipes"
        };
        recipeList.SetBinding(ItemsView.ItemsSourceProperty,
            nameof(RecipesOverviewViewModel.Recipes));
        recipeList.SetBinding(SelectableItemsView.SelectedItemProperty,
            nameof(RecipesOverviewViewModel.SelectedRecipe), BindingMode.TwoWay);
        recipeList.SetBinding(SelectableItemsView.SelectionChangedCommandProperty,
            nameof(RecipesOverviewViewModel.NavigateToSelectedDetailCommand));
        return recipeList;
    }

    private static View CreateRecipeRow()
    {
        var favoriteLabel = new Label
        {
            Text = "★",
            TextColor = Colors.Orange,
            FontSize = 18,
            VerticalOptions = LayoutOptions.Center
        };
        favoriteLabel.SetBinding(IsVisibleProperty,
            nameof(RecipeListItemViewModel.IsFavorite), BindingMode.OneTime);

        var titleLabel = new Label { FontSize = 16, VerticalOptions = LayoutOptions.Center };
        titleLabel.SetBinding(Label.TextProperty,
            nameof(RecipeListItemViewModel.Title), BindingMode.OneTime);

        var removeButton = new Button { Text = "✕", WidthRequest = 44, Padding = 0 };
        removeButton.SetBinding(Button.CommandProperty, new Binding(
            nameof(RecipesOverviewViewModel.RemoveRecipeCommand),
            source: new RelativeBindingSource(
                RelativeBindingSourceMode.FindAncestorBindingContext,
                typeof(RecipesOverviewViewModel))));
        removeButton.SetBinding(Button.CommandParameterProperty, ".");

        var row = new Grid
        {
            Padding = new Thickness(15, 6),
            ColumnSpacing = 8,
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };
        row.Add(favoriteLabel, column: 0);
        row.Add(titleLabel, column: 1);
        row.Add(removeButton, column: 2);
        return row;
    }
}