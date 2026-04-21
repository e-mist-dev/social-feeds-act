using System.Collections.ObjectModel;
using SocialFeed.Models;

namespace SocialFeed;

public partial class MainPage : ContentPage
{
    private readonly ObservableCollection<PostViewModel> _posts = new();

    public MainPage()
    {
        InitializeComponent();

        FeedCollection.ItemsSource = _posts;
    }

    private async void OnAddPostClicked(object? sender, EventArgs e)
    {
        var addPage = new AddPostPage();
        addPage.Disappearing += (s, args) =>
        {
            if (addPage.NewPost is not null)
            {
                _posts.Insert(0, new PostViewModel(addPage.NewPost));
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
    public string TimeAgo
    {
        get
        {
            var span = DateTime.Now - CreatedAt;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
            return CreatedAt.ToString("MMM d");
        }
    }

    public PostViewModel(Post post) => Post = post;
}
