using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Net.Xeophin.Utils.Version
{
  public static class BuildHelper
  {
    const string bowerPath = "Assets/bower.json";

    [MenuItem ("Build/Save Version Information")]
    public static void SaveVersionInformation ()
    {
      string companyName;
      string gameName;
      string informalVersion;

      if (!File.Exists (bowerPath)) {
        return;
      }

      //Check if bower.json exists, else return

      using (StreamReader reader = File.OpenText (bowerPath)) {
        JObject o = (JObject)JToken.ReadFrom (new JsonTextReader (reader));

        informalVersion = (string)o ["version"];
        companyName = (string)o ["company"];
        gameName = (string)o ["name"];

      }
     
      SemVer versionObject = new SemVer (informalVersion);

      // Set PlayerSettings
      PlayerSettings.companyName = companyName;
      PlayerSettings.productName = gameName;
      PlayerSettings.bundleVersion = versionObject.StableRelease;

      // Save AssemblyVersion
      string fileContents = string.Format (
                              "using System.Reflection;\n\n" +
                              "[assembly:AssemblyVersion (\"{0}\")]\n" +
                              "[assembly:AssemblyCompany (\"{1}\")]\n" +
                              "[assembly:AssemblyProduct (\"{2}\")]\n" +
                              "[assembly:AssemblyInformationalVersion (\"{3}\")]",
                              versionObject.MicrosoftFormat,
                              companyName,
                              gameName,
                              versionObject.CompleteVersionInformation);

      File.WriteAllText ("Assets/AssemblyVersion.cs", fileContents);
    }
  }

  /// <summary>
  /// A semantic version number.
  /// </summary>
  public class SemVer
  {
    public int Major, Minor, Patch;
    public string Prerelease, Build;

    public string StableRelease {
      get {
        return string.Format ("{0}.{1}.{2}", Major, Minor, Patch);
      }
    }

    public string UnstableRelease {
      get {
        if (string.IsNullOrEmpty (Prerelease)) {
          return StableRelease;
        }
        return string.Format ("{0}-{1}", StableRelease, Prerelease);
      }
    }

    public string CompleteVersionInformation {
      get {
        if (string.IsNullOrEmpty (Build)) {
          return UnstableRelease;
        }
        return string.Format ("{0}+{1}", UnstableRelease, Build);
      }
    }

    /// <summary>
    /// Gets the Microsoft format for the version, leaving * for Build and Revision number.
    /// </summary>
    /// <value>The microsoft format.</value>
    public string MicrosoftFormat {
      get {
        return string.Format (
          "{0}.{1}.*.*",
          Major,
          Minor);
      }
    }

    public override string ToString ()
    {
      return CompleteVersionInformation;
    }

    public SemVer (string versionString)
    {
      string[] vAndBuild = versionString.Split ('+');

      if (vAndBuild.Length > 1) {
        Build = vAndBuild [1];
      } else {
        Build = string.Empty;
      }

      string prereleaseString = vAndBuild [0];
      string[] vAndPrerelease = prereleaseString.Split ('-');

      if (vAndPrerelease.Length > 1) {
        Prerelease = vAndPrerelease [1];
      } else {
        Prerelease = string.Empty;
      }

      string pureVersionString = vAndPrerelease [0];

      string[] versionNumbers = pureVersionString.Split ('.');

      Major = int.Parse (versionNumbers [0]);
      Minor = int.Parse (versionNumbers [1]);
      Patch = int.Parse (versionNumbers [2]);

    }
  }
}