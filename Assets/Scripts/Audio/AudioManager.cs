using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource zoneMusicSource;

    [Header("Audio Clip")]
    public AudioClip backgroundMusic;
    public AudioClip zoneMusicClip;
    public AudioClip death;
    public AudioClip jump;
    public AudioClip dash;
    public AudioClip gemPickup;
    public AudioClip arrowDashPickup;
    public AudioClip jumpPadBounce;
    public AudioClip doorUnlock;
    public AudioClip enemyDeath;
    public AudioClip bossDeath;


    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();

        // Preload zone music to avoid hiccups
        zoneMusicSource.clip = zoneMusicClip;
        zoneMusicSource.Play();
        zoneMusicSource.Pause();  // Ngừng phát ngay lập tức để tránh phát âm thanh lúc đầu
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void EnterMusicZone()
    {
        musicSource.Stop();
        zoneMusicSource.clip = zoneMusicClip;
        zoneMusicSource.Play();
    }

    public void ExitMusicZone()
    {
        if (zoneMusicSource != null)
        {
            zoneMusicSource.Stop();
        }

        if (musicSource != null)
        {
            musicSource.Play();
        }
    }
}