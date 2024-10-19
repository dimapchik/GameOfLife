using UnityEngine;
using UnityEngine.UI;

public class PatternButton : MonoBehaviour
{
    public Pattern pattern;
    public Board board;
    private Button button;

    void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick() {
        board.SetCurrentPattern(pattern.name);
    }
}
