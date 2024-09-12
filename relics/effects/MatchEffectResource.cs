using Godot;
using System;
using System.Diagnostics.Contracts;

[GlobalClass, Tool]
public partial class MatchEffectResource : Resource
{
	[Export]
	protected int matchNumber;
	[Export]
	protected GemType gemType;
	[Export]
	protected EffectResource effect;
	public void execute(Node node, Match match) {
		if (match.ingredients.Count >= matchNumber && gemType.Equals(match.GetGemType())){
			effect.execute(node);
		}
		
	}
}
