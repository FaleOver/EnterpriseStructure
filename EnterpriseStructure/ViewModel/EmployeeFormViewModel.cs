using Business.Abstract;
using Common.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.Abstract;
using System.Windows;

namespace Presentation.ViewModel
{
    public partial class EmployeeFormViewModel : BaseViewModel
    {
        private readonly IEmployeeService _employeeService;

        private int? _employeeId = null;
        private readonly string? _oldLastName;
        private readonly string? _oldFirstName;
        private readonly string? _oldMiddleName;

        public EmployeeDto? SavedEmployee { get; private set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string? lastName;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string? firstName;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string? middleName;

        private bool IsEmpty =>
            string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(FirstName) ||
            string.IsNullOrWhiteSpace(MiddleName);
        private bool IsModified => 
            (LastName != _oldLastName || 
            FirstName != _oldFirstName || 
            MiddleName != _oldMiddleName) && !IsEmpty;

        public EmployeeFormViewModel(IEmployeeService employeeService, IErrorMediator errorMediator, 
            EmployeeDto? employee = null) : base(errorMediator)
        {
            _employeeService = employeeService;

            if (employee != null)
            {
                _employeeId = employee.Id;

                LastName = employee.LastName;
                FirstName = employee.FirstName;
                MiddleName = employee.MiddleName;
            }

            _oldLastName = LastName;
            _oldFirstName = FirstName;
            _oldMiddleName = MiddleName;
        }

        [RelayCommand(CanExecute = nameof(IsModified))]
        private async Task SaveAsync(Window window)
        {
            await SafeExecuteAsync(async () =>
            {
                string cleanLastName = LastName!.Trim();
                string cleanFirstName = FirstName!.Trim();
                string cleanMiddleName = MiddleName!.Trim();

                if (_employeeId == null)
                    SavedEmployee = await _employeeService.CreateAsync(cleanLastName!, cleanFirstName!, cleanMiddleName!);
                else
                    SavedEmployee = await _employeeService.UpdateAsync(_employeeId!.Value, cleanLastName!, 
                        cleanFirstName!, cleanMiddleName!);

                if (window != null)
                    window.DialogResult = true;
            });
        }
    }
}
