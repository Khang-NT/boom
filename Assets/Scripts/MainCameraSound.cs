using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainCameraSound : MonoBehaviour {

	public Image toggleSoundImage;
	public Sprite soundEnabledIcon, soundDisabledIcon;

	void Start() {
		updateState ();
	}

	public void soundOnOff() {
		GamePlay.getInstance ().IsSoundEnabled = !GamePlay.getInstance ().IsSoundEnabled;
		updateState ();
	}

	void updateState() {
		AudioSource audioSource = GetComponent<AudioSource> ();
		audioSource.volume = GamePlay.getInstance ().IsSoundEnabled ? 1 : 0;

		toggleSoundImage.sprite = GamePlay.getInstance ().IsSoundEnabled ? soundEnabledIcon : soundDisabledIcon;
	}
}
