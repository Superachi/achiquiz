using AchiQuiz.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;

namespace AchiQuiz
{
    public static class ScenePaths
    {
        public static string FileExtension => ".tscn";
        public static List<string> FilePaths { get; private set; }

        public static void Initialize()
        {
            FilePaths = GetFilesRecursive("res://Scenes");
            AchiLogger.Log($"Found scenes:");
            foreach (var path in FilePaths)
            {
                AchiLogger.Log($"  - {path}");
            }
        }

        public static List<string> GetFilesRecursive(string folderPath)
        {
            var scenes = new List<string>();

            using var dir = DirAccess.Open(folderPath);
            if (dir == null) return scenes;

            dir.ListDirBegin();
            string fileName;

            while ((fileName = dir.GetNext()) != "")
            {
                if (fileName == "." || fileName == "..")
                    continue;

                string fullPath = $"{folderPath}/{fileName}";

                if (dir.CurrentIsDir())
                {
                    scenes.AddRange(GetFilesRecursive(fullPath));
                }
                else if (fileName.EndsWith(FileExtension))
                {
                    scenes.Add(fullPath);
                }
            }

            dir.ListDirEnd();
            return scenes;
        }

        public static string FindFilePath(Type nodeType)
        {
            if (Activator.CreateInstance(nodeType) is not Node)
            {
                throw new Exception("Type must be a Godot Node.");
            }

            var nodeName = nodeType.Name;
            foreach (var path in FilePaths)
            {
                if (path.Contains($"{nodeName}") && path.EndsWith(FileExtension))
                {
                    return path;
                }
            }

            throw new FileNotFoundException($"Scene '{nodeName}' not found in Scenes directory.");
        }

        public static T GetScene<T>() where T : Node
        {
            var path = FindFilePath(typeof(T));
            var packedScene = ResourceLoader.LoadResource<PackedScene>(path);
            var scene = packedScene.Instantiate<T>();
            return scene;
        }
    }
}
