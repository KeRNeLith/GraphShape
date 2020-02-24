namespace GraphShape.Algorithms.Layout.Simple.Tree
{
	public class SimpleTreeLayoutParameters : LayoutParametersBase
	{
		private double vertexGap = 10;
		/// <summary>
		/// Gets or sets the gap between the vertices.
		/// </summary>
		public double VertexGap
		{
			get { return vertexGap; }
			set
			{
				if ( vertexGap != value )
				{
					vertexGap = value;
                    OnPropertyChanged();
				}
			}
		}

		private double layerGap = 10;
		/// <summary>
		/// Gets or sets the gap between the layers.
		/// </summary>
		public double LayerGap
		{
			get { return layerGap; }
			set
			{
				if ( layerGap != value )
				{
					layerGap = value;
                    OnPropertyChanged();
				}
			}
		}

		private LayoutDirection direction = LayoutDirection.TopToBottom;
		/// <summary>
		/// Gets or sets the direction of the layout.
		/// </summary>
		public LayoutDirection Direction
		{
			get { return direction; }
			set
			{
				if ( direction != value )
				{
					direction = value;
                    OnPropertyChanged();
				}
			}
		}

		private SpanningTreeGeneration spanningTreeGeneration = SpanningTreeGeneration.DFS;
		/// <summary>
		/// Gets or sets the direction of the layout.
		/// </summary>
		public SpanningTreeGeneration SpanningTreeGeneration
		{
			get { return spanningTreeGeneration; }
			set
			{
				if ( spanningTreeGeneration != value )
				{
					spanningTreeGeneration = value;
                    OnPropertyChanged();
				}
			}
		}

        private bool optimizeWidthAndHeight = false;

        public bool OptimizeWidthAndHeight
        {
            get { return optimizeWidthAndHeight; }
            set
            {
                if (value == optimizeWidthAndHeight)
                    return;

                optimizeWidthAndHeight = value;
                OnPropertyChanged();
            }
        }

        private double widthPerHeight = 1.0;

        public double WidthPerHeight
        {
            get { return widthPerHeight; }
            set
            {
                if (value == widthPerHeight)
                    return;

                widthPerHeight = value;
                OnPropertyChanged();
            }
        }
	}
}