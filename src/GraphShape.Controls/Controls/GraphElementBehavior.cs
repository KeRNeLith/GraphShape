using System;
using System.Diagnostics;
using System.Windows;
using JetBrains.Annotations;

namespace GraphShape.Controls
{
    /// <summary>
    /// Graph element behavior.
    /// </summary>
    public static class GraphElementBehaviour
    {
        #region Highlight event

        /// <summary>
        /// Highlight event.
        /// </summary>
        [NotNull]
        public static readonly RoutedEvent HighlightEvent = EventManager.RegisterRoutedEvent(
            "Highlight", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GraphElementBehaviour));

        /// <summary>
        /// Adds a new <see cref="HighlightEvent"/> handler.
        /// </summary>
        public static void AddHighlightHandler(DependencyObject d, RoutedEventHandler handler)
        {
            if (d is UIElement uiElement)
            {
                uiElement.AddHandler(HighlightEvent, handler);
            }
        }

        /// <summary>
        /// Removes the given <paramref name="handler"/> from <see cref="HighlightEvent"/>.
        /// </summary>
        public static void RemoveHighlightHandler(DependencyObject d, RoutedEventHandler handler)
        {
            if (d is UIElement uiElement)
            {
                uiElement.RemoveHandler(HighlightEvent, handler);
            }
        }

        #endregion

        #region Unhighlight event

