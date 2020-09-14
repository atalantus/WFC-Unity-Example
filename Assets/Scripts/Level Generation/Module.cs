using UnityEngine;

namespace LevelGeneration
{
    /// <summary>
    /// Scriptable Object asset for one specific module.
    /// </summary>
    [CreateAssetMenu(fileName = "New Module", menuName = "Map Generation/Module")]
    public class Module : ScriptableObject
    {
        /// <summary>
        /// Different edge connection types.
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
        /// The module`s game object.
        /// </summary>
        public GameObject moduleGO;

        /// <summary>
        /// The module`s edge connections starting with the bottom one going counter clockwise.
        ///
        /// [bottom, right, top, left]
        /// </summary>
        public EdgeConnectionTypes[] edgeConnections = new EdgeConnectionTypes[4];
    }
}