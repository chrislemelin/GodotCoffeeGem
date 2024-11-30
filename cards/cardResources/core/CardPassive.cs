using Godot;
using System;

[GlobalClass, Tool]
public partial class CardPassive : Passive
{
	[Export] public bool expireAfterTurnEnd;
	[Export] public bool expireAfterNextCardPlayed;
	
	[Export] public float rangeModification;
	[Export] public int costModification;
	[Export] public int valueModification;
}
