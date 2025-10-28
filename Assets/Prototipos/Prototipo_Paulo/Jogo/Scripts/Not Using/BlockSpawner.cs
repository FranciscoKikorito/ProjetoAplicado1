using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameObject blockWhite;
    public GameObject blockBlack;
    public Transform player;

    public float spawnInterval = 3f;
    public float spawnHeight = 10f;
    public float spawnDistance = 20f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;

            // Decide aleatoriamente se spawna branco ou preto
            bool spawnWhite = Random.value > 0.5f;
            GameObject prefab = spawnWhite ? blockWhite : blockBlack;

            Vector3 spawnPos = new Vector3(
                player.position.x,
                spawnHeight,
                player.position.z + spawnDistance
            );

            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}
