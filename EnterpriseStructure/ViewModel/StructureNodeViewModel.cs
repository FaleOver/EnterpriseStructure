using Common.DTOs;
using Common.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Presentation.ViewModel
{
    public partial class StructureNodeViewModel : ObservableObject
    {
        public int Id { get; }
        public NodeType NodeType { get; }
        public int? ParentId { get; }
        public ObservableCollection<StructureNodeViewModel> Children { get; } = new();

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private EmployeeDto? employee;

        [ObservableProperty]
        private DateOnly? hireDate;

        [ObservableProperty]
        private bool isSelected;

        public StructureNodeViewModel(StructureNodeDto structureNode)
        {
            Id = structureNode.Id;
            NodeType = structureNode.NodeType;
            ParentId = structureNode.ParentId;
            name = structureNode.Name;
            employee = structureNode.Employee;
            hireDate = structureNode.HireDate;
        }
    }
}
