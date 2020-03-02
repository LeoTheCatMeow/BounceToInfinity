using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    public GameObject header;
    public GameObject mobileInput;
    public GameObject pauseBtn;
    public GameObject pauseMenu;
    public GameObject menuBtn;
    public GameObject tutorial;
    public Text score;
    public Text message;
    public Text popUp;
    public Image fader;
    public Animator popUpAnim;

    private List<(string message, float duration)> messages = new List<(string message, float duration)> {
        ("Whoops", 1.5f), ("Just a small misstep", 2f), ("Falling is just another way of flying", 2.5f),
        ("Sometimes you can only blame the game", 2.5f), ("Merely a strategic retreat", 2f), ("Yikes", 1.5f),
        ("Need a break anyway", 2f), ("That was a practice run", 2f), ("Back to square one", 2f),
        ("Failure builds the foundation for success", 2.5f), ("The ground is slippery", 2f), ("It's definitely the physics engine", 2.5f),
        ("Effort outweighs results", 2f), ("Wonky controls", 2f), ("Worse than real life struggles", 2f),
        ("The struggle is real", 2f), ("Lag", 1.5f), ("That was utterly impossible", 2f), ("What", 1.5f),
        ("It's no fun without a challenge", 2f), ("A brave fall", 2f), ("Acceptable miscalculation", 2f), ("That just happened", 2f),
        ("An inevitable fate", 2f), ("Well well", 1.5f), ("Don't give up", 2f), ("Falter not, victory lies wait in persistance", 2.5f),
        ("Oh", 1.5f), ("That's unfortunate", 2f), ("Nobody said going up is the right way", 2.5f), ("Again and again, the cylce continues", 2.5f),
        ("A moment of silence", 3f)
    };
    private List<(string message, float duration)> _messages;

    private bool paused = false;

    void Awake()
    {
        GameEvents._GameStart += ShowGameplayElements;
        GameEvents._GameOver += OnGameOver;
        GameEvents._GameReset += OnGameReset;
        GameEvents._ReturnToMenu += HideGameplayElements;
        GameEvents._PlayTutorial += PlayTutorial;
        GameEvents._UpdateScore += UpdateScore;
        GameEvents._DisplayPopUp += DisplayPopUp;
    }

    void OnDestroy()
    {
        GameEvents._GameStart -= ShowGameplayElements;
        GameEvents._GameOver -= OnGameOver;
        GameEvents._GameReset -= OnGameReset;
        GameEvents._ReturnToMenu -= HideGameplayElements;
        GameEvents._PlayTutorial -= PlayTutorial;
        GameEvents._UpdateScore -= UpdateScore;
        GameEvents._DisplayPopUp -= DisplayPopUp;
    }

    void Start()
    {
        _messages = new List<(string message, float duration)>(messages);
        fader.enabled = true;
        FadeIn();
        HideGameplayElements();
    }

    private void OnGameReset()
    {
        FadeIn();
    }

    private void OnGameOver()
    {
        pauseBtn.SetActive(false);
        FadeToBlack();
        StartCoroutine(DisplayRandomMessage());
    }

    private void HideGameplayElements()
    {
        header.SetActive(false);
        mobileInput.SetActive(false);
    }

    private void ShowGameplayElements()
    {
        header.SetActive(true);
        mobileInput.SetActive(true);
    }

    private void FadeToBlack()
    {
        fader.CrossFadeAlpha(1f, 0.6f, true);
    }

    private void FadeIn()
    {
        fader.CrossFadeAlpha(0f, 0.8f, true);
    }

    private void PlayTutorial()
    {
        tutorial.SetActive(true);
        PlayerData.tutorialEnabled = false;
    }

    private void UpdateScore(float value)
    {
        if (value == 0f)
        {
            score.text = "0.00";
            return;
        }
        score.text = value.ToString("#.00");
    }

    private IEnumerator DisplayRandomMessage()
    {
        if (_messages.Count == 0)
        {
            _messages = new List<(string message, float duration)>(messages);
        }

        (string message, float duration) pair = _messages[Random.Range(0, _messages.Count)];
        _messages.Remove(pair);
        
        message.text = pair.message;
        message.gameObject.SetActive(true);
        yield return new WaitForSeconds(pair.duration);
        message.gameObject.SetActive(false);

        GameEvents.GameReset();
        pauseBtn.SetActive(true);
    }

    private void DisplayPopUp(string info, float duration)
    {
        popUp.text = info;
        StartCoroutine(PopUpAnim(duration));
    }

    private IEnumerator PopUpAnim(float duration)
    {
        popUpAnim.Play("Enter");
        yield return new WaitForSeconds(duration);
        popUpAnim.Play("Exit");
    }

    public void PauseOrUnpause()
    {
        if (!paused)
        {
            pauseMenu.SetActive(true);
            menuBtn.SetActive(true);
            Time.timeScale = 0f;
            paused = true;
        } else
        {
            pauseMenu.SetActive(false);
            menuBtn.SetActive(false);
            Time.timeScale = 1f;
            paused = false;
        }
    }
}
