using Business.Abstract;
using Common.DTOs;
using Common.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Entities;
using Presentation.Abstract;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModel
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IStructureService _structureService;
        private readonly IEmployeeService _employeeService;
        private readonly IHRService _hrService;
        private readonly IDialogService _dialogService;
        private readonly IConfirmationMediator _confirmationMediator;
        private readonly IErrorMediator _errorMediator;

        public ObservableCollection<StructureNodeViewModel> Nodes { get; } = new();
        private readonly Dictionary<int, StructureNodeViewModel> _nodeLookup = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddNodeCommand), nameof(RenameCommand), nameof(DeleteCommand),
            nameof(SelectEmployeeCommand), nameof(HireEmployeeCommand), nameof(DismissEmployeeCommand))]
        [NotifyPropertyChangedFor(nameof(IsPositionSelected), nameof(HasEmployee), 
            nameof(HasAssignedEmployee), nameof(HasNoAssignedEmployee), nameof(DisplayLastName), 
            nameof(DisplayFirstName), nameof(DisplayMiddleName))]
        private StructureNodeViewModel? selectedNode;
        
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(HireEmployeeCommand), nameof(CancelSelectCommand))]
        [NotifyPropertyChangedFor(nameof(HasPendingAssignment), nameof(HasAssignedEmployee), nameof(HasNoAssignedEmployee))]
        private int? pendingEmployeeId;
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayLastName))]
        private string? pendingLastName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayFirstName))]
        private string? pendingFirstName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayMiddleName))]
        private string? pendingMiddleName;

        [ObservableProperty]
        private DateTime hireDate;

        private bool IsNodeSelected => SelectedNode != null;
        public bool IsPositionSelected => IsNodeSelected && SelectedNode!.NodeType == NodeType.Position;
        private bool IsNotPositionSelected => IsNodeSelected && SelectedNode!.NodeType != NodeType.Position;
        private bool IsNotRootSelected => IsNodeSelected && SelectedNode?.ParentId != null;

        public bool HasPendingAssignment => PendingEmployeeId != null;
        public bool HasEmployee => SelectedNode?.Employee != null;
        public bool HasAssignedEmployee => HasPendingAssignment || HasEmployee;
        public bool HasNoAssignedEmployee => !HasAssignedEmployee;

        private bool CanHireEmployee => IsPositionSelected && HasPendingAssignment;
        private bool CanDismissEmployee => IsPositionSelected && SelectedNode?.Employee != null;

        public string? DisplayLastName => PendingLastName ?? SelectedNode?.Employee?.LastName;
        public string? DisplayFirstName => PendingFirstName ?? SelectedNode?.Employee?.FirstName;
        public string? DisplayMiddleName => PendingMiddleName ?? SelectedNode?.Employee?.MiddleName;

        public MainViewModel(IStructureService structureService, IEmployeeService employeeService,
            IHRService hRService, IDialogService dialogService, IConfirmationMediator confirmationMediator,
            IErrorMediator errorMediator) : base(errorMediator)
        {
            _structureService = structureService;
            _employeeService = employeeService;
            _hrService = hRService;
            _dialogService = dialogService;
            _confirmationMediator = confirmationMediator;
            _errorMediator = errorMediator;
            HireDate = DateTime.Today;
            _ = LoadTreeAsync();
        }

        private async Task LoadTreeAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var nodes = await _structureService.GetAllAsync();
                BuildTree(nodes);
            });
        }

        private void BuildTree(IReadOnlyList<StructureNodeDto> nodes)
        {
            _nodeLookup.Clear();
            Nodes.Clear();

            foreach (var node in nodes)
                _nodeLookup[node.Id] = new StructureNodeViewModel(node);

            foreach (var node in nodes)
            {
                var vm = _nodeLookup[node.Id];
                if (node.ParentId == null)
                {
                    Nodes.Add(vm);
                }
                else
                {
                    var parent = _nodeLookup[node.ParentId.Value];
                    parent.Children.Add(vm);
                }
            }
        }

        private void RefreshNodeDependentProperties()
        {
            OnPropertyChanged(nameof(DisplayLastName));
            OnPropertyChanged(nameof(DisplayFirstName));
            OnPropertyChanged(nameof(DisplayMiddleName));
            OnPropertyChanged(nameof(HasEmployee));
            OnPropertyChanged(nameof(HasAssignedEmployee));
            OnPropertyChanged(nameof(HasNoAssignedEmployee));
            DismissEmployeeCommand.NotifyCanExecuteChanged();
        }

        private void ClearPending()
        {
            PendingEmployeeId = null;
            PendingLastName = null;
            PendingFirstName = null;
            PendingMiddleName = null;
        }

        partial void OnSelectedNodeChanged(StructureNodeViewModel? value)
        {
            ClearPending();
            HireDate = value?.HireDate?.ToDateTime(new TimeOnly(0, 0)) ?? DateTime.Today;
        }

        [RelayCommand(CanExecute = nameof(IsNotPositionSelected))]
        private async Task AddNodeAsync()
        {
            var newNodeViewModel = new NewNodeViewModel(_structureService, _errorMediator, SelectedNode!.Id);
            if (_dialogService.ShowDialog(newNodeViewModel))
            {
                var newChildNode = new StructureNodeViewModel(newNodeViewModel.SavedNode!);
                SelectedNode.Children.Add(newChildNode);
                _nodeLookup.Add(newChildNode.Id, newChildNode);

                newChildNode.IsSelected = true;
            }
        }

        [RelayCommand(CanExecute = nameof(IsNotRootSelected))]
        private async Task RenameAsync()
        {
            var renameViewModel = new RenameNodeViewModel(_structureService, _errorMediator, 
                SelectedNode!.Id, SelectedNode!.Name);

            if (_dialogService.ShowDialog(renameViewModel))
            {
                SelectedNode.Name = renameViewModel.Name!;
            }
        }

        [RelayCommand(CanExecute = nameof(IsNotRootSelected))]
        private async Task DeleteAsync()
        {
            if (_confirmationMediator.Confirm($"Удалить '{SelectedNode!.Name}'?"))
            {
                await SafeExecuteAsync(async () =>
                {
                    await _structureService.DeleteAsync(SelectedNode!.Id);

                    var parent = _nodeLookup[SelectedNode!.ParentId!.Value];
                    _nodeLookup.Remove(SelectedNode.Id);
                    parent?.Children.Remove(SelectedNode);
                });
            }
        }

        [RelayCommand]
        private void OpenEmplDirectory() =>
            _dialogService.ShowDialog(new EmplDirectoryViewModel(_employeeService, _hrService, 
                _dialogService, _confirmationMediator, _errorMediator));

        [RelayCommand(CanExecute = nameof(IsPositionSelected))]
        private void SelectEmployee()
        {
            var emplDirectoryViewModel = new EmplDirectoryViewModel(_employeeService, _hrService,
                _dialogService, _confirmationMediator, _errorMediator, SelectedNode!.Id);
            if (_dialogService.ShowDialog(emplDirectoryViewModel))
            {
                var selectedEmployee = emplDirectoryViewModel.SelectedEmployee;

                if (selectedEmployee != null)
                {
                    PendingLastName = selectedEmployee.LastName;
                    PendingFirstName = selectedEmployee.FirstName;
                    PendingMiddleName = selectedEmployee.MiddleName;
                    PendingEmployeeId = selectedEmployee.Id;
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanHireEmployee))]
        private async Task HireEmployeeAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await _hrService.HireEmployeeAsync(SelectedNode!.Id,
                    PendingEmployeeId!.Value,
                    DateOnly.FromDateTime(HireDate));

                SelectedNode.Employee = await _employeeService.GetByIdAsync(PendingEmployeeId!.Value);
                SelectedNode.HireDate = DateOnly.FromDateTime(HireDate);

                ClearPending();
                RefreshNodeDependentProperties();
            });
        }

        [RelayCommand(CanExecute = nameof(CanDismissEmployee))]
        private async Task DismissEmployeeAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await _hrService.DismissEmployeeAsync(SelectedNode!.Id);

                SelectedNode.Employee = null;
                SelectedNode.HireDate = null;
                HireDate = DateTime.Today;

                RefreshNodeDependentProperties();
            });
        }

        [RelayCommand(CanExecute = nameof(HasPendingAssignment))]
        private void CancelSelect()
        {
            ClearPending();
        }
    }
}
