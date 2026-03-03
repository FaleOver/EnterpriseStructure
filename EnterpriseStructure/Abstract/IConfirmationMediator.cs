using System;
using System.Collections.Generic;
using System.Text;

namespace Presentation.Abstract
{
    public interface IConfirmationMediator
    {
        bool Confirm(string message, string title = "Подтверждение");
    }
}
