using NUnit.Framework;
using UnityEngine;
using System.Collections;

public class FlourUIManagerTests
{
    public class StubFlourUIManager : FlourUIManager1
    {
        public bool WasShowAndHideUICalled = false;

        public override System.Collections.IEnumerator ShowAndHideUI()
        {
            WasShowAndHideUICalled = true; 
            yield break; 
        }
    }

    [Test]
    public void Stub_ShowFlourUI_CallsShowAndHideUI()
    {
        var uiManager = new GameObject("UIManager").AddComponent<StubFlourUIManager>();
        uiManager.flourScreenUI = new GameObject("FlourScreen");

        uiManager.ShowFlourUI();

        Assert.IsTrue(uiManager.WasShowAndHideUICalled, "ShowAndHideUI was not called.");
    }
}
