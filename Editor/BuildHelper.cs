using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public static class BuildHelper
{

  [MenuItem ("Build/Save Version Information")]
  public static void SaveVersionInformation ()
  {
    string companyName;
    string gameName;
    string informalVersion;

    using (StreamReader reader = File.OpenText ("Assets/bower.json")) {
      JObject o = (JObject)JToken.ReadFrom (new JsonTextReader (reader));

      informalVersion = (string)o ["version"];
      companyName = (string)o ["company"];
      gameName = (string)o ["name"];

    }
     

    string[] parts = informalVersion.Split (new char[]{ '.' });
    string version = string.Format ("{0}.{1}.*.*", parts [0], parts [1]);

    string fileContents = string.Format (
                            "using System.Reflection;\n\n" +
                            "[assembly:AssemblyVersion (\"{0}\")]\n" +
                            "[assembly:AssemblyCompany (\"{1}\")]\n" +
                            "[assembly:AssemblyProduct (\"{2}\")]\n" +
                            "[assembly:AssemblyInformationalVersion (\"{3}\")]",
                            version,
                            companyName,
                            gameName,
                            informalVersion);

    Debug.Log (fileContents);

    File.WriteAllText ("Assets/AssemblyVersion.cs", fileContents);
  }
}
