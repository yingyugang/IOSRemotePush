/*
 * 需要注意在Xcode的Signing&Capability加上Background Modes - RemoteNotification
 * 还有PushNotification这两个Capability
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PE.Utils;
using Unity.Notifications.iOS;
using UnityEngine;
using UnityEngine.UI;

class TestData
{
    public int index;
    public TestData(int index)
    {
        this.index = index;
    }
}

public class PushTest : MonoBehaviour
{
    public Text text;
    public Text text1;
    // Start is called before the first frame update
    async void Start()
    {
        //await RegisterDevice();
        StartCoroutine(RequestAuthorization());
        var notification = iOSNotificationCenter.GetLastRespondedNotification();
        if (notification != null)
        {
            var msg = "Last Received Notification: " + notification.Identifier;
            msg += "\n - Notification received: ";
            msg += "\n - .Title: " + notification.Title;
            msg += "\n - .Badge: " + notification.Badge;
            msg += "\n - .Body: " + notification.Body;
            msg += "\n - .CategoryIdentifier: " + notification.CategoryIdentifier;
            msg += "\n - .Subtitle: " + notification.Subtitle;
            msg += "\n - .Data: " + notification.Data;
            Debug.LogError(msg);
            text.text = msg;
        }
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        //TestPush();
    }

    private void OnApplicationPause(bool pause)
    {
        var notification = iOSNotificationCenter.GetLastRespondedNotification();
        if (notification != null)
        {
            var msg = "Last Received Notification: " + notification.Identifier;
            msg += "\n - Notification received: ";
            msg += "\n - .Title: " + notification.Title;
            msg += "\n - .Badge: " + notification.Badge;
            msg += "\n - .Body: " + notification.Body;
            msg += "\n - .CategoryIdentifier: " + notification.CategoryIdentifier;
            msg += "\n - .Subtitle: " + notification.Subtitle;
            msg += "\n - .Data: " + notification.Data;
            Debug.LogError(msg);
            text.text = msg;
        }
    }

    List<TestData> list = new List<TestData>();
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            list = new List<TestData>();
            for (int i = 0; i < 1000000; i++)
            {
                list.Add(new TestData(UnityEngine.Random.Range(1, 100000)));
            }

        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            var array = list.ToArray();
            float t = Time.realtimeSinceStartup;
            list.Sort((left, right) => left.index - right.index);
            text.text = (Time.realtimeSinceStartup - t).ToString();
            t = Time.realtimeSinceStartup;
            MergeSort.Sort(array, (left, right) => left.index - right.index);
            text1.text = (Time.realtimeSinceStartup - t).ToString();
        }
    }

    public async Task RegisterDevice()
    {
        var deviceToken = "";
        using (var authorizationRequest = new AuthorizationRequest(
            AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound,
            true))
        {
            while (!authorizationRequest.IsFinished)
            {
                await Task.Delay(10);
                UnityEngine.Debug.Log("DeviceToken:" + deviceToken);
            }
            deviceToken = authorizationRequest.DeviceToken;
            UnityEngine.Debug.Log("DeviceToken Finished:" + deviceToken);
            text1.text = deviceToken;
        }

    }


    IEnumerator RequestAuthorization()
    {
        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };

            string res = "\n RequestAuthorization:";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            Debug.Log(res);
        }
    }

    void TestPush()
    {
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(0, 0, 10),
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            // You can specify a custom identifier which can be used to manage the notification later.
            // If you don't provide one, a unique string will be generated automatically.
            Identifier = "_notification_01",
            Title = "Title",
            Body = "Scheduled at: " + DateTime.Now.ToShortDateString() + " triggered in 5 seconds",
            Subtitle = "This is a subtitle, something, something important...",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);
    }
}
