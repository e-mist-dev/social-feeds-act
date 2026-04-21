using SocialFeed.Models;

namespace SocialFeed;

public partial class AddPostPage : ContentPage
{
    public Post? NewPost { get; private set; }
    private string? _selectedImagePath;

    public AddPostPage()
    {
        InitializeComponent();
    }

    private void OnContentTextChanged(object? sender, TextChangedEventArgs e)
    {
        var length = e.NewTextValue?.Length ?? 0;
        CharCountLabel.Text = $"{length}/500";
        CharCountLabel.TextColor = length > 450 ? Color.FromArgb("#FF6B6B") : Color.FromArgb("#999999");
    }

    private void OnVisibilityToggled(object? sender, ToggledEventArgs e)
    {
        VisibilityLabel.Text = e.Value 
            ? "Public - Anyone can see this post" 
            : "Private - Only you can see this post";
    }

    private async void OnPickImageClicked(object? sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Select an image"
            });

            if (result is not null)
            {
                _selectedImagePath = result.FullPath;
                ImageFileLabel.Text = result.FileName;
                ImagePreview.Source = ImageSource.FromFile(result.FullPath);
                ImagePreview.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to pick image: {ex.Message}", "OK");
        }
    }

    private void OnRemoveImageClicked(object? sender, EventArgs e)
    {
        _selectedImagePath = null;
        ImageFileLabel.Text = "No image selected";
        ImagePreview.Source = null;
        ImagePreview.IsVisible = false;
    }

    private async void OnPostClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(DescriptionEntry.Text))
        {
            await DisplayAlert("Validation", "Please enter an author name.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(ContentEditor.Text) && string.IsNullOrWhiteSpace(_selectedImagePath))
        {
            await DisplayAlert("Validation", "Please enter some content or upload an image.", "OK");
            return;
        }

        NewPost = new Post
        {
            Author = DescriptionEntry.Text.Trim(),
            Content = ContentEditor.Text?.Trim() ?? string.Empty,
            ImagePath = _selectedImagePath,
            CreatedAt = DateTime.Now,
            IsPublic = VisibilitySwitch.IsToggled
        };

        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
