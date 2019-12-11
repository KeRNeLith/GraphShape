using System.Collections.Generic;
using System.Windows;

namespace GraphShape.Algorithms.OverlapRemoval
{
	public interface IOverlapRemovalAlgorithm<TObject> : IAlgorithm
	{
		IDictionary<TObject, Rect> Rectangles { get; }

		IOverlapRemovalParameters GetParameters();
	}

	public interface IOverlapRemovalAlgorithm<TObject, TParam> : IOverlapRemovalAlgorithm<TObject>
		where TParam : IOverlapRemovalParameters
	{
		TParam Parameters { get; }
	}
}