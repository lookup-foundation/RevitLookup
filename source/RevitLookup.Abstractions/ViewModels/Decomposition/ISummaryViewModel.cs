using CommunityToolkit.Mvvm.Input;
using RevitLookup.Abstractions.ObservableModels.Decomposition;

namespace RevitLookup.Abstractions.ViewModels.Decomposition;

/// <summary>
///     Represents the data for the Summary views.
/// </summary>
public interface ISummaryViewModel
{
    /// <summary>
    ///     The search query.
    /// </summary>
    string SearchText { get; set; }

    /// <summary>
    ///     The selected decomposed object.
    /// </summary>
    ObservableDecomposedObject? SelectedDecomposedObject { get; set; }
    
    /// <summary>
    ///     The list of decomposed objects.
    /// </summary>
    List<ObservableDecomposedObject> DecomposedObjects { get; set; }
    
    /// <summary>
    ///     Evaluate a member on demand.
    /// </summary>
    IAsyncRelayCommand<ObservableDecomposedMember> ForceEvaluateMemberCommand { get; }

    /// <summary>
    ///     Evaluate a member on demand inside a Revit transaction.
    /// </summary>
    IAsyncRelayCommand<ObservableDecomposedMember> EvaluateMemberWithTransactionCommand { get; }

    /// <summary>
    ///     Decompose members of the selected object avoiding cache.
    /// </summary>
    Task RefreshMembersAsync();

    /// <summary>
    ///     Navigate to the selected Decomposed object.
    /// </summary>
    void Navigate(object? value);
    
    /// <summary>
    ///     Navigate to the selected Decomposed object.
    /// </summary>
    void Navigate(ObservableDecomposedObject value);
    
    /// <summary>
    ///     Navigate to the selected Decomposed object.
    /// </summary>
    void Navigate(List<ObservableDecomposedObject> values);
}