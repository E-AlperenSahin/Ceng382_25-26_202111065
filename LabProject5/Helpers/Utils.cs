using System.Text.Json;
using System.Reflection;

namespace LabProject5.Helpers
{

    /* < Ai Prompt : Json Exportu ve Utils i nasıl yaparım */
    public class Utils
    {
        private static readonly Utils _instance = new Utils();
        public static Utils Instance => _instance;

        private Utils() { }

        
        public string ExportToJson<T>(List<T> data, List<string> selectedColumns)
        {
            if (selectedColumns == null || selectedColumns.Count == 0)
            {
                return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            }

            var filtered = data.Select(item =>
            {
                var obj = new Dictionary<string, object>();
                foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (selectedColumns.Contains(prop.Name))
                    {
                        obj[prop.Name] = prop.GetValue(item);
                    }
                }
                return obj;
            }).ToList();

            return JsonSerializer.Serialize(filtered, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
