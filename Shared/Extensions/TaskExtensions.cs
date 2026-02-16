namespace Remotely.Shared.Extensions;

public static class TaskExtensions
{
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
            catch
            {
                // Swallow handler exceptions to prevent secondary crashes in fire-and-forget
            }
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
            catch
            {
                // Swallow handler exceptions to prevent secondary crashes in fire-and-forget
            }
        }
    }
}
