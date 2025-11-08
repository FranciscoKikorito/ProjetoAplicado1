using UnityEngine;

public class CharacterSwitch : MonoBehaviour
{
    public Transform character1;
    public Transform character2;

    public float frontOffset = -0.1f;
    public float switchDuration = 0.5f;  // time for animation
    public float switchCooldown = 1f;    // delay before next switch

    private bool isCharacter1Front = true;
    private bool isSwitching = false;

    void Start()
    {
        UpdateCharacterOrderInstant();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isSwitching)
        {
            StartCoroutine(SwitchCharacters());
        }
    }

    System.Collections.IEnumerator SwitchCharacters()
    {
        isSwitching = true;
        isCharacter1Front = !isCharacter1Front;

        Vector3 char1Start = character1.position;
        Vector3 char2Start = character2.position;

        Vector3 char1Target = new Vector3(char1Start.x, char1Start.y, isCharacter1Front ? frontOffset : 0f);
        Vector3 char2Target = new Vector3(char2Start.x, char2Start.y, isCharacter1Front ? 0f : frontOffset);

        float t = 0f;
        while (t < switchDuration)
        {
            t += Time.deltaTime;
            float normalized = t / switchDuration;
            character1.position = Vector3.Lerp(char1Start, char1Target, normalized);
            character2.position = Vector3.Lerp(char2Start, char2Target, normalized);
            yield return null;
        }

        // Ensure final positions are accurate
        character1.position = char1Target;
        character2.position = char2Target;

        // Optional small cooldown
        yield return new WaitForSeconds(switchCooldown);

        isSwitching = false;
    }

    void UpdateCharacterOrderInstant()
    {
        if (isCharacter1Front)
        {
            character1.position = new Vector3(character1.position.x, character1.position.y, frontOffset);
            character2.position = new Vector3(character2.position.x, character2.position.y, 0f);
        }
        else
        {
            character1.position = new Vector3(character1.position.x, character1.position.y, 0f);
            character2.position = new Vector3(character2.position.x, character2.position.y, frontOffset);
        }
    }
}
