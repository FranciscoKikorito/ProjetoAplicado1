/*
using UnityEngine;
public class Block : MonoBehaviour
{
    public enum BlockColor { White, Black }
    public BlockColor color;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerFront"))
        {
            PlayerSelfController controller = other.GetComponentInParent<PlayerSelfController>();

            if (controller != null)
            {
                bool match = 
                    (color == BlockColor.White && controller.IsWhiteActive()) ||
                    (color == BlockColor.Black && !controller.IsWhiteActive());

                if (match)
                {
                    // Bloco "afastado" (desaparece)
                    Destroy(gameObject);
                }
                else
                {
                    // Jogador bate → trava (podes parar o movimento)
                    controller.StopMovement();
                }
            }
        }
    }
}
*/