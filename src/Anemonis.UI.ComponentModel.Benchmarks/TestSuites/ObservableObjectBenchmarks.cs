﻿using System.ComponentModel;

using Anemonis.UI.ComponentModel.Benchmarks.TestStubs;

using BenchmarkDotNet.Attributes;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestSuites
{
    public class ObservableObjectBenchmarks
    {
        private readonly TestObserverObject<PropertyChangedEventArgs> _observerObject = new();
        private readonly TestObservableObject _observableObject = new();
        private readonly TestObservableObject _observableObject0 = new();
        private readonly TestObservableObject _observableObject1 = new();
        private readonly TestObservableObject _observableObject2 = new();
        private readonly TestObservableObject _observableObject3 = new();

        public ObservableObjectBenchmarks()
        {
            _observableObject1.PropertyChanged += OnPropertyChanged;
            _observableObject2.Subscribe(_observerObject);
            _observableObject3.PropertyChanged += OnPropertyChanged;
            _observableObject3.Subscribe(_observerObject);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        [Benchmark(Description = "Subscribe-Unsubscribe")]
        public void SubscribeUnsubscribe()
        {
            _observableObject.Subscribe(_observerObject).Dispose();
        }

        [Benchmark(Description = "RaisePropertyChanged-NPC=N-OBS=N")]
        public void RaisePropertyChanged()
        {
            _observableObject0.InvokeRaisePropertyChanged("Value");
        }

        [Benchmark(Description = "RaisePropertyChanged-NPC=Y-OBS=N")]
        public void RaisePropertyChangedWithEventSubscriber()
        {
            _observableObject1.InvokeRaisePropertyChanged("Value");
        }

        [Benchmark(Description = "RaisePropertyChanged-NPC=N-OBS=Y")]
        public void RaisePropertyChangedWithObserverSubscriber()
        {
            _observableObject2.InvokeRaisePropertyChanged("Value");
        }

        [Benchmark(Description = "RaisePropertyChanged-NPC=Y-OBS=Y")]
        public void RaisePropertyChangedWithEventAndObserverSubscribers()
        {
            _observableObject3.InvokeRaisePropertyChanged("Value");
        }
    }
}
