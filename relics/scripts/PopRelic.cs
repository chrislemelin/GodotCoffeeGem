using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[GlobalClass, Tool]
public partial class PopRelic : RelicResource
{
	[Export]
	CardEffectGemType cardEffectGemType;
	public override void ingredientDestroyed(Gem gem)
	{
		if (cardEffectGemType.Equals(CardEffectGemType.None)) {
			tryIncrementCounter();
		} else {
			if (cardEffectGemType.GetGemType().Equals(gem.GetType())) {
				tryIncrementCounter();
			}
		}

	}

	private void tryIncrementCounter() {
		if (!resetCounterAfterReachingMax) {
			if (counter != counterMax){
				incrementCounter();
			}
		} else {
			incrementCounter();
		}

	}


}
