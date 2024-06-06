namespace RuneRealm.Tasks;

public class RSTaskScheduler
{
    public List<RSTask> Tasks { get; private set; } = new();

    public RSTaskScheduler()
    {
        
    }
    
    public void AddTask(RSTask task)
    {
        Tasks.Add(task);
    }

    public void ProcessTasks()
    {
        bool hasStrongTask = Tasks.Any(t => t.Type == QueueType.Strong);

        if (hasStrongTask)
        {
            // CloseModal();
            RemoveWeakTasks();
        }

        foreach (var task in Tasks.ToList())
        {
            if (task.Delay > 0)
            {
                task.Decrement();
                continue;
            }

            //if (task.Type == QueueType.Normal) // && HasModalInterface
            //{
            //    continue;
            //}

            //if (task.Type != QueueType.Strong) // && IsDelayed
            //{
            //    continue;
            //}

            if (task.IsReadyToExecute())
            {
                task.Execute();
                Tasks.Remove(task);
            }
        }
    }

    private void RemoveWeakTasks()
    {
        Tasks.RemoveAll(t => t.Type == QueueType.Weak);
    }
}