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

        //Quaternion rotation = matchFootRotation ? foot.rotation : Quaternion.identity;
        Quaternion rotation = Quaternion.Euler(0f, 90f, 0f);
        Instantiate(
            footVFXPrefab,
            foot.position,
            rotation
        );
    }
}
