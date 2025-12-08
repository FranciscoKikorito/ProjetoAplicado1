using UnityEngine;

public class TubeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerFront"))
            return;

        Animator anim = GetComponentInParent<Animator>();

        if (anim != null)
        {
            anim.SetTrigger("Activate");
            //Debug.Log($"TubeTrigger activated");
        }
    }
}
