using System.ComponentModel.DataAnnotations;

namespace ImagePreviews.WebApp.Settings
{
    public class CacheSettings
    {
        [Required]
        public string Dir { get; set; } = String.Empty;

        [Required]
        [Range(1, long.MaxValue)]
        public long WriteQueueMemoryMb { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public long CacheSizeMb { get; set; }

        public double MinAgeToDelete { get; set; } = 10.0d;
    }
}
