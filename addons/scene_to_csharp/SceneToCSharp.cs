using Godot;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using AchiQuiz.addons.scene_to_csharp;
using Newtonsoft.Json;

[Tool]
public partial class SceneToCSharp : EditorPlugin
{
    private const string MenuItemName = "Generate C# nodes from Scene";

    private static readonly Regex NodeRegex =
        new Regex(
            @"\[node\s+name=""(?<name>[^""]+)""\s+type=""(?<type>[^""]+)""\s+parent=""(?<parent>[^""]+)""",
            RegexOptions.Compiled
        );

    public override void _EnterTree()
    {
        AddToolMenuItem(MenuItemName, new Callable(this, nameof(Generate)));
    }

    public override void _ExitTree()
    {
        RemoveToolMenuItem(MenuItemName);
    }

    private void Generate()
    {
        var selection = EditorInterface
            .Singleton
            .GetSelection()
            .GetSelectedNodes();

        if (selection.Count == 0)
        {
            ShowError("No scene selected.");
            return;
        }

        // We expect a FileSystem item
        if (selection[0] is not Node)
        {
            ShowError("Select a .tscn file in the FileSystem dock.");
            return;
        }

        string path = GetSelectedFilePath();
        if (string.IsNullOrEmpty(path) || !path.EndsWith(".tscn"))
        {
            ShowError("Selected file is not a .tscn scene.");
            return;
        }

        string text = FileAccess.GetFileAsString(path);
        string output = Convert(text);

        DisplayServer.ClipboardSet(output);

        GD.Print("C# node bindings copied to clipboard.");
    }

    private string GetSelectedFilePath()
    {
        return EditorInterface.Singleton.GetCurrentPath();
    }

    private string Convert(string tscnText)
    {
        var nodes = new List<SceneNodeInfo>();

        foreach (Match match in NodeRegex.Matches(tscnText))
        {
            nodes.Add(new SceneNodeInfo(
                match.Groups["name"].Value,
                match.Groups["type"].Value,
                match.Groups["parent"].Value
            ));
        }

        GD.Print("Building paths for the following nodes:");
        foreach (var nodeInfo in nodes)
        {
            GD.Print($"  - Node: {nodeInfo.Name}, Parent: {nodeInfo.Parent}, Type: {nodeInfo.Type}");
        }

        BuildFullPaths(nodes);
        return GenerateCSharp(nodes);
    }

    private string GenerateCSharp(List<SceneNodeInfo> nodes)
    {
        var sb = new StringBuilder();

        foreach (var node in nodes)
        {
            sb.AppendLine(
                $"private {node.Type} _{ToCamelCase(node.Name)};");
        }

        sb.AppendLine();
        sb.AppendLine("public override void _Ready()");
        sb.AppendLine("{");

        foreach (var node in nodes)
        {
            sb.AppendLine(
                $"    _{ToCamelCase(node.Name)} = GetNode<{node.Type}>(\"{node.FullPath}\");");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private void BuildFullPaths(List<SceneNodeInfo> nodes)
    {
        var lookup = new Dictionary<string, SceneNodeInfo>();

        // Map node name → node
        foreach (var node in nodes)
            lookup[node.Name] = node;

        foreach (var node in nodes)
        {
            node.FullPath = BuildPath(node, lookup);
        }
    }

    private string BuildPath(SceneNodeInfo node, Dictionary<string, SceneNodeInfo> lookup)
    {
        if (node.Parent == ".")
            return node.Name;

        if (!lookup.TryGetValue(node.Parent, out var parentNode))
            return node.Name; // fallback (shouldn't happen in valid scenes)

        return $"{BuildPath(parentNode, lookup)}/{node.Name}";
    }

    private static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToLowerInvariant(input[0]) + input.Substring(1);
    }

    private void ShowError(string message)
    {
        GD.PushError(message);
    }
}
