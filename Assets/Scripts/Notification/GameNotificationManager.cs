#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
#endif
public class GameNotificationManager : SingletonMonoBehaviour<GameNotificationManager>
{
#if UNITY_ANDROID
    private AndroidNotificationChannel channel;

    public void Initialize()
    {
        channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();

        SendNotification_8h();
        SendNotification_20h();
        SendNotification_9h_to_20h();
        SendNotification_Rentention_3_7_Day();
        SendNotification_8h_DailyReward();

    }

    public void SendNotification_8h()
    {
        float hour_8h = 8f;
        float hour_interval = 24f;

        string[] messages_8h = new string[5]
        {
            "messages_8h_1",
            "messages_8h_2",
            "messages_8h_3",
            "messages_8h_4",
            "messages_8h_5"
        };

        var notification_8h = new AndroidNotification();
        notification_8h.Title = CustomLocalization.Get("good_morning");
        notification_8h.Text = CustomLocalization.Get(messages_8h[UnityEngine.Random.Range(0, messages_8h.Length)]);
        notification_8h.RepeatInterval = System.TimeSpan.FromDays(1);

        var now = System.DateTime.Now;
        var today_8h = now.Date.AddHours(hour_8h);
        if (now > today_8h)
        {
            notification_8h.FireTime = today_8h.AddHours(hour_interval);
        }
        else
        {
            notification_8h.FireTime = today_8h;
        }

        AndroidNotificationCenter.SendNotification(notification_8h, channel.Id);
    }

    public void SendNotification_20h()
    {
        float hour_20h = 20f;
        float hour_interval = 24f;

        string[] messages_20h = new string[5]
        {
            "messages_20h_1",
            "messages_20h_2",
            "messages_20h_3",
            "messages_20h_4",
            "messages_20h_5"
        };

        var notification_20h = new AndroidNotification();
        notification_20h.Title = CustomLocalization.Get("good_evening");
        notification_20h.Text = CustomLocalization.Get(messages_20h[UnityEngine.Random.Range(0, messages_20h.Length)]);
        notification_20h.RepeatInterval = System.TimeSpan.FromDays(1);

        var now = System.DateTime.Now;
        var today_20h = now.Date.AddHours(hour_20h);
        if (now > today_20h)
        {
            notification_20h.FireTime = today_20h.AddHours(hour_interval);
        }
        else
        {
            notification_20h.FireTime = today_20h;
        }

        AndroidNotificationCenter.SendNotification(notification_20h, channel.Id);
    }

    public void SendNotification_9h_to_20h()
    {
        float hour_moment = UnityEngine.Random.Range(9f, 20f);
        float hour_interval = 24f;

        var notification = new AndroidNotification();
        notification.Title = CustomLocalization.Get("free_gem_noti_tile");
        notification.Text = CustomLocalization.Get("free_gem_noti_text");
        notification.RepeatInterval = System.TimeSpan.FromDays(1);

        var now = System.DateTime.Now;
        var today_hour_moment = now.Date.AddHours(hour_moment);
        if (now > today_hour_moment)
        {
            notification.FireTime = today_hour_moment.AddHours(hour_interval);
        }
        else
        {
            notification.FireTime = today_hour_moment;
        }

        AndroidNotificationCenter.SendNotification(notification, channel.Id);
    }

    public void SendNotification_Rentention_3_7_Day()
    {
        var notificatio_3day = new AndroidNotification();
        notificatio_3day.Title = "Home Design Makeover";
        notificatio_3day.Text = CustomLocalization.Get("3day_noti_text");
        notificatio_3day.FireTime = System.DateTime.Now.AddDays(3);

        AndroidNotificationCenter.SendNotification(notificatio_3day, channel.Id);

        var notificatio_7day = new AndroidNotification();
        notificatio_7day.Title = "Home Design Makeover";
        notificatio_7day.Text = CustomLocalization.Get("7day_noti_text");
        notificatio_7day.FireTime = System.DateTime.Now.AddDays(7);

        AndroidNotificationCenter.SendNotification(notificatio_7day, channel.Id);
    }

    public void SendNotification_8h_DailyReward()
    {
        //float hour_8h = 8f;
        //float hour_interval = 24f;

        //var notification= new AndroidNotification();
        //notification.Title = "Home Design Master";
        //notification.Text = "Your daily reward is ready to grab. Come back and get it now!";
        //notification.RepeatInterval = System.TimeSpan.FromDays(1);

        //var now = System.DateTime.Now;
        //var today_8h = now.Date.AddHours(hour_8h);
        //if (now > today_8h)
        //{
        //    notification.FireTime = today_8h.AddHours(hour_interval);
        //}
        //else
        //{
        //    notification.FireTime = today_8h;
        //}

        //AndroidNotificationCenter.SendNotification(notification, channel.Id);
    }

    public void OnApplicationQuit()
    {
        Stamina stamina = PlayerData.current.stamina;
        float infinityRemainingTime = stamina.GetInfinityRemainingTime();
        float remainingTime = stamina.GetRemainingTime();

        if (infinityRemainingTime > 300f)
        {
            var noti = new AndroidNotification();
            noti.Title = "Home Design Makeover";
            noti.Text = CustomLocalization.Get("5m_inf_live");
            noti.FireTime = System.DateTime.Now.AddSeconds(infinityRemainingTime - 300f);

            AndroidNotificationCenter.SendNotification(noti, channel.Id);
        }

        if (remainingTime > 0f)
        {
            float notiFireTimeDelay = Mathf.Max(0f, (Stamina.MaxStaminaCount - 1 - stamina.count) * Stamina.StaminaFillDuration + remainingTime);

            var noti = new AndroidNotification();
            noti.Title = "Home Design Makeover";
            noti.Text = CustomLocalization.Get("full_live");
            noti.FireTime = System.DateTime.Now.AddSeconds(notiFireTimeDelay);

            AndroidNotificationCenter.SendNotification(noti, channel.Id);
        }

        if (StarterPackUtility.Available())
        {
            float starterPackRemainingTime = StarterPackUtility.GetRemainingTimeInSeconds();
            if (starterPackRemainingTime > 12f * 3600f)
            {
                var noti = new AndroidNotification();
                noti.Title = "Home Design Makeover";
                noti.Text = CustomLocalization.Get("12h_starter_pack");
                noti.FireTime = System.DateTime.Now.AddSeconds(starterPackRemainingTime - 12 * 3600f);

                AndroidNotificationCenter.SendNotification(noti, channel.Id);
            }
        }

        float remainingFreeSpinAdsDuration = SpinUtility.GetRemainingFreeSpinAdsDuration();
        if (remainingFreeSpinAdsDuration > 0f)
        {
            var noti = new AndroidNotification();
            noti.Title = "Home Design Makeover";
            noti.Text = CustomLocalization.Get("free_spin_available");
            noti.FireTime = System.DateTime.Now.AddSeconds(remainingFreeSpinAdsDuration);

            AndroidNotificationCenter.SendNotification(noti, channel.Id);
        }
    }
#endif
}