        /// <summary>
        /// Unhighlight event.
        /// </summary>
        [NotNull]
        public static readonly RoutedEvent UnhighlightEvent = EventManager.RegisterRoutedEvent(
            "Unhighlight", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GraphElementBehaviour));

        /// <summary>
        /// Adds a new <see cref="UnhighlightEvent"/> handler.
        /// </summary>
        public static void AddUnhighlightHandler(DependencyObject d, RoutedEventHandler handler)
        {
            if (d is UIElement uiElement)
            {
                uiElement.AddHandler(UnhighlightEvent, handler);
            }
        }

        /// <summary>
        /// Removes the given <paramref name="handler"/> from <see cref="UnhighlightEvent"/>.
        /// </summary>
        public static void RemoveUnhighlightHandler(DependencyObject d, RoutedEventHandler handler)
        {
            if (d is UIElement uiElement)
            {
                uiElement.RemoveHandler(UnhighlightEvent, handler);
            }
        }

        #endregion

        #region HighlightTriggered event

        /// <summary>
        /// Highlight trigger event.
        /// </summary>
        [NotNull]
        internal static readonly RoutedEvent HighlightTriggeredEvent = EventManager.RegisterRoutedEvent(
            "HighlightTriggered", RoutingStrategy.Bubble, typeof(HighlightTriggerEventHandler), typeof(GraphElementBehaviour));

        /// <summary>
        /// Adds a new <see cref="HighlightTriggeredEvent"/> handler.
        /// </summary>
        public static void AddHighlightTriggeredHandler(DependencyObject d, RoutedEventHandler handler)
        {
            if (d is UIElement uiElement)
            {
                uiElement.AddHandler(HighlightTriggeredEvent, handler);
            }
        }

        /// <summary>
        /// Removes the given <paramref name="handler"/> from <see cref="HighlightTriggeredEvent"/>.
        /// </summary>
        public static void RemoveHighlightTriggeredHandler(DependencyObject d, RoutedEventHandler handler)
        {
            if (d is UIElement uiElement)
            {
                uiElement.RemoveHandler(HighlightTriggeredEvent, handler);
            }
        }

        #endregion

        #region HighlightInfoChanged event

        /// <summary>
        /// Highlight information changed event.
        /// </summary>
        [NotNull]
        public static readonly RoutedEvent HighlightInfoChangedEvent = EventManager.RegisterRoutedEvent(
            "HighlightInfoChanged", RoutingStrategy.Bubble, typeof(HighlightInfoChangedEventHandler), typeof(GraphElementBehaviour));

        /// <summary>
        /// Adds a new <see cref="HighlightInfoChangedEvent"/> handler.
        /// </summary>
        public static void AddHighlightInfoChangedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            if (d is UIElement uiElement)
            {
                uiElement.AddHandler(HighlightInfoChangedEvent, handler);
            }
        }

        /// <summary>
        /// Removes the given <paramref name="handler"/> from <see cref="HighlightInfoChangedEvent"/>.
        /// </summary>
        public static void RemoveHighlightInfoChangedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            if (d is UIElement uiElement)
            {
                uiElement.RemoveHandler(HighlightInfoChangedEvent, handler);
            }
        }

        #endregion

        #region HighlightTrigger

        /// <summary>
        /// Highlight trigger dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty HighlightTriggerProperty = DependencyProperty.RegisterAttached(
            "HighlightTrigger", typeof(bool), typeof(GraphElementBehaviour), new UIPropertyMetadata(false, null, CoerceHighlightTrigger));

        /// <summary>
        /// Gets the <see cref="HighlightTriggerProperty"/> value.
        /// </summary>
        public static bool GetHighlightTrigger(DependencyObject obj)
        {
            return (bool)obj.GetValue(HighlightTriggerProperty);
        }

        /// <summary>
        /// Sets the <see cref="HighlightTriggerProperty"/> value.
        /// </summary>
        public static void SetHighlightTrigger(DependencyObject obj, bool value)
        {
            obj.SetValue(HighlightTriggerProperty, value);
        }

        private static object CoerceHighlightTrigger(DependencyObject d, object baseValue)
        {
            var control = d as UIElement;
            if (control is null)
                return baseValue;

            if ((bool)baseValue == GetHighlightTrigger(d))
                return baseValue;

            var args = new HighlightTriggeredEventArgs(HighlightTriggeredEvent, d, (bool)baseValue);
            try
            {
                control.RaiseEvent(args);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during CoerceHighlightTrigger - likely the graph is still animating: {ex}");
            }

            return args.Cancel ? GetHighlightTrigger(d) : baseValue;
        }

        #endregion

        #region IsHighlighted

        /// <summary>
        /// Is highlighted property key.
        /// </summary>
        [NotNull]
        private static readonly DependencyPropertyKey IsHighlightedPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "IsHighlighted", typeof(bool), typeof(GraphElementBehaviour), new UIPropertyMetadata(false, OnIsHighlightedPropertyChanged));

        /// <summary>
        /// Is highlighted dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty IsHighlightedProperty = IsHighlightedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="IsHighlightedProperty"/> value.
        /// </summary>
        public static bool GetIsHighlighted(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHighlightedProperty);
        }

        /// <summary>
        /// Sets the <see cref="IsHighlightedProperty"/> value.
        /// </summary>
        internal static void SetIsHighlighted(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHighlightedPropertyKey, value);
        }

        // When the IsHighlighted Property changes we should raise the 
        // Highlight and Unhighlight RoutedEvents.
        private static void OnIsHighlightedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var control = d as UIElement;
            if (control is null)
                return;

            if ((bool)args.NewValue)
            {
                control.RaiseEvent(new RoutedEventArgs(HighlightEvent, d));
            }
            else
            {
                control.RaiseEvent(new RoutedEventArgs(UnhighlightEvent, d));
            }
        }

        #endregion

        #region HighlightInfo

        /// <summary>
        /// Highlight information property key.
        /// </summary>
        [NotNull]
        private static readonly DependencyPropertyKey HighlightInfoPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "HighlightInfo", typeof(object), typeof(GraphElementBehaviour), new PropertyMetadata(null, OnHighlightInfoPropertyChanged));

        /// <summary>
        /// Highlight information dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty HighlightInfoProperty = HighlightInfoPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="HighlightInfoProperty"/> value.
        /// </summary>
        public static object GetHighlightInfo(DependencyObject obj)
        {
            return obj.GetValue(HighlightInfoProperty);
        }

        /// <summary>
        /// Sets the <see cref="HighlightInfoProperty"/> value.
        /// </summary>
        internal static void SetHighlightInfo(DependencyObject obj, object value)
        {
            obj.SetValue(HighlightInfoPropertyKey, value);
        }

        private static void OnHighlightInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var control = d as UIElement;
            control?.RaiseEvent(
                new HighlightInfoChangedEventArgs(HighlightInfoChangedEvent, d, args.OldValue, args.NewValue));
        }

        #endregion

        #region IsSemiHighlighted

        /// <summary>
        /// Is semi highlighted dependency property.
        /// </summary>
        [NotNull]
        private static readonly DependencyPropertyKey IsSemiHighlightedPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "IsSemiHighlighted", typeof(bool), typeof(GraphElementBehaviour), new PropertyMetadata(false));

        /// <summary>
        /// Is semi highlighted property key.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty IsSemiHighlightedProperty = IsSemiHighlightedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="IsSemiHighlightedProperty"/> value.
        /// </summary>
        public static bool GetIsSemiHighlighted(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSemiHighlightedProperty);
        }

        /// <summary>
        /// Sets the <see cref="IsSemiHighlightedProperty"/> value.
        /// </summary>
        internal static void SetIsSemiHighlighted(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSemiHighlightedPropertyKey, value);
        }

        #endregion

        #region SemiHighlightInfo

        /// <summary>
        /// Semi highlight information property key.
        /// </summary>
        [NotNull]
        private static readonly DependencyPropertyKey SemiHighlightInfoPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "SemiHighlightInfo", typeof(object), typeof(GraphElementBehaviour), new PropertyMetadata(null));

        /// <summary>
        /// Semi highlight information dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty SemiHighlightInfoProperty = SemiHighlightInfoPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="SemiHighlightInfoProperty"/> value.
        /// </summary>
        public static object GetSemiHighlightInfo(DependencyObject obj)
        {
            return obj.GetValue(SemiHighlightInfoProperty);
        }

        /// <summary>
        /// Sets the <see cref="SemiHighlightInfoProperty"/> value.
        /// </summary>
        internal static void SetSemiHighlightInfo(DependencyObject obj, object value)
        {
            obj.SetValue(SemiHighlightInfoPropertyKey, value);
        }

        #endregion

        #region LayoutInfo

        /// <summary>
        /// Layout information dependency property.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty LayoutInfoProperty = DependencyProperty.RegisterAttached(
            "LayoutInfo", typeof(object), typeof(GraphElementBehaviour), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets the <see cref="LayoutInfoProperty"/> value.
        /// </summary>
        public static object GetLayoutInfo(DependencyObject obj)
        {
            return obj.GetValue(LayoutInfoProperty);
        }

        /// <summary>
        /// Sets the <see cref="LayoutInfoProperty"/> value.
        /// </summary>
        public static void SetLayoutInfo(DependencyObject obj, object value)
        {
            obj.SetValue(LayoutInfoProperty, value);
        }

        #endregion
    }
}