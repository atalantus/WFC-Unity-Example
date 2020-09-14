namespace LevelGeneration
{
    /// <summary>
    /// Edge filter.
    /// </summary>
    public class EdgeFilter
    {
        /// <summary>
        /// The edge type that gets filtered.
        /// </summary>
        private readonly Module.EdgeConnectionTypes _filterType;

        /// <summary>
        /// If the filter filters inclusive or exclusive.
        ///
        /// NOTE:
        /// We can either filter cells so that remaining modules must contain (inclusive) this type of edge or must not
        /// contain (exclusive) this type of edge.
        /// </summary>
        private readonly bool _isInclusive;

        /// <summary>
        /// The edges`s direction.
        /// 
        /// [bottom, right, top, left]
        /// </summary>
        private readonly int _edgeDirection;

        public EdgeFilter(int edgeDirection, Module.EdgeConnectionTypes filterType, bool isInclusive)
        {
            _edgeDirection = edgeDirection;
            _filterType = filterType;
            _isInclusive = isInclusive;
        }

        /// <summary>
        /// Checks if a <see cref="Module"/> matches this filter.
        /// </summary>
        /// <param name="module">The <see cref="Module"/>.</param>
        /// <returns>True if the <see cref="Module"/> matches this filter.</returns>
        public bool CheckModule(Module module)
        {
            var edge = (_edgeDirection + 2) % 4;
            var match = module.edgeConnections[edge] == _filterType;

            return _isInclusive ? !match : match;
        }
    }
}