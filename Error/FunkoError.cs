namespace FunkosApi.Error;

public record FunkoError(
    string Error)
{
    public string Error { get; set; } = Error;
};
public record FunkoNotFoundError(string error) : FunkoError(error);
public record FunkoBadRequestError(string error) : FunkoError(error);
public record FunkoValidationError(string error) : FunkoError(error);

public class FileSizeExceededException : Exception
{
    public FileSizeExceededException(string s)
    {
        throw new NotImplementedException();
    }
}

public class InvalidFileTypeException : Exception
{
    public InvalidFileTypeException(string s)
    {
        throw new NotImplementedException();
    }
}