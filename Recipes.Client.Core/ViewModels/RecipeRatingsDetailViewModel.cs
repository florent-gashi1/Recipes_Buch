using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Recipes.Client.Core.ViewModels;

public class UserReviewViewModel
{
    public string ReviewerName { get; }
    public double Rating { get; }
    public string Review { get; }

    public UserReviewViewModel(string reviewerName, double rating, string review)
    {
        ReviewerName = reviewerName;
        Rating = rating;
        Review = review;
    }
}

// CollectionView grouping: a list with a Key
public class RatingGroup : List<UserReviewViewModel>
{
    public string Key { get; }

    public RatingGroup(string key, List<UserReviewViewModel> reviews) : base(reviews)
    {
        Key = key;
    }
}

public class RecipeRatingsDetailViewModel : ObservableObject
{
    public List<UserReviewViewModel> Reviews { get; } = new()
    {
        new("Anna", 5, "Best Caesar salad I've ever made!"),
        new("Marco", 4.8, "Great dressing, I added extra garlic."),
        new("Lea", 4.4, "Very good, the croutons tip is gold."),
        new("Tom", 3, "Fine, but a bit too salty for me."),
        new("Sara", 3.2, "Okay. My kids didn't like the anchovies."),
        new("Jonas", 4.6, "Quick and tasty."),
        new("Mia", 1.5, "Not my thing."),
        new("Ben", 5, "Perfect for summer evenings."),
    };

    // CollectionView grouping: list of groups
    private List<RatingGroup> _groupedReviews = new();
    public List<RatingGroup> GroupedReviews
    {
        get => _groupedReviews;
        private set => SetProperty(ref _groupedReviews, value);
    }

    // CollectionView multiple selection: SelectedItems
    public ObservableCollection<object> SelectedReviews { get; } = new();

    public IRelayCommand ReportReviewsCommand { get; }

    private string _reportMessage = "Select reviews to report them.";
    public string ReportMessage
    {
        get => _reportMessage;
        private set => SetProperty(ref _reportMessage, value);
    }

    public RecipeRatingsDetailViewModel()
    {
        // CollectionView grouping: group the reviews by rating
        GroupedReviews = Reviews
            .GroupBy(r => Math.Round(r.Rating / .5) * .5)
            .OrderByDescending(g => g.Key)
            .Select(g => new RatingGroup(g.Key.ToString(), g.ToList()))
            .ToList();

        // CanExecute: only enabled while something is selected
        ReportReviewsCommand = new RelayCommand(ReportReviews, () => SelectedReviews.Any());

        // ObservableCollection: CollectionChanged re-checks CanExecute
        SelectedReviews.CollectionChanged += SelectedReviews_CollectionChanged;
    }

    private void SelectedReviews_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => ReportReviewsCommand.NotifyCanExecuteChanged();

    private void ReportReviews()
    {
        var selected = SelectedReviews.Cast<UserReviewViewModel>().ToList();

        ReportMessage = $"Reported: {string.Join(", ", selected.Select(r => r.ReviewerName))}";

        SelectedReviews.Clear();
    }
}
