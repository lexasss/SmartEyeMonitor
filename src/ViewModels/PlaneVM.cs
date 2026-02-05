using SmartEyeMonitor.Models;
using System.ComponentModel;

namespace SmartEyeMonitor.ViewModels;

public class PlaneVM : INotifyPropertyChanged
{
    public string Name => _plane.Name;

    public bool IsEnabled
    {
        get => _plane.IsEnabled;
        set
        {
            if (!value && IsFocused)
            {
                _plane.Exit();
            }

            _plane.IsEnabled = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
        }
    }

    public bool IsFocused
    {
        get => _plane.IsFocused;
        set
        {
            _plane.IsFocused = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFocused)));
        }
    }

    public long Counter
    {
        get => field;
        private set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Counter)));
        }
    } = 0;

    public event PropertyChangedEventHandler? PropertyChanged;

    public PlaneVM(Plane plane)
    {
        _plane = plane;
        _plane.Focused += Plane_Focused;
        _plane.Left += Plane_Left;

        if (plane.IsFocused)
        {
            Plane_Focused(this, EventArgs.Empty);
        }
    }

    // Internal

    readonly Plane _plane;

    long _startMs = 0;

    private void Plane_Focused(object? sender, EventArgs e)
    {
        _startMs = Services.Timestamp.Ms + 1;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFocused)));
    }

    private void Plane_Left(object? sender, EventArgs e)
    {
        if (_startMs > 0)
        {
            Counter += Services.Timestamp.Ms - _startMs;
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFocused)));
    }
}
