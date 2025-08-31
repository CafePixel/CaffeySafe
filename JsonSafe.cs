using System.Text.Json;

namespace Caffey.Utils;

public static class JsonSafe
{

    public static void SaveToJson<T>(string path, T data, bool over = false)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path), "Path can't be void.");

        if (File.Exists(path) && !over)
            throw new UnauthorizedAccessException("Can't overwrite Json.");

        string json = null;

        try
        {
            json = JsonSerializer.Serialize(data, new JsonSerializerOptions{ WriteIndented = true });
        }
        catch (Exception e)
        {
            throw new Exception($"Can't serialize : {e.Message}", e);
        }

        if (json == null)
            throw new Exception("Can't make json, unknown error.");

        string? directory = Path.GetDirectoryName(path);

        if (directory == null)
            throw new NullReferenceException("Can't make directory.");

        if (!Directory.Exists(directory))
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (Exception e)
            {
                throw new Exception($"Can't make directory : {e.Message}", e);
            }

        try
        {
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            throw new Exception($"Can't write json file at {path} /n Error : {e.Message}", e);
        }

    }

    public static T LoadToJson<T>(string path)
    {

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path), "Path is empty");
            
        if (!File.Exists(path))
            throw new ArgumentNullException($"File not exist à : {path}");

        string? json = null;

        try
        {
            json = File.ReadAllText(path);
        }
        catch (Exception e)
        {
            throw new Exception("Can't read json.", e);
        }

        if (string.IsNullOrWhiteSpace(json))
            throw new JsonException("Can't read json, json null.");

        try
        {
            var obj = JsonSerializer.Deserialize<T>(json);

            if (obj == null)
                throw new NullReferenceException("Can't deserialise Json.");
            
            return obj;
        }
        catch (Exception e)
        {
            throw new Exception($"Deserialization error, {e.Message}", e);
        }

    }

}