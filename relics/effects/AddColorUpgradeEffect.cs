using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class AddColorUpgradeEffect : EffectResource
{	

	[Export] ColorUpgrade colorUpgrade;
	public AddColorUpgradeEffect() {
		 
	}
	
	public override void execute(Node node) {
		FindObjectHelper.getScore(node).addColorUpgrade(colorUpgrade);
	}
}
