using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImageSorter.Models
{
    internal class AppStateJsonWriter
    {
        private JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = true };

        // Event handler for writing app state to json on change
        public void WriteAppState(object sender, EventArgs e)
        {
            if (sender is AppState appState)
            {
                if (appState.JsonWriterEnabled)
                {
                    File.WriteAllText(appState.AppStateFilePath, JsonSerializer.Serialize(appState, this.JsonOptions));
                }
            }
        }
    }
}
