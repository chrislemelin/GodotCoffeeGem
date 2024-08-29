using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class SmallerBoardCardEffect : CardEffectIF
{
	[Export] private float multIncrease;

	public SmallerBoardCardEffect() {
	}

	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		FindObjectHelper.getScore(node).addMult(matchBoard.getColumnsRemoved() * multIncrease);
	}

	public override string getValueString()
	{
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		if (matchBoard == null)
		{
			return "";
		}
		return matchBoard.getColumnsRemoved() + "";
	}

	public override void init (){
		base.init();
		FindObjectHelper.getMatchBoard(node).boardSizeChanged += () => EmitSignal(SignalName.CustomTextChanged);

	}
}
