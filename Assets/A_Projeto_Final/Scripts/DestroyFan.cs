using UnityEngine;

public class DestroyFan : MonoBehaviour
{

    private SlopesAndJumping playerJumpScript;
    private PlayerLives playerLivesScript;
    void Start()
    {
        playerJumpScript = GameObject.Find("Player").GetComponent<SlopesAndJumping>();
        playerLivesScript = GameObject.Find("Player").GetComponent<PlayerLives>();
        //Debug.Log(playerJumpScript);
        //Debug.Log(playerLivesScript);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("PlayerFront")) {
            Destructible destructible = GetComponentInParent<Destructible>();
            //Debug.Log("Fan Triggered");
            //Debug.Log(destructible);

            bool isDashing = playerJumpScript.dashCheck();
            if (isDashing)
            {
                destructible.Explode();
            } else if (!isDashing)
            {
                playerLivesScript.ApplyDamage(1);
            }
        }
    }


}