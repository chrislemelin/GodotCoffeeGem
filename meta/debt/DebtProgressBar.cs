using Godot;
using Godot.Collections;
using System;

public partial class DebtProgressBar : Node
{
	[Export] TextureProgressBar progressBar;
	[Export] RichTextLabel progressText;
	[Export] RichTextLabel startText;
	[Export] RichTextLabel endText;
	[Export] TextureRect reward;
	[Export] AudioPlayer audioStreamPlayerBlip;
	[Export] AudioPlayer audioStreamPlayerReward;
	[Export] public ToggleVisibilityOnButtonPress debtReward;
	[Export] public Array<int> segmentValues = new();
	[Export] int segmentValue = 50;
	[Export] bool segmented = false;
	[Export] Resource firstLevelUpDialouge;

	private int maxDebt;
	[Export] public int currentDebtPaid;
	public int currentDebtPaidTarget = 0;
	public int currentDebtPaidStep = 10;


	public override void _Ready()
	{
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
		maxDebt = gameManagerIF.getDebtMax();
		currentDebtPaid = maxDebt - gameManagerIF.getDebt();
		updateDebt();

		Timer timer = new();
		AddChild(timer);
		timer.WaitTime = .1f;
		timer.OneShot = false;
		timer.Timeout += updateProgress;
		timer.Start();

		//addDebtProgress(1500);

	}
	public override void _Process(double delta)
	{
		//updateProgress();
		//updateDebt();
	}

	private void updateProgress() {
		if (currentDebtPaidTarget != 0) {
			int currentSegments = getSegmentValue(currentDebtPaid);
			//GD.Print(currentSegments);
			currentDebtPaid += currentDebtPaidStep;
			if (currentDebtPaid > currentDebtPaidTarget) {
				currentDebtPaid = currentDebtPaidTarget;
				currentDebtPaidTarget = 0;
			}
			int segmentsAfterUpdate = getSegmentValue(currentDebtPaid);
			if (currentSegments != segmentsAfterUpdate) {
				audioStreamPlayerReward.Play();
				if (debtReward != null) {
					debtReward.setVisible(true);
					if (currentSegments == 0) {
						DialogueManagerRuntime.DialogueManager.ShowDialogueBalloon(firstLevelUpDialouge);
					}
				}
			}
			updateDebt();
			audioStreamPlayerBlip.Play();
			currentDebtPaidStep ++;
		}
	}

	// private int getSegements(int value) {
	// 	return value/segmentValue;
	// }

	private int getSegmentValue(int value) {
		if (value >= segmentValues[segmentValues.Count-1]) {
			return segmentValues.Count +  (value-segmentValues[segmentValues.Count-1])/segmentValue;
		} else {
			for  (int count = 0; count < segmentValues.Count; count++) {
				if (value < segmentValues[count]) {
					return count;
				}
			}
			return 0;
		}
	}
	private int getSegmentMax(int value) {
		if (value < 0) {
			return 0;
		}
		if (value < segmentValues.Count) {
			return segmentValues[value];
		} else {
			return segmentValues[segmentValues.Count-1] + segmentValue * (value + 1 - segmentValues.Count);
		}
	}
	// private int getSegmentMin(int value) {
	// 	return 0;
	// }

	public int segmentsPassed(int startingDebtProgress, int debtProgress) {
		int startingSegements = getSegmentValue(startingDebtProgress);
		int endingSegements = getSegmentValue(startingDebtProgress + debtProgress);
		return endingSegements - startingSegements;
	}


	public void addDebtProgress(int value) {
		//Tween tween = GetTree().CreateTween();
		currentDebtPaidTarget = currentDebtPaid+value;
		//tween.TweenProperty(this, "currentDebtPaid", currentDebtPaid+value, 1.0f);
	}

	private void updateDebt() {
		if (segmented) {
			int currentDebtPaidSegmented = currentDebtPaid;
			int segmentsPast = getSegmentValue(currentDebtPaidSegmented);
			
			int startValue = getSegmentMax(segmentsPast-1);
			int endValue =  getSegmentMax(segmentsPast);
			int difference = endValue - startValue;
			startText.Text = startValue.ToString();
			endText.Text = "[right]"+endValue;
			reward.Visible = true;

			progressBar.Value = (currentDebtPaid - startValue)/(double)difference * 100.0f;
			progressText.Text = TextHelper.centered(currentDebtPaid.ToString());			
		} else {
			reward.Visible = false;
			startText.Visible = false;
			endText.Text = maxDebt.ToString();
			progressBar.Value = (double)currentDebtPaid/maxDebt * 100.0f;
			progressText.Text = TextHelper.centered("$"+currentDebtPaid+"/$"+maxDebt);
		}
	}

	
}
