using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[GlobalClass, Tool]
public partial class PopRelic : RelicResource
{
	public override void ingredientDestroyed(Gem gem)
	{
		if (counter != counterMax){
			incrementCounter();
		}
	}


}
