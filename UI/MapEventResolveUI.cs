using Godot;
using System;

public partial class MapEventResolveUI : ToggleVisibilityOnButtonPress
{
	[Export] RichTextLabel title;
	[Export] RichTextLabel description;


	public void setUp(string title, string description) {
		this.title.Text = TextHelper.centered(title);
		this.description.Text = TextHelper.justify(description);
		resetAnimation();
		setVisible(true);
	}

}
