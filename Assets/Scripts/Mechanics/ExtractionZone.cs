using UnityEngine;

public class ExtractionZone : MonoBehaviour
{
    [SerializeField] private Transform player;

    private bool playerInside;

    public bool IsPlayerInside()
    {
        return playerInside;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (
            other.attachedRigidbody != null &&
            other.attachedRigidbody.transform == player
        )
        {
            playerInside = true;

            Debug.Log("Player je u Extraction Zone.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (
            other.attachedRigidbody != null &&
            other.attachedRigidbody.transform == player
        )
        {
            playerInside = false;

            Debug.Log("Player je napustio Extraction Zone.");
        }
    }
}