using UnityEngine;

public class VFXCleanup : MonoBehaviour
{
    // The time (in seconds) the script waits before destroying the entire GameObject.
    public float delay = 2.0f;

    void Start()
    {
        // This is the command that destroys the GameObject the script is attached to 
        // after the time specified in 'delay' has passed.
        Destroy(gameObject, delay);
    }
}