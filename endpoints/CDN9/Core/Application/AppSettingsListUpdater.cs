using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CDN9.Core.Application;

public static class AppSettingsListUpdater
{
    private const string FilePath = "appsettings.env.json";

    public static void AddToList(IWebHostEnvironment env, string section, string newValue)
    {
        var json = JObject.Parse(File.ReadAllText(FilePath.Replace("env", env.EnvironmentName)));

        var list = (JArray)json[section];
        if (!list.Contains(newValue))
            list.Add(newValue);

        File.WriteAllText($"{env.ContentRootPath}/{FilePath.Replace("env", env.EnvironmentName)}", json.ToString(Formatting.Indented));
    }

    public static void RemoveFromList(IWebHostEnvironment env, string section, string valueToRemove)
    {
        var json = JObject.Parse(File.ReadAllText(FilePath.Replace("env", env.EnvironmentName)));

        var list = (JArray)json[section];
        var b = list.ToList();
        b.Remove(valueToRemove);

        json[section] = JArray.FromObject(b);

        File.WriteAllText($"{env.ContentRootPath}/{FilePath.Replace("env", env.EnvironmentName)}", json.ToString(Formatting.Indented));
    }
}
