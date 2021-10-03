using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour {
    public bool Playing { get; private set; }
    public Camera camera;
    public RectTransform backgroundPosition;

    [SerializeField] private Button buttonPrefab;

    private Button playPauseButton;
    private Button disableButton;

    private void Start() {
        playPauseButton = Instantiate(buttonPrefab, transform);
        playPauseButton.onClick.AddListener(PlayPausePressed);
        
        disableButton = Instantiate(buttonPrefab, transform);
        disableButton.onClick.AddListener(PlayPausePressed);
    }

    private void Update() {
        float width = camera.scaledPixelWidth;
        float height = camera.scaledPixelHeight;
        
        float buttonSize = backgroundPosition.sizeDelta.y * 0.7f;
        float buttonSpacing = buttonSize * 0.25f;

        RectTransform buttonTransform = playPauseButton.GetComponent<RectTransform>();
        buttonTransform.sizeDelta = new Vector2(buttonSize, buttonSize);
        buttonTransform.anchoredPosition = new Vector2(width - backgroundPosition.anchoredPosition.y - buttonSize / 2, backgroundPosition.anchoredPosition.y);
        
        buttonTransform = disableButton.GetComponent<RectTransform>();
        buttonTransform.sizeDelta = new Vector2(buttonSize, buttonSize);
        buttonTransform.anchoredPosition = new Vector2(width - backgroundPosition.anchoredPosition.y - buttonSize * (3/2) - buttonSpacing , backgroundPosition.anchoredPosition.y);
    }

    private void PlayPausePressed() {
    }

    private void DisablePressed() {
        
    }
}
