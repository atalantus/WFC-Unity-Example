using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelGeneration
{
    /// <summary>
    /// Controls the grid
    /// </summary>
    public class Grid : MonoBehaviour
    {
        /// <summary>
        /// Width of grid
        /// </summary>
        public int width;

        /// <summary>
        /// Height of grid
        /// </summary>
        public int height;

        /// <summary>
        /// Cell prefab
        /// </summary>
        public GameObject cellPrefab;

        /// <summary>
        /// Cells matrix ([width, height])
        /// </summary>
        public Cell[,] cells;

        /// <summary>
        /// <see cref="LevelGenerator"/>
        /// </summary>
        private LevelGenerator _levelGenerator;

        /// <summary>
        /// RNG seed
        /// </summary>
        public int seed;

        private void Awake()
        {
            _levelGenerator = LevelGenerator.Instance;

            GenerateGrid();

            // Wave-function-collapse algorithm
            _levelGenerator.GenerateLevelWFC(ref cells, seed != -1 ? seed : Environment.TickCount);
        }

        /// <summary>
        /// Generates the two-dimensional grid
        /// </summary>
        public void GenerateGrid()
        {
            if (width > 0 && height > 0)
            {
                // Generate grid
                cells = new Cell[width, height];

                var scale = cellPrefab.transform.localScale;
                var origin = transform.position;
                var topLeft = new Vector3(origin.x - width / 2f + scale.x / 2, origin.y,
                    origin.z + height / 2f - scale.z / 2);
                var curPos = topLeft;

                for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    curPos = new Vector3((topLeft.x + i % width) * scale.x, curPos.y,
                        (topLeft.z - j % height) * scale.z);

                    // Create new cell
                    var cellObj = Instantiate(cellPrefab, curPos, Quaternion.identity, gameObject.transform);
                    cellObj.name = $"({i}, {j})";
                    var cell = cellObj.GetComponent<Cell>();
                    cells[i, j] = cell;
                }

                // Assign neighbours for every cell
                for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    var cell = cells[i, j];
                    for (int k = 0; k < 4; k++)
                    {
                        int x = -1, y = -1;

                        switch (k)
                        {
                            case 0:
                                x = i;
                                y = j + 1;
                                break;
                            case 1:
                                x = i + 1;
                                y = j;
                                break;
                            case 2:
                                x = i;
                                y = j - 1;
                                break;
                            case 3:
                                x = i - 1;
                                y = j;
                                break;
                        }

                        if (x < 0 || y < 0 || x > width - 1 || y > height - 1)
                        {
                            // Outside of grid`s dimensions
                            cell.neighbourCells[k] = null;
                        }
                        else
                        {
                            cell.neighbourCells[k] = cells[x, y];
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Impossible grid dimensions!", gameObject);
            }
        }

        /// <summary>
        /// Destroys the current grid
        /// </summary>
        public void RemoveGrid()
        {
            foreach (Transform child in gameObject.transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Checks if the grid is valid
        /// </summary>
        /// <returns>true if the grid is valid</returns>
        public bool CheckGrid()
        {
            var notMatchingCells = _levelGenerator.CheckGeneratedLevel(ref cells);

            return notMatchingCells.Count == 0;
        }
    }
}