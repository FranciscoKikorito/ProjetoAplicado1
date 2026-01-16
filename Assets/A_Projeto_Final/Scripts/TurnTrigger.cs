using UnityEngine;

public class TurnTrigger : MonoBehaviour
{
    private MovePlatform parentMover;

    [Header("VFX Settings")]
    public GameObject vfxPrefab;

    [Header("SFX Settings")]
    public AudioClip collisionSFX;
    public AudioSource audioSource;

    [Header("References")]
    public SlopesAndJumping slopesAndJumping;

    private void Start()
    {
        parentMover = GameObject.Find("PathList").GetComponent<MovePlatform>();

        if (!slopesAndJumping)
            slopesAndJumping = GetComponent<SlopesAndJumping>();

        if (!audioSource)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TurnTrigger"))
        {
            Vector3 newDirection = other.transform.forward * 10f;
            parentMover.SetMoveDirection(-newDirection);

            Quaternion newRotation = Quaternion.LookRotation(newDirection, Vector3.up);
            transform.rotation = newRotation;

            if (collisionSFX && audioSource)
                audioSource.PlayOneShot(collisionSFX);

            if (vfxPrefab && slopesAndJumping && slopesAndJumping.checkGrounded())
            {
                Vector3 collisionPoint = other.ClosestPoint(transform.position);
                Quaternion vfxRotation = transform.rotation * Quaternion.Euler(0f, 90f, 0f);

                GameObject vfxInstance = Instantiate(vfxPrefab, collisionPoint, vfxRotation);
                vfxInstance.transform.parent = transform;
                Destroy(vfxInstance, 3f);
            }
        }
    }

}
