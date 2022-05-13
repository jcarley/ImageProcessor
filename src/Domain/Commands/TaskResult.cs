namespace Domain.Commands;

public abstract class TaskResultBase<T> where T : TaskResultBase<T>
{
    protected readonly List<string> _errors = new();

    public bool Succeeded { get; protected set; }

    public IEnumerable<string> Errors => _errors;

    public static T Success()
    {
        T taskResult = Activator.CreateInstance<T>();

        taskResult.Succeeded = true;

        return taskResult;
    }

    public static T Failed(params string[] errors)
    {
        T taskResult = Activator.CreateInstance<T>();
        taskResult.Succeeded = false;
        if (errors is { Length: > 0 })
        {
            taskResult._errors.AddRange(errors);
        }

        return taskResult;
    }
}

public abstract class TaskResult<T, TResult> : TaskResultBase<T> where T : TaskResult<T, TResult>
{
    public TResult? Data { get; protected set; }

    public static T Success(TResult data)
    {
        T taskResult = Activator.CreateInstance<T>();

        taskResult.Succeeded = true;
        taskResult.Data = data;

        return taskResult;
    }
}