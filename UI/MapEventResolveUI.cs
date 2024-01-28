using Godot;
using System;

public partial class MapEventResolveUI : ToggleVisibilityOnButtonPress
{
	[Export] RichTextLabel title;
	[Export] RichTextLabel description;


	public void setUp(string title, string description) {
		this.title.Text = title;
		this.description.Text = description;
		Visible = true;
	}

}
