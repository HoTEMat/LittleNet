using UnityEngine;
using UnityEngine.UI;

public enum PlayState {
    Stopped,
    Paused,
    Playing
}

public class PlayManager : MonoBehaviour {
    public PlayState State { get; set; }
    
    public Camera camera;
    public RectTransform backgroundPosition;

    [SerializeField] private Button buttonPrefab;

    private Button playPauseButton;
    private Button stopButton;

    private void Start() {
        playPauseButton = Instantiate(buttonPrefab, transform);
        playPauseButton.onClick.AddListener(PlayPausePressed);
        
        stopButton = Instantiate(buttonPrefab, transform);
        stopButton.onClick.AddListener(StopPressed);
    }

    private void Update() {
        float width = camera.scaledPixelWidth;
        float height = camera.scaledPixelHeight;
        
        float buttonSize = backgroundPosition.sizeDelta.y * 0.7f;
        float buttonSpacing = buttonSize * 0.25f;

        RectTransform buttonTransform = playPauseButton.GetComponent<RectTransform>();
        buttonTransform.sizeDelta = new Vector2(buttonSize, buttonSize);
        buttonTransform.anchoredPosition = new Vector2(width - backgroundPosition.anchoredPosition.y - buttonSize * (3/2f), backgroundPosition.anchoredPosition.y);
        
        buttonTransform = stopButton.GetComponent<RectTransform>();
        buttonTransform.sizeDelta = new Vector2(buttonSize, buttonSize);
        buttonTransform.anchoredPosition = new Vector2(width - backgroundPosition.anchoredPosition.y - buttonSize * (5/2f) - buttonSpacing, backgroundPosition.anchoredPosition.y);

        stopButton.interactable = State != PlayState.Stopped;
    }

    private void PlayPausePressed() {
        switch (State) {
            case PlayState.Paused:
            case PlayState.Stopped:
                State = PlayState.Playing;
                break;
            case PlayState.Playing:
                State = PlayState.Paused;
                break;
        }
    }

    private void StopPressed() {
        State = PlayState.Stopped;
    }
}
