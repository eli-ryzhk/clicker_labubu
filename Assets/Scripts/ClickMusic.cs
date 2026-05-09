using UnityEngine;

public class ClickMusic: MonoBehaviour
{
    public AudioSource audioSource;

    public void PlaySound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}