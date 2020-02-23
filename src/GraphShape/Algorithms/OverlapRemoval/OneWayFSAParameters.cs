namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Enumeration of possible FSA algorithm orientations (way).
    /// </summary>
    public enum OneWayFSAWayEnum
    {
        /// <summary>
        /// Horizontal.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Vertical.
        /// </summary>
        Vertical
    }

    /// <summary>
    /// One way FSA algorithm parameters.
    /// </summary>
    public class OneWayFSAParameters : OverlapRemovalParameters
    {
        private OneWayFSAWayEnum _way;

        /// <summary>
        /// Overlap removal orientation (way).
        /// </summary>
        public OneWayFSAWayEnum Way
        {
            get => _way;
            set
            {
                if (_way == value)
                    return;
                _way = value;
                OnPropertyChanged();
            }
        }
    }
}