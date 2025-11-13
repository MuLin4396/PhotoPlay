namespace PhotoPlay;

public sealed partial class MainWindow : Microsoft.UI.Xaml.Window
{
	private readonly MainWindow _current;

	private readonly Microsoft.UI.Windowing.AppWindow _appWindow;

	private readonly Microsoft.UI.Windowing.OverlappedPresenter _overlappedPresenter;

	public MainWindow()
	{
		InitializeComponent();

		_current = this;

		SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop { Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt };

		_appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(Microsoft.UI.Win32Interop.GetWindowIdFromWindow(WinRT.Interop.WindowNative.GetWindowHandle(_current)));

		_overlappedPresenter = (Microsoft.UI.Windowing.OverlappedPresenter)_appWindow.Presenter;

		_overlappedPresenter.SetBorderAndTitleBar(true, false);

		ExtendsContentIntoTitleBar = true;

		DevWinUI.DragMoveAndResizeHelper.SetDragMove(_current, MyRectangle);

		_appWindow.Changed += AppWindow_Changed;

		Closed += (s, e) => _appWindow.Changed -= AppWindow_Changed;
	}

	private void AppWindow_Changed(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowChangedEventArgs args)
	{
		if (_overlappedPresenter.State is Microsoft.UI.Windowing.OverlappedPresenterState.Maximized) WindowMaximiseIcon.Glyph     = "\uE923";
		else if (_overlappedPresenter.State is Microsoft.UI.Windowing.OverlappedPresenterState.Restored) WindowMaximiseIcon.Glyph = "\uE922";
	}

	private string GetAppTitleFromSystem()
	{
		string appTitle = Windows.ApplicationModel.Package.Current.DisplayName;
		return appTitle;
	}

	private string GetVersionForBuild()
	{
		string name = Windows.ApplicationModel.Package.Current.PublisherDisplayName;
	#if DEBUG
		string build = "Debug";
	#elif RELEASE
			string build = "Release";
	#endif
		return $"{name} · {build}";
	}

	private void WindowsButton_OnClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs routedEventArgs)
	{
		// Microsoft.UI.Xaml.Controls.Control? control1 = sender as Microsoft.UI.Xaml.Controls.Control;
		dynamic control = sender;
		string? Tag     = control.Tag as string;

		switch (Tag)
		{
			case "WindowClose": Close(); break;
			case "WindowMinimise": _overlappedPresenter.Minimize(); break;
			case "WindowMaximise" when _overlappedPresenter.State == Microsoft.UI.Windowing.OverlappedPresenterState.Restored: _overlappedPresenter.Maximize(); break;
			case "WindowMaximise" when _overlappedPresenter.State == Microsoft.UI.Windowing.OverlappedPresenterState.Maximized: _overlappedPresenter.Restore(); break;
			case "WindowTheme":
				if (_current.Content is Microsoft.UI.Xaml.FrameworkElement rootElement) rootElement.RequestedTheme = (rootElement.ActualTheme == Microsoft.UI.Xaml.ElementTheme.Light ? Microsoft.UI.Xaml.ElementTheme.Dark : Microsoft.UI.Xaml.ElementTheme.Light);
				break;
		}
	}

	private void Block1_OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		var grid = sender as Microsoft.UI.Xaml.Controls.Grid;
		grid.UpdateLayout();

		MyRectangle.Width  = grid.ActualWidth;
		MyRectangle.Height = grid.ActualHeight;
	}

	private void Block_OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		var textBlock = sender as Microsoft.UI.Xaml.Controls.TextBlock;
		textBlock.UpdateLayout();

		MyBorder.Width  = textBlock.ActualWidth + 12;
		MyBorder.Height = textBlock.ActualHeight + 6;
	}

	private void NavigationBackButton_OnClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		NavigationView.IsPaneOpen = !NavigationView.IsPaneOpen;
		// todo NavigationBackButton_OnClick
	}

	private void NavigationGlobalButton_OnClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		NavigationView.IsPaneOpen = !NavigationView.IsPaneOpen;
	}

	private void NavigationView_OnItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
	{
		string selectedPage = args.InvokedItem.ToString();

		switch (selectedPage)
		{
			case "Home": NavigationViewFrame.Navigate(typeof(Pages.HomePage)); break;
			default: break;
		}
	}
}