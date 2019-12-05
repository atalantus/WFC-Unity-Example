using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelGeneration
{
    /// <summary>
    /// Scriptable Object asset for one specific module
    /// </summary>
    [CreateAssetMenu(fileName = "New Module", menuName = "Map Generation/Module")]
    public class Module : ScriptableObject
    {
        /// <summary>
        /// Different edge connection types
        /// </summary>
        public enum EdgeConnectionTypes
        {
            NONE,
            Block,
            Open,
            BorderLeft,
            BorderRight,
            BorderBoth
        }
        
        /// <summary>
        /// The module`s game object
        /// </summary>
        public GameObject moduleGO;
        
        /// <summary>
        /// The module`s edge connections starting with the bottom one going counter clockwise (bottom, right, top, left)
        /// </summary>
        public EdgeConnectionTypes[] edgeConnections = new EdgeConnectionTypes[4];

        /// <summary>
        /// Checks this module for a specific edge filter
        /// </summary>
        /// <param name="filter">The filter</param>
        /// <returns>Does this model depend on the given edge filter</returns>
        public bool CheckModule(EdgeFilter filter)
        {
            //Debug.Log($"Checking {moduleGO.name} for edge filter {filter.EdgeIndex}, {filter.FilterType}");
            
            // Get receiving edge index of this module
            var edge = filter.EdgeIndex + 2;
            if (edge == 4) edge = 0;
            else if (edge == 5) edge = 1;
            
            // Check if module matches a given filter
            return edgeConnections[edge] == filter.FilterType;
        }
    }

    /// <summary>
    /// Edge filter
    /// </summary>
    public struct EdgeFilter
    {
        /// <summary>
        /// The edge`s index (See <see cref="Module.edgeConnections"/>)
        /// </summary>
        public int EdgeIndex;
        
        /// <summary>
        /// The edge type that gets filtered out
        /// </summary>
        public Module.EdgeConnectionTypes FilterType;

        public EdgeFilter(int edgeIndex, Module.EdgeConnectionTypes filterType)
        {
            EdgeIndex = edgeIndex;
            FilterType = filterType;
        }
    }
}