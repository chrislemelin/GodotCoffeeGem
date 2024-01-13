using Godot;
using System;

public partial class Mult : Control
{
	[Export] RichTextLabel richTextLabel;

	public void setMult(float mult) {
		String waveText = "";
		if (mult >= 1.5) {
			waveText = "[wave amp=25.0 freq=2.0 connected=1]";
		} if (mult >= 2) {
			waveText = "[wave amp=50.0 freq=3.0 connected=1]";
		} if (mult >= 3) {
			waveText = "[wave amp=50.0 freq=6.0 connected=1]";
		}
		String rainbowText = "";
		if (mult >= 2) {
			rainbowText = "[rainbow freq=.5 sat=0.4 val=0.8]";
		}
		if (mult >= 3) {
			rainbowText = "[rainbow freq=.5 sat=0.8 val=0.8]";
		}
		richTextLabel.Text = waveText + rainbowText +"x" + mult;
	}
}
