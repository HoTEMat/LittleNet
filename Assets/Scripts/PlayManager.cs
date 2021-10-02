using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour {
    public bool Playing { get; private set; }
    public RectTransform backgroundPosition;

    [SerializeField] private Button buttonPrefab;

    private Button playPauseButton;
    private Button DisableButton;

    private int buttonSpacing = 8;
    private float buttonSize;

    private void Start() {
        buttonSize = buttonPrefab.GetComponent<RectTransform>().rect.width;

        playPauseButton = Instantiate(buttonPrefab, transform);
        playPauseButton.onClick.AddListener(PlayPausePressed);

        RectTransform buttonTransform = playPauseButton.GetComponent<RectTransform>();

        var yOffset = backgroundPosition.anchoredPosition.y - buttonSize / 2;
        
        buttonTransform.anchoredPosition = new Vector2(-yOffset, yOffset);
    }

    private void PlayPausePressed() {
    }
}
