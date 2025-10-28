/*
using UnityEngine;

public class ColorOrb : MonoBehaviour
{
    public enum OrbType { Verde, Castanha }
    public OrbType orbType; // Escolhes no Inspector

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que tocou tem o PlayerSelfController (root)
        PlayerSelfController player = other.GetComponentInParent<PlayerSelfController>();
        if (player != null && !collected)
        {
            collected = true;

            // Dependendo do tipo da orb e do jogador ativo
            if (orbType == OrbType.Verde && player.IsWhiteActive())
            {
                Debug.Log("White player recolheu a orb verde -> pintar folhas 🌿");
                Object.FindFirstObjectByType<ColorChangeManager>().StartLeafColorChange();
            }
            else if (orbType == OrbType.Castanha && !player.IsWhiteActive())
            {
                Debug.Log("Black player recolheu a orb castanha -> pintar troncos 🌳");
                Object.FindFirstObjectByType<ColorChangeManager>().StartTrunkColorChange();
            }

            // Efeito visual simples: desativa a orb
            gameObject.SetActive(false);
        }
    }
}
*/