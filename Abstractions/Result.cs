namespace Spike.Abstractions;

public class Result
{
    private Result(bool isSuccess, Error? resultError)
    {
        if ( isSuccess && resultError != Error.None || !isSuccess && resultError == Error.None)
        {
            throw new ArgumentException("Invalid Result object.");
        }
    }

    private readonly bool isSuccess;
    
    public Result Success() => new(true, Error.None);

    public Result Failure(Error newError) => new(false, newError);
    
    public bool IsSuccess => isSuccess;
}