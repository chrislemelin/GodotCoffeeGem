using Godot;
using System;

public partial class CoffeePot : TextureProgressBar
{
	[Export] public AudioPlayer audioPourStreamPlayer2D;
	[Export] public Node2D scoreRewards;
	[Export] public TextureProgressBar progressBar;
	[Export] private double progressStep = 2;
	[Export] private double progressIncreaseStep = .5f;

	[Export] public PackedScene sparkleEffect;
	[Export] public ReferenceRect sparkleArea;

	private double currentProgressStep;
	private bool barMoving = false;
	private bool refreshedPot = false;
	private double progressValue = 0;

	public override void _Ready() {
		progressBar.Value = 0;

		Timer timer2 = new();
		AddChild(timer2);
		timer2.WaitTime = 10.0f;
		timer2.OneShot = false;
		timer2.Timeout += makeCoffeeSparkle;
		timer2.Start();

		FindObjectHelper.getScore(this).scoreChange += 
			(int newScore, int scoreNeeded) => GetTree().CreateTimer(.25f).Timeout += 
				() => scoreChanged(newScore, scoreNeeded);


		Timer timer = new Timer();
		AddChild(timer);
		timer.WaitTime = .10;
		timer.Timeout += () => updateScoreChanged();
		timer.Start();

	}


	private void scoreChanged(int score, int scoreNeeded)
	{
		double newProgressValue = (int)((double)score / (scoreNeeded) * 100);

		if (newProgressValue > progressValue) {
			audioPourStreamPlayer2D.setBaseVolumeMult(1.0f);
			barMoving = true;
			currentProgressStep = progressStep;
		}
		progressValue = newProgressValue;
	}

	private void updateScoreChanged()
	{
		double progressValueCalculated = progressValue;
		if (refreshedPot) {
			progressValueCalculated = getCoffeeProgressValue(progressValue);
		}
		
		double currentProgress = progressBar.Value;
		if (Math.Abs(currentProgress - progressValueCalculated) > currentProgressStep)
		{
			progressBar.Value = getCoffeeProgressValue(currentProgress + currentProgressStep);
			barMoving = true;
			if (!audioPourStreamPlayer2D.Playing) {
				audioPourStreamPlayer2D.Play();
			}
			currentProgressStep += progressIncreaseStep;
		}
		else
		{
			progressBar.Value = getCoffeeProgressValue(progressValue);
			barMoving = false;
			fadeOutPourAudio();
		}
	}

	private double getCoffeeProgressValue (double value) {
		if (value > 200) {
			refreshedPot = true;
			scoreRewards.Visible = true;
			return 100;
		}
		if (value > 100) {
			refreshedPot = true;
			scoreRewards.Visible = true;
			return value - 100;
		}
		return value;
	}
	private void fadeOutPourAudio() {
		if (!barMoving) {
			audioPourStreamPlayer2D.setBaseVolumeMult(audioPourStreamPlayer2D.baseVolumeMult - .2f);
			if (audioPourStreamPlayer2D.baseVolumeMult > 0) {
				GetTree().CreateTimer(.1f).Timeout += () => fadeOutPourAudio();
			} else {
				audioPourStreamPlayer2D.Stop();
			}
		}
	}
	private void makeCoffeeSparkle() {
		if (progressBar.Value > 10) {
			Node2D shimmerNode = (Node2D)sparkleEffect.Instantiate();
			shimmerNode.GetChild<GpuParticles2D>(0,false).Emitting = true;
			sparkleArea.AddChild(shimmerNode);
			//progressBar.TextureProgress.DrawRect
			double x =  GD.RandRange(0, sparkleArea.GetRect().Size.X);
			double y =  GD.RandRange(sparkleArea.GetRect().Size.Y - sparkleArea.GetRect().Size.Y * ((progressBar.Value-10)/100), sparkleArea.GetRect().Size.Y );
			shimmerNode.Position = new Vector2((float)x,(float)y);
		}
	}
}
