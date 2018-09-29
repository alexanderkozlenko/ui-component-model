## Anemonis.UI.ComponentModel

A set of basic components for building XAML-based UI using the `model-view-viewmodel` pattern.

[![NuGet package](https://img.shields.io/nuget/v/Anemonis.UI.ComponentModel.svg?style=flat-square)](https://www.nuget.org/packages/Anemonis.UI.ComponentModel)

### Important Features

- The bindable object as a simple base type for view models.
- The bindable command as a simple and extensible command implementation.
- The event broker as a simple messaging bus for UI events based on the `publish–subscribe` pattern.

### Important Features: Bindable Object

- The component supports working with a synchronization context.
- The component supports publishing events to an observer.
- The `GetValue` method uses the specified default value if the target object is `null`.
- The `SetValue` method does nothing if the target object is `null`.
- The `SetValue` method can invoke an optional callback if the value was changed.

### Important Features: Bindable Command

- The component supports working with a synchronization context.
- The component supports publishing events to an observer.
- The component supports automatic state update based on an update of another object.

### Important Features: Event Broker

- Event subscription is based on channel name and data type.

### Usage Examples: Bindable Object

```cs
public class BindableComponent : BindableObject
{
    private int _value;

    private void OnValueUpdated()
    {
    }

    public int Value1
    {
        get => GetValue(ref _value);
        set => SetValue(ref _value, value);
    }

    public int Value2
    {
        get => GetValue(ref _value, nameof(OnValueUpdated));
        set => SetValue(ref _value, value, nameof(OnValueUpdated));
    }
}
```
```cs
public class BindableComponent : BindableObject
{
    private TargetObject _target;

    private void OnValueUpdated()
    {
    }

    public int Value1
    {
        get => GetValue(_target, nameof(_target.Value), 0);
        set => SetValue(_target, nameof(_target.Value), value);
    }

    public int Value2
    {
        get => GetValue(_target, nameof(_target.Value), 0, nameof(OnValueUpdated));
        set => SetValue(_target, nameof(_target.Value), value, nameof(OnValueUpdated));
    }
}
```

### Usage Examples: Bindable Command

```cs
public class BindableComponent : BindableObject
{
    private readonly IObservableCommand _command;

    private int _value;

    public BindableComponent()
    {
        _command = new ObservableCommand<string>(ExecuteCommand);
    }

    public override void Subscribe()
    {
        _command.Subscribe(this, nameof(Value));
    }

    private void ExecuteCommand(string parameter)
    {
    }

    public int Value
    {
        get => GetValue(ref _value);
        set => SetValue(ref _value, value);
    }
}
```

### Usage Examples: Event Broker

```cs
public class BindableComponent : ObservableObject
{
    private readonly IDataEventBroker _events;

    public BindableComponent(IDataEventBroker events)
    {
        _events = events;
    }

    public override void Subscribe()
    {
        _events.Subscribe("pipe-1", OnChannelEvent);
    }

    public override void Unsubscribe()
    {
        _events.Unsubscribe("pipe-1", OnChannelEvent);
    }

    private void OnChannelEvent(DataEventArgs args)
    {
        _events.Publish("pipe-2", $"Channel: {args.ChannelName}, Value: {args.Value}");
    }
}
```

### Usage Examples

- Example of GUI application for .NET Core: https://github.com/alexanderkozlenko/avalonia-puzzle-15