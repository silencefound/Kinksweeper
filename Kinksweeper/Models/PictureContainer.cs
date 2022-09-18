using System.Threading.Tasks;

namespace Kinksweeper.Models;

public class PictureContainer
{
    public readonly string PictureURL;
    public readonly long postID;
    
    private Avalonia.Media.Imaging.Bitmap? _picture;

    public PictureContainer(string pictureUrl, long postId)
    {
        PictureURL = pictureUrl;
        postID = postId;
    }

    public async Task<Avalonia.Media.Imaging.Bitmap?> GetPicture()
    {
        if (_picture is null)
        {
            await using var stream = await Rule34ImageProvider.DownloadImage(PictureURL);
            if (stream != null) _picture = new Avalonia.Media.Imaging.Bitmap(stream);
        }

        return _picture;
    }
}