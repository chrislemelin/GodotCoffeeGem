using Godot;
using System;

public partial class CustomToolTipButton : Button
{
	public override GodotObject _MakeCustomTooltip(string forText)
	{
	

		PackedScene scene = GD.Load<PackedScene>("res://UI/tooltip/ToolTip.tscn"); 
		ToolTip toolTip =  (ToolTip)scene.Instantiate(); 
		toolTip.label.Text = forText;
		return toolTip;
	}
}
