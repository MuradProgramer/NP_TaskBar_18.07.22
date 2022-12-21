using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services;

public class Service
{
    public static void JsonSerialize<T>(List<T> list, string filename)
    {
        if (File.Exists($"{filename}.json"))
            return;
        var str = JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText($"{filename}.json", str);
    }

    public static void JsonSerialize(string filename, string jsonStr)
    {
        File.WriteAllText($"{filename}.json", jsonStr);
    }

    public static void JsonDeserialize<T>(ref List<T>? list, string filename)
    {
        if (!File.Exists(filename))
            return;
        var jsonStr = File.ReadAllText(filename);
        list = JsonConvert.DeserializeObject<List<T>>(jsonStr);
    }

    public static List<T> JsonDeserialize<T>(string filename)
    {
        if (!File.Exists($"{filename}.json"))
            return null;
        var jsonStr = File.ReadAllText($"{filename}.json");
        var list = JsonConvert.DeserializeObject<List<T>>(jsonStr);
        return list;
    }

    public static string GetJSONString(string filename)
    {
        return File.ReadAllText($"{filename}.json"); ;
    }
}