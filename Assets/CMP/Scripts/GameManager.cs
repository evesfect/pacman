using System.Collections.Generic;
using CMP.Scripts.Helper;
using CMP.Scripts.AiStates;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public AssetDatabase AssetDatabase;
        public GameSettingsData GameSettingsData;

        public GameObject GameOverPanel;
        private Pacman _pacman;
        private InputManager _inputManager;
        private GameMode _gameMode = GameMode.Scatter;
        private readonly List<Ghost> _ghosts = new();

        private void Awake()
        {
            GameSettings.Initialize(GameSettingsData);
        }

        private void Start()
        {
            Application.targetFrameRate = 60;

            var gridData = AssetDatabase.GridData;
            CreateBackground(gridData);
            AdjustCamera(gridData);

            _inputManager = Instantiate(AssetDatabase.InputManagerPrefab);

            _pacman = Instantiate(AssetDatabase.PacmanPrefab);
            var movementController = _pacman.gameObject.GetComponent<GridMovementController>();
            if (movementController == null) { movementController = _pacman.gameObject.AddComponent<GridMovementController>(); }
            _pacman.Initialize(movementController, _inputManager, gridData);

            SpawnGhosts(gridData);
            Ghost.OnPacmanSpotted += HandlePacmanSpotted;

            if (GameOverPanel != null) { GameOverPanel.SetActive(false);}
        }

        private void OnDestroy()
        {
            Ghost.OnPacmanSpotted -= HandlePacmanSpotted;
        }

        private void Update() 
        {
            if (_gameMode == GameMode.GameOver)
            {
                return;
            }

            // Check if pacman is caught
            float catchSqrDistance = GameSettings.CatchDistance * GameSettings.CatchDistance;

            foreach (var ghost in _ghosts)
            {
                float sqrDistance = (ghost.transform.position - _pacman.transform.position).sqrMagnitude;
                if (sqrDistance <= catchSqrDistance)
                {
                    TriggerGameOver();
                    return;
                }
            }
        }

        private void HandlePacmanSpotted()
        {
            if (_gameMode == GameMode.Scatter)
            {
                _gameMode = GameMode.Chase;
                foreach (var ghost in _ghosts)
                {
                    if (ghost.Blackboard.CurrentStateEnum == CMP.Scripts.GhostState.Scatter)
                    {
                        ghost.ChangeState(new ChaseState(ghost.Blackboard), CMP.Scripts.GhostState.Chase);
                    }
                }
            }
        }

        private void SpawnGhosts(GridData gridData)
        {
            List<Vector2Int> spawnCells = gridData.GetCoordsOfCellType(CellType.AiSpawnZone);
            
            for (int i = 0; i < GameSettings.AiCharacterCount; i++)
            {
                Ghost newGhost = Instantiate(AssetDatabase.Ghost);
                
                Vector2Int startCell = i < spawnCells.Count ? spawnCells[i] : spawnCells[0]; // if there isn't enough spaces default to index 0

                newGhost.Initialize(gridData, _pacman, i, startCell);
                newGhost.ChangeState(new InHouseState(newGhost.Blackboard), CMP.Scripts.GhostState.InHouse); // force the start state
                _ghosts.Add(newGhost);
            }
        }

        private void CreateBackground(GridData gridData)
        {
            var targetTexture = MapTextureGenerator.Generate(gridData, AssetDatabase.MapVisualSettings);
            var textureObject = new GameObject("MapTexture");
            textureObject.transform.position = new Vector3(-0.5f, -0.5f, 0f);
            var targetSprite = Sprite.Create(targetTexture, new Rect(0f, 0f, targetTexture.width, targetTexture.height),
                Vector2.zero,AssetDatabase.MapVisualSettings.pixelsPerCell);
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

        private void TriggerGameOver()
        {
            _gameMode = GameMode.GameOver;
            
            _pacman.MovementController.StopMovement();
            _pacman.enabled = false;

            foreach (var ghost in _ghosts)
            {
                ghost.Blackboard.MovementController.StopMovement();
                ghost.enabled = false;
            }

            _pacman.PlayFailAnimation();

            if (_inputManager != null)
            {
                _inputManager.gameObject.SetActive(false);
            }

            if (GameOverPanel != null)
            {
                GameOverPanel.SetActive(true);
            }
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}