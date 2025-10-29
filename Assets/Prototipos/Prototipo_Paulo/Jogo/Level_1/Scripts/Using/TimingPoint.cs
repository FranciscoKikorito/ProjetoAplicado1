using UnityEngine;
using System.Collections;

public class TimingPoint : MonoBehaviour
{
    public enum ActionType { Build, Destroy }
    public ActionType actionType;

    [Header("Objeto associado")]
    public GameObject associatedObject; // ponte ou parede relacionada

    [Header("Animação da Ponte")]
    public float bridgeRiseHeight = 2f;
    public float bridgeRiseTime = 0.6f;

    [Header("Efeitos de destruição")]
    public GameObject destroyParticlePrefab;
    
    [Header("Visual")]
    public Vector3 gizmoSize = new Vector3(2f, 0.2f, 1f);

    private Vector3 originalBridgePosition;
    
    private void OnDrawGizmos()
    {
        Color fillColor = actionType == ActionType.Build ? new Color(0f, 1f, 0f, 0.4f) : new Color(1f, 0f, 0f, 0.4f);
        Color lineColor = actionType == ActionType.Build ? Color.green : Color.red;

        Gizmos.color = fillColor;
        Gizmos.DrawCube(transform.position, gizmoSize);

        Gizmos.color = lineColor;
        Gizmos.DrawWireCube(transform.position, gizmoSize);
    }
    public IEnumerator AnimateBridgeRise(GameObject bridge)
    {
        if (!bridge.activeInHierarchy)
            bridge.SetActive(true);

        Vector3 endPos = bridge.transform.position;
        Vector3 startPos = endPos - Vector3.up * bridgeRiseHeight;

        bridge.transform.position = startPos;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / bridgeRiseTime;
            bridge.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        bridge.transform.position = endPos;
    }

    // 🔹 Parede desce suavemente e depois some
    public IEnumerator AnimateWallFall(GameObject wall, System.Action onComplete = null)
    {
        if (wall == null) yield break;

        // Captura posição inicial e final
        Vector3 startPos = wall.transform.position;
        Vector3 endPos = startPos - Vector3.up * bridgeRiseHeight;

        float elapsed = 0f;
        while (elapsed < bridgeRiseTime)
        {
            if (wall == null) yield break; // se for destruído antes do fim, sai
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / bridgeRiseTime);
            wall.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        wall.transform.position = endPos; // garante posição final

        // Executa partículas e destruição **depois da animação**
        onComplete?.Invoke();
    }

    // 🔹 Partículas de destruição
    public void SpawnDestroyParticles(Vector3 position)
    {
        if (destroyParticlePrefab != null)
        {
            GameObject particles = Instantiate(destroyParticlePrefab, position, Quaternion.identity);
            Destroy(particles, 2f); // destrói partículas depois de 2s
        }
    }
}
