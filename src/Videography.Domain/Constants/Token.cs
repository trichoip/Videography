namespace Videography.Domain.Constants;
public abstract class Token
{
    public const string RefreshToken = nameof(RefreshToken);
    public const string Bearer = nameof(Bearer);
    public const string ResetPassword = nameof(ResetPassword);
    public const int ExpiresIn = 3;

}
