using UnityEngine;

public class PickUp : MonoBehaviour
{
    public string playerTag = "Player"; // Tag of the player object

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag)
        {
            // Disable collisions between player and this object
            // This will prevent the player from bumping into the object after it's picked up
            this.GetComponent<Collider>().enabled = false;

            // Make the object inactive (disappear)
            this.gameObject.SetActive(false);
        }
    }
}
