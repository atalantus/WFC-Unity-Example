using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelGeneration
{
    public class Cell : MonoBehaviour, IHeapItem<Cell>
    {
        public bool isCellSet;

        public List<Module> possibleModules;

        public LevelGenerator levelGenerator;

        /// <summary>
        /// (bottom, right, top, left)
        /// </summary>
        public Cell[] neighbours = new Cell[4];

        public int HeapIndex { get; set; }

        private void Awake()
        {
            possibleModules = new List<Module>();
        }

        public void PopulateCell()
        {
            // At the beginning every module is possible
            for (var i = 0; i < levelGenerator.modules.Count; i++)
            {
                possibleModules.Add(levelGenerator.modules[i]);
            }
        }

        public void FilterCell(EdgeFilter filter)
        {
            if (possibleModules.Count == 1) return;

            var removingModules = new List<Module>();

            // Filter possible Modules list for a given filter
            for (var i = 0; i < possibleModules.Count; i++)
            {
                var module = possibleModules[i];
                var removeModule = filter.CheckModule(module);

                if (removeModule)
                {
                    // Remove module
                    removingModules.Add(possibleModules[i]);
                }
            }

            // Now remove filtered modules
            for (var i = 0; i < removingModules.Count; i++)
            {
                RemoveModule(removingModules[i]);
            }
        }

        public void RemoveModule(Module module)
        {
            // Remove module from possibility space
            possibleModules.Remove(module);

            // Update item on the heap
            levelGenerator.OrderedCells.UpdateItem(this);

            for (var j = 0; j < neighbours.Length; j++)
            {
                // Only check if cell has a neighbour on this edge
                if (neighbours[j] == null) continue;

                var edgeType = module.edgeConnections[j];
                var lastWithEdgeType = true;

                // Search in other possible modules for the same edge type
                for (var i = 0; i < possibleModules.Count; i++)
                {
                    if (possibleModules[i].edgeConnections[j] == edgeType)
                    {
                        lastWithEdgeType = false;
                        break;
                    }
                }

                if (lastWithEdgeType)
                {
                    // Populate edge changes to neighbour cell
                    var edgeFilter = new EdgeFilter(j, edgeType, false);

                    neighbours[j].FilterCell(edgeFilter);
                }
            }
        }

        public void SetModule(Module module)
        {
            possibleModules = new List<Module> {module};

            // Update item on the heap
            levelGenerator.OrderedCells.UpdateItem(this);

            // check if it fits to already set neighbour cells
            for (var i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i] == null || !neighbours[i].isCellSet) continue;

                if (module.edgeConnections[i] != neighbours[i].possibleModules[0].edgeConnections[(i + 2) % 4])
                    Debug.LogError(
                        $"Setting module {module} would not fit already set neighbour {neighbours[i].gameObject}!",
                        gameObject);
            }

            // Propagate changes to neighbours
            for (var i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i] == null) continue;

                // Populate edge changes to neighbour cell
                var edgeFilter = new EdgeFilter(i, module.edgeConnections[i], true);
                neighbours[i].FilterCell(edgeFilter);
            }

            Instantiate(module.moduleGO, transform.position, Quaternion.identity, transform);

            isCellSet = true;
        }

        /// <summary>
        /// Compares two cells using their solved score
        /// </summary>
        /// <param name="other">Cell to compare</param>
        /// <returns></returns>
        public int CompareTo(Cell other)
        {
            var compare = possibleModules.Count.CompareTo(other.possibleModules.Count);
            if (compare == 0)
            {
                var r = Random.Range(1, 3);
                return r == 1 ? -1 : 1;
            }

            return -compare;
        }
    }
}