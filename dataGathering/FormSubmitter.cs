using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class FormSubmitter : HttpRequest
{
	String urlform = "https://docs.google.com/forms/d/e/1FAIpQLSfIQ7RMk7pz1Qs4DmNkajjnQ4KdCPYQRQguVK-YK6HI0EVsNQ/formResponse";

	string[] headers = {"Content-Type: application/x-www-form-urlencoded"};

	private Action callBack;

	public override void _EnterTree()
	{
		base._EnterTree();
		RequestCompleted += executeCallBack;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
	}

	public void submitData(String reasonQuiting, GameManagerIF gameManager){
		this.submitData(reasonQuiting, gameManager, null);
	}

	public void submitData(String reasonQuiting, GameManagerIF gameManager, Action callBack)
	{	
		GD.Print("data collection allowed "+ gameManager.getCollectData());
		if (OS.HasFeature("standalone") && gameManager.getCollectData() && gameManager.isIntialized())
		{

			String data = "";
			List<String> deckList = gameManager.getDeckList().Select(card => card.Title).ToList();
			data = appendData(data, "entry.739229763", String.Join(",", deckList));

			data = appendData(data, "entry.151133640", reasonQuiting);


			List<String> relicList = gameManager.getRelics().Select(relic => relic.title).ToList();
			data = appendData(data, "entry.1639561478", String.Join(",", relicList));

			data = appendData(data, "entry.1849250167", gameManager.getLevel().ToString());

			Score score = FindObjectHelper.getScore(this);
			if (score != null) {
				data = appendData(data, "entry.1921693847", score.getScore().ToString());
			}

			data = appendData(data, "entry.1410312729", gameManager.getCoins().ToString());
			data = appendData(data, "entry.1443935098", gameManager.getAllCoinsGained().ToString());

			DeckSelectionResource deckSelection =  gameManager.getDeckSelection();
			if (deckSelection != null){
				data = appendData(data, "entry.1435208426", deckSelection.title);
			}

			this.callBack = callBack;
			var response = Request(urlform, headers, HttpClient.Method.Post, data);
		} else {
			if (callBack != null) {
				callBack.Invoke();
			}
		}

	}

	private void executeCallBack(long result, long responseCode, string[] headers, byte[] body) {
		if(callBack != null) {
			callBack.Invoke();
		}
		callBack = null;
	}

	private String appendData(String data, String entryKey, String entryValue, bool last = false) {
		data += entryKey + "=" + entryValue;
		if (!last) {
			data += "&";
		}
		return data;
	}
	/*
	entry.639787288: userId
	entry.739229763: Cards in deck
	entry.1435208426: starter deck
	entry.1639561478: relics
	entry.1849250167: level reached
	entry.1921693847: score reached
	entry.151133640: how did the run end
	entry.1443935098: totalcoins
	entry.1410312729: coins
	*/

}
