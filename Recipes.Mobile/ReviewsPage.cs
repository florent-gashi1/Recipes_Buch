using Recipes.Client.Core.ViewModels;

namespace Recipes.Mobile;

public class ReviewsPage : ContentPage, IQueryAttributable
{
    private readonly Label _headerLabel = new()
    {
        FontAttributes = FontAttributes.Bold,
        FontSize = 22
    };

    public ReviewsPage()
    {
        Title = "Reviews";

        // CollectionView: grouping + multiple selection
        var reviewsView = new CollectionView
        {
            IsGrouped = true,
            SelectionMode = SelectionMode.Multiple,

            // Grouping: header per group
            GroupHeaderTemplate = new DataTemplate(() =>
            {
                var label = new Label { Margin = new Thickness(0, 25, 0, 0), FontSize = 16, FontAttributes = FontAttributes.Bold };
                label.SetBinding(Label.TextProperty, new Binding(
                    nameof(RatingGroup.Key), BindingMode.OneTime, stringFormat: "{0} stars reviews"));
                return label;
            }),

            // Grouping: footer per group
            GroupFooterTemplate = new DataTemplate(() =>
            {
                var label = new Label { FontSize = 12, TextColor = Colors.Gray };
                label.SetBinding(Label.TextProperty, new Binding(
                    nameof(RatingGroup.Count), BindingMode.OneTime, stringFormat: "{0} reviews"));
                return label;
            }),

            // DataTemplate: one review
            ItemTemplate = new DataTemplate(() =>
            {
                var nameLabel = new Label { FontAttributes = FontAttributes.Bold };
                nameLabel.SetBinding(Label.TextProperty, new Binding(
                    nameof(UserReviewViewModel.ReviewerName), BindingMode.OneTime));

                var ratingLabel = new Label { TextColor = Colors.Orange };
                ratingLabel.SetBinding(Label.TextProperty, new Binding(
                    nameof(UserReviewViewModel.Rating), BindingMode.OneTime, stringFormat: "★ {0}"));

                var reviewLabel = new Label();
                reviewLabel.SetBinding(Label.TextProperty, new Binding(
                    nameof(UserReviewViewModel.Review), BindingMode.OneTime));

                return new VerticalStackLayout
                {
                    Padding = new Thickness(10, 8),
                    Children =
                    {
                        new HorizontalStackLayout { Spacing = 8, Children = { nameLabel, ratingLabel } },
                        reviewLabel
                    }
                };
            })
        };

        reviewsView.SetBinding(ItemsView.ItemsSourceProperty,
            nameof(RecipeRatingsDetailViewModel.GroupedReviews));
        // Multiple selection: SelectedItems
        reviewsView.SetBinding(SelectableItemsView.SelectedItemsProperty,
            nameof(RecipeRatingsDetailViewModel.SelectedReviews));

        var reportButton = new Button { Text = "Report review(s)" };
        // CanExecute: button enables/disables itself
        reportButton.SetBinding(Button.CommandProperty,
            nameof(RecipeRatingsDetailViewModel.ReportReviewsCommand));

        var reportMessageLabel = new Label { FontAttributes = FontAttributes.Italic };
        reportMessageLabel.SetBinding(Label.TextProperty,
            nameof(RecipeRatingsDetailViewModel.ReportMessage));

        var backButton = new Button { Text = "Go back" };
        backButton.Clicked += OnBackClicked;

        // CollectionView needs a limited height (* row), never inside a ScrollView
        var grid = new Grid
        {
            Padding = 20,
            RowSpacing = 10,
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            }
        };
        grid.Add(_headerLabel, column: 0, row: 0);
        grid.Add(reviewsView, column: 0, row: 1);
        grid.Add(new VerticalStackLayout
        {
            Spacing = 10,
            Children = { reportMessageLabel, reportButton, backButton }
        }, column: 0, row: 2);

        Content = grid;

        BindingContext = new RecipeRatingsDetailViewModel();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("recipeTitle", out var value) && value is string title)
        {
            _headerLabel.Text = $"Reviews for {title}";
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");
}
