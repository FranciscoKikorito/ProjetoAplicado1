using UnityEngine;

public class PlayerOrbTrigger2 : MonoBehaviour
{
    public enum OrbType { Verde, Castanha }
    public OrbType orbType;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        Debug.Log($"Orb '{gameObject.name}' tocada por '{other.name}'");

        //PlayerSelfController player = other.GetComponentInParent<PlayerSelfController>();
        //if (player == null) return;

        var colorManager = Object.FindFirstObjectByType<ColorChangeManager>();
        if (colorManager == null)
        {
            Debug.LogError("❌ Nenhum ColorChangeManager encontrado na cena!");
            return;
        }

        // if (orbType == OrbType.Verde && player.IsWhiteActive())
        {
            Debug.Log("✅ White player apanhou orb verde → pintar folhas 🌿");
            colorManager.StartLeafColorChange();
            collected = true;
        }
        //else if (orbType == OrbType.Castanha && !player.IsWhiteActive())
        {
            Debug.Log("✅ Black player apanhou orb castanha → pintar troncos 🌳");
            colorManager.StartTrunkColorChange();
            collected = true;
        }

        if (collected)
        {
            // Efeito opcional: desativar orb
            gameObject.SetActive(false);
        }
    }
}