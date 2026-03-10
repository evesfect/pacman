using System.Collections.Generic;
using CMP.Scripts.Helper;
using CMP.Scripts.AiStates;
using UnityEngine;

namespace CMP.Scripts
{
    public enum GameMode
    {
        Scatter,
        Chase,
        GameOver,
    }

    public class GameManager : MonoBehaviour
    {
        private Pacman _pacman;
        private InputManager _inputManager;
        private GameMode _gameMode = GameMode.Scatter;
        private readonly List<Ghost> _ghosts = new();

        private void Start()
        {
            var gridData = AssetDatabase.Instance.GridData;

            _inputManager = Instantiate(AssetDatabase.Instance.InputManagerPrefab);

            _pacman = Instantiate(AssetDatabase.Instance.PacmanPrefab);
            var movementController = _pacman.gameObject.GetComponent<GridMovementController>();
            if (movementController == null) { movementController = _pacman.gameObject.AddComponent<GridMovementController>(); }
            _pacman.Initialize(movementController, _inputManager, gridData);

            SpawnGhosts(gridData);
            
            CreateBackground(gridData);
            AdjustCamera(gridData);
        }

        private void CreateBackground(GridData gridData)
        {
            var targetTexture = MapTextureGenerator.Generate(gridData, AssetDatabase.Instance.MapVisualSettings);
            var textureObject = new GameObject("MapTexture");
            textureObject.transform.position = new Vector3(-0.5f, -0.5f, 0f);
            var targetSprite = Sprite.Create(targetTexture, new Rect(0f, 0f, targetTexture.width, targetTexture.height),
                Vector2.zero,AssetDatabase.Instance.MapVisualSettings.pixelsPerCell);
            var spriteRenderer = textureObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = targetSprite;
            spriteRenderer.sortingOrder = -1;
        }
        
        private void AdjustCamera(GridData gridData)
        {
            var mainCamera = Camera.main;
            mainCamera.orthographicSize = gridData.Height + GameSettings.CameraPadding;
            mainCamera.transform.position = new Vector3(gridData.Width / 2f - 0.5f, 0f, -10f);
        }

        private void SpawnGhosts(GridData gridData)
        {
            // Get all available spawn cells in the ghost house
            List<Vector2Int> spawnCells = gridData.GetCoordsOfCellType(CellType.AiSpawnZone);
            
            for (int i = 0; i < GameSettings.AiCharacterCount; i++)
            {
                Ghost newGhost = Instantiate(AssetDatabase.Instance.Ghost);
                
                // If we have fewer spawn points than ghosts, default to the first spawn point to avoid a crash
                Vector2Int startCell = i < spawnCells.Count ? spawnCells[i] : spawnCells[0];

                // Initialize the Ghost and its Blackboard
                newGhost.Initialize(gridData, _pacman, i, startCell);

                // Force the ghost into its starting state
                newGhost.ChangeState(new InHouseState(newGhost.Blackboard), CMP.Scripts.GhostState.InHouse);
                
                _ghosts.Add(newGhost);
            }
        }

        private void Update()
        {
            if (_gameMode == GameMode.GameOver) return;

            Vector2Int pacmanCell = _pacman.GetComponent<GridMovementController>().CurrentCell;

            // 1. Check Game Over (Catch) Condition
            foreach (var ghost in _ghosts)
            {
                float distance = Vector3.Distance(ghost.transform.position, _pacman.transform.position);
                if (distance <= GameSettings.CatchDistance)
                {
                    TriggerGameOver();
                    return;
                }
            }

            // 2. Check Line of Sight (LOS) Condition if we are in Scatter Mode
            if (_gameMode == GameMode.Scatter)
            {
                bool losTriggered = false;
                
                foreach (var ghost in _ghosts)
                {
                    // Only active, wandering ghosts can trigger the chase
                    if (ghost.Blackboard.CurrentStateEnum != CMP.Scripts.GhostState.Scatter) continue;

                    // The case study mentions "belli bir mesafeden daha yakınsa". We define 10 cells as this max sight range.
                    if (CheckLineOfSight(ghost, pacmanCell, 10))
                    {
                        losTriggered = true;
                        break;
                    }
                }

                if (losTriggered)
                {
                    Debug.Log("Line of Sight triggered! All active ghosts entering Chase Mode!");
                    _gameMode = GameMode.Chase;
                    
                    foreach (var ghost in _ghosts)
                    {
                        // Convert all scattering ghosts to chasing ghosts
                        if (ghost.Blackboard.CurrentStateEnum == CMP.Scripts.GhostState.Scatter)
                        {
                            ghost.ChangeState(new ChaseState(ghost.Blackboard), CMP.Scripts.GhostState.Chase);
                        }
                    }
                }
            }
        }

