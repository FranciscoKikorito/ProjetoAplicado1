using UnityEngine;

public class GameStartController : MonoBehaviour
{
    [Header("Player")]
    public Animator playerAnimator;
    public static bool canJump = false;
    
    [Header("Gameplay")]
    public MovePlatform[] allPlatforms;  
    public float platformStartSpeed = -10f;

    [Header("Cameras")]
    public GameObject introCAM;        // câmera da intro (ativa no início)
    public GameObject gameplayCAM;     // câmera da gameplay (ativa depois do clique)
    
    [Header("UI")]
    public GameObject pressStartUI;
    
    [Header("Audio")]
    public MusicManager musicManager;
    
    private bool gameStarted = false;
    private bool animationPlayed = false;

    void Start()
    {
        // parar plataformas
        foreach (var p in allPlatforms)
            p.SetMoveDirection(Vector3.zero);

        // garantir idle inicial
        playerAnimator.Play("Idle_Start");

        // Ativa introCAM + UI
        introCAM.SetActive(true);
        gameplayCAM.SetActive(false);
        pressStartUI.SetActive(true);
        
        if (musicManager != null)
            musicManager.PlayIntroMusic();
    }

    void Update()
    {
        if (!gameStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                gameStarted = true;

                // troca de câmeras →
                introCAM.SetActive(false);
                gameplayCAM.SetActive(true);
                // esconde a UI
                pressStartUI.SetActive(false);
                // start do player
                playerAnimator.SetTrigger("StartGame");
            }

            return;
        }

        // quando a animação StandUp acabar → começa o jogo
        AnimatorStateInfo state = playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (!animationPlayed && state.IsName("Rig|Run"))
        {
            animationPlayed = true;
            canJump = true;
            
            foreach (var p in allPlatforms)
                p.SetMoveDirection(Vector3.forward * platformStartSpeed);
            
            if (musicManager != null)
                musicManager.PlayGameplayMusic();
        }
    }
}