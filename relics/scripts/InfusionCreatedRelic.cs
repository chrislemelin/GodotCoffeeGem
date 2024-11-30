using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class InfusionCreatedRelic : RelicResource
{
	[Export] private GemAddonType infusionType;
	public override void setUp()
	{
		base.setUp();
		FindObjectHelper.getMatchBoard(node).addonsAddedToGems += (int addon) => {
			GD.Print("int " + addon + " " + (GemAddonType)addon);
			if ((GemAddonType)addon == infusionType) {
				executeEffectsOrIncreaseCount();
			}
		};

	}
}
