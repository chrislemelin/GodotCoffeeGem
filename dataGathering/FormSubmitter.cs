using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

public partial class FormSubmitter : HttpRequest
{
	String urlform = "https://docs.google.com/forms/d/e/1FAIpQLSfIQ7RMk7pz1Qs4DmNkajjnQ4KdCPYQRQguVK-YK6HI0EVsNQ/formResponse";

	string[] headers = { "Content-Type: application/x-www-form-urlencoded" };

	private Action callBack;

	public override void _EnterTree()
	{
		base._EnterTree();
		RequestCompleted += (result, responseCode, headers, body) => executeCallBack();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
	}

	public void submitData(String reasonQuiting, GameManagerIF gameManager)
	{
		this.submitData(reasonQuiting, gameManager, null);
	}

	public void submitData(String reasonQuiting, GameManagerIF gameManager, Action callBack)
	{
		if (OS.HasFeature("standalone") && gameManager.getCollectData() && gameManager.isIntialized())
		{
			GD.Print("data collection allowed " + gameManager.getCollectData() + "userId " + gameManager.getUserId());

			String data = "";
			Score score = FindObjectHelper.getScore(this);
			if (score != null && score.getScore() == 0 && gameManager.getLevel() == 1)
			{
				if (callBack != null)
				{
					callBack.Invoke();
				}
				// they have just started to run, don't collect data
				return;
			}
			data = appendData(data, "entry.639787288", gameManager.getUserId() + "");

			List<String> deckList = gameManager.getDeckList().Select(card => card.Title).ToList();
			data = appendData(data, "entry.739229763", String.Join(",", deckList));

			data = appendData(data, "entry.151133640", reasonQuiting);

			TimeSpan ts = TimeSpan.FromMilliseconds((Time.GetTicksMsec() - gameManager.getStartTime()));
			data = appendData(data, "entry.1814658543", ts.ToString(@"hh\:mm\:ss"));


			List<String> relicList = gameManager.getRelics().Select(relic => relic.title).ToList();
			data = appendData(data, "entry.1639561478", String.Join(",", relicList));

			data = appendData(data, "entry.1849250167", gameManager.getLevel().ToString());

			if (score != null)
			{
				data = appendData(data, "entry.1921693847", score.getScore().ToString());
				data = appendData(data, "entry.1041408558", score.getScoreNeeded().ToString());
			}

			data = appendData(data, "entry.1410312729", gameManager.getCoins().ToString());
			data = appendData(data, "entry.1443935098", gameManager.getAllCoinsGained().ToString());

			data = appendData(data, "entry.537817310", gameManager.getGridSize().ToString());

			DeckSelectionResource deckSelection = gameManager.getDeckSelection();
			if (deckSelection != null)
			{
				data = appendData(data, "entry.1435208426", deckSelection.title);
			}
			data = appendData(data, "entry.687983885", Version.version);
			this.callBack = callBack;

			GetTree().CreateTimer(.1f).Timeout += () => Request(urlform, headers, HttpClient.Method.Post, data);

			//Thread myThread = new Thread(new ThreadStart(() => Request(urlform, headers, HttpClient.Method.Post, data)));
			//myThread.Start();
			//var response = Request(urlform, headers, HttpClient.Method.Post, data);
			//GetTree().CreateTimer(5).Timeout += () => executeCallBack();
			
		}
		if (callBack != null)
		{
			callBack.Invoke();
		}
		
	}

	private void executeCallBack()
	{
		if (callBack != null)
		{
			callBack.Invoke();
		}
		callBack = null;
	}

	private String appendData(String data, String entryKey, String entryValue, bool last = false)
	{
		entryValue = HttpUtility.UrlEncode(entryValue);
		data += entryKey + "=" + entryValue;
		if (!last)
		{
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
	entry.1041408558: score required
	entry.151133640: how did the run end
	entry.1443935098: totalcoins
	entry.1410312729: coins
	entry.1814658543: run duration
	entry.687983885: version
	entry.537817310: board size
	*/

}
