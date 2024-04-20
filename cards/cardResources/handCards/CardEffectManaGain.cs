using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardEffectManaGain : CardEffectIF
{
	public CardEffectManaGain() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		int valueMod = 0;
		if (matchBoard.getMatchesThisTurn(GemType.Coffee).Count >= 1){} {
			valueMod = 1;
		}
		mana.modifyMana(getValue() + valueMod);
	}
	public override String getCustomText() {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		if (matchBoard == null)
		{
			return "";
		}
		if (matchBoard.getMatchesThisTurn(GemType.Coffee).Count >= 1)
			return "[color=#2c8518]✓[/color]";
		return "";
	}
	public override void init()
	{
		FindObjectHelper.getMatchBoard(node).ingredientMatched += (match) => EmitSignal(SignalName.CustomTextChanged);
	}
}
