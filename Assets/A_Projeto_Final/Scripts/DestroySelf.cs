using UnityEngine;

public class DestroySelf : MonoBehaviour
{

    private SlopesAndJumping playerJumpScript;
    void Start()
    {
        playerJumpScript = GameObject.Find("Player").GetComponent<SlopesAndJumping>();
        //Debug.Log(playerJumpScript);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("PlayerFront")) {
            Debug.Log("Correct tag detected");
            bool isDashing = playerJumpScript.dashCheck();
            if (isDashing)
            {
                Debug.Log("Destroying object: " + gameObject.name);
                Destroy(gameObject);
            } 
        }
    }

}