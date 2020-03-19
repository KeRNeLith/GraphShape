namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// One way FSA algorithm parameters.
    /// </summary>
    public class OneWayFSAParameters : OverlapRemovalParameters
    {
        private OneWayFSAWay _way;

        /// <summary>
        /// Overlap removal orientation (way).
        /// </summary>
        public OneWayFSAWay Way
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