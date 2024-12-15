using Godot;
using System;

public partial class CustomToolTip : Control
{
	[Export] protected RichTextLabel toolTipText;
	[Export] protected Control toolTipControl;
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
		if (getToolTip() != null) {
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
		base._Process(delta);
		if(toolTipOn && followBase) {
			getToolTip().GlobalPosition = GlobalPosition - toolTipOffset;
		}
	}

	private Control getToolTip() {
		if (toolTipControl!= null) {
			return toolTipControl;
		}
		if (toolTipText != null) {
			return toolTipText;
		}
		return null;
	}

	private void checkForLeftOrRight() {
		if (rightSideVisibleCheck == null) {
			return;
		}
		Control toolTip = getToolTip();
		toolTip.GetParent().RemoveChild(toolTip);
		if (rightSideVisibleCheck.IsOnScreen()) {
			toolTipRightSide.AddChild(toolTip);
		} else{
			toolTipLeftSide.AddChild(toolTip);
		}
		toolTip.Position = new Vector2(0,0);
	}

	private void makeToolTip() {
		checkForLeftOrRight();
		Control toolTip = getToolTip();
		if (checkForText() && !GetTree().Root.GetChildren().Contains(toolTip)) {
			toolTipOn = true;
			toolTipOffset = GlobalPosition + (PivotOffset * (Scale - Vector2.One)) - toolTip.GlobalPosition;
			
			Vector2 position = toolTip.GlobalPosition;
			toolTip.GetParent().RemoveChild(toolTip);
			GetTree().Root.AddChild(toolTip);
			GetTree().Root.MoveChild(toolTip, GetTree().Root.GetChildCount()-1);
			toolTip.GlobalPosition = position;
			toolTip.Visible = true;
		}
	}

	private bool checkForText() {
		if(getToolTip() is RichTextLabel label){
			return label.Text.Length > 0;
		}
		return true;
	}

	public void deleteToolTip() {
		Control toolTip = getToolTip();
		if (checkForText() && GetTree().Root.GetChildren().Contains(toolTip)) {
			toolTip.Visible = false;
			toolTipOn = false;
			
			Vector2 position = toolTip.GlobalPosition;
			GetTree().Root.RemoveChild(toolTip);
			AddChild(toolTip);
			toolTip.GlobalPosition = position;
		}
	}

}
