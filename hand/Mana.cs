using Godot;
using System;

public partial class Mana : Node2D
{
	[Export] 
	public RichTextLabel costText;


	public int manaValue {get; protected set;} = 0;
	int manaMax = 99;
	int manaRestValue = 3;



	public void modifyMana(int value){
		int newManaValue  = manaValue + value;
		setManaValue(Math.Clamp(newManaValue, 0, manaMax));
	}

	public void resetManaValue() {
		setManaValue(manaRestValue);
	}

	private void setManaValue(int value) {
		manaValue = value;
		costText.Text = TextHelper.centered(manaValue.ToString());
	}

	public override void _Ready()
	{ 
		resetManaValue();
	}

}