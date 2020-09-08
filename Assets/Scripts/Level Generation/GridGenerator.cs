using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelGeneration
{
    /// <summary>
    /// Controls the grid.
    /// </summary>
    public class GridGenerator : MonoBehaviour
    {
        /// <summary>
        /// Width of grid.
        /// </summary>
        public int width;

        /// <summary>
        /// Height of grid.
        /// </summary>
        public int height;

        /// <summary>
        /// Cell prefab.
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
            if (width <= 0 || height <= 0)
            {
                Debug.LogError("Impossible grid dimensions!", gameObject);
                return;
            }

            // Generate grid
            cells = new Cell[width, height];

            var scale = cellPrefab.transform.localScale;
            var origin = transform.position;
            var bottomLeft = new Vector3(
                origin.x - width * scale.x / 2f + scale.x / 2,
                origin.y,
                origin.z - height * scale.z / 2f + scale.z / 2
            );

            for (var x = 0; x < width; x++)
            for (var z = 0; z < height; z++)
            {
                var curPos = new Vector3(bottomLeft.x + x * scale.x, bottomLeft.y, bottomLeft.z + z * scale.z);

                // Create new cell
                var cellObj = Instantiate(cellPrefab, curPos, Quaternion.identity, gameObject.transform);
                cellObj.name = $"({x}, {z})";
                var cell = cellObj.GetComponent<Cell>();
                cells[x, z] = cell;

                /*
                 * assign neighbours
                 */

                if (x > 0)
                {
                    var leftCell = cells[x - 1, z];
                    cell.neighbours[3] = leftCell;
                    leftCell.neighbours[1] = cell;
                }

                if (z > 0)
                {
                    var backCell = cells[x, z - 1];
                    cell.neighbours[2] = backCell;
                    backCell.neighbours[0] = cell;
                }
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