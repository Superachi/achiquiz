using AchiQuiz.Helpers;
using Godot;

public partial class StartScreen : Node2D
{
    private Button _buttonControl;
    private Button _buttonPresentation;
    private Label _label;
    private Label _label2;
    private Label _label3;

    public override void _Ready()
    {
        _buttonControl = GetNode<Button>("ButtonControl");
        _buttonPresentation = GetNode<Button>("ButtonPresentation");
        _label = GetNode<Label>("Label");
        _label2 = GetNode<Label>("Label2");
        _label3 = GetNode<Label>("Label3");

        _buttonControl.Pressed += () =>
        {
            AchiLogger.Log("Control Button Pressed");
        };

        _buttonPresentation.Pressed += () =>
        {
            AchiLogger.Log("Presentation Button Pressed");
        };
    }

    public override void _Process(double delta)
	{
	}
}
