using Godot;
using System;

[GlobalClass, Tool]
public partial class HandPassive : Passive
{
	[Export] public bool expireAfterTurnEnd;
	[Export] public bool expireAfterNextCardPlayed;
	[Export] public int cardsOnTurnStart;
}
