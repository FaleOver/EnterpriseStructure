using Business.Abstract;
using Common;
using Common.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.SqlServer.Server;
using Presentation.Abstract;
using System.Collections.ObjectModel;
using System.Windows;

namespace Presentation.ViewModel
{
    public partial class EmplDirectoryViewModel : BaseViewModel
    {
        private readonly IEmployeeService _employeeService;
        private readonly IHRService _hrService;
        private readonly IDialogService _dialogService;
        private readonly IConfirmationMediator _confirmationMediator;
        private readonly IErrorMediator _errorMediator;

        public ObservableCollection<EmployeeDto> Employees { get; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SelectCommand))]
        private int? positionId;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditCommand), nameof(DeleteCommand), nameof(SelectCommand))]
        private EmployeeDto? selectedEmployee;

        [ObservableProperty]
        private string? searchLastName;

        [ObservableProperty]
        private string? searchFirstName;

        [ObservableProperty]
        private string? searchMiddleName;

        private CancellationTokenSource? _searchCts;

        private bool IsEmployeeSelected => SelectedEmployee != null;
        public bool CanAssign => PositionId != null && SelectedEmployee != null;
        public bool IsAssignVisible => PositionId != null;

        public EmplDirectoryViewModel(IEmployeeService employeeService, IHRService hrService,
            IDialogService dialogService, IConfirmationMediator confirmationMediator, 
            IErrorMediator errorMediator, int? nodeId = null) : base(errorMediator)
        {
            _employeeService = employeeService;
            _hrService = hrService;
            _dialogService = dialogService;
            _confirmationMediator = confirmationMediator;
            _errorMediator = errorMediator;
            PositionId = nodeId;

            _ = LoadEmployeesAsync();
        }

        partial void OnSearchLastNameChanged(string? value) => ScheduleSearch();

        partial void OnSearchFirstNameChanged(string? value) => ScheduleSearch();

        partial void OnSearchMiddleNameChanged(string? value) => ScheduleSearch();

        private void ScheduleSearch()
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(400, token);
                    if (!token.IsCancellationRequested)
                    {
                        _ = Application.Current.Dispatcher.Invoke(() =>
                            _ = LoadEmployeesAsync());
                    }
                }
                catch (TaskCanceledException) { }
            }, token);
        }

        private async Task LoadEmployeesAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var allEmployees = await _employeeService.SearchAsync(SearchFirstName, SearchMiddleName, SearchLastName);

                Employees.Clear();
                foreach (var employee in allEmployees)
                    Employees.Add(employee);
            });
        }

        [RelayCommand(CanExecute = nameof(CanAssign))]
        private void Select(Window window)
        {
            if (window != null)
                window.DialogResult = true;
        }

        [RelayCommand]
        private void Add()
        {
            var employeeFormViewModel = new EmployeeFormViewModel(_employeeService, _errorMediator);
            if (_dialogService.ShowDialog(employeeFormViewModel) && employeeFormViewModel.SavedEmployee != null)
            {
                var savedEmployee = employeeFormViewModel.SavedEmployee;
                Employees.Add(savedEmployee);

                SelectedEmployee = savedEmployee;
            }
        }

        [RelayCommand(CanExecute = nameof(IsEmployeeSelected))]
        private void Edit()
        {
            var employeeFormViewModel = new EmployeeFormViewModel(_employeeService,
                _errorMediator, SelectedEmployee);
            if (_dialogService.ShowDialog(employeeFormViewModel) && employeeFormViewModel.SavedEmployee != null)
            {
                var savedEmployee = employeeFormViewModel.SavedEmployee;
                Employees[Employees.IndexOf(SelectedEmployee!)] = savedEmployee;
                SelectedEmployee = savedEmployee;
            }
        }

        [RelayCommand(CanExecute = nameof(IsEmployeeSelected))]
        private async Task DeleteAsync()
        {
            if (_confirmationMediator.Confirm($"Удалить '{SelectedEmployee!.FullName}'?"))
            {
                await SafeExecuteAsync(async () =>
                {
                    await _hrService.DeleteEmployeeAsync(SelectedEmployee.Id);
                    Employees.Remove(SelectedEmployee);
                });
            }
        }
    }
}
