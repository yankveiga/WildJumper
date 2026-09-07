using NUnit.Framework;
using UnityEngine;

public class ControllerScenarioTests
{
    [Test]
    public void RecycleMovesPlatformForwardAndUpdatesOffset()
    {
        GameObject controllerObject = new GameObject("controller");
        GameObject platform = new GameObject("platform");
        controller_scenario controller = controllerObject.AddComponent<controller_scenario>();
        controller.offset = 64;

        controller.recycle(platform, 32);

        Assert.AreEqual(new Vector3(0f, 0f, 64f), platform.transform.position);
        Assert.AreEqual(96, controller.offset);

        Object.DestroyImmediate(platform);
        Object.DestroyImmediate(controllerObject);
    }

    [Test]
    public void RecycleScenarioMovesScenarioForwardAndUpdatesOffset()
    {
        GameObject controllerObject = new GameObject("controller");
        GameObject scenario = new GameObject("scenario");
        controller_scenario controller = controllerObject.AddComponent<controller_scenario>();
        controller.offset_scenario = 300;

        controller.recycle_scenario(scenario, 145, 10.7f);

        Assert.AreEqual(10.7f, scenario.transform.position.x);
        Assert.AreEqual(0.0001f, scenario.transform.position.y, 0.00001f);
        Assert.AreEqual(300f, scenario.transform.position.z);
        Assert.AreEqual(445, controller.offset_scenario);

        Object.DestroyImmediate(scenario);
        Object.DestroyImmediate(controllerObject);
    }

    [Test]
    public void ControllerStartsWithExpectedEnvironmentSteps()
    {
        GameObject controllerObject = new GameObject("controller");
        controller_scenario controller = controllerObject.AddComponent<controller_scenario>();

        Assert.AreEqual(4, controller.environmentSteps.Count);
        Assert.AreEqual("desert", controller.environmentSteps[0].name);
        Assert.AreEqual(1400f, controller.environmentSteps[0].distanceToStart);
        Assert.AreEqual(3, controller.environmentSteps[0].firstScenarioIndex);
        Assert.AreEqual("mountain", controller.environmentSteps[3].name);

        Object.DestroyImmediate(controllerObject);
    }
}
