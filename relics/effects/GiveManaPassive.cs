using Godot;
using System;

[GlobalClass, Tool]
public partial class GiveManaPassive : EffectResource
{
	public GiveManaPassive()
	{

	}

	public override void execute(Node node)
	{
		FindObjectHelper.getMana(node).modifyMana(value);
	}
}
