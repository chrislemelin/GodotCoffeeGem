using Godot;
using System;

public partial class AudioPlayer : AudioStreamPlayer2D
{
	[Export] bool soundEffect;
	[Export] float minValue = -20.0f;
	[Export] float maxValue = 40.0f;
	public float baseVolumeMult = 1.0f;

	public override void _Ready()
	{
		if (!(soundEffect)) {
			GD.Print("music ready");
		}
		base._Ready();
		setUp();
	}

	public void setUp () {
		SettingsMenu settingsMenu = FindObjectHelper.getSettingsMenu(this);
		if (settingsMenu == null &&!(soundEffect)) {
			GD.Print("cannot find settings menu");
		}
		 if (settingsMenu != null) {
			if (soundEffect) {
				settingsMenu.sfxSlider.ValueChanged += (double value) => {
					recalculateVolume();
				};
			} else {
				GD.Print("setting up music control");
				if (settingsMenu.musicSliderSetUp == false) {
					settingsMenu.musicSlider.ValueChanged += (double value) => {
						recalculateVolume();
					};
					settingsMenu.musicSliderSetUp = true;
				}
			}
		recalculateVolume();
		}
	}

	private void recalculateVolume() {
		SettingsMenu settingsMenu = FindObjectHelper.getSettingsMenu(this);
		GameManagerIF gameManager = FindObjectHelper.getGameManager(this);
		if (settingsMenu != null) {
			float settingsVolume;
			if (soundEffect) {
				settingsVolume = (float)(minValue + (maxValue - minValue) * (settingsMenu.sfxSlider.Value * baseVolumeMult));
				if (gameManager.isSFXMuted()){
					settingsVolume = float.MinValue;
				}
			} else {
				settingsVolume = (float)(minValue + (maxValue - minValue) * settingsMenu.musicSlider.Value * baseVolumeMult);
				if (gameManager.isMusicMuted()){
					settingsVolume = float.MinValue;
				}
			}
			VolumeDb = settingsVolume;
		}
	}

	public void setBaseVolumeMult(float value) {
		baseVolumeMult = value;
		recalculateVolume();
	}

}
