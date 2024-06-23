using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class FolderPathViewer : PropertyAttribute
    {
        public string ParentFolder { get; private set; }
        public FolderPathType FolderPathType { get; private set; }

        public FolderPathViewer(FolderPathType folderPathType, string parentFolder = "")
        {
            ParentFolder = parentFolder;
            FolderPathType = folderPathType;
        }
    }

    public enum FolderPathType
    {
        Absolute,
        Relative,
        GUID
    }
}
