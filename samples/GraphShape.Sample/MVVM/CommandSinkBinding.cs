using System;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;

namespace GraphShape.Sample.MVVM
{
    /// <summary>
    /// A <see cref="CommandBinding"/> subclass that will attach its
    /// CanExecute and Executed events to the event handling
    /// methods on the object referenced by its <see cref="CommandSink"/> property.
    /// Set the attached <see cref="CommandSink"/> property on the element 
    /// whose <see cref="CommandBinding"/>s collection contain <see cref="CommandSinkBinding"/>s.
    /// If you dynamically create an instance of this class and add it 
    /// to the <see cref="CommandBinding"/>s of an element, you must explicitly set
    /// its <see cref="CommandSink"/> property.
    /// </summary>
    internal sealed class CommandSinkBinding : CommandBinding
    {
        #region CommandSink

        private ICommandSink _commandSink;

        public ICommandSink CommandSink
        {
            get => _commandSink;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                if (_commandSink != null)
                    throw new InvalidOperationException("Cannot set CommandSink more than once.");

                _commandSink = value;

                CanExecute += (sender, args) =>
                {
                    args.CanExecute = _commandSink.CanExecuteCommand(args.Command, args.Parameter, out bool handled);
                    args.Handled = handled;
                };

                Executed += (sender, args) =>
                {
                    _commandSink.ExecuteCommand(args.Command, args.Parameter, out bool handled);
                    args.Handled = handled;
                };
            }
        }

        #endregion

        #region CommandSink

        public static ICommandSink GetCommandSink([NotNull] DependencyObject obj)
        {
            return (ICommandSink)obj.GetValue(CommandSinkProperty);
        }

        public static void SetCommandSink([NotNull] DependencyObject obj, [NotNull] ICommandSink value)
        {
            obj.SetValue(CommandSinkProperty, value);
        }

        [NotNull]
        public static readonly DependencyProperty CommandSinkProperty = DependencyProperty.RegisterAttached(
            "CommandSink", typeof(ICommandSink), typeof(CommandSinkBinding), new UIPropertyMetadata(null, OnCommandSinkChanged));

        private static void OnCommandSinkChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var commandSink = args.NewValue as ICommandSink;

            if (!ConfigureDelayedProcessing(d, commandSink))
            {
                ProcessCommandSinkChanged(d, commandSink);
            }
        }

        // This method is necessary when the CommandSink attached property is set on an element 
        // in a template, or any other situation in which the element's CommandBindings have not 
        // yet had a chance to be created and added to its CommandBindings collection.
        private static bool ConfigureDelayedProcessing([NotNull] DependencyObject d, ICommandSink commandSink)
        {
            bool isDelayed = false;

            var elem = new CommonElement(d);
            if (elem.IsValid && !elem.IsLoaded)
            {
                void Handler(object sender, RoutedEventArgs args)
                {
                    elem.Loaded -= Handler;
                    ProcessCommandSinkChanged(d, commandSink);
                }

                elem.Loaded += Handler;
                isDelayed = true;
            }

            return isDelayed;
        }

        private static void ProcessCommandSinkChanged([NotNull] DependencyObject d, ICommandSink commandSink)
        {
            CommandBindingCollection cmdBindings = GetCommandBindings(d);
            if (cmdBindings is null)
                throw new ArgumentException($"The {nameof(CommandSinkBinding)}.{nameof(CommandSink)} attached property was set on an element that does not support {nameof(CommandBinding)}.");

            foreach (CommandBinding cmdBinding in cmdBindings)
            {
                if (cmdBinding is CommandSinkBinding sink && sink.CommandSink is null)
                {
                    sink.CommandSink = commandSink;
                }
            }
        }

        private static CommandBindingCollection GetCommandBindings([NotNull] DependencyObject d)
        {
            var elem = new CommonElement(d);
            return elem.IsValid ? elem.CommandBindings : null;
        }

        #endregion

        #region CommonElement

        /// <summary>
        /// This class makes it easier to write code that works 
        /// with the common members of both the <see cref="FrameworkElement"/>
        /// and <see cref="FrameworkContentElement"/> classes.
        /// </summary>
        private class CommonElement
        {
            private readonly FrameworkElement _fe;
            private readonly FrameworkContentElement _fce;

            public readonly bool IsValid;

            public CommonElement([CanBeNull] DependencyObject d)
            {
                _fe = d as FrameworkElement;
                _fce = d as FrameworkContentElement;

                IsValid = _fe != null || _fce != null;
            }

            public CommandBindingCollection CommandBindings
            {
                get
                {
                    Verify();

                    if (_fe is null)
                        return _fce.CommandBindings;
                    return _fe.CommandBindings;
                }
            }

            public bool IsLoaded
            {
                get
                {
                    Verify();

                    if (_fe is null)
                        return _fce.IsLoaded;
                    return _fe.IsLoaded;
                }
            }

            public event RoutedEventHandler Loaded
            {
                add
                {
                    Verify();

                    if (_fe is null)
                        _fce.Loaded += value;
                    else
                        _fe.Loaded += value;
                }
                remove
                {
                    Verify();

                    if (_fe is null)
                        _fce.Loaded -= value;
                    else
                        _fe.Loaded -= value;
                }
            }

            private void Verify()
            {
                if (!IsValid)
                    throw new InvalidOperationException($"Cannot use an invalid {nameof(CommonElement)}.");
            }
        }

        #endregion
    }
}