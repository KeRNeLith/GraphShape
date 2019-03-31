using System.Windows;
using System.Collections.Generic;
using GraphSharp.Sample.ViewModel;

namespace GraphSharp.Sample
{
	public class LayoutManager : DependencyObject
	{
		private static LayoutManager instance;

		public static LayoutManager Instance
		{
			get
			{
				if (instance == null)
					instance = new LayoutManager();

				return instance;
			}
		}

		protected LayoutManager()
		{

		}

		public void ContinueLayout()
		{
			foreach (var layout in graphLayouts)
			{
				layout.ContinueLayout();
			}
		}

		public void Relayout()
		{
			foreach (var layout in graphLayouts)
			{
				layout.Relayout();
			}
		}

		protected readonly HashSet<PocGraphLayout> graphLayouts = new HashSet<PocGraphLayout>();

		public static readonly DependencyProperty ManagedLayoutProperty =
			DependencyProperty.RegisterAttached("ManagedLayout", typeof(bool), typeof(LayoutManager), new PropertyMetadata(false, ManagedLayout_PropertyChanged));

		[AttachedPropertyBrowsableForChildren]
		public static bool GetManagedLayout(DependencyObject obj)
		{
			return (bool)obj.GetValue(ManagedLayoutProperty);
		}

		public static void SetManagedLayout(DependencyObject obj, bool value)
		{
			obj.SetValue(ManagedLayoutProperty, value);
		}

		protected static void ManagedLayout_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var graphLayout = obj as PocGraphLayout;
			if (graphLayout == null)
				return;

			if ((bool)e.NewValue)
			{
				//the layout became managed
				Instance.graphLayouts.Add(graphLayout);
				graphLayout.Unloaded += GraphLayout_Unloaded;
			}
			else if ((bool)e.OldValue && (((bool)e.NewValue) == false) && Instance.graphLayouts.Contains(graphLayout))
			{
				//the layout became unmanaged
				Instance.graphLayouts.Remove(graphLayout);
				graphLayout.Unloaded -= GraphLayout_Unloaded;
			}
		}

		private static void GraphLayout_Unloaded(object s, RoutedEventArgs args)
		{
			if (s is PocGraphLayout)
				Instance.graphLayouts.Remove(s as PocGraphLayout);
		}
	}
}