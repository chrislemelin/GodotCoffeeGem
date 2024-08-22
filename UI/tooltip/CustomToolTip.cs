using Godot;
using System;

public partial class CustomToolTip : Control
{
	[Export] protected RichTextLabel toolTipText;
	[Export] protected Control target;
	private Vector2 globalPositionToolTip;
	private Vector2 toolTipOffset;
	private bool toolTipOn = false;
	[Export] private bool followBase = true;



	public override void _Ready()
	{
		base._Ready();
		Control focus = target;
		if (focus == null) {
			focus = this;
		}
		FocusEntered += () => {
			makeToolTip();
		};
		focus.MouseEntered += () => { 
			makeToolTip();
		};

		FocusExited += () => {
			deleteToolTip();
		};
		focus.MouseExited += () => {
			deleteToolTip();
		};
	}

	public override void _Process(double delta) {
		
		if(toolTipOn && followBase) {
			toolTipText.GlobalPosition = GlobalPosition - toolTipOffset;
		}
	}

	private void makeToolTip() {
		if (toolTipText.Text.Length > 0) {
			toolTipOn = true;
			toolTipOffset = GlobalPosition + (PivotOffset * (Scale - Vector2.One)) - toolTipText.GlobalPosition;
			
			Vector2 position = toolTipText.GlobalPosition;
			RemoveChild(toolTipText);
			GetTree().Root.AddChild(toolTipText);
			GetTree().Root.MoveChild(toolTipText, GetTree().Root.GetChildCount()-1);
			toolTipText.GlobalPosition = position;
			toolTipText.Visible = true;
		}
	}

	private void deleteToolTip() {
		if (toolTipText.Text.Length > 0) {
			toolTipText.Visible = false;
			toolTipOn = false;
			
			Vector2 position = toolTipText.GlobalPosition;
			GetTree().Root.RemoveChild(toolTipText);
			AddChild(toolTipText);
			toolTipText.GlobalPosition = position;
		}
	}

}
