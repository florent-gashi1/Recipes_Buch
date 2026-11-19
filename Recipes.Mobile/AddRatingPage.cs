namespace Recipes.Mobile;

public class AddRatingPage : ContentPage
{
    public AddRatingPage()
    {
        Shell.SetPresentationMode(this, PresentationMode.ModalAnimated);

        var ratingLabel = new Label { FontSize = 40, HorizontalOptions = LayoutOptions.Center };
        var ratingStepper = new Stepper { Minimum = 1, Maximum = 5, Value = 3, HorizontalOptions = LayoutOptions.Center };
        ratingLabel.Text = $"{ratingStepper.Value} ★";
        ratingStepper.ValueChanged += (sender, e) => ratingLabel.Text = $"{e.NewValue} ★";

        var saveButton = new Button { Text = "Save" };
        var cancelButton = new Button { Text = "Cancel" };
        saveButton.Clicked += OnCloseClicked;
        cancelButton.Clicked += OnCloseClicked;

        Content = new VerticalStackLayout
        {
            Padding = new Thickness(20, 60, 20, 20),
            Spacing = 20,
            Children =
            {
                new Label { Text = "Rate this recipe", FontSize = 22, FontAttributes = FontAttributes.Bold },
                ratingLabel,
                ratingStepper,
                saveButton,
                cancelButton
            }
        };
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");
}
