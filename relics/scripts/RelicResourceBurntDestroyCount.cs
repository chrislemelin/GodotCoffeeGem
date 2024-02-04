using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class RelicResourceBurntDestroyCount : RelicResource
{
	public override void ingredientDestroyed(Gem gem)
	{
		base.ingredientDestroyed(gem);
		if (gem.Type == GemType.Black) {
			incrementCounter();
		}
	}

}
