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
    public class LevelGenerator
    {
        private static LevelGenerator _instance;

        /// <summary>
        /// The Level Generator
        /// </summary>
        public static LevelGenerator Instance => _instance ?? (_instance = new LevelGenerator());

        public Heap<Cell> OrderedCells;

        private LevelGenerator()
        {
        }

        /// <summary>
        /// Wave-function-collapse algorithm
        /// TODO: Multithreading?
        /// </summary>
        /// <param name="cells">The grid`s cells</param>
        /// <param name="seed">RNG seed</param>
        public void GenerateLevelWFC(ref Cell[,] cells, int seed)
        {
            // Set RNG seed
            Random.InitState(seed);

            // Instantiate cells heap
            OrderedCells = new Heap<Cell>(cells.GetLength(0) * cells.GetLength(1));

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    OrderedCells.Add(cells[i, j]);
                }
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Debug.LogWarning("Start Wave-function-collapse algorithm");

            ApplyInitialConstraints(ref cells);

            while (true)
            {
                //Debug.Log("Starting another iteration! Removing next module.");

                // Remove finished cells from heap
                while (OrderedCells.Count > 0)
                {
                    var cell = OrderedCells.GetFirst();

                    if (cell.SolvedScore == 1)
                    {
                        OrderedCells.RemoveFirst();
                    }
                    else
                    {
                        break;
                    }
                }

                // Remove random module from cell
                if (OrderedCells.Count > 0)
                {
                    var cell = OrderedCells.GetFirst();
                    cell.RemoveModule(cell.possibleModulesIndices[Random.Range(0, cell.possibleModulesIndices.Count)]);
                }
                else
                {
                    // Finished
                    break;
                }
            }

            stopwatch.Stop();
            Debug.LogWarning(
                $"Wave-function-collapse algorithm finished in {stopwatch.Elapsed.TotalMilliseconds}ms (Seed: {seed})");
        }

        /// <summary>
        /// Checks if the cells of the generated level matches with each other
        /// </summary>
        /// <param name="cells">The grid`s cells</param>
        /// <returns>List of not matching cells` (x, y)-coordinates</returns>
        public List<Tuple<int, int>> CheckGeneratedLevel(ref Cell[,] cells)
        {
            var notMatchingCells = new List<Tuple<int, int>>();

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    var cell = cells[i, j];
                    var bCell = cell.neighbourCells[0];
                    var rCell = cell.neighbourCells[1];

                    var matchesNeighbours = true;

                    if (bCell != null)
                    {
                        if (ModuleManager.Instance.modules[cell.possibleModulesIndices[0]].edgeConnections[0] !=
                            ModuleManager.Instance.modules[bCell.possibleModulesIndices[0]].edgeConnections[2])
                        {
                            matchesNeighbours = false;
                            Debug.LogWarning($"CheckGeneratedLevel | ({i}, {j}) not matching with ({i}, {j + 1})");
                        }
                    }

                    if (rCell != null)
                    {
                        if (ModuleManager.Instance.modules[cell.possibleModulesIndices[0]].edgeConnections[1] !=
                            ModuleManager.Instance.modules[rCell.possibleModulesIndices[0]].edgeConnections[3])
                        {
                            matchesNeighbours = false;
                            Debug.LogWarning($"CheckGeneratedLevel | ({i}, {j}) not matching with ({i + 1}, {j})");
                        }
                    }

                    if (!matchesNeighbours) notMatchingCells.Add(new Tuple<int, int>(i, j));
                }
            }

            return notMatchingCells;
        }

        /// <summary>
        /// Resolve all initial constraints
        /// </summary>
        /// <param name="cells">The grid`s cells</param>
        private void ApplyInitialConstraints(ref Cell[,] cells)
        {
            Debug.LogWarning("Resolve initial constraints");

            StartGoalConstraint(ref cells);
            BorderOutsideConstraint(ref cells);
        }

        /// <summary>
        /// Initial constraint: There can only be border on the outside
        /// </summary>
        /// <param name="cells">The grid`s cells</param>
        private void BorderOutsideConstraint(ref Cell[,] cells)
        {
            var edgeTypes = (Module.EdgeConnectionTypes[]) Enum.GetValues(typeof(Module.EdgeConnectionTypes));

            for (int i = 0; i < 2; i++)
            {
                i = (cells.GetLength(0) - 1) * i;

                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    var cell = cells[i, j];

                    foreach (var edgeType in edgeTypes)
                    {
                        if (edgeType == Module.EdgeConnectionTypes.Block) continue;

                        var edgeFilter = new EdgeFilter(i == 0 ? 1 : 3, edgeType);

                        cell.FilterCell(edgeFilter);
                    }
                }
            }

            for (int j = 0; j < 2; j++)
            {
                j = (cells.GetLength(1) - 1) * j;

                for (int i = 0; i < cells.GetLength(0); i++)
                {
                    var cell = cells[i, j];

                    foreach (var edgeType in edgeTypes)
                    {
                        if (edgeType == Module.EdgeConnectionTypes.Block) continue;

                        var edgeFilter = new EdgeFilter(j == 0 ? 0 : 2, edgeType);

                        cell.FilterCell(edgeFilter);
                    }
                }
            }
        }

        /// <summary>
        /// Initial constraint: Place start and goal module
        /// </summary>
        /// <param name="cells">The grid`s cells</param>
        private void StartGoalConstraint(ref Cell[,] cells)
        {
            var startCell = cells[Random.Range(0, cells.GetLength(0)), Random.Range(1, cells.GetLength(1))];
            Cell goalCell = null;
            
            startCell.SetSpecialModule(0);

            do
            {
                goalCell = cells[Random.Range(0, cells.GetLength(0)), Random.Range(0, cells.GetLength(1) - 1)];
            } while (goalCell == startCell);
            
            goalCell.SetSpecialModule(1);

            // SimpleRemove start and goal modules from other cells
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    var cell = cells[i, j];

                    if (cell != startCell && cell != goalCell)
                    {
                        // Remove all special modules
                        for (int k = 0; k < 2; k++)
                        {
                            cell.SimpleRemoveModule(k);
                        }
                    }
                }
            }
        }
    }
}