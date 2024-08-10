using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class AddColorUpgradeEffect : EffectResource
{	

	[Export] ColorUpgrade colorUpgrade;
	public AddColorUpgradeEffect() {
		 
	}
	
	protected override void executeEffect(Node node) {
		FindObjectHelper.getScore(node).addColorUpgrade(colorUpgrade);
	}
}
