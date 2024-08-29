using Godot;
using System;

public partial class EndDayButton : CustomButton
{
	public override void _Ready()
	{
		Visible = false;
		//FindObjectHelper.getScore(this).levelPassed += () => {
			//if(!((GameManager)FindObjectHelper.getGameManager(this)).gameBeaten()) {
		//		Visible = true;
			//}
		//};
		//Pressed += () => FindObjectHelper.getScore(this).evaluateLevel();
	}


}
