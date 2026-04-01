// Copyright (c) Lookup Foundation and Contributors
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// THIS PROGRAM IS PROVIDED "AS IS" AND WITH ALL FAULTS.
// NO IMPLIED WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE IS PROVIDED.
// THERE IS NO GUARANTEE THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.

using System.Reflection;
using System.Windows.Input;
using LookupEngine.Abstractions.Configuration;
using LookupEngine.Abstractions.Decomposition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nice3point.Revit.Toolkit.External;
using RevitLookup.Abstractions.Decomposition;
using RevitLookup.Abstractions.Services.Presentation;
using RevitLookup.Abstractions.ViewModels.Decomposition;
using RevitLookup.Core.Decomposition.Extensions;
using RevitLookup.UI.Framework.Extensions;
using RevitLookup.UI.Framework.Views.EditDialogs;
using Wpf.Ui.Controls;
using ContextMenu = System.Windows.Controls.ContextMenu;
using ParameterExtensions = Nice3point.Revit.Extensions.ParameterExtensions;

namespace RevitLookup.Core.Decomposition.Descriptors;

public sealed partial class ParameterDescriptor : Descriptor, IDescriptorResolver, IDescriptorExtension, IContextMenuConnector
{
    private readonly Parameter _parameter;

    public ParameterDescriptor(Parameter parameter)
    {
        _parameter = parameter;
        Name = parameter.Definition.Name;
    }

    public Func<IVariant>? Resolve(string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Parameter.ClearValue) => Variants.Disabled,
            nameof(Parameter.HasValue) => () => _parameter.Element is null ? Variants.Disabled() : Variants.Value(_parameter.HasValue),
            nameof(Parameter.AsString) => () => _parameter.Element is null ? Variants.Disabled() : Variants.Value(_parameter.AsString()),
            _ => null
        };
    }

    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Define(nameof(ParameterExtensions.AsBool)).Register(() => Variants.Value(_parameter.AsBool()));
        manager.Define(nameof(ParameterExtensions.AsColor)).Register(() => Variants.Value(_parameter.AsColor()));
        manager.Define(nameof(FamilyManager.GetAssociatedFamilyParameter)).Register(() => Variants.Value(_parameter.Element?.Document.FamilyManager.GetAssociatedFamilyParameter(_parameter)));
    }

    public void RegisterMenu(ContextMenu contextMenu, IServiceProvider serviceProvider)
    {
        contextMenu.AddMenuItem("EditMenuItem")
            .SetHeader("Edit value")
            .SetAvailability(!_parameter.IsReadOnly && _parameter.StorageType != StorageType.None)
            .SetCommand(_parameter, parameter => EditParameter(parameter, serviceProvider))
            .SetShortcut(Key.F2);
    }

    private static async Task EditParameter(Parameter parameter, IServiceProvider serviceProvider)
    {
        try
        {
            var dialog = serviceProvider.GetRequiredService<EditValueDialog>();
            var result = await dialog.ShowAsync(parameter.Definition.Name, parameter.GetStringValue(), "Update the parameter");
            if (result == ContentDialogResult.Primary)
            {
                var parameterValue = dialog.Value; // Share between threads

                await SetParameterValueAsyncEvent.RaiseAsync(parameter, parameterValue);

                var decompositionViewModel = serviceProvider.GetRequiredService<IDecompositionSummaryViewModel>();
                await decompositionViewModel.RefreshMembersAsync();
            }
        }
        catch (Exception exception)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ParameterDescriptor>>();
            var notificationService = serviceProvider.GetRequiredService<INotificationService>();

            logger.LogError(exception, "Update value error");
            notificationService.ShowError("Updating parameter value error", exception);
        }
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private static void SetParameterValue(Parameter parameter, string parameterValue)
    {
        using var transaction = new Transaction(parameter.Element.Document);
        transaction.Start("Set parameter value");

        var result = parameter.TrySetStringValue(parameterValue);
        if (!result)
        {
            throw new ArgumentException("Invalid parameter value");
        }

        transaction.Commit();
    }
}