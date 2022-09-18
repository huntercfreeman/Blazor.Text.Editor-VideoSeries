using System.ComponentModel;
using BlazorTextEditor.ClassLib.TreeView;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TreeView;

public partial class TreeViewDisplay<TItem> : ComponentBase, IDisposable
{
    [Parameter, EditorRequired]
    public TreeViewModel<TItem> TreeViewModel { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment<TreeViewModel<TItem>> ItemRenderFragment { get; set; } = null!;
    [Parameter]
    public Func<TreeViewModel<TItem>>? GetRootFunc { get; set; }
    [Parameter]
    public int Depth { get; set; }

    private const int PADDING_LEFT_PER_DEPTH_IN_PIXELS = 16;
    
    private TreeViewModel<TItem>? _previousTreeViewModel;
    
    private TreeViewModel<TItem> Root => GetRootFunc is null
        ? TreeViewModel
        : GetRootFunc.Invoke();

    private string IsActiveDescendantClassCss => IsActiveDescendant
        ? "bte_active"
        : string.Empty;
    
    private bool IsActiveDescendant =>
        Root.ActiveDescendant is not null && Root.ActiveDescendant.Id == TreeViewModel.Id;

    public int PaddingLeft => Depth * PADDING_LEFT_PER_DEPTH_IN_PIXELS;

    protected override Task OnParametersSetAsync()
    {
        if (_previousTreeViewModel is null ||
            _previousTreeViewModel.Id != TreeViewModel.Id)
        {
            if (_previousTreeViewModel is not null)
                _previousTreeViewModel.OnStateChanged -= TreeViewModelOnStateChanged;

            TreeViewModel.OnStateChanged += TreeViewModelOnStateChanged;
        }
        
        return base.OnParametersSetAsync();
    }

    private void ToggleIsExpandedOnClick()
    {
        TreeViewModel.IsExpanded = !TreeViewModel.IsExpanded;
        FireAndForgetLoadChildren();
    }
    
    private void FireAndForgetLoadChildren()
    {
        _ = Task.Run(async () =>
        {
            await TreeViewModel.LoadChildrenFuncAsync.Invoke(TreeViewModel);
            await InvokeAsync(StateHasChanged);
        });
    }

    private void SetActiveDescendantOnClick()
    {
        SetActiveDescendantAndRerender();
    }
    
    private void SetActiveDescendantAndRerender()
    {
        var previousActiveDescendant = Root.ActiveDescendant; 
        
        Root.ActiveDescendant = TreeViewModel;

        if (previousActiveDescendant is not null)
            previousActiveDescendant.InvokeOnStateChanged(false);
        
        Root.ActiveDescendant.InvokeOnStateChanged(true);
    }
    
    private async void TreeViewModelOnStateChanged(object? sender, bool e)
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        TreeViewModel.OnStateChanged -= TreeViewModelOnStateChanged;
    }
}