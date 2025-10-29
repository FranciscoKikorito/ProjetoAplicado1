using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeManager : MonoBehaviour
{
    [Header("Renderers a pintar")]
    public List<Renderer> leafRenderers = new List<Renderer>();
    public List<Renderer> trunkRenderers = new List<Renderer>();

    [Header("Cores alvo")]
    public Color leafTargetColor = Color.green;
    public Color trunkTargetColor = new Color(0.36f, 0.25f, 0.2f); // castanho

    public float colorChangeSpeed = 1f;

    private bool leafChanging = false;
    private bool trunkChanging = false;

    public void StartLeafColorChange()
    {
        if (!leafChanging)
        {
            leafChanging = true;
            StartCoroutine(ChangeColorsGradually(leafRenderers, leafTargetColor));
        }
    }

    public void StartTrunkColorChange()
    {
        if (!trunkChanging)
        {
            trunkChanging = true;
            StartCoroutine(ChangeColorsGradually(trunkRenderers, trunkTargetColor));
        }
    }

    private IEnumerator ChangeColorsGradually(List<Renderer> renderers, Color targetColor)
    {
        float t = 0;
        List<Color> originalColors = new List<Color>();

        foreach (var rend in renderers)
        {
            if (rend != null)
                originalColors.Add(rend.material.color);
        }

        while (t < 1f)
        {
            t += Time.deltaTime * colorChangeSpeed;
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] != null)
                {
                    Color newColor = Color.Lerp(originalColors[i], targetColor, t);
                    renderers[i].material.color = newColor;
                }
            }
            yield return null;
        }
    }
}