using Business.Abstract;
using Common.DTOs;
using Common.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.Abstract;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace Presentation.ViewModel
{
    public partial class NewNodeViewModel : BaseViewModel
    {
        private readonly IStructureService _structureService;
        private readonly int _parentId;

        public NodeType[] NodeTypes { get; } = Enum.GetValues<NodeType>();
        public StructureNodeDto? SavedNode { get; private set; }

        [ObservableProperty]
        private NodeType selectedNodeType;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CreateNodeCommand))]
        private string? name;

        private bool CanCreate => !string.IsNullOrWhiteSpace(Name);

        public NewNodeViewModel(IStructureService structureService, IErrorMediator errorMediator, 
            int parentId) : base(errorMediator)
        {
            _structureService = structureService;
            _parentId = parentId;
        }

        [RelayCommand(CanExecute = nameof(CanCreate))]
        private async Task CreateNodeAsync(Window window)
        {
            await SafeExecuteAsync(async () =>
            {
                SavedNode = await _structureService.CreateAsync(Name!, SelectedNodeType, _parentId);

                if (window != null)
                    window.DialogResult = true;
            });
        }
    }
}
