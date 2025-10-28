using UnityEngine;

// Faz o objecto (sombra) seguir o target nos eixos X/Z e manter-se no groundY.
// Também controla visual de "sombra" (scale + alpha).
public class ShadowFollow : MonoBehaviour
{
    [Tooltip("O transform que esta sombra acompanha (normalmente o corpo activo)")]
    public Transform target;

    public float followSpeed = 10f;
    public float groundY = 0.05f; // altura da sombra sobre o plano
    public Vector3 shadowScale = new Vector3(1f, 0.25f, 1f);
    public Vector3 uprightScale = Vector3.one;

    Renderer rend;
    Color originalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend) originalColor = rend.material.color;
    }

    void Update()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(target.position.x, groundY, target.position.z);
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * followSpeed);
    }

    // chama-se para alternar entre ser sombra (achatado e escuro/transparente)
    // ou ser a forma normal (vertical, opaco).
    public void SetAsShadow(bool isShadow, bool isWhiteActive)
    {
        if (rend == null) return;

        if (isShadow)
        {
            transform.localScale = shadowScale;
            rend.material = new Material(rend.material);

            Color c;
            if (isWhiteActive)
            {
                // Se o branco está activo → sombra preta
                c = Color.black;
                c.a = 0.8f;
            }
            else
            {
                // Se o preto está activo → sombra branca
                c = Color.white;
                c.a = 0.8f;
            }

            rend.material.color = c;
        }
        else
        {
            transform.localScale = uprightScale;
            rend.material = new Material(rend.material);
            rend.material.color = originalColor;
        }
    }
}