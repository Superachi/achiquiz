using Godot;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

[Tool]
public partial class SceneToCSharp : EditorPlugin
{
    private const string MenuItemName = "Generate C# nodes from Scene";

    private static readonly Regex NodeRegex =
        new Regex(@"\[node\s+name=""(?<name>[^""]+)""\s+type=""(?<type>[^""]+)""",
            RegexOptions.Compiled);

    public override void _EnterTree()
    {
        AddToolMenuItem(MenuItemName, Callable.From(Generate));
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
        var nodes = new List<(string Name, string Type)>();

        foreach (Match match in NodeRegex.Matches(tscnText))
        {
            nodes.Add((
                match.Groups["name"].Value,
                match.Groups["type"].Value
            ));
        }

        return GenerateCSharp(nodes);
    }

    private string GenerateCSharp(List<(string Name, string Type)> nodes)
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
                $"    _{ToCamelCase(node.Name)} = GetNode<{node.Type}>(\"{node.Name}\");");
        }

        sb.AppendLine("}");

        return sb.ToString();
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
