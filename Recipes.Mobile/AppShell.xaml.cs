namespace Recipes.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute("reviews", typeof(ReviewsPage));
		Routing.RegisterRoute("addrating", typeof(AddRatingPage));
	}
}
