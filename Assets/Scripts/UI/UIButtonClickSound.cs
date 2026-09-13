using UnityEngine;
using UnityEngine.UI;

public class UIButtonClickSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private Button[] buttons;


    private void Awake()
    {
        buttons =
            GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(
                PlayClickSound
            );
        }
    }


    private void PlayClickSound()
    {
        if (
            audioSource == null ||
            audioSource.clip == null
        )
        {
            return;
        }

        audioSource.PlayOneShot(
            audioSource.clip
        );
    }


    private void OnDestroy()
    {
        if (buttons == null)
        {
            return;
        }

        foreach (Button button in buttons)
        {
            if (button != null)
            {
                button.onClick.RemoveListener(
                    PlayClickSound
                );
            }
        }
    }
}