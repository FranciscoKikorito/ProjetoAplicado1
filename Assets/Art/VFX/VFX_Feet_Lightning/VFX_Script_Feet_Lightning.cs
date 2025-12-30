using UnityEngine;

public class FootVFXSpawner : MonoBehaviour
{
    [Header("VFX")]
    public GameObject footVFXPrefab;

    [Header("Feet Transforms")]
    public Transform leftFoot;
    public Transform rightFoot;

    [Header("Feet SFX")]
    public AudioSource audioSource;
    public AudioClip footstepSoundLeft;
    public AudioClip footstepSoundRight;  
    //[Header("Optional Settings")]
    //public bool matchFootRotation = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();    
    }

    public void SpawnLeftFootVFX()
    {
        
        SpawnVFX(leftFoot, footstepSoundLeft);
    }

    public void SpawnRightFootVFX()
    {
        SpawnVFX(rightFoot, footstepSoundRight);
    }

    private void SpawnVFX(Transform foot, AudioClip footstepSound)
    {
        if (footVFXPrefab == null || foot == null)
            return;

        // Instantiate as child of the foot
        GameObject vfxInstance = Instantiate(
            footVFXPrefab,
            foot.position,
            foot.rotation, // start with foot's rotation
            foot
        );

        audioSource.PlayOneShot(footstepSound);

        // Apply a rotation offset relative to the foot
        vfxInstance.transform.localRotation *= Quaternion.Euler(0f, 90f, 0f);
    }

}
