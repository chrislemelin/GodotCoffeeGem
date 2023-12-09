using Godot;
using System;

public partial class RelicUI : Control
{
	[Export] RelicResource relicResource;
	[Export] TextureRect picture;

	[Export] RichTextLabel counterLabel;


	public void setRelic(RelicResource relicResource) {
		this.relicResource = relicResource;
		setUp();
	}

	private void setUp() {
		picture.Texture = relicResource.image;
		TooltipText = relicResource.description;
		renderCounter(0);

		relicResource.CounterChanged += renderCounter;
	}

	private void renderCounter(int count) {
		if(relicResource.counterMax == 0) {
			counterLabel.Visible = false;
		} else {
			counterLabel.Text = relicResource.counter.ToString();
		}
	}

	public override void _Ready()
	{
		base._Ready();
	}

	protected override void Dispose(bool disposing) {
		if (relicResource != null) {
			relicResource.CounterChanged -= renderCounter;
		}
		base.Dispose(disposing);
	}
}
