namespace GraphShape.Algorithms.Layout.Simple.FDP
{
	public class LinLogLayoutParameters : LayoutParametersBase
	{
		internal double attractionExponent = 1.0;

		public double AttractionExponent
		{
			get { return attractionExponent; }
			set
			{
				attractionExponent = value;
                OnPropertyChanged();
			}
		}

		internal double repulsiveExponent;

		public double RepulsiveExponent
		{
			get { return repulsiveExponent; }
			set
			{
				repulsiveExponent = value;
                OnPropertyChanged();
			}
		}

		internal double gravitationMultiplier = 0.1;

		public double GravitationMultiplier
		{
			get { return gravitationMultiplier; }
			set
			{
				gravitationMultiplier = value;
                OnPropertyChanged();
			}
		}

		internal int iterationCount = 100;

		public int IterationCount
		{
			get { return iterationCount; }
			set
			{
				iterationCount = value;
                OnPropertyChanged();
			}
		}
	}
}