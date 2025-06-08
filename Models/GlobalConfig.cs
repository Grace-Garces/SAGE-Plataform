// In your GlobalConfig.cs (assuming it's in Models folder)
namespace SagePlataform.Models
{
    public class GlobalConfig
    {
        public int ConfigID { get; set; } // Assuming an ID property

        public required string ConfigKey { get; set; } // Notice the 'required' keyword here
        public required decimal ConfigValue { get; set; } // And here
        public  DateTime LastUpdated { get; set; } // Assuming LastUpdated is also part of it
    }
}