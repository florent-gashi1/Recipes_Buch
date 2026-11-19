using Recipes.Client.Core.ViewModels;
using Recipes.Mobile.TemplateSelectors;

namespace Recipes.Mobile;

public class RecipeDetailPage : ContentPage
{
    private readonly RecipeDetailViewModel _viewModel = new();

    public RecipeDetailPage()
    {
        var titleLabel = new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = 22
        };
        titleLabel.SetBinding(
            Label.TextProperty,
            nameof(RecipeDetailViewModel.Title),
            BindingMode.OneTime);

        var allergenCheckBox = new CheckBox();
        allergenCheckBox.SetBinding(
            CheckBox.IsCheckedProperty,
            nameof(RecipeDetailViewModel.ShowAllergenInformation),
            BindingMode.OneWayToSource);

        var allergenLabel = new Label { Text = "allergen information" };
        allergenLabel.SetBinding(
            IsVisibleProperty,
            nameof(RecipeDetailViewModel.ShowAllergenInformation),
            BindingMode.OneWay);

        var allergenSection = new VerticalStackLayout
        {
            Padding = 10,
            Children =
            {
                new HorizontalStackLayout
                {
                    Children =
                    {
                        new Label
                        {
                            FontAttributes = FontAttributes.Italic,
                            Text = "Show Allergen information",
                            VerticalOptions = LayoutOptions.Center
                        },
                        allergenCheckBox
                    }
                },
                allergenLabel
            }
        };

        var servingsLabel = new Label
        {
            Margin = new Thickness(10, 0),
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center
        };
        servingsLabel.SetBinding(
            Label.TextProperty,
            "IngredientsList.NumberOfServings",
            BindingMode.OneWay);

        var servingsStepper = new Stepper { Minimum = 1, Maximum = 8 };
        servingsStepper.SetBinding(
            Stepper.ValueProperty,
            "IngredientsList.NumberOfServings",
            BindingMode.TwoWay);

        var servingsSection = new HorizontalStackLayout
        {
            Padding = 10,
            Children =
            {
                new Label { Text = "Number of servings:", VerticalOptions = LayoutOptions.Center },
                servingsLabel,
                servingsStepper
            }
        };

        // Collections: the three list sections
        var ingredientsSection = CreateIngredientsSection();
        var shoppingListSection = CreateShoppingListSection();
        var instructionsSection = CreateInstructionsSection();

        var reviewsButton = new Button { Text = "Show reviews (push)" };
        reviewsButton.Clicked += OnShowReviewsClicked;

        var addRatingButton = new Button { Text = "Add rating (modal)" };
        addRatingButton.Clicked += OnAddRatingClicked;

