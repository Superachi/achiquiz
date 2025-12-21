namespace AchiQuiz.addons.scene_to_csharp;

public sealed class SceneNodeInfo
{
    public string Name { get; }
    public string Type { get; }
    public string Parent { get; }

    // Computed later
    public string FullPath { get; set; }

    public SceneNodeInfo(string name, string type, string parent)
    {
        Name = name;
        Type = type;
        Parent = parent;
    }
}

