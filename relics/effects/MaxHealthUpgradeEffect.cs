using Godot;
using System;

[GlobalClass, Tool]
public partial class MaxHealthUpgradeEffect : EffectResource
{

	public MaxHealthUpgradeEffect()
	{

	}

	protected override void executeEffect(Node node)
	{
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(node);
		gameManagerIF.setMaxHealth(gameManagerIF.getMaxHealth() + value);
		gameManagerIF.setHealth(gameManagerIF.getHealth() + value);
	}
}