        var navigationSection = new VerticalStackLayout
        {
            Padding = 10,
            Spacing = 10,
            Children = { reviewsButton, addRatingButton }
        };

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 10,
                Children =
                {
                    titleLabel, allergenSection, servingsSection,
                    ingredientsSection, shoppingListSection, instructionsSection,
                    navigationSection
                }
            }
        };

        BindingContext = _viewModel;
    }

    // BindableLayout: ingredients list
    private static View CreateIngredientsSection()
    {
        var ingredientsStack = new VerticalStackLayout { Margin = new Thickness(0, 10), Spacing = 10 };

        // BindableLayout: ItemsSource (what to show)
        ingredientsStack.SetBinding(
            BindableLayout.ItemsSourceProperty,
            "IngredientsList.Ingredients",
            BindingMode.OneTime);

        // BindableLayout: ItemTemplate (how one item looks)
        BindableLayout.SetItemTemplate(ingredientsStack, new DataTemplate(CreateIngredientRow));

        return new VerticalStackLayout
        {
            Padding = 10,
            Children =
            {
                new Label { Text = "Ingredients", FontAttributes = FontAttributes.Italic | FontAttributes.Bold, FontSize = 16 },
                ingredientsStack
            }
        };
    }

    // DataTemplate: one ingredient row, BindingContext = one ingredient
    private static View CreateIngredientRow()
    {
        var addButton = new Button { Text = "+", WidthRequest = 44, Padding = 0 };

        // RelativeSource binding: command from the page ViewModel
        addButton.SetBinding(Button.CommandProperty, new Binding(
            nameof(RecipeDetailViewModel.AddToShoppingListCommand),
            source: new RelativeBindingSource(
                RelativeBindingSourceMode.FindAncestorBindingContext,
                typeof(RecipeDetailViewModel))));

        // CommandParameter "." = this row's item
        addButton.SetBinding(Button.CommandParameterProperty, ".");

        return CreateIngredientLine(addButton);
    }

    // DataTemplate: labels shared by ingredients and shopping list
    private static View CreateIngredientLine(Button button)
    {
        var nameLabel = new Label { FontAttributes = FontAttributes.Bold, FontSize = 16, VerticalOptions = LayoutOptions.Center };
        nameLabel.SetBinding(Label.TextProperty, new Binding(
            nameof(RecipeIngredientViewModel.IngredientName), BindingMode.OneTime, stringFormat: "{0}:"));

        var amountLabel = new Label { VerticalOptions = LayoutOptions.Center };
        amountLabel.SetBinding(Label.TextProperty, new Binding(
            nameof(RecipeIngredientViewModel.DisplayAmount), BindingMode.OneWay, stringFormat: "{0:0.##}"));

        var measurementLabel = new Label { VerticalOptions = LayoutOptions.Center };
        measurementLabel.SetBinding(Label.TextProperty,
            nameof(RecipeIngredientViewModel.Measurement), BindingMode.OneTime);

        return new HorizontalStackLayout
        {
            Spacing = 5,
            Children = { button, nameLabel, amountLabel, measurementLabel }
        };
    }

    // ObservableCollection: shopping list
    private static View CreateShoppingListSection()
    {
        var shoppingStack = new VerticalStackLayout { Margin = new Thickness(0, 10), Spacing = 10 };

        shoppingStack.SetBinding(
            BindableLayout.ItemsSourceProperty,
            nameof(RecipeDetailViewModel.ShoppingList),
            BindingMode.OneTime);

        // BindableLayout: EmptyView
        BindableLayout.SetEmptyView(shoppingStack, "Nothing added");

        BindableLayout.SetItemTemplate(shoppingStack, new DataTemplate(() =>
        {
            var removeButton = new Button { Text = "−", WidthRequest = 44, Padding = 0 };
            removeButton.SetBinding(Button.CommandProperty, new Binding(
                nameof(RecipeDetailViewModel.RemoveFromShoppingListCommand),
                source: new RelativeBindingSource(
                    RelativeBindingSourceMode.FindAncestorBindingContext,
                    typeof(RecipeDetailViewModel))));
            removeButton.SetBinding(Button.CommandParameterProperty, ".");

            return CreateIngredientLine(removeButton);
        }));

        return new VerticalStackLayout
        {
            Margin = new Thickness(10, 0),
            Padding = 10,
            Children =
            {
                new Label { Text = "Shopping list", FontAttributes = FontAttributes.Italic | FontAttributes.Bold, FontSize = 16 },
                shoppingStack
            }
        };
    }

    // DataTemplateSelector: instructions and notes
    private static View CreateInstructionsSection()
    {
        var instructionTemplate = new DataTemplate(() =>
        {
            var indexLabel = new Label { FontSize = 20 };
            indexLabel.SetBinding(Label.TextProperty, new Binding(
                nameof(InstructionViewModel.Index), BindingMode.OneTime, stringFormat: "{0:D2}."));

            var descriptionLabel = new Label { Margin = new Thickness(10, 0) };
            descriptionLabel.SetBinding(Label.TextProperty,
                nameof(InstructionViewModel.Description), BindingMode.OneTime);

            return new VerticalStackLayout { Spacing = 10, Children = { indexLabel, descriptionLabel } };
        });

        var noteTemplate = new DataTemplate(() =>
        {
            var noteLabel = new Label { FontAttributes = FontAttributes.Italic, TextColor = Colors.LightSlateGray };
            noteLabel.SetBinding(Label.TextProperty, nameof(NoteViewModel.Note), BindingMode.OneTime);

            var grid = new Grid
            {
                Margin = new Thickness(20, 0),
                ColumnDefinitions = { new ColumnDefinition(35), new ColumnDefinition(GridLength.Star) }
            };
            grid.Add(new Label { Text = "💡", FontSize = 20 }, column: 0);
            grid.Add(noteLabel, column: 1);
            return grid;
        });

        var instructionsStack = new VerticalStackLayout { Margin = new Thickness(0, 10), Spacing = 10 };
        instructionsStack.SetBinding(
            BindableLayout.ItemsSourceProperty,
            nameof(RecipeDetailViewModel.Instructions),
            BindingMode.OneTime);

        // DataTemplateSelector: picks a template per item
        BindableLayout.SetItemTemplateSelector(instructionsStack, new InstructionsDataTemplateSelector
        {
            InstructionTemplate = instructionTemplate,
            NoteTemplate = noteTemplate
        });

        return new VerticalStackLayout
        {
            Padding = 10,
            Children =
            {
                new Label { Text = "Instructions", FontAttributes = FontAttributes.Italic | FontAttributes.Bold, FontSize = 16 },
                instructionsStack
            }
        };
    }

    private async void OnShowReviewsClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("reviews", new Dictionary<string, object>
        {
            { "recipeTitle", _viewModel.Title }
        });

    private async void OnAddRatingClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("addrating");
}
