using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Respawn Settings")]
    [Tooltip("Drag your RespawnPoint object here.")]
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        // 1. We changed this line to look exactly for your custom "Player1" tag!
        if (other.CompareTag("Player1"))
        {
            // 2. Grab the Character Controller on the player
            CharacterController cc = other.GetComponent<CharacterController>();

            if (cc != null)
            {
                // 3. The teleport trick! 
                cc.enabled = false;
                other.transform.position = respawnPoint.position;
                cc.enabled = true;
                
                Debug.Log("Player1 fell in the pit! Respawning...");
            }
        }
    }
}