using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using GraphSharp.Helpers;

namespace GraphSharp.Controls
{
	public class EdgeControl : Control, IPoolObject, IDisposable
	{
		#region Dependency Properties

		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register( "Source",
																							   typeof( VertexControl ),
																							   typeof( EdgeControl ),
																							   new UIPropertyMetadata( null ) );

		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register( "Target",
																							   typeof( VertexControl ),
																							   typeof( EdgeControl ),
																							   new UIPropertyMetadata( null ) );

		public static readonly DependencyProperty RoutePointsProperty = DependencyProperty.Register( "RoutePoints",
																									typeof( Point[] ),
																									typeof( EdgeControl ),
																									new UIPropertyMetadata(
																										null ) );

		public static readonly DependencyProperty EdgeProperty = DependencyProperty.Register( "Edge", typeof( object ),
																							 typeof( EdgeControl ),
																							 new PropertyMetadata( null ) );

        public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner( typeof(EdgeControl),
                                                                                                                    new UIPropertyMetadata(2.0) );

		#endregion

		#region Properties
		public VertexControl Source
		{
			get { return (VertexControl)GetValue( SourceProperty ); }
			internal set { SetValue( SourceProperty, value ); }
		}

		public VertexControl Target
		{
			get { return (VertexControl)GetValue( TargetProperty ); }
			internal set { SetValue( TargetProperty, value ); }
		}

		public Point[] RoutePoints
		{
			get { return (Point[])GetValue( RoutePointsProperty ); }
			set { SetValue( RoutePointsProperty, value ); }
		}

		public object Edge
		{
			get { return GetValue( EdgeProperty ); }
			set { SetValue( EdgeProperty, value ); }
		}

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        #endregion
        
		static EdgeControl()
		{
			//override the StyleKey
			DefaultStyleKeyProperty.OverrideMetadata( typeof( EdgeControl ), new FrameworkPropertyMetadata( typeof( EdgeControl ) ) );
		}

		#region IPoolObject Members

		public void Reset()
		{
			Edge = null;
			RoutePoints = null;
			Source = null;
			Target = null;
		}

		public void Terminate()
		{
			//nothing to do, there are no unmanaged resources
		}

		public event DisposingHandler Disposing;

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if ( Disposing != null )
				Disposing( this );
		}

		#endregion
	}
}