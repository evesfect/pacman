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
    }
}