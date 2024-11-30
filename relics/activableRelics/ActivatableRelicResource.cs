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


	public void setUp(Node node) {
		recipe.setUp(node);
		recipe.finishedRecipe += addCharge;
	}

	public bool canExecute() {
		return charges >= chargesNeededToActivate;
	}

	public void addCharge() {
		charges++;
		if(!canHoldMultipleCharges) {
			charges = 1;
		}
		EmitSignal(SignalName.ChargesChanged);
	}

	public int getCharges() {
		return charges;
	}

	public void doEffects(Node node) {
		if (canExecute()) {
			foreach(EffectResource effect in effects) {
				effect.execute(node);
			}
			charges -= chargesNeededToActivate;
			EmitSignal(SignalName.ChargesChanged);
		}
	}

	public string getDescription() {
		String returnString = TextHelper.centered(description);
		if (chargesNeededToActivate > 1) {
			returnString += "\n"+TextHelper.centered("Requires "+chargesNeededToActivate + " charges to activate.");
		}
		return returnString;
	}
}

public enum SelectionMode {
	None,
	SingleTile,
	SingleFutureTile
}
