using NUnit.Framework;
using UnityEngine;

public class UiControllerTests
{
    [Test]
    public void AddCoinIncrementsCoinEvenWithoutTextReference()
    {
        GameObject uiObject = new GameObject("ui");
        ui_controller ui = uiObject.AddComponent<ui_controller>();

        ui.add_coin();
        ui.add_coin();

        Assert.AreEqual(2, ui.coin);

        Object.DestroyImmediate(uiObject);
    }
}
