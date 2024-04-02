namespace ImagePreviews.WebApp.Services
{
    public interface IGhostScript
    {
        (string fileName, string outputFile) ConvertPageToImage(string path, string page, string imageFormat);
    }
}
