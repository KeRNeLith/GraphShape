using System.Collections.Generic;
using System.Windows;
using GraphShape.Sample.Controls;
using JetBrains.Annotations;

namespace GraphShape.Sample
{
    /// <summary>
    /// Layout manager.
    /// </summary>
    internal class LayoutManager : DependencyObject
    {
        #region Singleton management

        private LayoutManager()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static LayoutManager Instance { get; } = InstanceHandler.InternalInstance;

        private static class InstanceHandler
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static InstanceHandler()
            {
            }

            internal static readonly LayoutManager InternalInstance = new LayoutManager();
        }

        #endregion

        [NotNull, ItemNotNull]
        private readonly HashSet<PocGraphLayout> _graphLayouts = new HashSet<PocGraphLayout>();

        public void ContinueLayout()
        {
            foreach (PocGraphLayout layout in _graphLayouts)
            {
                layout.ContinueLayout();
            }
        }

        public void Relayout()
        {
            foreach (PocGraphLayout layout in _graphLayouts)
            {
                layout.Relayout();
            }
        }

        public static readonly DependencyProperty ManagedLayoutProperty = DependencyProperty.RegisterAttached(
            "ManagedLayout", typeof(bool), typeof(LayoutManager), new PropertyMetadata(false, OnManagedLayoutPropertyChanged));

        [AttachedPropertyBrowsableForChildren]
        public static bool GetManagedLayout(DependencyObject obj)
        {
            return (bool)obj.GetValue(ManagedLayoutProperty);
        }

        public static void SetManagedLayout(DependencyObject obj, bool value)
        {
            obj.SetValue(ManagedLayoutProperty, value);
        }

        protected static void OnManagedLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var graphLayout = d as PocGraphLayout;
            if (graphLayout is null)
                return;

            if ((bool)args.NewValue)
            {
                // The layout became managed
                Instance._graphLayouts.Add(graphLayout);
                graphLayout.Unloaded += OnGraphLayoutUnloaded;
            }
            else if ((bool)args.OldValue && !(bool)args.NewValue && Instance._graphLayouts.Contains(graphLayout))
            {
                // The layout became unmanaged
                Instance._graphLayouts.Remove(graphLayout);
                graphLayout.Unloaded -= OnGraphLayoutUnloaded;
            }
        }

        private static void OnGraphLayoutUnloaded(object sender, RoutedEventArgs args)
        {
            if (sender is PocGraphLayout graphLayout)
                Instance._graphLayouts.Remove(graphLayout);
        }
    }
}