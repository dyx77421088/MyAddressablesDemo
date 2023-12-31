using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileUtils
{
    /// <summary>
    /// 本地存放版本号的文件路径
    /// </summary>
    public static string versionPath = "version";
    public static string _catalogPath;

    public static string GetTextByResources(string path)
    {
        return Resources.Load<TextAsset>(path).text;
    }

    /// <summary>
    /// 在文件下的目录，和Assets同级
    /// </summary>
    public static string GetTextByPath(string path, string extension = ".txt")
    {
        using(FileStream fs = new FileStream(path + extension, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
        {
            using(StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default))
            {
                if (null == sr) return null;
                string str = sr.ReadToEnd();
                return str;
            }
        }
    }
    /// <summary>
    /// 把txt写到path路径对应的文件中
    /// </summary>
    /// <param name="path"></param>
    /// <param name="text"></param>
    public static void WriteTextByPath(string path, string text)
    {
        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        {
            using (StreamWriter sr = new StreamWriter(fs, System.Text.Encoding.Default))
            {
                if (null == sr) return;
                sr.Write(text);
            }
        }
    }

    public static void DeleteCatalog()
    {
        if (Directory.Exists(_catalogPath))
        {
            Directory.Delete(_catalogPath, true);
        }
    }

    /// <summary>
    /// 把ab包写到path路径对应的文件中
    /// </summary>
    /// <param name="path"></param>
    /// <param name="text"></param>
    public static void WriteABByPath(string path, AssetBundle ab)
    {
        //string filePath = Application.persistentDataPath + "/mybundle";
        //File.WriteAllBytes("", ab.get);
        //using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        //{
        //    using (StreamWriter sr = new StreamWriter(fs, System.Text.Encoding.Default))
        //    {
        //        if (null == sr) return;
        //        sr.Write(text);
        //    }
        //}
    }
}
