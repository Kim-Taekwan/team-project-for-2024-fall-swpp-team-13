using NUnit.Framework;
using UnityEngine;

public class FlourShooterTests
{
    public class StubFlourShooter : FlourShooter1
    {
        public override Vector3 GetRandomTargetPosition()
        {
            return new Vector3(3, 0, 3);
        }
    }

    public class SpyFlourShooter : FlourShooter1
    {
        public bool WasLaunchFlourCalled = false;

        public override void LaunchFlour()
        {
            WasLaunchFlourCalled = true; 
        }
    }

    [Test]
    public void Stub_GetRandomTargetPosition_ReturnsFixedPosition()
    {
        var flourShooter = new GameObject().AddComponent<StubFlourShooter>();
        flourShooter.spawnPoint = new GameObject().transform;

        Vector3 target = flourShooter.GetRandomTargetPosition();

        Assert.AreEqual(new Vector3(3, 0, 3), target, "Stub did not return the expected position.");
    }

    [Test]
    public void Spy_LaunchFlour_VerifiesMethodCall()
    {
        var flourShooter = new GameObject().AddComponent<SpyFlourShooter>();

        flourShooter.LaunchFlour();

        Assert.IsTrue(((SpyFlourShooter)flourShooter).WasLaunchFlourCalled, "LaunchFlour was not called.");
    }
}
