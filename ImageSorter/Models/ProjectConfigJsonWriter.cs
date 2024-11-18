using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImageSorter.Models
{
    internal class ProjectConfigJsonWriter
    {
        private JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = true };

        // Event handler for writing app state to json on change
        public void WriteProjectConfigState(object? sender, EventArgs e)
        {
            if (sender is ProjectConfig projectConfig)
            {
                if (projectConfig.JsonWriterEnabled)
                {
                    File.WriteAllText(projectConfig.ProjectConfigPath, JsonSerializer.Serialize(projectConfig, this.JsonOptions));
                }
            }
        }
    }
}
