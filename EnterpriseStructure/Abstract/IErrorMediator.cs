namespace Presentation.Abstract
{
    public interface IErrorMediator
    {
        void ShowError(string message, string title = "Ошибка");
    }
}
