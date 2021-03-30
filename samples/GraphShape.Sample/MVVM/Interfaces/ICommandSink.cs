using System.Windows.Input;
using JetBrains.Annotations;

namespace GraphShape.Sample.MVVM
{
    /// <summary>
    /// Represents an object that is capable of being notified of 
    /// a routed command execution by a <see cref="CommandSinkBinding"/>. This
    /// interface is intended to be implemented by a ViewModel class
    /// that honors a set of routed commands.
    /// </summary>
    internal interface ICommandSink
    {
        /// <summary>
        /// Returns true if the specified command can be executed by the command sink.
        /// </summary>
        /// <param name="command"> The command whose execution status is being queried.</param>
        /// <param name="parameter">An optional command parameter.</param>
        /// <param name="handled">Set to true if there is no need to continue querying for an execution status.</param>
        bool CanExecuteCommand([NotNull] ICommand command, [CanBeNull] object parameter, out bool handled);

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command being executed.</param>
        /// <param name="parameter">An optional command parameter.</param>
        /// <param name="handled">Set to true if the command has been executed and there is no need for others to respond.</param>
        void ExecuteCommand([NotNull] ICommand command, [CanBeNull] object parameter, out bool handled);
    }
}