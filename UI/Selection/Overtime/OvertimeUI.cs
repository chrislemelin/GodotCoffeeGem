using Godot;
using System;

public partial class OvertimeUI : ToggleVisibilityOnButtonPress
{
	public override void _Ready()
	{
		FindObjectHelper.getScore(this).levelPassed += () => { 
			setVisible(true);
		};
	}
}
