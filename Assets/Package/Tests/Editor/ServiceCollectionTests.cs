using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace FinalClick.Services.Editor.Tests
{
    public class ServiceCollectionTests
    {
        private TestService _testService1;
        private TestService _testService2;
        private ServiceCollection _serviceCollection;

        [SetUp]
        public void SetUp()
        {
            _testService1 = new TestService();
            _testService2 = new TestService();

            var managedServices = new List<IService> { _testService1, _testService2 };
            var registeredServices = new Dictionary<Type, object>
            {
                { typeof(IService), _testService1 }
            };

            _serviceCollection = new ServiceCollection(managedServices, registeredServices);
        }

        [Test]
        public void TryGet_ShouldReturnTrue_WhenServiceIsRegistered()
        {
            var result = _serviceCollection.TryGet<IService>(out var service);

            Assert.IsTrue(result);
            Assert.AreEqual(_testService1, service);
        }

        [Test]
        public void TryGet_ShouldReturnFalse_WhenServiceIsNotRegistered()
        {
            var result = _serviceCollection.TryGet<string>(out var service);

            Assert.IsFalse(result);
            Assert.IsNull(service);
        }

        [Test]
        public void Get_ShouldReturnService_WhenServiceIsRegistered()
        {
            var service = _serviceCollection.Get<IService>();

            Assert.AreEqual(_testService1, service);
        }

        [Test]
        public void Get_ShouldThrow_WhenServiceIsNotRegistered()
        {
            Assert.Throws<InvalidOperationException>(() => _serviceCollection.Get<string>());
        }

        [Test]
        public void StartServices_ShouldCallOnServiceStart_OnAllManagedServices()
        {
            _serviceCollection.StartServices();

            Assert.IsTrue(_testService1.StartCalled);
            Assert.IsTrue(_testService2.StartCalled);
        }

        [Test]
        public void UpdateServices_ShouldCallOnServiceUpdate_OnAllManagedServices()
        {
            _serviceCollection.StartServices();
            _serviceCollection.UpdateServices();

            Assert.IsTrue(_testService1.UpdateCalled);
            Assert.IsTrue(_testService2.UpdateCalled);
        }

        [Test]
        public void StopServices_ShouldCallOnServiceStop_OnAllManagedServices()
        {
            _serviceCollection.StartServices();
            _serviceCollection.StopServices();

            Assert.IsTrue(_testService1.StopCalled);
            Assert.IsTrue(_testService2.StopCalled);
        }

        [Test]
        public void StopServices_ShouldNotThrow_IfServicesNotStarted()
        {
            Assert.DoesNotThrow(() => _serviceCollection.StopServices());
        }

        private class TestService : IService
        {
            public bool StartCalled { get; private set; }
            public bool UpdateCalled { get; private set; }
            public bool StopCalled { get; private set; }

            public void OnServiceStart() => StartCalled = true;
            public void OnServiceUpdate() => UpdateCalled = true;
            public void OnServiceStop() => StopCalled = true;
        }
    }
}
