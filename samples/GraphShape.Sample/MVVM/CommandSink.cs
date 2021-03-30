using System;
using System.Collections.Generic;
using System.Windows.Input;
using JetBrains.Annotations;

namespace GraphShape.Sample.MVVM
{
    /// <summary>
    /// This implementation of <see cref="ICommandSink"/> can serve as a base
    /// class for a ViewModel or as an object embedded in a ViewModel.
    /// It provides a means of registering commands and their callback 
    /// methods, and will invoke those callbacks upon request.
    /// </summary>
    internal class CommandSink : ICommandSink, ICommandRegister
    {
        #region Data

        [NotNull]
        private readonly Dictionary<ICommand, CommandCallbacks> _commandToCallbacksMap =
            new Dictionary<ICommand, CommandCallbacks>();

        #endregion

        #region Command Registration

        /// <inheritdoc />
        public void RegisterCommand(ICommand command, Predicate<object> canExecute, Action<object> execute)
        {
            VerifyArgument(command, nameof(command));
            VerifyArgument(canExecute, nameof(canExecute));
            VerifyArgument(execute, nameof(execute));

            _commandToCallbacksMap[command] = new CommandCallbacks(canExecute, execute);
        }

        /// <inheritdoc />
        public void UnregisterCommand(ICommand command)
        {
            VerifyArgument(command, nameof(command));

            if (_commandToCallbacksMap.ContainsKey(command))
            {
                _commandToCallbacksMap.Remove(command);
            }
        }

        #endregion

        #region ICommandSink

        /// <inheritdoc />
        public bool CanExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            VerifyArgument(command, nameof(command));

            if (_commandToCallbacksMap.ContainsKey(command))
            {
                handled = true;
                return _commandToCallbacksMap[command].CanExecute(parameter);
            }

            return handled = false;
        }

        /// <inheritdoc />
        public void ExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            VerifyArgument(command, nameof(command));

            if (_commandToCallbacksMap.ContainsKey(command))
            {
                handled = true;
                _commandToCallbacksMap[command].Execute(parameter);
            }
            else
            {
                handled = false;
            }
        }

        #endregion

        #region VerifyArgument

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void VerifyArgument([NotNull] object arg, [NotNull] string argName)
        {
            if (arg is null)
                throw new ArgumentNullException(argName);
        }

        #endregion

        #region CommandCallbacks

        internal readonly struct CommandCallbacks
        {
            public readonly Predicate<object> CanExecute;
            public readonly Action<object> Execute;

            public CommandCallbacks([NotNull] Predicate<object> canExecute, [NotNull] Action<object> execute)
            {
                CanExecute = canExecute;
                Execute = execute;
            }
        }

        #endregion
    }
}