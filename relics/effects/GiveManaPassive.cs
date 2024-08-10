using Godot;
using System;

[GlobalClass, Tool]
public partial class GiveManaPassive : EffectResource
{
	public GiveManaPassive()
	{

	}

	protected override void executeEffect(Node node)
	{
		FindObjectHelper.getMana(node).modifyMana(value);
	}
}
