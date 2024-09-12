using Godot;
using System;

[GlobalClass, Tool]
public partial class SpawnBurntEffect : EffectResource
{	

	public SpawnBurntEffect() {
		 
	}
	
	protected override void executeEffect(Node node) {
		FindObjectHelper.getMatchBoard(node).spawnBlacks = true;
		FindObjectHelper.getMatchBoard(node).blacksMatchable = true;

	}
}
