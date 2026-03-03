using Business.Abstract;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.Abstract;
using System.Windows;

namespace Presentation.ViewModel
{
    public partial class RenameNodeViewModel : BaseViewModel
    {
        private readonly IStructureService _structureService;
        private readonly int _id;
        private readonly string _oldName;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RenameNodeCommand))]
        private string? name;

        private bool IsNameChanged => Name != _oldName && !string.IsNullOrWhiteSpace(Name);

        public RenameNodeViewModel(IStructureService structureService, IErrorMediator errorMediator, 
            int Id, string currentName) : base(errorMediator)
        {
            _structureService = structureService;
            _id = Id;
            Name = currentName;
            _oldName = currentName;
        }

        [RelayCommand(CanExecute = nameof(IsNameChanged))]
        private async Task RenameNodeAsync(Window window)
        {
            await SafeExecuteAsync(async () =>
            {
                await _structureService.Rename(_id, Name!.Trim());

                if (window != null)
                    window.DialogResult = true;
            });
        }
    }
}
