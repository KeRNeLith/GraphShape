using System;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;

namespace GraphShape.Sample.MVVM
{
    internal sealed class MapperCommandBinding : CommandBinding
    {
        private ICommand _mappedToCommand;

        /// <summary>
        /// The command which will executed instead of the 'Command'.
        /// </summary>
        public ICommand MappedToCommand
        {
            get => _mappedToCommand;
            set
            {
                // Mapped command cannot be null
                if (value is null)
                    throw new ArgumentException(nameof(value));

                if (_mappedToCommand != null)
                {
                    CanExecute -= OnCanExecute;
                    Executed -= OnExecuted;
                }

                _mappedToCommand = value;

                CanExecute += OnCanExecute;
                Executed += OnExecuted;
            }
        }

        private void OnCanExecute([NotNull] object sender, [NotNull] CanExecuteRoutedEventArgs args)
        {
            if (MappedToCommand is RoutedCommand command && args.Source is IInputElement source)
                args.CanExecute = command.CanExecute(args.Parameter, source);
            else
                args.CanExecute = MappedToCommand.CanExecute(args.Parameter);
            
            args.Handled = true;
            args.ContinueRouting = false;
        }

        private void OnExecuted([NotNull] object sender, [NotNull] ExecutedRoutedEventArgs args)
        {
            if (MappedToCommand is RoutedCommand command && args.Source is IInputElement source)
                command.Execute(args.Parameter, source);
            else
                MappedToCommand.Execute(args.Parameter);
            
            args.Handled = true;
        }
    }
}