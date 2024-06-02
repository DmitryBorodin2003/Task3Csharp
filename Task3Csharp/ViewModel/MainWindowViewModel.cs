using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using Task3CSharp.Models;
using Microsoft.Win32;
using System.Windows;

namespace Task3CSharp.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private FlyingVehicleLoader _loader;
    private Type _selectedType;
    private MethodInfo _selectedMethod;
    private string _methodResult;
    private string _assemblyPath;

    public ObservableCollection<string> FigureNames { get; set; }
    public ObservableCollection<MethodInfo> Methods { get; set; }
    public ObservableCollection<ParameterViewModel> ConstructorParameters { get; set; }
    public ICommand LoadAssemblyCommand { get; set; }
    public ICommand ExecuteMethodCommand { get; set; }

    public string AssemblyPath
    {
        get => _assemblyPath;
        set
        {
            _assemblyPath = value;
            OnPropertyChanged(nameof(AssemblyPath));
        }
    }

    public string SelectedFigureName
    {
        get => _selectedType?.Name;
        set
        {
            _selectedType = _loader.FlyingVehiclesTypes.Find(t => t.Name == value);
            Methods.Clear();
            ConstructorParameters.Clear();
            foreach (var method in _loader.GetMethods(_selectedType))
            {
                Methods.Add(method);
            }

            var constructor = _selectedType.GetConstructors().First();
            foreach (var param in constructor.GetParameters())
            {
                ConstructorParameters.Add(new ParameterViewModel { Name = param.Name, Type = param.ParameterType });
            }
        }
    }

    public MethodInfo SelectedMethod
    {
        get => _selectedMethod;
        set
        {
            _selectedMethod = value;
            OnPropertyChanged(nameof(SelectedMethod));
        }
    }

    public string MethodResult
    {
        get => _methodResult;
        set
        {
            _methodResult = value;
            OnPropertyChanged(nameof(MethodResult));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public MainWindowViewModel()
    {
        _loader = new FlyingVehicleLoader();
        FigureNames = new ObservableCollection<string>();
        Methods = new ObservableCollection<MethodInfo>();
        ConstructorParameters = new ObservableCollection<ParameterViewModel>();
        LoadAssemblyCommand = new RelayCommand(LoadAssembly);
        ExecuteMethodCommand = new RelayCommand(ExecuteMethod);
    }

    private Visibility _beforeLoadVisibility = Visibility.Visible;
    public Visibility BeforeLoadVisibility
    {
        get { return _beforeLoadVisibility; }
        set
        {
            _beforeLoadVisibility = value;
            OnPropertyChanged(nameof(BeforeLoadVisibility));
        }
    }

    private Visibility _afterLoadVisibility = Visibility.Collapsed;
    public Visibility AfterLoadVisibility
    {
        get { return _afterLoadVisibility; }
        set
        {
            _afterLoadVisibility = value;
            OnPropertyChanged(nameof(AfterLoadVisibility));
        }
    }

    private void LoadAssembly(object parameter)
    {
        var openFileDialog = new OpenFileDialog();
        if (openFileDialog.ShowDialog() == true)
        {
            AssemblyPath = openFileDialog.FileName;
            _loader.LoadAssembly(AssemblyPath);
            FigureNames.Clear();
            foreach (var type in _loader.FlyingVehiclesTypes)
            {
                FigureNames.Add(type.Name);
            }

            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Width = 350;
                mainWindow.Height = 550;
            }

            BeforeLoadVisibility = Visibility.Collapsed;
            AfterLoadVisibility = Visibility.Visible;
        }
    }

    private void ExecuteMethod(object parameter)
    {
        if (_selectedType != null && _selectedMethod != null)
        {
            var constructor = _selectedType.GetConstructors().First();
            var constructorArgs = ConstructorParameters.Select(p => ConvertParameterValue(p.Type, p.Value)).ToArray();
            var instance = constructor.Invoke(constructorArgs);

            var result = _selectedMethod.Invoke(instance, Array.Empty<object>());

            MethodResult = result == null ? "" : result.ToString();
        }
    }

    private object ConvertParameterValue(Type type, object value)
    {
        try
        {
            return Convert.ChangeType(value, type);
        } catch (Exception ex) {
            System.Windows.MessageBox.Show("Поля для ввода не должны быть пустыми", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        return null;
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}