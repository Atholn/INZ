using UnityEngine;
using System.IO;

public static class Path
{
    public static string GameAppPath
    {
        get
        {
#if UNITY_EDITOR
            return Application.dataPath;
#else
            return Directory.GetCurrentDirectory();
#endif
        }
    }
}
