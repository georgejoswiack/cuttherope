using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using CutTheRopeDX.Framework.Diagnostics;
using CutTheRopeDX.Helpers;

namespace CutTheRopeDX.GameMain
{
    /// <summary>
    /// Defines a selectable menu theme loaded from tema.json.
    /// </summary>
    internal sealed record ThemeDefinition(
        string Id,
        string Name,
        MenuStyle MenuStyle,
        int ManualTheme,
        string Background,
        IReadOnlyDictionary<string, string> Replacements);

    /// <summary>
    /// Loads and provides access to the themes declared in tema.json.
    /// </summary>
    internal static class ThemeRegistry
    {
        private const string ManifestFileName = "tema.json";

        private static readonly List<ThemeDefinition> themes = LoadManifest();

        public static IReadOnlyList<ThemeDefinition> Themes => themes;

        public static int Count => themes.Count;

        public static int CurrentIndex { get; set; } = 0;

        public static ThemeDefinition Current => Get(CurrentIndex);

        public static ThemeDefinition Get(int index)
        {
            if (index < 0 || index >= themes.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return themes[index];
        }

        private static List<ThemeDefinition> LoadManifest()
        {
            List<ThemeDefinition> result = [];

            string manifestPath = Path.Combine(
                ContentPaths.GetContentRootAbsolute(),
                ManifestFileName);

            try
            {
                using Stream stream = File.OpenRead(manifestPath);
                using JsonDocument document = JsonDocument.Parse(stream);

                if (document.RootElement.ValueKind != JsonValueKind.Array)
                {
                    return result;
                }

                foreach (JsonElement entry in document.RootElement.EnumerateArray())
                {
                    if (entry.ValueKind != JsonValueKind.Object)
                    {
                        continue;
                    }

                    if (!entry.TryGetProperty("id", out JsonElement idElement) ||
                        !entry.TryGetProperty("name", out JsonElement nameElement) ||
                        !entry.TryGetProperty("menuStyle", out JsonElement menuStyleElement) ||
                        !entry.TryGetProperty("manualTheme", out JsonElement manualThemeElement))
                    {
                        continue;
                    }

                    string id = idElement.GetString();
                    string name = nameElement.GetString();
                    string menuStyleValue = menuStyleElement.GetString();

                    string background = entry.TryGetProperty("background", out JsonElement backgroundElement)
                        ? backgroundElement.GetString()
                        : null;

                    Dictionary<string, string> replacements = new(StringComparer.OrdinalIgnoreCase);
                    foreach (JsonProperty property in entry.EnumerateObject())
                    {
                        if (property.NameEquals("id") ||
                            property.NameEquals("name") ||
                            property.NameEquals("menuStyle") ||
                            property.NameEquals("manualTheme") ||
                            property.NameEquals("background"))
                        {
                            continue;
                        }

                        if (property.Value.ValueKind == JsonValueKind.String)
                        {
                            string value = property.Value.GetString();

                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                replacements[property.Name] = value;
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(id) ||
                        string.IsNullOrWhiteSpace(name) ||
                        string.IsNullOrWhiteSpace(menuStyleValue) ||
                        !Enum.TryParse(menuStyleValue, true, out MenuStyle menuStyle) ||
                        !manualThemeElement.TryGetInt32(out int manualTheme))
                    {
                        continue;
                    }

                    result.Add(new ThemeDefinition(
                        id,
                        name,
                        menuStyle,
                        manualTheme,
                        background,
                        replacements));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Unable to load theme manifest: {ManifestFileName}");
                Console.WriteLine(exception);
            }

            return result;
        }
    }
}




