using System.Collections.ObjectModel;
using System.Text.Json;
using SocialFeed.Models;

namespace SocialFeed;

public partial class PostDetailPage : ContentPage
{
    private readonly ObservableCollection<string> _comments = new();
    private readonly string _postId;

    public PostDetailPage(Post post)
    {
        InitializeComponent();

        _postId = post.Id;

        LoadComments();
        UpdateCommentCount();

        CommentsCollection.BindingContext = _comments;
        CommentsCollection.ItemsSource = _comments;
    }

    private void LoadComments()
    {
        var key = $"comments_{_postId}";
        var json = Preferences.Get(key, string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            var saved = JsonSerializer.Deserialize<List<string>>(json);
            if (saved != null)
            {
                foreach (var c in saved)
                    _comments.Add(c);
            }
        }
    }

    private void SaveComments()
    {
        var key = $"comments_{_postId}";
        var json = JsonSerializer.Serialize(_comments.ToList());
        Preferences.Set(key, json);
    }

    private void UpdateCommentCount()
    {
        var count = _comments.Count;
        CommentCountLabel.Text = count switch
        {
            0 => "No comments",
            1 => "1 comment",
            _ => $"{count} comments"
        };
    }

    private void OnCommentSubmitted(object? sender, EventArgs e)
    {
        var text = CommentEntry.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(text))
        {
            _comments.Add(text);
            CommentEntry.Text = string.Empty;
            SaveComments();
            UpdateCommentCount();
        }
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
