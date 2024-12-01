using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class ActivatableRelicResource : Resource
{
	[Export] private bool canHoldMultipleCharges = true;
	[Export] private int chargesNeededToActivate = 1;

	[Export] public RecipeResource recipe;
	[Export] Array<EffectResource> effects;
	[Export] public string title;
	[Export(PropertyHint.MultilineText)] public string description;
	[Export] public Texture2D picture;

	private int charges;
	[Signal] public delegate void ChargesChangedEventHandler();

	[Signal] public delegate void ActivatedEventHandler();

	public void setUp(Node node)
	{
		recipe.setUp(node, this);
		recipe.finishedRecipe += addCharge;
	}

	public bool canExecute()
	{
		return charges >= chargesNeededToActivate;
	}

	public void addCharge()
	{
		if (atMaxCapacity())
		{
			return;
		}
		charges++;
		if (!canHoldMultipleCharges)
		{
			charges = 1;
		}
		EmitSignal(SignalName.ChargesChanged);
	}

	public int getCharges()
	{
		return charges;
	}

	public void doEffects(Node node)
	{
		if (canExecute())
		{
			foreach (EffectResource effect in effects)
			{
				effect.execute(node);
			}
			charges -= chargesNeededToActivate;
			EmitSignal(SignalName.ChargesChanged);
			EmitSignal(SignalName.Activated);
		}
	}

	public string getDescription()
	{
		String returnString = TextHelper.centered(description);
		if (chargesNeededToActivate > 1)
		{
			returnString += "\n" + TextHelper.centered("Requires " + chargesNeededToActivate + " charges to activate.");
		}
		if (canHoldMultipleCharges)
		{
			returnString += "\n" + TextHelper.centered("Can hold multiple charges.");
		}
		return returnString;
	}

	public bool atMaxCapacity()
	{
		if (canHoldMultipleCharges)
		{
			return false;
		}
		if (charges >= chargesNeededToActivate)
		{
			return true;
		}
		return false;
	}
}

public enum SelectionMode
{
	None,
	SingleTile,
	SingleFutureTile
}
