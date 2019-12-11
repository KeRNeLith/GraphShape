using System.Windows;

namespace GraphShape.Controls
{
	interface ICompoundVertexControl
	{
		/// <summary>
		/// Gets the 'borderthickness' of the control around the inner canvas.
		/// </summary>
		Thickness VertexBorderThickness { get; }

		/// <summary>
		/// Gets the size of the inner canvas.
		/// </summary>
		Size InnerCanvasSize { get; }

		event RoutedEventHandler Expanded;
		event RoutedEventHandler Collapsed;
	}
}
