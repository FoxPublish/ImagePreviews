using System.ComponentModel.DataAnnotations;

namespace ImagePreviews.WebApp.Settings
{
    public class PdfRedirectSettings
    {
        public bool Active { get; set; } = false;

        [Url]
        public string Url { get; set; } = String.Empty; 
    }
}
