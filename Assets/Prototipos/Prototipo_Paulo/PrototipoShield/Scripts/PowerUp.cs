using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float rotateSpeed = 90f; 
    public GameObject visualObject; 

    private void Update()
    {
        if (visualObject != null)
            visualObject.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCollect(other.gameObject);
            Destroy(gameObject);
        }
    }

    void PlayerCollect(GameObject player)
    {
        Debug.Log("PowerUp apanhado!");

        Health health = player.GetComponent<Health>();

        if (health != null)
        {
            if (!health.headbandOnHead.activeSelf)
            {
                health.RestoreHeadband();
                Debug.Log("Headband restaurada ao jogador!");
            }
            else
            {
                Debug.Log("Jogador já tem headband. PowerUp sem efeito.");
            }
        }
    }
}