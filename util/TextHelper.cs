using Godot;
using System;
using System.Text.RegularExpressions;

public static class TextHelper
{
	public static string centered(string text)
	{
		return "[center]" + text + "[/center]";
	}
	public static string bold(string text)
	{
		return "[b]" + text + "[/b]";
	}

	public static string right(string text)
	{
		return "[right]" + text + "[/right]";
	}

	public static string toolTip(string text, string toolTip)
	{
		return "[hint={" + toolTip + "}]" + text + "[/hint]";
	}
	public static string spliceText(string text, int lineLength) {
		return Regex.Replace(text, "(.{" + lineLength + "})", "$1" + System.Environment.NewLine);
	}
	public static string colorText(string text, string color) {
		return "[color=" + color + "]" + text + "[/color]";
	}
	public static string shake(string text, int level, int shakeRate) {
		return "[shake rate="+shakeRate+" level="+level+" connected=1]"+text+"[/shake]";
	}

	public static string pulse(string text) {
		return 	"[pulse freq=1.0 color=#ffffff40 ease=-2.0]"+text+"[/pulse]";
	}

	public static string colorText(string text, Color color) {
		return "[color=" + color.ToHtml() + "]" + text + "[/color]";
	}
	public static string justify(string text) {
		return "[fill]" + text + "[/fill]";
	}

	public static string getIngredientImage(GemType gemType) {
		if (gemType.Equals(GemType.Coffee)) {
			return imgStarter()+"res://sprites/cards/icons/bean-icon.png[/img]";
		}
		if (gemType.Equals(GemType.Leaf)) {
			return imgStarter()+"res://sprites/cards/icons/leaf-icon.png[/img]";
		}
		if (gemType.Equals(GemType.Sugar)) {
			return imgStarter()+"res://sprites/cards/icons/sugar-icon.png[/img]";
		}
		if (gemType.Equals(GemType.Milk)) {
			return imgStarter()+"res://sprites/cards/icons/milk-icon.png[/img]";
		}
		if (gemType.Equals(GemType.Vanilla)) {
			return imgStarter()+"res://sprites/cards/icons/vanilla-icon.png[/img]";
		}
		if (gemType.Equals(GemType.Black)) {
			return imgStarter()+"res://sprites/cards/icons/burn-icon.png[/img]";
		}
		if (gemType.Equals(GemType.Lead)) {
			return imgStarter()+"res://sprites/ingredients/final/Lead-placeholder.png[/img]";
		}
		return "";
	}

	public static string imgStarter(){
		return "[img with=30 height=30]";
	}

	public static string getEnergyImage() {
		return imgStarter()+"res://sprites/cards/icons/energy-icon.png[/img]";
	}

	public static string getCardImage() {
		return imgStarter()+"res://sprites/cards/icons/card-icon.png[/img]";
	}
	public static string getCoinImage() {
		return imgStarter()+"res://sprites/cards/icons/coin-icon.png[/img]";
	}


	public static string getEnergyIconString() {
		return "$energy";
	}

	public static string getCardIconString() {
		return "$draw";
	}

	public static string getCoinIconString() {
		return "$coin";
	}

	public static string getIngredientIconString(GemType gemType) {
		return "$"+gemType.getString();
	}

}
