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
	[Export] private Control toolTipLeftSide;
	[Export] private Control toolTipRightSide;
	[Export] private VisibleOnScreenNotifier2D rightSideVisibleCheck;



	public override void _Ready()
	{
		base._Ready();
		Control focus = target;
		if (toolTipText != null) {
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
		} else {
			GD.Print("no tooltip at " + GetPath());
		}

	}

	public override void _Process(double delta) {
		
		if(toolTipOn && followBase) {
			toolTipText.GlobalPosition = GlobalPosition - toolTipOffset;
		}
	
	}

	private void checkForLeftOrRight() {
		if (rightSideVisibleCheck == null) {
			return;
		}
		toolTipText.GetParent().RemoveChild(toolTipText);
		if (rightSideVisibleCheck.IsOnScreen()) {
			toolTipRightSide.AddChild(toolTipText);
		} else{
			toolTipLeftSide.AddChild(toolTipText);
		}
		toolTipText.Position = new Vector2(0,0);
	}

	private void makeToolTip() {
		checkForLeftOrRight();
		if (toolTipText.Text.Length > 0 && !GetTree().Root.GetChildren().Contains(toolTipText)) {
			toolTipOn = true;
			toolTipOffset = GlobalPosition + (PivotOffset * (Scale - Vector2.One)) - toolTipText.GlobalPosition;
			
			Vector2 position = toolTipText.GlobalPosition;
			toolTipText.GetParent().RemoveChild(toolTipText);
			GetTree().Root.AddChild(toolTipText);
			GetTree().Root.MoveChild(toolTipText, GetTree().Root.GetChildCount()-1);
			toolTipText.GlobalPosition = position;
			toolTipText.Visible = true;
		}
	}

	private void deleteToolTip() {
		if (toolTipText.Text.Length > 0 && GetTree().Root.GetChildren().Contains(toolTipText)) {
			toolTipText.Visible = false;
			toolTipOn = false;
			
			Vector2 position = toolTipText.GlobalPosition;
			GetTree().Root.RemoveChild(toolTipText);
			AddChild(toolTipText);
			toolTipText.GlobalPosition = position;
		}
	}

}
