using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ActivatableRelicUI : Control
{
	[Export] RichTextLabel titleLabel;
	[Export] RichTextLabel descriptionLabel;

	[Export] TextureRect textureRect;
	[Export] Color disabledColor;
	[Export] RichTextLabel chargesLabel;
	[Export] RecipeUI recipeUI;
	[Export] ActivatableRelicResource relicResource;
	[Export] TextureProgressBar textureProgressBar;
	[Export] AnimationPlayer animationPlayer;
	private Color normalColor;


	public override void _Ready()
	{
		normalColor = textureRect.Modulate;
		if (relicResource != null)
		{
			setUp(relicResource);
		}
		else
		{
			GameManagerIF gameManager = FindObjectHelper.getGameManager(this);
			List<ActivatableRelicResource> relics = gameManager.getActivatableRelics();
			if (relics.Count > 0)
			{
				setUp(relics[0]);
			}
		}

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
		descriptionLabel.Text = relicResource.getDescription();
		if (relicResource.picture != null)
		{
			textureRect.Texture = relicResource.picture;
		}
		updateCharges();
		relicResource.ChargesChanged += updateCharges;
		recipeUI.loadRecipe(relicResource.recipe);
		GuiInput += (inputEvent) =>
		{
			if (inputEvent.IsActionPressed("click"))
			{
				tryDoAction();
			}
		};
		relicResource.recipe.statusChanged += updateProgressBar;
		setImageColor();
	}

	private void setImageColor()
	{
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
		if (relicResource.canExecute())
		{
			animationPlayer.Play("activated");
			relicResource.doEffects(this);
			updateProgressBar();
		}
		setImageColor();
	}

	private void updateCharges()
	{
		chargesLabel.Text = TextHelper.right(relicResource.getCharges() + "");
		setImageColor();
	}
}

