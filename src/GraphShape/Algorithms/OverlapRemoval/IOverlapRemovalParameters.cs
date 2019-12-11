namespace GraphShape.Algorithms.OverlapRemoval
{
	public interface IOverlapRemovalParameters : IAlgorithmParameters
	{
		float VerticalGap { get; }
		float HorizontalGap { get; }
	}
}