namespace Spt.Custom.Models
{
    /// <summary>
    /// Created by: SPT-Spt team
    /// Link: https://dev.sp-tarkov.com/SPT-AKI/Modules/src/branch/master/project/Spt.Custom/Models/BundleInfo.cs
    /// </summary>
    public class BundleInfo
    {
        public string Key { get; }
        public string Path { get; set; }
        public string[] DependencyKeys { get; }

        public BundleInfo(string key, string path, string[] dependencyKeys)
        {
            Key = key;
            Path = path;
            DependencyKeys = dependencyKeys;
        }
    }
}
