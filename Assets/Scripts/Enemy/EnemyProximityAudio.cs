using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyProximityAudio : MonoBehaviour
{
    [Header("Enemy Sound")]
    [SerializeField] private AudioClip enemySound;

    [Header("Distance")]
    [SerializeField] private float maxDistance = 12f;
    [SerializeField] private float fullVolumeDistance = 2f;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)]
    private float maxVolume = 0.5f;

    [Header("Sound Interval")]
    [SerializeField] private float repeatDelay = 5f;

    private AudioSource audioSource;
    private Transform player;

    private float timer = 0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning(
                "EnemyProximityAudio: Player with tag 'Player' is not found!"
            );
        }
    }

    private void Update()
    {
        if (player == null || enemySound == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance > maxDistance)
        {
            timer = 0f;
            return;
        }

        float volume;

        if (distance <= fullVolumeDistance)
        {
            volume = maxVolume;
        }
        else
        {
            float normalizedDistance = Mathf.InverseLerp(
                maxDistance,
                fullVolumeDistance,
                distance
            );

            volume = normalizedDistance * maxVolume;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            audioSource.PlayOneShot(enemySound, volume);

            timer = repeatDelay;
        }
    }
}