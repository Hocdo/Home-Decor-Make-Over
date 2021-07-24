using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialTask 
{
    public TutorialTask()
    {
        TutorialManager.Instance.AddTask(this);
    }

    public Func<bool> Predicate;

    public UnityAction OnPredicateTrue;

    public UnityAction OnStart;

    public UnityAction OnTrigger;

    public bool finish = false;

    public bool pending;

    public bool active;

    public float startTime;

    public float exitTime;

    public bool ShouldBePending()
    {
        pending = true;
        if (Predicate != null)
        {
            pending = Predicate.Invoke();
        }

        if (pending) OnPredicateTrue?.Invoke();

        return pending;
    }

    public virtual void Start()
    {
        pending = false;
        active = true;

        OnStart?.Invoke();      
    }

    public virtual void Finish()
    {
        active = false;
        finish = true;
        
    }

    public virtual void Update(float dt)
    {
        
    }
}

public class PressButtonTask : TutorialTask
{
    public Button triggerButton;

    public override void Start()
    {
        base.Start();

        triggerButton.onClick.AddListener(OnTrigger);
        triggerButton.onClick.AddListener(Finish);
    }

    public override void Finish()
    {
        base.Finish();

        triggerButton.onClick.RemoveAllListeners();
    }
}

public class TouchTask : TutorialTask
{
    public Image touchedPanel;

    public override void Start()
    {
        base.Start();        
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => OnTrigger());
        entry.callback.AddListener((data) => Finish());
        touchedPanel.GetComponent<EventTrigger>().triggers.Add(entry);
    }

    public override void Finish()
    {
        base.Finish();

        touchedPanel.GetComponent<EventTrigger>().triggers.Clear();
    }
}

public class MoveTask : TutorialTask
{
    public enum Side
    {
        Top, Bottom, Left, Right
    }

    public float mininumDragMagnitude;

    public float maxBeginTouchDistance;

    public Vector2 startLocation;

    public Side side;

    public Image touchedPanel;

    private Vector2 beginTouchLocation;

    public override void Start()
    {
        base.Start();

        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerDown;
        entry1.callback.AddListener((data) => 
        {
            PointerEventData pointerData = data as PointerEventData;
            beginTouchLocation = pointerData.position;

        });
        touchedPanel.GetComponent<EventTrigger>().triggers.Add(entry1);

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.Drag;
        entry2.callback.AddListener((data) => 
        {
            PointerEventData pointerData = data as PointerEventData;
              
            if ((beginTouchLocation - startLocation).sqrMagnitude <= maxBeginTouchDistance * maxBeginTouchDistance)
            {
                Vector2 position = pointerData.position;
                Vector2 delta = position - beginTouchLocation;

                if (delta.sqrMagnitude > mininumDragMagnitude * mininumDragMagnitude)
                {
                    float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
                    if (angle < 0f) angle += 360f;

                    bool result = false;

                    if (angle > 45f && angle < 135f && side == Side.Top) result = true;
                    else if (angle > 135f && angle < 225 && side == Side.Left) result = true;
                    else if (angle > 225 && angle < 315 && side == Side.Bottom) result = true;
                    else if (side == Side.Right) result = true;

                    if (result)
                    {
                        OnTrigger();
                        Finish();
                    }
                }
            }
        });
        touchedPanel.GetComponent<EventTrigger>().triggers.Add(entry2);
    }

    public override void Finish()
    {
        base.Finish();

        touchedPanel.GetComponent<EventTrigger>().triggers.Clear();
    }
}
