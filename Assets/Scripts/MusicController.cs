using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public AudioSource musicSource;
    public GameObject musicOnButton;
    public GameObject musicOffButton;

    private bool isMusicOn = true;

    void Start()
    {
        isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;

        musicSource.mute = !isMusicOn;
        UpdateButtonStates();
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        musicSource.mute = !isMusicOn;

        PlayerPrefs.SetInt("MusicOn", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateButtonStates();
    }

    void UpdateButtonStates()
    {
        musicOnButton.SetActive(isMusicOn);
        musicOffButton.SetActive(!isMusicOn);
    }
}