using NUnit.Framework;
using UnityEngine.SceneManagement;

namespace FinalClick.Services.Tests.Runtime
{
    public class SceneServicesTests
    {
        [Test]
        public void HasStarted_ShouldReturnTrue_WhenPlayModeIsEntered()
        {
            var activeScene = SceneManager.GetActiveScene();
            
            Assert.IsTrue(ApplicationServices.HasStarted(), "ApplicationServices should automatically be started after entering play mode.");
            Assert.IsTrue(SceneServices.HasStartedForScene(activeScene), "SceneServices should return true when service is registered");
        }
    }
}