using Godot;
using System;

public partial class RelicUI : CustomToolTip
{
	[Export] public RelicResource relicResource;
	[Export] public TextureRect picture;
	[Export] RichTextLabel titleLabel;
	[Export] RichTextLabel counterLabel;
	[Export] RichTextLabel costValueLabel;
	[Export] public bool showPrice = false;
	[Export] public bool showTitle = false;

	[Export] Vector2 minSizeWithCost;
	[Export] private Control costControl;
	[Export] public Button buyButton;

	public void setRelic(RelicResource relicResource)
	{
		this.relicResource = relicResource;
		relicResource.CounterChanged += renderCounter;
		render();
	}

	public void setShowPrice(bool value) {
		showPrice = value;
		render();
	}

	private void render() {
		titleLabel.Text = TextHelper.centered(relicResource.title);
		picture.Texture = relicResource.image;
		picture.TooltipText = relicResource.description;
		renderCost();
		renderCounter(0);
	}

	private void renderTitle() {
		titleLabel.Text = TextHelper.centered(relicResource.title);
		titleLabel.Visible = showTitle;
	}

	private void renderCost()
	{
		costValueLabel.Text = relicResource.cost.ToString();
		costControl.Visible = showPrice;
		if (showPrice)
		{
			CustomMinimumSize = minSizeWithCost;
		}
	}

	private void renderCounter(int count)
	{
		if (relicResource.counterMax == 0)
		{
			counterLabel.Visible = false;
		}
		else
		{
			counterLabel.Text = relicResource.counter.ToString();
		}
	}

	public override void _Ready()
	{
		base._Ready();
	}

	protected override void Dispose(bool disposing)
	{
		if (relicResource != null)
		{
			relicResource.CounterChanged -= renderCounter;
		}
		base.Dispose(disposing);
	}
}
