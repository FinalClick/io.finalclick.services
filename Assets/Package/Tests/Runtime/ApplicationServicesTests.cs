using NUnit.Framework;
using UnityEngine;

namespace FinalClick.Services.Tests.Runtime
{
    public class ApplicationServicesTests
    {
        [Test]
        public void HasStarted_ShouldReturnTrue_WhenPlayModeIsEntered()
        {
            Assert.IsTrue(ApplicationServices.HasStarted(), "ApplicationServices should automatically be started after entering play mode.");
        }
        
                
        [Test]
        public void ApplicationServicesUpdaterCreated_ShouldReturnTrue_WhenPlayModeIsEntered()
        {
            Assert.IsTrue(ApplicationServices.HasStarted(), "ApplicationServices should automatically be started after entering play mode.");
            Assert.IsNotNull(Object.FindFirstObjectByType<ApplicationServicesUpdater>(), "ApplicationServicesUpdater should be created");
        }
        
                        
        [Test]
        public void ApplicationServicesMarkerCreated_ShouldReturnTrue_WhenPlayModeIsEntered()
        {
            Assert.IsTrue(ApplicationServices.HasStarted(), "ApplicationServices should automatically be started after entering play mode.");
            Assert.IsNotNull(Object.FindFirstObjectByType<ApplicationServicesMarker>(), "ApplicationServicesMarker should be created");
        }
    }
}