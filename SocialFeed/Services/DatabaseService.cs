using SQLite;
using SocialFeed.Models;

namespace SocialFeed.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    public DatabaseService()
    {
    }

    private async Task InitAsync()
    {
        if (_database is not null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "socialfeed.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        await _database.CreateTableAsync<Post>();
    }

    public async Task<List<Post>> GetPostsAsync()
    {
        await InitAsync();
        return await _database!.Table<Post>().OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<Post?> GetPostAsync(string id)
    {
        await InitAsync();
        return await _database!.Table<Post>().Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SavePostAsync(Post post)
    {
        await InitAsync();
        
        if (string.IsNullOrEmpty(post.Id))
            post.Id = Guid.NewGuid().ToString();
        
        var existingPost = await GetPostAsync(post.Id);
        if (existingPost != null)
        {
            return await _database!.UpdateAsync(post);
        }
        else
        {
            return await _database!.InsertAsync(post);
        }
    }

    public async Task<int> DeletePostAsync(Post post)
    {
        await InitAsync();
        return await _database!.DeleteAsync(post);
    }

    public async Task<int> DeleteAllPostsAsync()
    {
        await InitAsync();
        return await _database!.DeleteAllAsync<Post>();
    }
}
