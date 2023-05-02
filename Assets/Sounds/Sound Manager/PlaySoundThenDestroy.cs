using UnityEngine;

public class PlaySoundThenDestroy : MonoBehaviour
{
    public string SoundToPlay;

    void Start()
    {
        AudioSource source = AudioManager.AM.INeedSoundSource(SoundToPlay, gameObject);

        if (source != null)
        {
            source.Play();
            Invoke(nameof(DestroyAfterSeconds), source.clip.length);

        }
    }

    private void DestroyAfterSeconds()
    {
        //print("sound finished playing");
        Destroy(gameObject);
    }

}
