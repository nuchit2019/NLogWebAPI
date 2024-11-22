namespace NLogWebAPI.Models
{
    public class LogEntity
    {
        public DateTime Timestamp { get; set; }
        public string? Message { get; set; }
        public string? Level { get; set; }
      
        public string? Detail { get; set; } 
    }
}
