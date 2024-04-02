using System.ComponentModel.DataAnnotations;

namespace ImagePreviews.WebApp.Settings
{
    public class GhostScriptSettings
    {
        [Required]
        public string PathExecutable { get; set; } = String.Empty;

        [Required]
        public string Variables { get; set; } = String.Empty;

        [Required]
        [Range(1000, int.MaxValue)]
        public int TimeoutMs { get; set; }
    }
}
