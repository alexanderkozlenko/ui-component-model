﻿// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Threading;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents a bindable command component.</summary>
    /// <typeparam name="T">The type of the parameter for action and predicate.</typeparam>
    public partial class BindableCommand<T> : IBindableCommand, IDisposable
    {
        private readonly Action<T> _actionMethod;
        private readonly Predicate<T> _predicateMethod;
        private readonly SynchronizationContext _synchronizationContext;

        /// <summary>Initializes a new instance of the <see cref="BindableCommand{T}" /> class.</summary>
        /// <param name="actionMethod">The method to execute when the command is executed.</param>
        /// <param name="synchronizationContext">The synchronization context for interaction with UI.</param>
        /// <exception cref="ArgumentNullException"><paramref name="actionMethod" /> is <see langword="null" />.</exception>
        public BindableCommand(Action<T> actionMethod, SynchronizationContext synchronizationContext = null)
        {
            if (actionMethod is null)
            {
                throw new ArgumentNullException(nameof(actionMethod));
            }

            _actionMethod = actionMethod;
            _synchronizationContext = synchronizationContext;
        }

        /// <summary>Initializes a new instance of the <see cref="BindableCommand{T}" /> class.</summary>
        /// <param name="actionMethod">The method to execute when the command is executed.</param>
        /// <param name="predicateMethod">The method which is used as a predicate to check if the command can be executed.</param>
        /// <param name="synchronizationContext">The synchronization context for interaction with UI.</param>
        /// <exception cref="ArgumentNullException"><paramref name="actionMethod" /> or <paramref name="predicateMethod" /> is <see langword="null" />.</exception>
        public BindableCommand(Action<T> actionMethod, Predicate<T> predicateMethod, SynchronizationContext synchronizationContext = null)
            : this(actionMethod, synchronizationContext)
        {
            if (predicateMethod is null)
            {
                throw new ArgumentNullException(nameof(predicateMethod));
            }

            _predicateMethod = predicateMethod;
        }

        /// <summary />
        ~BindableCommand()
        {
            Dispose(false);
        }

        private protected static EventArgs CreateCanExecuteChangedEventArgs()
        {
            return EventArgs.Empty;
        }

        private void UnsafeRaiseCanExecuteChanged(SynchronizationContext synchronizationContext)
        {
            if ((synchronizationContext is null) || (synchronizationContext == SynchronizationContext.Current))
            {
                UnsafeRaiseCanExecuteChanged();
            }
            else
            {
                var adapter = new DispatcherCallbackAdapter(UnsafeRaiseCanExecuteChanged);

                synchronizationContext.Post(adapter.Post, null);
            }
        }

        private protected virtual EventArgs UnsafeRaiseCanExecuteChanged()
        {
            var eventArgs = default(EventArgs);
            var eventHandler = CanExecuteChanged;

            if (eventHandler is not null)
            {
                eventArgs = CreateCanExecuteChangedEventArgs();
                eventHandler.Invoke(this, eventArgs);
            }

            return eventArgs;
        }

        /// <summary>Releases all subscriptions to the command state changed event and to the property changed event of the currently observing objects.</summary>
        /// <param name="disposing">The value that indicates whether the method call comes from a dispose method (its value is <see langword="true" />) or from a finalizer (its value is <see langword="false" />).</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CanExecuteChanged = null;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public virtual void RaiseCanExecuteChanged()
        {
            UnsafeRaiseCanExecuteChanged(_synchronizationContext);
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return (_predicateMethod is null) || _predicateMethod.Invoke((T)parameter);
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
            _actionMethod.Invoke((T)parameter);
        }

        /// <summary>Gets or sets the synchronization context for interaction with UI.</summary>
        public SynchronizationContext SynchronizationContext
        {
            get => _synchronizationContext;
        }

        /// <summary>Occurs when changes occur that affect whether or not the command should execute.</summary>
        public event EventHandler CanExecuteChanged;
    }
}
