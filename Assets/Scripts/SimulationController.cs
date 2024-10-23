using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private Text speedUpdateText;
    [SerializeField] private Text leastP1;
    [SerializeField] private Text leastP2;
    [SerializeField] private Text score;
    [SerializeField] private GameObject popupPanel;
    public Button winnerMainMenuButton;
    public Button closeWinnerPanel;
    [SerializeField] private Text winnerPanelScore;
    [SerializeField] private Text winnerPanelTitle;



    private float speedUpdate = 1f;

    private float timer = 0.0f;
    private bool isRunning = false;

    public Slider speedSlider;
    public Button startButton;
    public Button pauseButton;
    public Button randomFillButton;
    public Button clearButton;
    public Button mainMenuButton;
    public InputField sizeInputField;

    public Button p1;
    public Button p2;

    public int max_player = 1;
    public int curr_player = 0;
    public int max_start_square;

    private Vector3Int lastCell = new Vector3Int(0, 0, 0);

    private void Start() {
        max_player = PlayerPrefs.GetInt("maxPlayers");
        speedSlider.onValueChanged.AddListener(OnSpeedChanged);
        startButton.onClick.AddListener(StartSimulation);
        pauseButton.onClick.AddListener(PauseSimulation);
        randomFillButton.onClick.AddListener(RandomFill);
        clearButton.onClick.AddListener(ClearBoard);
        mainMenuButton.onClick.AddListener(GoToMenu);
        if (max_player == 2) {
            p1.onClick.AddListener(ChangeToP1);
            p2.onClick.AddListener(ChangeToP2);
            winnerMainMenuButton.onClick.AddListener(GoToMenu);
            closeWinnerPanel.onClick.AddListener(HidePopup);
            HidePopup();
        }
        sizeInputField.onValueChanged.AddListener(UpdateFieldSize);
        
        max_start_square = (board.maxSizeField / 2) * 3;
        UpdateText();
        UpdateSpeedText();
    }

    private void Update() {
        UpdateSpeedText();
        UpdateText();
        if (isRunning) {
            timer += Time.deltaTime;
            if (timer >= speedUpdate) {
                board.UpdateBoard();
                timer = 0.0f;
                
            }
        } else if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            board.isPlacingPattern = false;
        } else if (board.isPlacingPattern && Input.GetMouseButtonDown(0)) {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt(worldPos.x);
            int y = Mathf.FloorToInt(worldPos.y);
            board.PlacePattern(x, y);
            return;
        } else if (Input.GetMouseButton(0) && !board.isPlacingPattern) {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt(worldPos.x);
            int y = Mathf.FloorToInt(worldPos.y);
            Vector3Int currentCell = new Vector3Int(x, y, 0);
            if (currentCell != lastCell) {
                board.AddOnceSquare(x, y);
                lastCell = currentCell;
            }
        }
    }

    public void StartSimulation() {
        isRunning = true;
    }

    public void PauseSimulation() {
        isRunning = false;
        if (max_player == 2) {
            ShowPopup();
        }
    }

    public void RandomFill() {
        if (isRunning) return;
        board.RandomFill();
    }

    public void ClearBoard() {
        isRunning = false;
        board.Clear();
    }

    public void GoToMenu() {
        isRunning = false;
        board.Clear();
        SceneManager.LoadScene("MainMenu");
    }

    public void OnSpeedChanged(float value) {
        speedUpdate = 1 / value;
        UpdateSpeedText();
    }

    private void UpdateSpeedText() {
        speedUpdateText.text = "Current speed update: " +  (1 / speedUpdate).ToString("F2") + " iter / sec";
    }

    private void UpdateText() {
        if (max_player == 1) {
            max_start_square = board.maxSizeField / 2;
            return;
        }
        leastP1.text = "Least P1 squares: " + (max_start_square - board.countAlive[0]).ToString();
        leastP2.text = "Least P2 squares: " + (max_start_square - board.countAlive[1]).ToString();
        score.text = "Player 1 " + board.countAlive[0].ToString() + " : "
        + board.countAlive[1].ToString() + " Player 2";
    }

    public void UpdateFieldSize(string value) {
        if (isRunning) return;
        if (int.TryParse(value, out int newSize))
        {
            board.SetFieldSize(newSize);
        }
    }

    public void ChangeToP1() {
        curr_player = 0;
    }

    public void ChangeToP2() {
        curr_player = 1;
    }

    public void ShowPopup() {
        popupPanel.SetActive(true);
        winnerPanelTitle.text = "";
        if (board.countAlive[0] == board.countAlive[1]) {
            winnerPanelTitle.text = "DRAW!";
        } else {
            winnerPanelTitle.text = "WIN PLAYER" + (board.countAlive[0] > board.countAlive[1] ? 1 : 2);
        }
        winnerPanelScore.text = "Player 1 " + board.countAlive[0].ToString() + " : "
        + board.countAlive[1].ToString() + " Player 2";
    }

    public void HidePopup() {
        popupPanel.SetActive(false);
    }
}
