using NUnit.Framework;
using UnityEngine;

public class ElectricWhiskTests
{
    public class StubElectricWhisk : ElectricWhisk1
    {
        public override void ApplyPushBasedOnDistance()
        {
            player.GetComponent<Rigidbody>().AddForce(Vector3.forward * 100f, ForceMode.Impulse);
        }
    }

    public class SpyElectricWhisk : ElectricWhisk1
    {
        public bool WasApplyVibrationCalled = false;

        public override void ApplyVibration()
        {
            WasApplyVibrationCalled = true;
            base.ApplyVibration();
        }
    }

    [Test]
    public void ApplyPushBasedOnDistance_AppliesForceWhenPlayerIsInRange()
    {
        var whisk = new GameObject("Whisk").AddComponent<ElectricWhisk1>();
        var whiskCenter = new GameObject("WhiskCenter");
        var player = new GameObject("Player");

        whisk.whiskCenter = whiskCenter.transform;
        whisk.player = player.transform;

        var playerRigidbody = player.AddComponent<Rigidbody>();
        playerRigidbody.mass = 1.0f;

        whiskCenter.transform.position = Vector3.zero; 
        player.transform.position = new Vector3(10, 0, 0);

        whisk.pushRadius = 15f; 
        whisk.pushForce = 50f;  

        whisk.ApplyPushBasedOnDistance();

        Vector3 expectedDirection = (player.transform.position - whiskCenter.transform.position).normalized;

        Assert.AreEqual(expectedDirection.x * whisk.pushForce, playerRigidbody.velocity.x, 50f, "X-Axis Force is incorrect.");
        Assert.AreEqual(expectedDirection.z * whisk.pushForce, playerRigidbody.velocity.z, 0f, "Z-Axis Force is incorrect.");
        Assert.AreEqual(expectedDirection.z * whisk.pushForce, playerRigidbody.velocity.y, 0f, "Y-Axis Force is incorrect."); 
    }

    [Test]
    public void ApplyPushBasedOnDistance_DoesNotApplyForceWhenPlayerIsOutOfRange()
    {
        var whisk = new GameObject("Whisk").AddComponent<ElectricWhisk1>();
        var whiskCenter = new GameObject("WhiskCenter");
        var player = new GameObject("Player");

        whisk.whiskCenter = whiskCenter.transform;
        whisk.player = player.transform;

        var playerRigidbody = player.AddComponent<Rigidbody>();

        whiskCenter.transform.position = Vector3.zero; 
        player.transform.position = new Vector3(30, 0, 0); 

        whisk.pushRadius = 15f; 
        whisk.pushForce = 50f;  

        whisk.ApplyPushBasedOnDistance();

        Assert.AreEqual(Vector3.zero, playerRigidbody.velocity, "Force should not be applied when player is out of range.");
    }



    [Test]
    public void Spy_ApplyVibration_VerifiesMethodCall()
    {
        var whisk = new GameObject().AddComponent<SpyElectricWhisk>();

        whisk.whiskCenter = new GameObject("WhiskCenter").transform;
        whisk.player = new GameObject("Player").transform;
        whisk.player.gameObject.AddComponent<Rigidbody>();

        whisk.Update();

        Assert.IsTrue(((SpyElectricWhisk)whisk).WasApplyVibrationCalled, "ApplyVibration was not called during Update.");
}

}
