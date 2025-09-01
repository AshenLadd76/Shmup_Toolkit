using System;

namespace ToolBox.Services.Web
{
    public interface IFormData
    {
        string JsonData { get; set; }
        string Url { get; set; }
        string Title { get; set; }
    }

    public class FormData : IFormData
    {
        public string JsonData { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }

        public FormData( string title, string url, string jsonData )
        {
            // Validate parameters
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(url) || string.IsNullOrEmpty( jsonData ))
                throw new ArgumentException("All parameters must be non-null and non-empty.");
            
            
            this.JsonData = jsonData;
            this.Url = url;
            this.Title = title;
        }
    }
}