        private bool CheckLineOfSight(Ghost ghost, Vector2Int pacmanCell, int maxRange)
        {
            Direction facingDir = ghost.Blackboard.CurrentDirection;
            if (facingDir == Direction.None) return false;

            Vector2Int stepDir = facingDir.ToVector2Int();
            Vector2Int currentCell = ghost.GetComponent<GridMovementController>().CurrentCell;
            var gridData = AssetDatabase.Instance.GridData;

            // Cast a "ray" cell by cell in the direction the ghost is facing
            for (int i = 1; i <= maxRange; i++)
            {
                Vector2Int checkCell = currentCell + stepDir * i;

                if (!gridData.GetInBounds(checkCell) || gridData.GetCellAt(checkCell) == CellType.Wall)
                {
                    return false; // Vision is blocked by a wall
                }

                if (checkCell == pacmanCell)
                {
                    return true; // We see Pac-Man!
                }
            }
            return false;
        }

        private void TriggerGameOver()
        {
            Debug.Log("Game Over! Pac-Man was caught.");
            _gameMode = GameMode.GameOver;
            
            // Stop movement components so everything freezes
            _pacman.GetComponent<GridMovementController>().StopMovement();
            foreach (var ghost in _ghosts)
            {
                ghost.GetComponent<GridMovementController>().StopMovement();
            }

            // Disable brain scripts so their update() stops
            _pacman.enabled = false;
            foreach (var ghost in _ghosts)
            {
                ghost.enabled = false;
            }

            _pacman.PlayFailAnimation();
        }
        private void OnDrawGizmos()
        {
            if (_ghosts == null || AssetDatabase.Instance == null || AssetDatabase.Instance.GridData == null) return;

            var gridData = AssetDatabase.Instance.GridData;
            int maxRange = 10; // Must match the range used in CheckLineOfSight

            foreach (var ghost in _ghosts)
            {
                if (ghost.Blackboard == null) continue;
                
                Direction facingDir = ghost.Blackboard.CurrentDirection;
                if (facingDir == Direction.None) continue;

                Vector2Int stepDir = facingDir.ToVector2Int();
                Vector2Int currentCell = ghost.GetComponent<GridMovementController>().CurrentCell;
                
                // Determine Gizmo color based on state
                Gizmos.color = ghost.Blackboard.CurrentStateEnum == CMP.Scripts.GhostState.Chase ? Color.red : Color.yellow;
                
                Vector3 startPos = ghost.transform.position;
                Vector3 endPos = startPos;

                // Cast the visual ray exactly how CheckLineOfSight does it
                for (int i = 1; i <= maxRange; i++)
                {
                    Vector2Int checkCell = currentCell + stepDir * i;
                
                    if (!gridData.GetInBounds(checkCell) || gridData.GetCellAt(checkCell) == CellType.Wall)
                    {
                        // Hit a wall, stop drawing the line here
                        break;
                    }
                    endPos = new Vector3(checkCell.x, checkCell.y, startPos.z);
                }

                Gizmos.DrawLine(startPos, endPos);
            }
        }
    }
}