namespace TaskTrack.Service.Exceptions;

public sealed class AccountHasCreatedTasksException : Exception
{
    public AccountHasCreatedTasksException() : base("This account cannot be deleted because it has created tasks.")
    {
    }
}