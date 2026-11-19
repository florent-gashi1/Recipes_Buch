using Recipes.Client.Core.ViewModels;

namespace Recipes.Mobile;

public class RecipesOverviewPage : ContentPage
{
    public RecipesOverviewPage()
    {
        Title = "Recipes";


        var collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            ItemTemplate = new DataTemplate(CreateRecipeRow),
            EmptyView = "No recipes"
        };

        collectionView.SetBinding(ItemsView.ItemsSourceProperty,
            nameof(RecipesOverviewViewModel.Recipes));


        collectionView.SetBinding(SelectableItemsView.SelectedItemProperty,
            nameof(RecipesOverviewViewModel.SelectedRecipe), BindingMode.TwoWay);
        collectionView.SetBinding(SelectableItemsView.SelectionChangedCommandProperty,
            nameof(RecipesOverviewViewModel.NavigateToSelectedDetailCommand));

        var statusLabel = new Label
        {
            Padding = new Thickness(15, 10),
            FontAttributes = FontAttributes.Italic,
            MaxLines = 1,
            LineBreakMode = LineBreakMode.TailTruncation
        };
        statusLabel.SetBinding(Label.TextProperty, nameof(RecipesOverviewViewModel.StatusMessage));

        var newRecipeEntry = new Entry { Placeholder = "New recipe" };
        newRecipeEntry.SetBinding(Entry.TextProperty,
            nameof(RecipesOverviewViewModel.NewRecipeTitle), BindingMode.TwoWay);

        var addButton = new Button { Text = "Add" };
        addButton.SetBinding(Button.CommandProperty, nameof(RecipesOverviewViewModel.AddRecipeCommand));

        var addRow = new Grid
        {
            Padding = new Thickness(15, 5),
            ColumnSpacing = 10,
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }
        };
        addRow.Add(newRecipeEntry, column: 0);
        addRow.Add(addButton, column: 1);

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star)
            }
        };
        grid.Add(addRow, column: 0, row: 0);
        grid.Add(statusLabel, column: 0, row: 1);
        grid.Add(collectionView, column: 0, row: 2);
        Content = grid;

        BindingContext = new RecipesOverviewViewModel();
    }


    private static View CreateRecipeRow()
    {
        var titleLabel = new Label { FontSize = 16, VerticalOptions = LayoutOptions.Center };
        titleLabel.SetBinding(Label.TextProperty, nameof(RecipeListItemViewModel.Title), BindingMode.OneTime);

        var favoriteLabel = new Label
            { Text = "★", TextColor = Colors.Orange, FontSize = 18, VerticalOptions = LayoutOptions.Center };
        favoriteLabel.SetBinding(IsVisibleProperty, nameof(RecipeListItemViewModel.IsFavorite), BindingMode.OneTime);

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