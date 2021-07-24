using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    public Match3.TutorialView tutorialView;

    private Queue<TutorialTask> taskQueue = new Queue<TutorialTask>();

    private TutorialTask currentTask;

    private bool isOnExitTime = false;

    public void AddTask(TutorialTask task)
    {
        taskQueue.Enqueue(task);

        if (taskQueue.Count == 1 && currentTask == null)
        {
            currentTask = taskQueue.Dequeue();
        }          
    }

    public void ClearTask()
    {
        currentTask = null;
        taskQueue.Clear();  
    }

    public void Update()
    {
        if (currentTask != null)
        {      
            if (currentTask.active)
            {
                currentTask.Update(Time.deltaTime);
            }

            if (currentTask.finish && isOnExitTime == false)
            {
                if (taskQueue.Count > 0)
                {
                    isOnExitTime = true;

                    this.ExecuteAfterSeconds(currentTask.exitTime, () =>
                    {
                        isOnExitTime = false;
                        currentTask = taskQueue.Dequeue();
                    }); 
                }
                else
                {
                    currentTask = null;
                    return;
                }         
            }

            if (!currentTask.pending && !currentTask.active && !currentTask.finish)
            {
                if (currentTask.ShouldBePending())
                {
                    this.ExecuteAfterSeconds(currentTask.startTime, () =>
                    {
                        currentTask.Start();
                    });
                }
            }
        }
    }
}

