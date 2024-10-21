namespace Remotely.Shared.Extensions;

public static class TaskExtensions
{
    /// <summary>
    /// Returns an item wrapped in a completed <see cref="Task{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    public static Task<T> AsTaskResult<T>(this T item)
    {
        return Task.FromResult(item);
    }

    public static async void Forget(this Task task, Func<Exception, Task>? exceptionHandler = null)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            if (exceptionHandler is null)
            {
                return;
            }

            try
            {
                await exceptionHandler(ex);
            }
            catch { }
        }
    }

    public static async void Forget<T>(this Task<T> task, Func<Exception, Task>? exceptionHandler = null)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            if (exceptionHandler is null)
            {
                return;
            }

            try
            {
                await exceptionHandler(ex);
            }
            catch { }
        }
    }
}
