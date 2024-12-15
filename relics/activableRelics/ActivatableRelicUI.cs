using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ActivatableRelicUI : Control
{
	[Export] RichTextLabel titleLabel;
	[Export] RichTextLabel descriptionLabel;

	[Export] public TextureRect textureRect;
	[Export] Color disabledColor;
	[Export] RichTextLabel chargesLabel;
	[Export] RecipeUI recipeUI;
	[Export] ActivatableRelicResource relicResource;
	[Export] TextureProgressBar textureProgressBar;
	[Export] AnimationPlayer animationPlayer;
	[Export] public RichTextLabel costLabel;
	[Export] public Control costControl;
	[Export] bool loadFromInventory = true;
	[Export] bool buyable = false;
	[Export] CustomToolTip customToolTip;
	[Export] Control hitBox;
	[Export] bool titleBlack = true;

	private Color normalColor;


	public override void _Ready()
	{
		normalColor = textureRect.Modulate;
		GameManagerIF gameManager = FindObjectHelper.getGameManager(this);

		if (relicResource != null)
		{
			setUp(relicResource);
		}
		else if (loadFromInventory)
		{
			pullFromInventory(gameManager);
		}
		gameManager.activableRelicResourceChanged += () => {
			pullFromInventory(gameManager);
		};
	}

	private void pullFromInventory(GameManagerIF gameManager) {
		List<ActivatableRelicResource> relics = gameManager.getActivatableRelics();
		if (relics.Count > 0)
		{
			clear();
			setUp(relics[0]);
		} else{
			clear();
		}
	}

	private void clear() {
		if (relicResource != null)
		{
			relicResource.ChargesChanged -= updateCharges;
			relicResource.recipe.statusChanged -= updateProgressBar;
		}
	}

	public void showCost() {
		costControl.Visible = true;
		costLabel.Text = TextHelper.right(relicResource.cost+"");
		textureProgressBar.Visible = false;
		hitBox.GuiInput+= (inputEvent) => {
			if (InputHelper.isSelectedAction(inputEvent) && canPurchase()) {
				FindObjectHelper.getGameManager(this).replaceActivatableRelics(relicResource);
				customToolTip.deleteToolTip();
				QueueFree();
			}
		};
	}

	private bool canPurchase() {
		return FindObjectHelper.getGameManager(this).getCoins() >= relicResource.cost;
	}

	public ActivatableRelicResource getActivatableRelic()
	{
		return relicResource;
	}

	public void setUp(ActivatableRelicResource relicResource)
	{
		this.relicResource = relicResource;
		relicResource.setUp(this);

		titleLabel.Text = TextHelper.centered(relicResource.title);
		if (!titleBlack) {
			//Label.add_theme_color_override("font_color", color)
			//titleLabel.AddThemeColorOverride("default_color", new Color("#FFFFFF"));
		}
		descriptionLabel.Text = relicResource.getDescription();
		if (relicResource.picture != null)
		{
			textureRect.Texture = relicResource.picture;
		}
		relicResource.ChargesChanged += updateCharges;
		recipeUI.loadRecipe(relicResource.recipe);
		hitBox.GuiInput += (inputEvent) =>
		{
			if (inputEvent.IsActionPressed("click"))
			{
				tryDoAction();
			}
		};
		relicResource.recipe.statusChanged += updateProgressBar;
		setImageColor();
		updateProgressBar();
		updateCharges();
		if (buyable) {
			showCost();
		}

	}

	private void setImageColor() {
		if (buyable) {
			if(canPurchase()) {
				textureRect.Modulate = normalColor;
			} else {
				textureRect.Modulate = disabledColor;
			}
			return;
		}
		if (relicResource.canExecute())
		{
			textureRect.Modulate = normalColor;
		}
		else
		{
			textureRect.Modulate = disabledColor;
		}
	}

	private void updateProgressBar()
	{
		if (relicResource.atMaxCapacity())
		{
			textureProgressBar.Value = 100;
			return;
		}
		List<Tuple<GemType, bool>> list = relicResource.recipe.getCompletionStatus();
		int numberOfThingsCompleted = list.Where(item => item.Item2).Count();
		int totalNumber = list.Count();
		textureProgressBar.Value = (float)numberOfThingsCompleted / totalNumber * 100.0;
	}

	private void tryDoAction()
	{
		if (relicResource.canExecute()) {
			if (relicResource.canRunNotInLevel() || FindObjectHelper.getMatchBoard(this) != null) {
				animationPlayer.Play("activated");
				relicResource.doEffects(this);
				updateProgressBar();
			}
	
		}
		setImageColor();
	}

	private void updateCharges()
	{
		chargesLabel.Text = TextHelper.right(relicResource.getCharges() + "");
		setImageColor();
	}
	

	protected override void Dispose(bool disposing)
	{
		clear();
		base.Dispose(disposing);
	}
}

