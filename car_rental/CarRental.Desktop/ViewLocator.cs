using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CarRental.Desktop.ViewModels;

namespace CarRental.Desktop;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;
        
        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        Console.WriteLine($"[DEBUG] ViewLocator: Param={param.GetType().Name}, TargetView={name}, TypeFound={type != null}");

        if (type != null)
        {
            try {
                var view = (Control)Activator.CreateInstance(type)!;
                return view;
            } catch (Exception ex) {
                var inner = ex.InnerException != null ? $"\nInner Error: {ex.InnerException.Message}\nStack: {ex.InnerException.StackTrace}" : "";
                Console.WriteLine($"[ERROR] ViewLocator failed to create {name}: {ex.Message}{inner}");
                return new TextBlock { Text = $"Error creating {name}: {ex.Message}" };
            }
        }
        
        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
