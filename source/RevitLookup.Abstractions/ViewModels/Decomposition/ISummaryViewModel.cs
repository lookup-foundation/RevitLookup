using CommunityToolkit.Mvvm.Input;
using RevitLookup.Abstractions.Decomposition;

namespace RevitLookup.Abstractions.ViewModels.Decomposition;

/// <summary>
///     Defines a contract that represents the data for the Summary views.
/// </summary>
public interface ISummaryViewModel
{
    /// <summary>
    ///     Gets or sets the search query.
    /// </summary>
    string SearchText { get; set; }

    /// <summary>
    ///     Gets or sets the selected decomposed object.
    /// </summary>
    ObservableDecomposedObject? SelectedDecomposedObject { get; set; }

    /// <summary>
    ///     Gets or sets the list of decomposed objects.
    /// </summary>
    List<ObservableDecomposedObject> DecomposedObjects { get; set; }

    /// <summary>
    ///     Gets the command that evaluates a member on demand.
    /// </summary>
    IAsyncRelayCommand<ObservableDecomposedMember> ForceEvaluateMemberCommand { get; }

    /// <summary>
    ///     Gets the command that evaluates a member on demand inside a Revit transaction.
    /// </summary>
    IAsyncRelayCommand<ObservableDecomposedMember> EvaluateMemberWithTransactionCommand { get; }

    /// <summary>
    ///     Decomposes the members of <see cref="SelectedDecomposedObject" />, bypassing the cache.
    /// </summary>
    /// <returns>A task that represents the asynchronous refresh operation.</returns>
    Task RefreshMembersAsync();

    /// <summary>
    ///     Decomposes the specified value and navigates to it.
    /// </summary>
    /// <param name="value">The object to decompose, which can be <see langword="null" />.</param>
    void Navigate(object? value);

    /// <summary>
    ///     Navigates to the specified decomposed object.
    /// </summary>
    /// <param name="value">The decomposed object to navigate to.</param>
    void Navigate(ObservableDecomposedObject value);

    /// <summary>
    ///     Navigates to the specified collection of decomposed objects.
    /// </summary>
    /// <param name="values">The decomposed objects to navigate to.</param>
    void Navigate(List<ObservableDecomposedObject> values);
}
