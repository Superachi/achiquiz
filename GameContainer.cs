using Achi.Godot.Common;
using CardRoguelike.Extensions;
using Godot;

namespace AchiQuiz;

public partial class GameContainer : Node
{
    public static Singleton<GameContainer> Singleton { get; private set; } = new();

    public override void _Ready()
    {
        Singleton.MarkAsSingleton(this);
        CallDeferred(MethodName.Setup);
    }

    private void Setup()
    {
        var root = GetTree().Root;

        var autoloadContainer = new Node
        {
            Name = AutoloadContainerName
        };

        root.AddChild(autoloadContainer);

        // Move all autoloads to the autoload container
        var branches = GetTree().Root.GetChildren();
        foreach (var branch in branches)
        {
            if (branch.Name == nameof(Main)) continue;
            if (branch == autoloadContainer) continue;
            if (branch == this) continue;

            root.RemoveChild(branch);
            autoloadContainer.AddChild(branch);
        }

        var controlContainer = new Control
        {
            Name = ControlContainerName
        };

        var worldContainer = new Node2D
        {
            Name = WorldContainerName
        };

        AddChild(controlContainer);
        AddChild(worldContainer);

        ScenePaths.Initialize();

        var startScreen = ScenePaths.GetScene<StartScreen>();
        AddChild(startScreen);
        startScreen.Position = this.ViewSize() / 2;
    }

    #region Public API

    public static SceneTree Tree => Singleton.Instance.GetTree();

    public static string AutoloadContainerName => "AutoloadContainer";
    public static Node GetAutoloadContainer() => Singleton.Instance._autoLoadContainer;
    private Node _autoLoadContainer => GetTree().Root.GetNode<Node>(AutoloadContainerName);

    public static string ControlContainerName => "ControlContainer";
    public static Control GetControlContainer() => Singleton.Instance._controlContainer;
    private Control _controlContainer => GetNode<Control>(ControlContainerName);

    public static string WorldContainerName => "WorldContainer";
    public static Node2D GetWorldContainer() => Singleton.Instance._worldContainer;
    private Node2D _worldContainer => GetNode<Node2D>(WorldContainerName);

    #endregion
}
