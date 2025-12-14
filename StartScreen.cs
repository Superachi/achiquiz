using Godot;

public partial class StartScreen : Node2D
{
	private Button _buttonPresentation = new Button();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_buttonPresentation = GetNode<Button>("ButtonPresentation");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
