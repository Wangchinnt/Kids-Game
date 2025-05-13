using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

public class LearnDirectionGame : MiniGame
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private GameObject mapArea;
    [SerializeField] private GameObject mapLayer;
    [SerializeField] private GameObject decorationLayer;
    [SerializeField] private GameObject playerLayer;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private GameObject questionText;
    [SerializeField] private GameObject answerPanel;
    [SerializeField] private List<Button> answerButtons;
    [SerializeField] private int targetCheckpoints = 2;
    [SerializeField] private GameObject nextButton;
    private int[,] map;

    private Vector2Int start;
    private Vector2Int goal;
    private List<Vector2Int> path;
    private Dictionary<Vector2Int, GameObject> spawnedObjects = new Dictionary<Vector2Int, GameObject>();
    private List<Vector2Int> selectedCheckpoints;
    private List<Vector2Int> blockedPoints;
    private System.Random rand;
    private bool isWaitingForAnswer = false;
    private Action<bool> onAnswered;
 
    private float originalMapY = 0f;
    private float raisedMapY = 138f;
    private float animationDuration = 0.5f;
    
    private int currentIndex = 0;
    private Coroutine currentPathCoroutine; 
    private int currentCheckpointIndex = -1;
    private int currentPathIndex = 0;
    private bool isGeneratingPath = false;

    private List<GameObject> _particles = new List<GameObject>();
    #region Serialized Fields
    [Header("Road Prefabs")]
    [SerializeField] private GameObject noRoadPrefab;
    [SerializeField] private GameObject roadHorizontalPrefab;
    [SerializeField] private GameObject roadVerticalPrefab;
    [SerializeField] private GameObject roadIntersectionPrefab;

    [Header("Corner Prefabs")]
    [SerializeField] private GameObject cornerLeftDownPrefab;
    [SerializeField] private GameObject cornerLeftUpPrefab;
    [SerializeField] private GameObject cornerRightDownPrefab;
    [SerializeField] private GameObject cornerRightUpPrefab;

    [Header("End Prefabs")]
    [SerializeField] private GameObject endUpPrefab;
    [SerializeField] private GameObject endDownPrefab;
    [SerializeField] private GameObject endLeftPrefab;
    [SerializeField] private GameObject endRightPrefab;

    [Header("Fork Prefabs")]
    [SerializeField] private GameObject forkUpPrefab;    // thiếu up
    [SerializeField] private GameObject forkDownPrefab;  // thiếu down
    [SerializeField] private GameObject forkLeftPrefab;  // thiếu left
    [SerializeField] private GameObject forkRightPrefab; // thiếu right

    [Header("Special Points")]
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject goalPrefabs;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject playerPrefab; // Add player prefab reference

    [Header("Decoration Prefabs")]
    [SerializeField] private GameObject sign_decorationPrefabs;
    [SerializeField] private List<GameObject> house_decorationPrefabs;
    [SerializeField] private List<GameObject> tree_decorationPrefabs;
    [SerializeField] private List<GameObject> small_decorationPrefabs;

    [Header("Particle Prefabs")]
    [SerializeField] private GameObject spawnParticleObject;
    [SerializeField] private List<GameObject> particlePrefab;
    [SerializeField] private GameObject ground;
    #endregion

    private enum RoadType
    {
        NoRoad = 0,
        Horizontal = 1,
        Vertical = 2,
        Intersection = 3,
        CornerLeftDown = 4,
        CornerLeftUp = 5,
        CornerRightDown = 6,
        CornerRightUp = 7,
        EndUp = 8,
        EndDown = 9,
        EndLeft = 10,
        EndRight = 11,
        ForkUp = 12,    
        ForkDown = 13,  
        ForkLeft = 14,  
        ForkRight = 15, 
        Start = 16,
        Goal = 17,
        Block = 18
    }
    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    Direction VectorToDirection(Vector2Int dir)
    {
        dir = new Vector2Int(dir.y, -dir.x);

        if (dir == Vector2Int.up) return Direction.Up;
        if (dir == Vector2Int.down) return Direction.Down;
        if (dir == Vector2Int.left) return Direction.Left;
        if (dir == Vector2Int.right) return Direction.Right;

        throw new Exception("Hướng không hợp lệ");
    }

    
    void Start()
    {   
        InitializeGame();
    }
    void Update()
    {

    }

    public override void InitializeGame()
    {
        gameName = "Learn Direction Game";
        isCompleted = false;
        isLooping = false;
        
        rand = new System.Random();
        path = new List<Vector2Int>();
        selectedCheckpoints = new List<Vector2Int>();
        blockedPoints = new List<Vector2Int>();
        GenerateMap();  
        RenderMap();
        PrintMap();
    
    }

    public override void StartGame()
    {
        throw new NotImplementedException();
    }

    public override bool CheckWin()
    {
       throw new NotImplementedException();
    }
    void GenerateMap()
    {   
        // set width and height of the map area rect transform
        mapLayer.GetComponent<RectTransform>().sizeDelta = new Vector2(width*100, height*100);
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int current = new Vector2Int(0, 0);
        map = new int[height, width];

        stack.Push(current);
        map[current.x, current.y] = 1;

        while (stack.Count > 0)
        {
            List<Vector2Int> neighbors = GetMazeNeighbors(current);
            if (neighbors.Count > 0)
            {
                Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];

                // mark the middle point as a path
                Vector2Int middle = (current + chosen) / 2;
                map[middle.x, middle.y] = 1;

                map[chosen.x, chosen.y] = 1;
                stack.Push(current);
                current = chosen;
            }
            else
            {
                current = stack.Pop();
            }
        }
        
        start = new Vector2Int(Random.Range(height - 3, height), Random.Range(width - 3, width));
        goal = new Vector2Int(Random.Range(0, height - 3), Random.Range(0, width - 3));
        while (map[start.x, start.y] == 1 || map[goal.x, goal.y] == 1)
        {
            start = new Vector2Int(Random.Range(height - 3, height), Random.Range(width - 3, width));
            goal = new Vector2Int(Random.Range(0, height - 3), Random.Range(0, width - 3));
        }    
        map[start.x, start.y] = 1;
        map[goal.x, goal.y] = 1;
        
        IdentifyRoadTypes();
        EnsureSinglePath(start, goal);
        IdentifyPath(start, goal);
        PlaceDecorations();
    
    }
    List<Vector2Int> GetMazeNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = {
            new Vector2Int(2, 0), new Vector2Int(-2, 0),
            new Vector2Int(0, 2), new Vector2Int(0, -2)
        };
        foreach (Vector2Int dir in directions)
        {
            Vector2Int next = pos + dir;
            if (next.x >= 0 && next.x < height && next.y >= 0 && next.y < width && map[next.x, next.y] == 0)
            {
                neighbors.Add(next);
            }
        }
        return neighbors;
    }

    void EnsureSinglePath(Vector2Int start, Vector2Int goal)
    {
        // Find all possible paths from start to goal
        List<List<Vector2Int>> allPaths = FindAllPaths(start, goal);
        
        Debug.Log($"Found {allPaths.Count} paths from start to goal");
        
       
        if (allPaths.Count <= 1)
        {
            Debug.Log("Only one or no path found, keeping the path as is");
            return;
        }
        
        // Filter out invalid paths (remove paths that are too long)
        List<List<Vector2Int>> validPaths = allPaths
            .Where(p => p.Count >= 2) 
            .ToList();

        if (validPaths.Count <= 1)
        {
            Debug.Log("Only one valid path found after filtering");
            return;
        }
        
 
        List<Vector2Int> mainPath = validPaths
            .OrderBy(p => p.Count)
            .First();
        
        List<List<Vector2Int>> alternativePaths = validPaths
            .Where(p => p != mainPath)
            .ToList();
        
        Debug.Log($"Main path length: {mainPath.Count}");
        Debug.Log($"Alternative paths: {alternativePaths.Count}");
        
        // Block alternative paths
        foreach (var path in alternativePaths)
        {
            // Find points that can be blocked (not start or goal)
            List<Vector2Int> blockablePpoints = path
                .Where(p => p != start && p != goal)
                .ToList();
                
            if (blockablePpoints.Count > 0)
            {
                // Choose a random point to block
                int randomIndex = Random.Range(0, blockablePpoints.Count);
                Vector2Int blockPoint = blockablePpoints[randomIndex];
                blockedPoints.Add(blockPoint);
                
            }
        }
        Debug.Log($"Blocked points: {blockedPoints.Count}");
        foreach (Vector2Int blockPoint in blockedPoints)
        {
            Debug.Log($"Blocked point: {blockPoint}");
        }
    }

    List<List<Vector2Int>> FindAllPaths(Vector2Int start, Vector2Int goal)
    {
        List<List<Vector2Int>> allPaths = new List<List<Vector2Int>>();
        Queue<List<Vector2Int>> queue = new Queue<List<Vector2Int>>();
        
        // Bắt đầu với đường đi chỉ có điểm start
        List<Vector2Int> initialPath = new List<Vector2Int> { start };
        queue.Enqueue(initialPath);
        
        while (queue.Count > 0)
        {
            List<Vector2Int> currentPath = queue.Dequeue();
            Vector2Int current = currentPath[currentPath.Count - 1];
            
            // If we've reached the goal, add the path to the result
            if (current == goal)
            {
                allPaths.Add(new List<Vector2Int>(currentPath));
                continue;
            }
            
            // Thử đi các hướng có thể
            foreach (Vector2Int dir in new Vector2Int[] {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1)})
            {
                Vector2Int next = current + dir;
                
                if (next.x >= 0 && next.x < height && next.y >= 0 && next.y < width &&
                    (map[next.x, next.y] == 1 || map[next.x, next.y] == 2 || // đường đi thẳng
                     (map[next.x, next.y] >= 3 && map[next.x, next.y] <= 15) || // các loại checkpoint
                     map[next.x, next.y] == 17) && // đích
                    !currentPath.Contains(next))
                {
                    List<Vector2Int> newPath = new List<Vector2Int>(currentPath) { next };
                    queue.Enqueue(newPath);
                }
            }
        }
    
        return allPaths;
    }

    private bool IsValidCell(int x, int y)
    {
        return x >= 0 && x < height && y >= 0 && y < width;
    }

    private (bool up, bool down, bool left, bool right) GetConnections(int x, int y)
    {
        bool up = IsValidCell(x - 1, y) && map[x - 1, y] != (int)RoadType.NoRoad;
        bool down = IsValidCell(x + 1, y) && map[x + 1, y] != (int)RoadType.NoRoad;
        bool left = IsValidCell(x, y - 1) && map[x, y - 1] != (int)RoadType.NoRoad;
        bool right = IsValidCell(x, y + 1) && map[x, y + 1] != (int)RoadType.NoRoad;

        // Xử lý đặc biệt cho các góc và biên
        if (x == 0) up = false;        // Biên trên
        if (x == height - 1) down = false;  // Biên dưới
        if (y == 0) left = false;      // Biên trái
        if (y == width - 1) right = false;  // Biên phải

        return (up, down, left, right);
    }

    private void IdentifyRoadTypes()
    {   
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                if (map[x, y] != (int)RoadType.NoRoad && 
                    map[x, y] != (int)RoadType.Start && 
                    map[x, y] != (int)RoadType.Goal)
                {
                    var (up, down, left, right) = GetConnections(x, y);
                    int connections = (up ? 1 : 0) + (down ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);

                    // Xử lý các biên
                    if (x == 0 || x == height - 1 || y == 0 || y == width - 1)
                    {
                        // Góc trên trái
                        if (x == 0 && y == 0)
                        {
                            if (down && right) map[x, y] = (int)RoadType.CornerLeftDown;
                            else if (down) map[x, y] = (int)RoadType.EndUp;
                            else if (right) map[x, y] = (int)RoadType.EndLeft;
                            else map[x, y] = (int)RoadType.NoRoad;
                        }
                        // Góc trên phải
                        else if (x == 0 && y == width - 1)
                        {
                            if (down && left) map[x, y] = (int)RoadType.CornerRightDown;
                            else if (down) map[x, y] = (int)RoadType.EndUp;
                            else if (left) map[x, y] = (int)RoadType.EndRight;
                            else map[x, y] = (int)RoadType.NoRoad;
                        }
                        // Góc dưới trái
                        else if (x == height - 1 && y == 0)
                        {
                            if (up && right) map[x, y] = (int)RoadType.CornerLeftUp;
                            else if (up) map[x, y] = (int)RoadType.EndDown;
                            else if (right) map[x, y] = (int)RoadType.EndLeft;
                            else map[x, y] = (int)RoadType.NoRoad;
                        }
                        // Góc dưới phải
                        else if (x == height - 1 && y == width - 1)
                        {
                            if (up && left) map[x, y] = (int)RoadType.CornerRightUp;
                            else if (up) map[x, y] = (int)RoadType.EndDown;
                            else if (left) map[x, y] = (int)RoadType.EndRight;
                            else map[x, y] = (int)RoadType.NoRoad;
                        }
                        // Biên trên
                        else if (x == 0)
                        {
                            if (down && left && right) map[x, y] = (int)RoadType.ForkUp;
                            else if (down && right) map[x, y] = (int)RoadType.CornerLeftDown;
                            else if (down && left) map[x, y] = (int)RoadType.CornerRightDown;
                            else if (left && right) map[x, y] = (int)RoadType.Horizontal;
                            else if (down) map[x, y] = (int)RoadType.EndUp;
                            else if (left) map[x, y] = (int)RoadType.EndRight;
                            else if (right) map[x, y] = (int)RoadType.EndLeft;
                            else map[x, y] = (int)RoadType.NoRoad;
                        }
                        // Biên dưới
                        else if (x == height - 1)
                        {
                            if (up && left && right) map[x, y] = (int)RoadType.ForkDown;
                            else if (up && right) map[x, y] = (int)RoadType.CornerLeftUp;
                            else if (up && left) map[x, y] = (int)RoadType.CornerRightUp;
                            else if (left && right) map[x, y] = (int)RoadType.Horizontal;
                            else if (up) map[x, y] = (int)RoadType.EndDown;
                            else if (left) map[x, y] = (int)RoadType.EndRight;
                            else if (right) map[x, y] = (int)RoadType.EndLeft;
                            else map[x, y] = (int)RoadType.NoRoad;
                        }
                        // Biên trái
                        else if (y == 0)
                        {
                            if (up && down && right) map[x, y] = (int)RoadType.ForkLeft;
                            else if (up && right) map[x, y] = (int)RoadType.CornerLeftUp;
                            else if (down && right) map[x, y] = (int)RoadType.CornerLeftDown;
                            else if (up && down) map[x, y] = (int)RoadType.Vertical;
                            else if (up) map[x, y] = (int)RoadType.EndDown;
                            else if (down) map[x, y] = (int)RoadType.EndUp;
                            else if (right) map[x, y] = (int)RoadType.EndLeft;
                            else map[x, y] = (int)RoadType.NoRoad;
                        }
                        // Biên phải
                        else if (y == width - 1)
                        {
                            if (up && down && left) map[x, y] = (int)RoadType.ForkRight;
                            else if (up && left) map[x, y] = (int)RoadType.CornerRightUp;
                            else if (down && left) map[x, y] = (int)RoadType.CornerRightDown;
                            else if (up && down) map[x, y] = (int)RoadType.Vertical;
                            else if (up) map[x, y] = (int)RoadType.EndDown;
                            else if (down) map[x, y] = (int)RoadType.EndUp;
                            else if (left) map[x, y] = (int)RoadType.EndRight;
                            else map[x, y] = (int)RoadType.NoRoad;
                        }
                    }
                    // Xử lý các ô bên trong
                    else switch (connections)
                    {
                        case 4:
                            map[x, y] = (int)RoadType.Intersection;
                            break;
                        case 3:
                            if (!up) map[x, y] = (int)RoadType.ForkUp;
                            else if (!down) map[x, y] = (int)RoadType.ForkDown;
                            else if (!left) map[x, y] = (int)RoadType.ForkLeft;
                            else map[x, y] = (int)RoadType.ForkRight;
                            break;
                        case 2:
                            if (up && down) map[x, y] = (int)RoadType.Vertical;
                            else if (left && right) map[x, y] = (int)RoadType.Horizontal;
                            else if (up && right) map[x, y] = (int)RoadType.CornerLeftUp;
                            else if (up && left) map[x, y] = (int)RoadType.CornerRightUp;
                            else if (down && right) map[x, y] = (int)RoadType.CornerLeftDown;
                            else if (down && left) map[x, y] = (int)RoadType.CornerRightDown;
                            break;
                        case 1:
                            if (up) map[x, y] = (int)RoadType.EndDown;
                            else if (down) map[x, y] = (int)RoadType.EndUp;
                            else if (left) map[x, y] = (int)RoadType.EndRight;
                            else map[x, y] = (int)RoadType.EndLeft;
                            break;
                        default:
                            map[x, y] = (int)RoadType.NoRoad;
                            break;
                    }
                }
            }
        }
    }

    void IdentifyPath(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == goal) break;

            foreach (Vector2Int dir in new Vector2Int[] {
                         new Vector2Int(1, 0), new Vector2Int(-1, 0),
                         new Vector2Int(0, 1), new Vector2Int(0, -1)})
            {
                Vector2Int next = current + dir;
                if (next.x >= 0 && next.x < height && next.y >= 0 && next.y < width &&
                    (map[next.x, next.y] == 1 || map[next.x, next.y] == 2 || // đường đi thẳng
                    (map[next.x, next.y] >= 3 && map[next.x, next.y] <= 15) || // các loại checkpoint
                    map[next.x, next.y] == 17) &&  !blockedPoints.Contains(next) && // đích && không chặn
                    !visited.Contains(next))
                {
                    queue.Enqueue(next);
                    visited.Add(next);
                    cameFrom[next] = current;
                }
            }
        }

        // reconstruct path từ goal ngược về start
        path.Clear();
        if (!cameFrom.ContainsKey(goal))
        {
            Debug.Log("Không tìm thấy đường!");
            ResetScene();
            return;
        }
        // if has path, play music
        AudioManager.Instance.PlayMusic("GamePlay Music for maths kids");

        Vector2Int node = goal;
        while (node != start)
        {
            path.Add(node);
            node = cameFrom[node];
        }
        path.Add(start);
        path.Reverse();

        Debug.Log($"Path length: {path.Count}");
        Debug.Log($"Path: {string.Join(", ", path)}");
    }

    
    void ShowDirectionQuestion(Vector2Int pos, Direction correctDirection, Action<bool> onAnswered)
    {
        // Animate map area moving up
        mapArea.GetComponent<RectTransform>()
            .DOAnchorPosY(raisedMapY, animationDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
        questionPanel.SetActive(true);
                answerPanel.SetActive(true);

                questionText.gameObject.GetComponent<TextMeshProUGUI>().text = 
                    $"Để đi đến đích, bạn phải đi hướng nào?";

        Direction[] options = new Direction[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        for (int i = 0; i < answerButtons.Count; i++)
        {
            Direction dir = options[i];
            string text = dir == Direction.Up? "Bên trên" : dir == Direction.Down? "Bên dưới" : dir == Direction.Left? "Bên trái" : "Bên phải";
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = text;
                    int index = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() =>
            {
                bool isCorrect = dir == correctDirection;
                        // Hide panels first
                questionPanel.SetActive(false);
                answerPanel.SetActive(false);
                        
                // Animate map area moving back down
                mapArea.GetComponent<RectTransform>()
                    .DOAnchorPosY(originalMapY, animationDuration)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() => {
                        onAnswered(isCorrect);
                        //isWaitingForAnswer = false;
                    });
            });
        }
            });
    }

    void OnAnswerSubmitted(bool isCorrect)
    {
        if (isCorrect)
        {   
            AudioManager.Instance.PlaySound("Object Done Right place");
            Debug.Log("Đúng hướng, tiếp tục!");
            isWaitingForAnswer = false;
          
        }
        else
        {
            Debug.Log("Sai hướng! Bạn phải chọn đúng mới được đi tiếp.");
            AudioManager.Instance.PlaySound("lion no no no no");
            mapArea.GetComponent<RectTransform>()
            .DOAnchorPosY(raisedMapY, animationDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                questionPanel.SetActive(true);
                answerPanel.SetActive(true);
            });
        
            isWaitingForAnswer = true;
         
        }
    }

    private void InitializeCheckpoints()
    {   
        // Filter out checkpoints from path (only take corners, forks, intersections)
        List<Vector2Int> availableCheckpoints = path.Where(pos => 
            map[pos.x, pos.y] >= 3 && map[pos.x, pos.y] <= 15).ToList();
        
        selectedCheckpoints = availableCheckpoints
            .OrderBy(x => Random.value)
            .Take(targetCheckpoints)
            .OrderBy(x => path.IndexOf(x))
            .ToList();

        if (!selectedCheckpoints.Contains(goal))
        {
            selectedCheckpoints.Add(goal);
        }
        Debug.Log($"Selected checkpoints: {string.Join(", ", selectedCheckpoints)}");
        currentCheckpointIndex = -1;
        currentPathIndex = 0;
        isGeneratingPath = false;
    }

    IEnumerator HandleCheckpointQuestion(Vector2Int checkpoint, int checkpointIndex)
    {
        if (checkpointIndex >= path.Count - 1) yield break;

        isWaitingForAnswer = true;

        Vector2Int nextPos = path[checkpointIndex + 1];
        Vector2Int direction = nextPos - checkpoint;
        Direction correctDirection = VectorToDirection(direction);
        Debug.Log($"Checkpoint: {checkpoint}, Next position: {nextPos}, Correct direction: {correctDirection}");
        ShowDirectionQuestion(checkpoint, correctDirection, OnAnswerSubmitted);

        yield return new WaitUntil(() => !isWaitingForAnswer);
    }

    private GameObject playerObject; // Reference to the instantiated player
    [SerializeField] private float playerMoveSpeed = 0.80f; // Speed of player movement

    IEnumerator GeneratePathToNextCheckpoint()
    {
        if (isGeneratingPath) yield break;
        isGeneratingPath = true;

        // Xác định checkpoint tiếp theo
        currentCheckpointIndex++;
        if (currentCheckpointIndex >= selectedCheckpoints.Count)
        {
            isGeneratingPath = false;
            yield break;
        }

        Vector2Int nextCheckpoint = selectedCheckpoints[currentCheckpointIndex];
        int targetIndex = path.IndexOf(nextCheckpoint);

        Debug.Log($"Generating path from index {currentPathIndex} to {targetIndex}");

        // Move player along the path to the checkpoint
        for (int i = currentPathIndex; i <= targetIndex; i++)
        {
            Vector2Int pos = path[i];
            
            // Move player to this position
            if (playerObject != null)
            {
                // Calculate the target position for the player
                Vector2 targetPosition = new Vector2(50 + pos.y * 100, -25 - pos.x * 100);
                // Move the player to the target position
                playerObject.GetComponent<RectTransform>().DOAnchorPos(targetPosition, playerMoveSpeed)
                    .SetEase(Ease.Linear);
                
                yield return new WaitForSeconds(playerMoveSpeed);
            }
        }
        // If the next checkpoint is a corner, fork or intersection, show the question
        if (map[nextCheckpoint.x, nextCheckpoint.y] >= 3 && map[nextCheckpoint.x, nextCheckpoint.y] <= 15)
        {
            yield return StartCoroutine(HandleCheckpointQuestion(nextCheckpoint, targetIndex));
            // Sau khi trả lời đúng, tiếp tục gen path đến checkpoint tiếp theo
            if (!isWaitingForAnswer)
            {
                currentPathIndex = targetIndex + 1;
                isGeneratingPath = false;
                StartCoroutine(GeneratePathToNextCheckpoint());
            }
        }
        if (goal.x == nextCheckpoint.x && goal.y == nextCheckpoint.y)
        {
            Debug.Log("Đã đến đích!");
            nextButton.GameObject().SetActive(true);
            AudioManager.Instance.PlaySound("Task complete whistel sound");
            // Instantiate particle effect
            SpawnParticles();
            AudioManager.Instance.PlaySound("Kids Cheering And Laughing");
            AudioManager.Instance.PlayMusic("Task Complete enjoy music");
            isWaitingForAnswer = false;
            currentPathIndex = targetIndex + 1;
            isGeneratingPath = false;
        }
    }

    public void SpawnParticles()
    {
        foreach (var prefab in particlePrefab)
        {
            GameObject particle = Instantiate(prefab, spawnParticleObject.transform.position, spawnParticleObject.transform.rotation);
            // Set trigger to destroy particle after animation
            particle.GetComponent<ParticleSystem>().trigger.SetCollider(0, ground.GetComponent<BoxCollider2D>());
            _particles.Add(particle);
        }
    }
    public void GeneratePath()
    {   
        Debug.Log("Starting game...");
        AudioManager.Instance.PlaySound("car_straring_sound");
        if (currentPathCoroutine != null)
        {
            StopCoroutine(currentPathCoroutine);
        }


        InitializeCheckpoints();
        currentCheckpointIndex = -1; // Reset về -1 để bắt đầu từ checkpoint đầu tiên
        currentPathIndex = 0;
        isGeneratingPath = false;
        isWaitingForAnswer = false;
        currentPathCoroutine = StartCoroutine(GeneratePathToNextCheckpoint());
    }
    
    bool HasNearbyPath(int x, int y)
    {
        return (map[x - 1, y] == 1 || map[x + 1, y] == 1 || map[x, y - 1] == 1 || map[x, y + 1] == 1);
    }

    void PlaceDecorations()
    {
        for (int x = 1; x < height - 1; x++)
        {
            for (int y = 1; y < width - 1; y++)
            {
                if (map[x, y] == 0 && Random.value < 0.15f && HasNearbyPath(x, y))
                {
                    map[x, y] = 0;
                }
            }
        }
    }
    void PrintMap()
    {
        string debugMap = "\nGenerated Map:\n";
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                debugMap += GetSymbol(map[i, j]) + " ";
            }
            debugMap += "\n";
        }
        Debug.Log(debugMap);
    }

    private char GetSymbol(int value)
    {
        return ((RoadType)value) switch
        {
            RoadType.NoRoad => '.',
            RoadType.Horizontal => '─',
            RoadType.Vertical => '│',
            RoadType.Intersection => '┼',
            RoadType.CornerLeftDown => '┌',
            RoadType.CornerLeftUp => '┘',
            RoadType.CornerRightDown => '┌',
            RoadType.CornerRightUp => '└',
            RoadType.EndUp => '╵',
            RoadType.EndDown => '╷',
            RoadType.EndLeft => '╴',
            RoadType.EndRight => '╶',
            RoadType.ForkUp => '┴',
            RoadType.ForkDown => '┬',
            RoadType.ForkLeft => '┤',
            RoadType.ForkRight => '├',
            RoadType.Start => 'S',
            RoadType.Goal => 'G',
            _ => '?'
        };
    }

    void RenderMap()
    {   
        // Render map: Layer 1: Road, Layer 2: Decoration, Layer 3: Player
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 position = new Vector3(j, 0, i);
                var mapLayerTransform = mapLayer.transform;

                GameObject prefab = map[i, j] switch
                {  
                    (int)RoadType.NoRoad => noRoadPrefab,
                    (int)RoadType.Horizontal => roadHorizontalPrefab,
                    (int)RoadType.Vertical => roadVerticalPrefab,
                    (int)RoadType.Intersection => roadIntersectionPrefab,
                    (int)RoadType.CornerLeftDown => cornerLeftDownPrefab,
                    (int)RoadType.CornerLeftUp => cornerLeftUpPrefab,
                    (int)RoadType.CornerRightDown => cornerRightDownPrefab,
                    (int)RoadType.CornerRightUp => cornerRightUpPrefab,
                    (int)RoadType.EndUp => endUpPrefab,
                    (int)RoadType.EndDown => endDownPrefab,
                    (int)RoadType.EndLeft => endLeftPrefab,
                    (int)RoadType.EndRight => endRightPrefab,
                    (int)RoadType.ForkUp => forkUpPrefab,
                    (int)RoadType.ForkDown => forkDownPrefab,
                    (int)RoadType.ForkLeft => forkLeftPrefab,
                    (int)RoadType.ForkRight => forkRightPrefab,
                    _ => null
                };

                GameObject spawnedObject = Instantiate(prefab, position, Quaternion.identity, mapLayerTransform);
                spawnedObjects[new Vector2Int(i, j)] = spawnedObject;
                // if pos was not goal or start pos , render decoration
                if (i != goal.x && j != goal.y && i != start.x && j != start.y) {
                    RenderDecoration(i, j);
                }
            }
        }
       
        // Render start and goal
        GameObject startObject  = Instantiate(startPrefab, Vector3.zero, Quaternion.identity, decorationLayer.transform);
        startObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(50+start.y*100, -50-start.x*100);
        GameObject goalObject = Instantiate(goalPrefabs, Vector3.zero, Quaternion.identity, decorationLayer.transform);
        goalObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(50+goal.y*100, -50-goal.x*100);
        

        // Render blocked points
        foreach (Vector2Int blockPoint in blockedPoints)
        {   
             GameObject block = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity, decorationLayer.transform);
             block.GetComponent<RectTransform>().anchoredPosition = new Vector2(50+blockPoint.y*100, -50-blockPoint.x*100);
        }
          // Create player if it doesn't exist
        if (playerObject == null && playerPrefab != null)
        {
            playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, playerLayer.transform);
            // Set initial position to start position
            playerObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(50 + start.y * 100, -25 - start.x * 100);
        }
    
    }
    void RenderDecoration(int x, int y)
    {   
        if (map[x, y] == (int)RoadType.NoRoad) {
            if (Random.value < 0.90f) {
            GameObject houseDecoration = Instantiate(house_decorationPrefabs[Random.Range(0, house_decorationPrefabs.Count)], Vector3.zero, Quaternion.identity, decorationLayer.transform);
            houseDecoration.GetComponent<RectTransform>().anchoredPosition = new Vector2(50+y*100, -25-x*100);
            }
        }

        // Render tree decoration, position is x*50, y*-50, close the road rate is 0.75
        // if the road is horizontal 50+y*100, -25(+or- 25)-x*100) 
        if (map[x, y] == (int)RoadType.Horizontal) {
            if (Random.value < 0.90f) {
                int random = Random.Range(0, 2);
               
                GameObject treeDecoration = Instantiate(tree_decorationPrefabs[Random.Range(0, tree_decorationPrefabs.Count)], Vector3.zero, Quaternion.identity, decorationLayer.transform);
                 if (random == 0) treeDecoration.GetComponent<RectTransform>().anchoredPosition = new Vector2(50+y*100,-50-x*100);
                 else treeDecoration.GetComponent<RectTransform>().anchoredPosition = new Vector2(50+y*100, 15-x*100); // -50 or 15 how to choose?
            }
        }
        // if the road is vertical 50(+or-35)+y*100, -25-x*100)
        if (map[x, y] == (int)RoadType.Vertical) {
            if (Random.value < 0.90f) {
                int random = Random.Range(0, 2);
                GameObject treeDecoration = Instantiate(tree_decorationPrefabs[Random.Range(0, tree_decorationPrefabs.Count)], Vector3.zero, Quaternion.identity, decorationLayer.transform);
                if (random == 0) treeDecoration.GetComponent<RectTransform>().anchoredPosition = new Vector2(15+y*100,-25-x*100);
                else treeDecoration.GetComponent<RectTransform>().anchoredPosition = new Vector2(85+y*100, -25-x*100); // 80 or -25 how to choose?
            }
        }
        // sign decoration rendering rate is 0.25
        if (!blockedPoints.Contains(new Vector2Int(x, y))) {
            // if fork or corner about left then 75+y*100, -25-x*100
            if (map[x, y] == (int)RoadType.ForkLeft || map[x, y] == (int)RoadType.CornerLeftDown || map[x, y] == (int)RoadType.CornerLeftUp) {
                if (Random.value < 0.25f) {
                    GameObject signageDecorationObject = Instantiate(sign_decorationPrefabs, Vector3.zero, Quaternion.identity, decorationLayer.transform);
                    signageDecorationObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(25+y*100, -25-x*100);
                }
            }
            // if fork or corner about right then 25+y*100, -25-x*100
            if (map[x, y] == (int)RoadType.ForkRight || map[x, y] == (int)RoadType.CornerRightDown || map[x, y] == (int)RoadType.CornerRightUp) {
                if (Random.value < 0.25f) {
                    GameObject signageDecorationObject = Instantiate(sign_decorationPrefabs, Vector3.zero, Quaternion.identity, decorationLayer.transform);
                    signageDecorationObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(75+y*100, -25-x*100);
                }
            }
            // if fork about up then 50+y*100, -75-x*100
            if (map[x, y] == (int)RoadType.ForkUp) {
                if (Random.value <0.25f) {
                    GameObject signageDecorationObject = Instantiate(sign_decorationPrefabs, Vector3.zero, Quaternion.identity, decorationLayer.transform);
                    signageDecorationObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(50+y*100, 15-x*100);
                }
            }
            // if fork about down then 50+y*100, 25-x*100
            if (map[x, y] == (int)RoadType.ForkDown) {
                if (Random.value < 0.25f) {
                    GameObject signageDecorationObject = Instantiate(sign_decorationPrefabs, Vector3.zero, Quaternion.identity, decorationLayer.transform);
                    signageDecorationObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(50+y*100, -50-x*100);
                }
            }
        }
            
    }


    public void ResetScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public override void CloseGame()
    {   
        nextButton.GameObject().SetActive(false);
        // Dọn dẹp resources, reset state
        if (currentPathCoroutine != null)
        {
            StopCoroutine(currentPathCoroutine);
        }
        
        // Clear các object đã spawn
        foreach (var obj in spawnedObjects.Values)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();
        
        // Destroy player
        if (playerObject != null)
        {
            Destroy(playerObject);
            playerObject = null;
        }
        
        // Reset các biến
        path.Clear();
        selectedCheckpoints.Clear();
        isWaitingForAnswer = false;
        isGeneratingPath = false;
        currentCheckpointIndex = -1;
        currentPathIndex = 0;
    }

    public override void SetSelectedAnswer(string answer, GameObject answerBox)
    {
        throw new NotImplementedException();
    }
    public override void EndGame()
    {
        Debug.Log("endGame");
    }
}   

