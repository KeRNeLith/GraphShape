using System;
using System.Windows.Input;
using JetBrains.Annotations;

namespace GraphShape.Sample.MVVM
{
    /// <summary>
    /// Commands container.
    /// </summary>
    internal interface ICommandRegister
    {
        /// <summary>
        /// Registers a command.
        /// </summary>
        void RegisterCommand([NotNull] ICommand command, [NotNull] Predicate<object> canExecute, [NotNull] Action<object> execute);

        /// <summary>
        /// Unregisters a command.
        /// </summary>
        void UnregisterCommand([NotNull] ICommand command);
    }
}