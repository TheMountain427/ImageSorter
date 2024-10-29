using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImageSorter.Models
{
    public static class Helpers
    {
        public static bool TestProjectConfig(ProjectConfig projectConfig)
        {
            return TestProjectConfigImagePaths(projectConfig)
                && TestProjectConfigOutputPaths(projectConfig);
        }

        public static bool TestProjectConfigImagePaths(ProjectConfig projectConfig)
        {
            if (projectConfig is null)
            {
                return false;
            }

            if (projectConfig.ImgDirectoryPaths is null || projectConfig.ImgDirectoryPaths.Count == 0)
            {
                return false;
            }

            foreach (var path in projectConfig.ImgDirectoryPaths)
            {
                if (!Directory.Exists(path))
                {
                    return false;
                }
            }

            return true;

        }

        public static bool TestProjectConfigOutputPaths(ProjectConfig projectConfig)
        {
            if (projectConfig is null)
            {
                return false;
            }

            if (projectConfig.OutputDirectoryPath is null || projectConfig.OutputDirectoryPath.Count == 0)
            {
                return false;
            }

            foreach (var path in projectConfig.OutputDirectoryPath)
            {
                if (!Directory.Exists(path))
                {
                    return false;
                }
            }

            return true;

        }

        public static string? TryGetProjectConfigPath(AppState appState, string projectName)
        {
            var path = Path.Join(appState.ProjectConfigsPath, Helpers.ProjectNameToFileName(projectName));
            if (File.Exists(path))
            {
                return path;
            }
            else
            {
                return null;
            }
        }

        public static string CheckProjectNameDuplicates(AppState appState, string ProjectName)
        {
            var files = Directory.GetFiles(appState.ProjectConfigsPath, "*.json");
            if (files.Length == 0)
            {
                return ProjectName;
            }
            else
            {
                var THIS_IS_AN_ENUMERABLE = files.AsEnumerable().Where(x => x.EndsWith(ProjectNameToFileName(ProjectName)));
                if (THIS_IS_AN_ENUMERABLE.Count() == 0)
                {
                    return ProjectName;
                }
                else
                {
                    return $"{ProjectName} ({THIS_IS_AN_ENUMERABLE.Count()})";
                }
            }
        }

        public static Uri UriFromPath(string path)
        {
            var builder = new UriBuilder();
            builder.Path = path;
            return builder.Uri;
        }

        public static ProjectConfig GetProjectConfigFromJson(string FilePath)
        {
            return JsonSerializer.Deserialize<ProjectConfig>(File.ReadAllText(FilePath));
        }

        public static string ProjectNameToFileName(string ProjectName)
        {
            return $"{ProjectName.Trim()}.json";
        }

    }
}
