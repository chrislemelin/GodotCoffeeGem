using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class BurnMasterCardEffect : CardEffectIF
{

	public BurnMasterCardEffect()
	{
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Score score = FindObjectHelper.getScore(matchBoard);
		score.addScoreFromNode(50 * getValue(), node);
	}
	public override int getValue() {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		if (matchBoard == null) {
			return 0;
		}
		return matchBoard.blackGemsCreatedThisTurn;
	}
	public override String getValueString() {
		return getValue().ToString();
	}
	public override void init() {
		FindObjectHelper.getMatchBoard(node).blackGemsCreatedThisTurnChanged += () => EmitSignal(SignalName.ValueChanged);
	}
}
