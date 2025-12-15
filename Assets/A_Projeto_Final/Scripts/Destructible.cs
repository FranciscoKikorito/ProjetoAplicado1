using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject BrokenPrefab;
    [SerializeField] private AudioClip DestructionClip;
    [SerializeField] private float ExplosiveForce = 1000f;
    [SerializeField] private float ExplosiveRadius = 2f;
    [SerializeField] private float PieceFadeSpeed = 0.25f;
    [SerializeField] private float PieceDestroyDelay = 5f;
    [SerializeField] private float PieceSleepCheckDelay = 0.1f;

    private Rigidbody rootRb;
    private AudioSource audioSource;

    private GameObject myParent;
    private void Awake()
    {
        myParent = transform.parent != null ? transform.parent.gameObject : null;
        rootRb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

    }

    public void Explode()
    {
        if (DestructionClip != null && audioSource != null)
            audioSource.PlayOneShot(DestructionClip);

        if (BrokenPrefab != null)
        {
            GameObject brokenInstance = Instantiate(BrokenPrefab, transform.position, transform.rotation);
            Rigidbody[] pieces = brokenInstance.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody piece in pieces)
            {
                if (rootRb != null)
                    piece.linearVelocity = rootRb.linearVelocity;

                piece.AddExplosionForce(ExplosiveForce, transform.position, ExplosiveRadius);
            }

            StartCoroutine(FadeOutRigidBodies(pieces));

            StartCoroutine(ParentAfterDelay(brokenInstance, 1.7f));
        }

        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
            rend.enabled = false;

        if (rootRb != null)
            Destroy(rootRb);

        Transform target = transform.parent.Find("Cylinder.001");
        //Debug.Log(target);
        //Debug.Log(target.gameObject);
        Destroy(target.gameObject);    
    }

    //efeito para seguir o mundo
    private IEnumerator ParentAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null && myParent != null)
            obj.transform.SetParent(myParent.transform, worldPositionStays: true);
    }


    private IEnumerator FadeOutRigidBodies(Rigidbody[] pieces)
    {
        WaitForSeconds wait = new WaitForSeconds(PieceSleepCheckDelay);

        while (true)
        {
            bool allSleeping = true;
            foreach (Rigidbody rb in pieces)
            {
                if (rb != null && !rb.IsSleeping())
                {
                    allSleeping = false;
                    break;
                }
            }

            if (allSleeping) break;
            yield return wait;
        }

        yield return new WaitForSeconds(PieceDestroyDelay);

        List<Renderer> renderers = new List<Renderer>();
        foreach (Rigidbody rb in pieces)
        {
            if (rb == null) continue;

            Collider col = rb.GetComponent<Collider>();
            if (col != null) Destroy(col);

            Renderer rend = rb.GetComponent<Renderer>();
            if (rend != null) renderers.Add(rend);

            Destroy(rb);
        }

        float time = 0f;
        while (time < 1f)
        {
            float step = Time.deltaTime * PieceFadeSpeed;
            foreach (Renderer rend in renderers)
            {
                if (rend != null)
                    rend.transform.Translate(Vector3.down * (step / rend.bounds.size.y), Space.World);
            }
            time += step;
            yield return null;
        }

        foreach (Renderer rend in renderers)
        {
            if (rend != null)
                Destroy(rend.gameObject);
        }

        Destroy(gameObject);
    }
}
