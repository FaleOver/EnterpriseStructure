namespace Presentation.Abstract
{
    public interface IConfirmationMediator
    {
        bool Confirm(string message, string title = "Подтверждение");
    }
}
