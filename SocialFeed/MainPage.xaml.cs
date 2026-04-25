using System.Collections.ObjectModel;
using SocialFeed.Models;
using SocialFeed.Services;

namespace SocialFeed;

public partial class MainPage : ContentPage
{
    private readonly ObservableCollection<PostViewModel> _posts = new();
    private readonly DatabaseService _databaseService;

    public MainPage(DatabaseService databaseService)
    {
        InitializeComponent();
        
        _databaseService = databaseService;
        FeedCollection.ItemsSource = _posts;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPostsAsync();
    }

    private async Task LoadPostsAsync()
    {
        try
        {
            var posts = await _databaseService.GetPostsAsync();
            _posts.Clear();
            
            foreach (var post in posts)
            {
                _posts.Add(new PostViewModel(post));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load posts: {ex.Message}", "OK");
        }
    }

    private async void OnAddPostClicked(object? sender, EventArgs e)
    {
        var addPage = new AddPostPage(_databaseService);
        addPage.Disappearing += async (s, args) =>
        {
            if (addPage.NewPost is not null)
            {
                await LoadPostsAsync();
            }
        };
        await Navigation.PushModalAsync(new NavigationPage(addPage));
    }

    private async void OnCommentsClicked(object? sender, TappedEventArgs e)
    {
        if (sender is Label label && label.BindingContext is PostViewModel vm)
        {
            await Navigation.PushModalAsync(new NavigationPage(new PostDetailPage(vm.Post)));
        }
    }
}

public class PostViewModel
{
    public Post Post { get; }
    public string Author => Post.Author;
    public string Content => Post.Content;
    public string? ImagePath => Post.ImagePath;
    public bool HasImage => !string.IsNullOrEmpty(Post.ImagePath);
    public DateTime CreatedAt => Post.CreatedAt;
    public string VisibilityIcon => Post.IsPublic ? "🌐" : "🔒";
    public string TimeAgo => CreatedAt.ToString("MMM dd, yyyy 'at' h:mm tt");

    public PostViewModel(Post post) => Post = post;
}
