using System;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;

namespace GraphShape.Sample.MVVM
{
    internal sealed class CommandSinkDependencyObject : DependencyObject, ICommandSink, ICommandRegister
    {
        [NotNull]
        private readonly CommandSink _sink = new CommandSink();

        #region ICommandSink

        /// <inheritdoc />
        public bool CanExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            return _sink.CanExecuteCommand(command, parameter, out handled);
        }

        /// <inheritdoc />
        public void ExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            _sink.ExecuteCommand(command, parameter, out handled);
        }

        #endregion

        #region ICommandRegister

        /// <inheritdoc />
        public void RegisterCommand(ICommand command, Predicate<object> canExecute, Action<object> execute)
        {
            _sink.RegisterCommand(command, canExecute, execute);
        }

        /// <inheritdoc />
        public void UnregisterCommand(ICommand command)
        {
            _sink.UnregisterCommand(command);
        }

        #endregion
    }
}