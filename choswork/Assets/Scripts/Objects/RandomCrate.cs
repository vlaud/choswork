using UnityEngine;

public class RandomCrate : MonoBehaviour
{
    public GameObject destroyedVersion;
    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint c in collision.contacts)
        {
            if(c.otherCollider.name == "KickPoint")
            {
                Debug.Log("box kick");
                // Spawn a shattered object
                Instantiate(destroyedVersion, transform.position, transform.rotation);
                // Remove the current object
                Destroy(gameObject);
            }
        }
    }
}
