using Godot;
using System;

[GlobalClass, Tool]
public partial class AddNewColPassive : EffectResource
{	
	[Export] bool permanent = true;
	public AddNewColPassive() {
		 
	}
	
	protected override void executeEffect(Node node) {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		if (permanent) {
			GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(node);
			for (int a = 0; a < value; a ++) {
				gameManagerIF.addGridUpgrade();
			}
			if (matchBoard != null) {
				matchBoard.redrawBoard();
			}
		} else {
			if (matchBoard != null) {
				matchBoard.addColumns(value);
			}
		}
	
	}
}
