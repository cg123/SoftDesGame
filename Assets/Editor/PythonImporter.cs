using UnityEditor;
using UnityEngine;
using System.IO;

public class CustomResxImporter : AssetPostprocessor
{
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets)
        {
            if (asset.EndsWith(".py"))
            {
                string filePath = asset.Substring(0, asset.Length - Path.GetFileName(asset).Length) + "../";
                string newFileName = filePath + Path.GetFileNameWithoutExtension(asset) + ".py.txt";

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                StreamReader reader = new StreamReader(asset);
                string fileData = reader.ReadToEnd();
                reader.Close();

                FileStream resourceFile = new FileStream(newFileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(resourceFile);
                writer.Write(fileData);
                writer.Close();
                resourceFile.Close();

                AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
        }
    }
}
