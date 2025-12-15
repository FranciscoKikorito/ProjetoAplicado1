using UnityEngine;

public class FootVFXSpawner : MonoBehaviour
{
    [Header("VFX")]
    public GameObject footVFXPrefab;

    [Header("Feet Transforms")]
    public Transform leftFoot;
    public Transform rightFoot;

    //[Header("Optional Settings")]
    //public bool matchFootRotation = false;

    public void SpawnLeftFootVFX()
    {
        SpawnVFX(leftFoot);
    }

    public void SpawnRightFootVFX()
    {
        SpawnVFX(rightFoot);
    }

    private void SpawnVFX(Transform foot)
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

        // Apply a rotation offset relative to the foot
        vfxInstance.transform.localRotation *= Quaternion.Euler(0f, 90f, 0f);
    }



}
