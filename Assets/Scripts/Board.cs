using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    [SerializeField] private List<Tile> aliveTiles;
    [SerializeField] private List<Tile> potentialTiles;
    [SerializeField] private Tile deadTile;
    [SerializeField] private Tile borderTile;
    [SerializeField] public int maxSizeField = 50;
    [SerializeField] private SimulationManager simMabager;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform contentPanel;
    [SerializeField] public List<Pattern> patterns;
    public string currentPatternName;
    public bool isPlacingPattern = false;

    private List<HashSet<Vector3Int>> aliveCells = new();
    private HashSet<Vector3Int> cellsToCheck = new();
    private HashSet<Vector3Int> cellsBorder = new();
    public List<int> countAlive = new();
    private void Start() {
        CreateBorder();
        LoadPatterns();
    }

    private void CreateBorder() {
        for (int i = -maxSizeField / 2; i <= maxSizeField / 2; ++i) {
            for (int j = -maxSizeField / 2; j <= maxSizeField / 2; ++j) {
                if (i == maxSizeField / 2 || i == -maxSizeField / 2 || j == maxSizeField / 2 || j == -maxSizeField / 2) {
                    Vector3Int cell = new Vector3Int(i, j, 0);
                    currentState.SetTile(cell, borderTile);
                    cellsBorder.Add(cell);
                }
            }
        }
    }

    private void LoadPatterns() {
        foreach (Pattern pattern in patterns) {
            GameObject button = Instantiate(buttonPrefab, contentPanel);
            PatternButton patternButton = button.GetComponent<PatternButton>();
            patternButton.pattern = pattern;
            patternButton.board = this;
            button.GetComponentInChildren<Text>().text = pattern.name; 
        }
    }
    public void AddOnceSquare(int x, int y) {
        Vector3Int cell = new Vector3Int(x, y); 
        bool is_alive = IsAlive(cell);
        if (is_alive && aliveTiles[simMabager.curr_player] == currentState.GetTile(cell)) {
            aliveCells[simMabager.curr_player].Remove(cell);
            currentState.SetTile(cell, deadTile);
            if (simMabager.max_player == 1) return;
            --countAlive[simMabager.curr_player];
        } else if (!is_alive && countAlive[simMabager.curr_player] < simMabager.max_start_square){
            aliveCells[simMabager.curr_player].Add(cell);
            currentState.SetTile(cell, aliveTiles[simMabager.curr_player]);
            if (simMabager.max_player == 1) return;
            ++countAlive[simMabager.curr_player];
        }
    }
    public void UpdateBoard() {
        cellsToCheck.Clear();
        for (int i = 0; i < 2; ++i) {
            foreach (Vector3Int cell in aliveCells[i]) {
                for (int x = -1; x < 2; ++x) {
                    for (int y = -1; y < 2; ++y) {
                        Vector3Int newCell = cell + new Vector3Int(x, y);
                        if (-maxSizeField / 2 < newCell.x && newCell.x < maxSizeField / 2 &&
                        -maxSizeField / 2 < newCell.y && newCell.y < maxSizeField / 2) {
                            cellsToCheck.Add(newCell);
                        }
                    }
                }
            }
        }

        foreach (Vector3Int cell in cellsToCheck) {
            List<int> countNeighbours = CountNeighbours(cell);
            bool isAlive = IsAlive(cell);
            if (countNeighbours[0] == countNeighbours[1]) {
                nextState.SetTile(cell, deadTile);
                if (isAlive) {
                    int insert_player = currentState.GetTile(cell) == aliveTiles[0] ? 0 : 1;
                    aliveCells[insert_player].Remove(cell);
                    aliveCells[1 - insert_player].Remove(cell);
                }
            } else {
                int insert_player = (countNeighbours[0] > countNeighbours[1]) ? 0 : 1;
                if ((countNeighbours[insert_player] < 2 || countNeighbours[insert_player] > 3) && isAlive) {
                    aliveCells[insert_player].Remove(cell);
                    nextState.SetTile(cell, deadTile);
                } else if (countNeighbours[insert_player] == 3 && !isAlive) {
                    aliveCells[insert_player].Add(cell);
                    nextState.SetTile(cell, aliveTiles[insert_player]);
                } else {
                    nextState.SetTile(cell, currentState.GetTile(cell));
                }
            }
        }

        SwapTilemaps();
        CreateBorder();
        countAlive[0] = aliveCells[0].Count;
        countAlive[1] = aliveCells[1].Count;
    }

    private List<int> CountNeighbours(Vector3Int cell) {
        int count_1 = 0;
        int count_2 = 0;
        for (int x = -1; x < 2; ++x) {
            for (int y = -1; y < 2; ++y) {
                if (x == 0 && y == 0) continue;
                if (IsAlive(cell + new Vector3Int(x, y))) {
                    if (currentState.GetTile(cell + new Vector3Int(x, y)) == aliveTiles[0]) ++count_1;
                    else ++count_2;
                }
            }
        }
        return new List<int>{count_1, count_2};
    }

    private bool IsAlive(Vector3Int cell) {
        return currentState.GetTile(cell) == aliveTiles[0] || currentState.GetTile(cell) == aliveTiles[1];
    }

    private void SwapTilemaps() {
        Tilemap temp = currentState;
        currentState = nextState;
        nextState = temp;
        nextState.ClearAllTiles();
    }

    public void RandomFill() {
        Clear();
        for (int i = -maxSizeField / 2 + 1; i < maxSizeField / 2 - 1; ++i) {
            for (int j = -maxSizeField / 2 + 1; j < maxSizeField / 2 - 1; ++j) {
                int isFill = Random.Range(0, 2);
                if (isFill == 1) {
                    Vector3Int cell = new Vector3Int(i, j, 0);
                    int player = Random.Range(0, simMabager.max_player);
                    aliveCells[player].Add(cell);
                    currentState.SetTile(cell, aliveTiles[player]);
                }
            }
        }
    }

    public void Clear() {
        aliveCells[0].Clear();
        aliveCells[1].Clear();
        cellsToCheck.Clear();
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
        CreateBorder();
        countAlive = new List<int> {0, 0};
    }

    void Awake() {
        aliveCells = new List<HashSet<Vector3Int>> {new HashSet<Vector3Int>(), new HashSet<Vector3Int>()};
        cellsToCheck = new HashSet<Vector3Int>();
        cellsBorder = new HashSet<Vector3Int>();
        countAlive = new List<int> {0, 0};
    }

    public void SetFieldSize(int newSize) {
        maxSizeField = newSize;
        Clear();
        CreateBorder();
    }

     public void SetCurrentPattern(string pattern_name) {
        currentPatternName = pattern_name;
        isPlacingPattern = true;
    }

    public void PlacePattern(int x, int y) {
        foreach (Pattern pat in patterns) {
            if (pat.name == currentPatternName) {
                foreach (Vector3Int offset in pat.cells) {
                    Vector3Int new_cell = new Vector3Int(x + offset.x, y + offset.y, 0);
                    if (currentState.GetTile(new_cell) == aliveTiles[1 - simMabager.curr_player] ||
                    currentState.GetTile(new_cell) == aliveTiles[simMabager.curr_player] ||
                    currentState.GetTile(new_cell) == borderTile || -maxSizeField / 2 > new_cell.x ||
                    new_cell.x > maxSizeField / 2 || -maxSizeField / 2 > new_cell.y || new_cell.y > maxSizeField / 2) return;
                }
                if (pat.cells.Length > simMabager.max_start_square - countAlive[simMabager.curr_player]
                && simMabager.max_player == 2) {
                    return;
                }
                foreach (Vector3Int offset in pat.cells) {
                    Vector3Int new_cell = new Vector3Int(x + offset.x, y + offset.y, 0);
                    currentState.SetTile(new_cell, aliveTiles[simMabager.curr_player]);
                    aliveCells[simMabager.curr_player].Add(new_cell);
                }
                countAlive[simMabager.curr_player] += pat.cells.Length;
            }
        } 
    }
}
