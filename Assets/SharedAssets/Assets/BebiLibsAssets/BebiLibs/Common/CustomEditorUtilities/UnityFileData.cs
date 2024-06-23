using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
namespace CustomEditorUtilities
{
    public class UnityFileData
    {
        public string MetaFilePath { get; }
        public string FilePath { get; }
        public string Guid { get; }
        public string FullGuidString { get; }
        public string AssetName { get; }
        public bool IsDirectory { get; }
        public bool IsTestable { get; }
        public UnityFileData(string metaFilePath)
        {
            MetaFilePath = metaFilePath;
            FilePath = UnityFileUtils.GetMainFilePath(metaFilePath);
            Guid = UnityFileUtils.GetGuid(metaFilePath);
            FullGuidString = UnityFileUtils.GetGuid(metaFilePath);
            Guid = FullGuidString.Replace("guid: ", "");
            IsDirectory = UnityFileUtils.IsDirectory(FilePath);
            AssetName = !IsDirectory ? Path.GetFileName(FilePath) : new DirectoryInfo(FilePath).Name;
            IsTestable = UnityFileUtils.IsTestableFile(FilePath);
        }

        public override string ToString()
        {
            return $"{FilePath} - {Guid} - {IsDirectory}";
        }

    }
}
#endif

