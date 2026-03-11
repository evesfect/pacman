using System.Collections.Generic;
using CMP.Scripts.Helper;
using CMP.Scripts.AiStates;
using CMP.Scripts.Shaders;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CMP.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public AssetDatabase AssetDatabase;
        public GameSettingsData GameSettingsData;

        public GameObject GameOverPanel;
        private Pacman _pacman;
        private InputManager _inputManager;
        public static GameMode CurrentGameMode { get; private set; } = GameMode.Scatter;
        private readonly List<Ghost> _ghosts = new();

        private void Awake()
        {
            CurrentGameMode = GameMode.Scatter;
            GameSettings.Initialize(GameSettingsData);
        }

        private void Start()
        {
            Application.targetFrameRate = 90;

            var gridData = AssetDatabase.GridData;
            CreateBackground(gridData);
            AdjustCamera(gridData);

            _inputManager = Instantiate(AssetDatabase.InputManagerPrefab);

            _pacman = Instantiate(AssetDatabase.PacmanPrefab);
            var movementController = _pacman.gameObject.GetComponent<GridMovementController>();
            if (movementController == null) { movementController = _pacman.gameObject.AddComponent<GridMovementController>(); }
            _pacman.Initialize(movementController, _inputManager, gridData);
            
            var pacmanDissolve = _pacman.GetComponent<DissolveController>();
            if (pacmanDissolve != null) {pacmanDissolve.Appear();}

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
            if (CurrentGameMode == GameMode.GameOver)
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
            if (CurrentGameMode == GameMode.Scatter)
            {
                CurrentGameMode = GameMode.Chase;
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
                
                var ghostDissolve = newGhost.GetComponent<DissolveController>();
                if (ghostDissolve != null) { ghostDissolve.Appear(); }
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
            CurrentGameMode = GameMode.GameOver;
            
            StartCoroutine(GameOverRoutine());
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private System.Collections.IEnumerator GameOverRoutine()
        {
            // stop movement and input, play pacman fail anim
            _pacman.MovementController.StopMovement();
            _pacman.enabled = false;
            _pacman.PlayFailAnimation();

            foreach (var ghost in _ghosts)
            {
                ghost.Blackboard.MovementController.StopMovement();
                ghost.enabled = false;
            }

            if (_inputManager != null)
            {
                _inputManager.gameObject.SetActive(false);
            }

            float waitTime = 1f;
            yield return new WaitForSeconds(waitTime);

            var pacmanDissolve = _pacman.GetComponent<DissolveController>();
            if (pacmanDissolve != null)
            {
                pacmanDissolve.Dissolve();
            }

            yield return new WaitForSeconds(waitTime);

            foreach (var ghost in _ghosts)
            {
                var ghostDissolve = ghost.GetComponent<DissolveController>();
                if (ghostDissolve != null)
                {
                    ghostDissolve.Dissolve();
                }
            }

            if (GameOverPanel != null)
            {
                GameOverPanel.SetActive(true);
                var gameOverPanelDissolve = GameOverPanel.GetComponent<DissolveController>();
                if(gameOverPanelDissolve != null) { gameOverPanelDissolve.Appear();}
            }
        }
    }
}