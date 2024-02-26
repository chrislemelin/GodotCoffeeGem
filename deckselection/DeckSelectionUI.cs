using Godot;
using System;
using System.Linq;

public partial class DeckSelectionUI : Control
{
	[Export] public DeckSelectionResource deckSelection;
	[Export] RichTextLabel title;
	[Export] TextureRect picture;
	[Export] public HighlightOnHover highlightOnHover;

	public void setDeckSelection(DeckSelectionResource deckSelection) {
		this.deckSelection = deckSelection;
		TooltipText = deckSelection.description;
		title.Text = TextHelper.centered(deckSelection.title);
		picture.Modulate = deckSelection.color;
	}

}
