using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace LevelGeneration
{
    /// <summary>
    /// Generates level using the wave-function-collapse algorithm
    /// </summary>
    public class LevelGenerator : GridGenerator
    {
        /// <summary>
        /// The modules
        /// </summary>
        public List<Module> modules;

        public Module startModule;

        public Module goalModule;

        /// <summary>
        /// Stores the cells in a heap having the closest cell to being solved as first element
        /// </summary>
        public Heap<Cell> OrderedCells;

        /// <summary>
        /// RNG seed
        /// </summary>
        public int seed;

        private void Start()
        {
            // Wave-function-collapse algorithm
            GenerateLevel();
        }

        /// <summary>
        /// Wave-function-collapse algorithm
        /// </summary>
        public void GenerateLevel()
        {
            RemoveGrid();
            GenerateGrid(this);

            var finalSeed = seed != -1 ? seed : Environment.TickCount;

            // Set RNG seed
            Random.InitState(finalSeed);

            // Instantiate cells heap
            OrderedCells = new Heap<Cell>(cells.GetLength(0) * cells.GetLength(1));

            for (var i = 0; i < cells.GetLength(0); i++)
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                OrderedCells.Add(cells[i, j]);
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Make sure the level fits our initial constraints
            ApplyInitialConstraints();

            // Wave-function-collapse Algorithm
            while (true)
            {
                // Remove finished cells from heap
                while (OrderedCells.Count > 0)
                {
                    var cell = OrderedCells.GetFirst();

                    if (cell.possibleModules.Count == 1)
                    {
                        if (cell.isCellSet) OrderedCells.RemoveFirst();
                        else cell.SetModule(cell.possibleModules[0]);
                    }
                    else
                    {
                        break;
                    }
                }

                // Remove random module from cell
                // TODO: Instead of removing a random module, set a random module
                if (OrderedCells.Count > 0)
                {
                    var cell = OrderedCells.GetFirst();
                    cell.RemoveModule(cell.possibleModules[Random.Range(0, cell.possibleModules.Count)]);
                }
                else
                {
                    // Finished
                    break;
                }
            }

            stopwatch.Stop();
            Debug.Log(
                $"Wave-function-collapse algorithm finished in {stopwatch.Elapsed.TotalMilliseconds}ms (Seed: {finalSeed})");
        }

        /// <summary>
        /// Resolve all initial constraints
        /// </summary>
        private void ApplyInitialConstraints()
        {
            Debug.Log("Resolve initial constraints");

            StartGoalConstraint();
            BorderOutsideConstraint();
        }

        /// <summary>
        /// Initial constraint: There can only be border on the outside
        /// </summary>
        private void BorderOutsideConstraint()
        {
            var bottomFilter = new EdgeFilter(2, Module.EdgeConnectionTypes.Block, true);
            var topFilter = new EdgeFilter(0, Module.EdgeConnectionTypes.Block, true);
            var leftFilter = new EdgeFilter(1, Module.EdgeConnectionTypes.Block, true);
            var rightFilter = new EdgeFilter(3, Module.EdgeConnectionTypes.Block, true);

            // filter bottom and top cells
            for (var i = 0; i < 2; i++)
            {
                var z = i * (height - 1);

                for (var x = 0; x < width; x++)
                {
                    cells[x, z].FilterCell(i == 0 ? bottomFilter : topFilter);
                }
            }

            // filter left and right cells
            for (var i = 0; i < 2; i++)
            {
                var x = i * (width - 1);

                for (var z = 0; z < height; z++)
                {
                    cells[x, z].FilterCell(i == 0 ? leftFilter : rightFilter);
                }
            }
        }

        /// <summary>
        /// Initial constraint: Place one start and one goal module
        /// </summary>
        private void StartGoalConstraint()
        {
            var startCell = cells[Random.Range(0, cells.GetLength(0)), Random.Range(0, cells.GetLength(1) - 1)];
            Cell goalCell;

            startCell.SetModule(startModule);

            do
            {
                goalCell = cells[Random.Range(0, cells.GetLength(0)), Random.Range(1, cells.GetLength(1))];
            } while (goalCell == startCell);

            goalCell.SetModule(goalModule);
        }
    }
}