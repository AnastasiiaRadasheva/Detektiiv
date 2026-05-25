using DetektivGame.Models;
using Microsoft.Maui.Controls.Shapes;

namespace DetektivGame.Views;

public class GamePage : ContentPage
{
    private Label _roomNameLabel = null!;
    private Label _timerLabel = null!;
    private Button _settingsBtn = null!;
    private Button _homeBtn = null!;
    private Button _hintBtn = null!;
    private Grid _topBar = null!;
    private Grid _invBar = null!;
    private Label _invTitle = null!;
    private Button _craftBtn = null!;
    private AbsoluteLayout _objectsLayout = null!;
    private Button _leftBtn = null!, _rightBtn = null!;
    private Ellipse _dot0 = null!, _dot1 = null!, _dot2 = null!;
    private HorizontalStackLayout _inventoryLayout = null!;
    private Border _msgBorder = null!;
    private Label _msgLabel = null!;
    private Grid _roomGrid = null!;

    private readonly Game _game;
    private CancellationTokenSource? _msgCts;
    private bool _isNavigating = false;
    private DateTime _startTime;
    private System.Timers.Timer? _gameTimer;

    public GamePage(string playerName, Theme? theme = null)
    {
        NavigationPage.SetHasNavigationBar(this, false);
        _game = new Game(playerName);
        _game.Player.InventoryChanged += () => MainThread.BeginInvokeOnMainThread(RefreshInventory);
        BuildUI();
        ApplyTheme(theme ?? Theme.Dark);
        RenderRoom();
        RefreshInventory();
        _startTime = DateTime.Now;
        StartGameTimer();
    }

    private void StartGameTimer()
    {
        _gameTimer = new System.Timers.Timer(1000) { AutoReset = true };
        _gameTimer.Elapsed += (s, e) =>
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var el = DateTime.Now - _startTime;
                _timerLabel.Text = $"{(int)el.TotalMinutes:00}:{el.Seconds:00}";
            });
        _gameTimer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _gameTimer?.Stop();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _gameTimer?.Start();
    }

    private void BuildUI()
    {
        _homeBtn = new Button { Text = "←", FontSize = 16, BackgroundColor = Colors.Transparent, Padding = new Thickness(4) };
        _homeBtn.Clicked += async (s, e) =>
        {
            _gameTimer?.Stop();
            await Navigation.PopAsync();
        };

        _roomNameLabel = new Label { FontSize = 17, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
        _timerLabel = new Label { Text = "00:00", FontSize = 13, Margin = new Thickness(8, 0), VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };

        _hintBtn = new Button { Text = "?", FontSize = 16, BackgroundColor = Colors.Transparent, Padding = new Thickness(4) };
        _hintBtn.Clicked += async (s, e) => await ShowMsg(GetNextHint(), true);

        _settingsBtn = new Button { Text = "...", FontSize = 16, BackgroundColor = Colors.Transparent, Padding = new Thickness(4) };
        _settingsBtn.Clicked += OnSettingsClicked;

        _topBar = new Grid { Padding = new Thickness(8, 10) };
        _topBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        _topBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        _topBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        _topBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        _topBar.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        _topBar.Add(_homeBtn, 0, 0);
        _topBar.Add(_roomNameLabel, 1, 0);
        _topBar.Add(_timerLabel, 2, 0);
        _topBar.Add(_hintBtn, 3, 0);
        _topBar.Add(_settingsBtn, 4, 0);

        _objectsLayout = new AbsoluteLayout
        {
            InputTransparent = false,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        _leftBtn = MakeNavBtn("◀");
        _rightBtn = MakeNavBtn("▶");
        _leftBtn.Clicked += async (s, e) => { if (!_isNavigating) { _isNavigating = true; await SlideOut(true); _game.GoLeft(); SlideIn(true); RenderRoom(); RefreshInventory(); _isNavigating = false; } };
        _rightBtn.Clicked += async (s, e) => { if (!_isNavigating) { _isNavigating = true; await SlideOut(false); _game.GoRight(); SlideIn(false); RenderRoom(); RefreshInventory(); _isNavigating = false; } };

        var navGrid = new Grid
        {
            InputTransparent = false,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        navGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        navGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        navGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        navGrid.Add(new Grid { InputTransparent = true }, 1, 0);
        navGrid.Add(_leftBtn, 0, 0);
        navGrid.Add(_rightBtn, 2, 0);
        _leftBtn.VerticalOptions = LayoutOptions.Center;
        _rightBtn.VerticalOptions = LayoutOptions.Center;

        _dot0 = MakeDot(); _dot1 = MakeDot(); _dot2 = MakeDot();
        var dots = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 0, 10),
            Spacing = 10,
            InputTransparent = true,
            Children = { _dot0, _dot1, _dot2 }
        };

        _roomGrid = new Grid { BackgroundColor = Colors.Transparent };
        _roomGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        _roomGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        _roomGrid.Add(_objectsLayout, 0, 0);
        _roomGrid.Add(navGrid,        0, 0);
        _roomGrid.Add(dots,           0, 0);

        _invTitle = new Label { Text = "Inventar", FontSize = 13, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
        _craftBtn = new Button
        {
            Text = "Craft",
            TextColor = Colors.Black,
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 14,
            Padding = new Thickness(14, 6)
        };
        _craftBtn.Clicked += OnCraftClicked;

        var invTopRow = new Grid();
        invTopRow.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        invTopRow.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        invTopRow.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        invTopRow.Add(_invTitle, 0, 0);
        invTopRow.Add(_craftBtn, 2, 0);

        _inventoryLayout = new HorizontalStackLayout { Spacing = 6, Padding = new Thickness(4, 2) };
        var invScroll = new ScrollView { Orientation = ScrollOrientation.Horizontal, HeightRequest = 110, Content = _inventoryLayout };

        _invBar = new Grid { Padding = new Thickness(10, 8) };
        _invBar.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        _invBar.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        _invBar.Add(invTopRow, 0, 0);
        _invBar.Add(invScroll, 0, 1);

        _msgLabel = new Label { FontSize = 14, HorizontalTextAlignment = TextAlignment.Center, MaxLines = 2 };
        _msgBorder = new Border
        {
            Padding = new Thickness(14, 10),
            Content = _msgLabel,
            IsVisible = false,
            StrokeThickness = 1,
            StrokeShape = new Rectangle(),
            VerticalOptions = LayoutOptions.End,
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(8, 0, 8, 10),
            InputTransparent = true
        };
        _roomGrid.Add(_msgBorder, 0, 0);

        var root = new Grid();
        root.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        root.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        root.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        root.Add(_topBar, 0, 0);
        root.Add(_roomGrid, 0, 1);
        root.Add(_invBar, 0, 2);
        Content = root;
    }

    private Button MakeNavBtn(string txt) => new()
    {
        Text = txt,
        FontSize = 28,
        BackgroundColor = Color.FromArgb("#88000000"),
        TextColor = Colors.White,
        CornerRadius = 36,
        WidthRequest = 56,
        HeightRequest = 56,
        Margin = new Thickness(8)
    };

    private Ellipse MakeDot() => new()
    {
        WidthRequest = 10,
        HeightRequest = 10,
        Fill = new SolidColorBrush(Color.FromArgb("#555555"))
    };

    private void ApplyTheme(Theme t)
    {
        _game.SetTheme(t);
        BackgroundColor = t.BackgroundColor;
        _topBar.BackgroundColor = t.CardColor;
        _roomNameLabel.TextColor = t.AccentColor;
        _timerLabel.TextColor = t.TextColor;
        _homeBtn.TextColor = t.AccentColor;
        _hintBtn.TextColor = t.AccentColor;
        _settingsBtn.TextColor = t.AccentColor;
        _invBar.BackgroundColor = t.InventoryColor;
        _invTitle.TextColor = t.AccentColor;
        _craftBtn.BackgroundColor = t.AccentColor;
        _leftBtn.TextColor = t.AccentColor;
        _rightBtn.TextColor = t.AccentColor;
        _msgBorder.BackgroundColor = t.CardColor;
        _msgBorder.Stroke = new SolidColorBrush(t.AccentColor);
    }

    private void RenderRoom()
    {
        var room = _game.CurrentRoom;
        _roomNameLabel.Text = room.Name;
        _leftBtn.IsVisible = room.HasLeftRoom;
        _rightBtn.IsVisible = room.HasRightRoom;
        UpdateDots();
        RenderObjects();
    }

    private void RenderObjects()
    {
        _objectsLayout.Children.Clear();

        var bg = new Image { Source = _game.CurrentRoom.BackgroundImage, Aspect = Aspect.AspectFill, InputTransparent = true };
        AbsoluteLayout.SetLayoutBounds(bg, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(bg, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.All);
        _objectsLayout.Add(bg);

        foreach (var obj in _game.CurrentRoom.Objects.Where(o => o.IsVisible))
            _objectsLayout.Add(CreateObjectView(obj));
    }

    private View CreateObjectView(RoomObject obj)
    {
        var img = new Image
        {
            Source = obj.Item.ImageSource,
            Aspect = Aspect.AspectFit,
            WidthRequest = obj.Width,
            HeightRequest = obj.Height,
            BackgroundColor = Colors.Transparent
        };

        var namePill = new Border
        {
            BackgroundColor = Color.FromArgb("#BB000000"),
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            Padding = new Thickness(7, 2),
            HorizontalOptions = LayoutOptions.Center,
            InputTransparent = true,
            Content = new Label
            {
                Text = obj.Item.Name,
                FontSize = 11,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center
            }
        };

        var col = new VerticalStackLayout { Spacing = 2, Children = { img, namePill } };

        var tap = new TapGestureRecognizer();
        tap.Tapped += async (s, e) =>
        {
            await col.ScaleTo(1.18, 80, Easing.SpringOut);
            await col.ScaleTo(1.0, 80);
            await HandleObjectTap(obj);
        };
        col.GestureRecognizers.Add(tap);

        AbsoluteLayout.SetLayoutBounds(col, new Rect(obj.X, obj.Y, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
        AbsoluteLayout.SetLayoutFlags(col, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.PositionProportional);

        return col;
    }

    private async Task HandleObjectTap(RoomObject obj)
    {
        var result = _game.InteractWithObject(obj.Item.Id);
        if (result.Success || result.UpdateRoom)
        {
            RenderRoom();
            RefreshInventory();
        }
        await ShowMsg(result.Message, result.Success);

        if (result.IsGameEnd)
        {
            var el = DateTime.Now - _startTime;
            int totalSec = (int)el.TotalSeconds;

            string raw = Preferences.Get("game_results", "");
            var entries = raw.Length > 0
                ? raw.Split(',').Where(x => x.Contains('|')).ToList()
                : new List<string>();
            entries.Insert(0, $"{_game.Player.Name}|{totalSec}");
            Preferences.Set("game_results", string.Join(",", entries.Take(5)));

            _gameTimer?.Stop();
            await DisplayAlert("Palju onnitleme!",
                $"{_game.Player.Name}, avastasid aarde!\n\nAeg: {(int)el.TotalMinutes:00}:{el.Seconds:00}",
                "Tagasi menüüsse");
            await Navigation.PopAsync();
        }
    }
    private string GetNextHint()
    {
        var p = _game.Player;

        if (p.HasItem("key"))
            return "Sul on voti — mine esimesse tuppa ja ava seif!";
        if (!_game.VaseDestroyed && p.HasItem("pickaxe"))
            return "Mine teise tuppa ja purusta kuvsin kirvega.";
        if (p.HasItem("stick_rope") && p.HasItem("stone"))
            return "Mine crafti — sega Kepp nooriga + Kivi = Kirves.";
        if (p.HasItem("stick") && p.HasItem("rope"))
            return "Mine crafti — sega Kepp + Noor = Kepp nooriga.";
        if (!_game.RavenFed && p.HasItem("apple"))
            return "Mine kolmandasse tuppa ja anna ronnile oun.";
        if (!p.HasItem("rope") && !_game.RavenFed)
            return p.HasItem("apple")
                ? "Mine kolmandasse tuppa — anna ronnile oun, saad noori."
                : "Vota esimesest toast oun, siis mine kolmandasse tuppa.";
        if (!p.HasItem("stone"))
            return "Vota teisest toast kivi.";
        if (!p.HasItem("stick") && !p.HasItem("stick_rope") && !p.HasItem("pickaxe"))
            return "Vota esimesest toast kepp.";

        return "Uuri koiki tube tahelepanelikult.";
    }

    private void RefreshInventory()
    {
        _inventoryLayout.Children.Clear();

        foreach (var item in _game.Player.Inventory)
            _inventoryLayout.Add(MakeInvSlot(item));

        int empty = 8 - _game.Player.Inventory.Count;
        for (int i = 0; i < empty; i++)
            _inventoryLayout.Add(MakeEmptySlot());
    }

    private View MakeInvSlot(Item item)
    {
        var img = new Image
        {
            Source = item.ImageSource,
            Aspect = Aspect.AspectFit,
            WidthRequest = 58,
            HeightRequest = 58,
            BackgroundColor = Colors.Transparent
        };
        var lbl = new Label
        {
            Text = item.Name,
            FontSize = 9,
            MaxLines = 1,
            LineBreakMode = LineBreakMode.TailTruncation,
            TextColor = Color.FromArgb("#C8DEFF"),
            HorizontalOptions = LayoutOptions.Center
        };

        var inner = new VerticalStackLayout { Spacing = 3, Children = { img, lbl } };

        var border = new Border
        {
            WidthRequest = 84,
            HeightRequest = 100,
            BackgroundColor = Color.FromArgb("#0E1B2E"),
            Stroke = new SolidColorBrush(Color.FromArgb("#2D5099")),
            StrokeThickness = 1.5,
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            Padding = new Thickness(6, 7, 6, 4),
            Content = inner
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += async (s, e) =>
        {
            await border.ScaleTo(1.18, 70, Easing.SpringOut);
            await border.ScaleTo(1.0, 70);
            await ShowMsg($"{item.Name}: {item.Description}", true);
        };
        border.GestureRecognizers.Add(tap);
        return border;
    }

    private View MakeEmptySlot() => new Border
    {
        WidthRequest = 84,
        HeightRequest = 100,
        BackgroundColor = Color.FromArgb("#070E1A"),
        Stroke = new SolidColorBrush(Color.FromArgb("#152236")),
        StrokeThickness = 1,
        StrokeShape = new RoundRectangle { CornerRadius = 16 }
    };

    private async Task SlideOut(bool left) =>
        await _roomGrid.TranslateTo(left ? 320 : -320, 0, (uint)160, Easing.CubicIn);

    private void SlideIn(bool fromLeft)
    {
        _roomGrid.TranslationX = fromLeft ? -320 : 320;
        _ = _roomGrid.TranslateTo(0, 0, (uint)200, Easing.CubicOut);
    }

    private void UpdateDots()
    {
        var accent = new SolidColorBrush(_game.CurrentTheme.AccentColor);
        var grey = new SolidColorBrush(Color.FromArgb("#333333"));
        _dot0.Fill = _game.CurrentRoomIndex == 0 ? accent : grey;
        _dot1.Fill = _game.CurrentRoomIndex == 1 ? accent : grey;
        _dot2.Fill = _game.CurrentRoomIndex == 2 ? accent : grey;
    }

    private async Task ShowMsg(string text, bool ok)
    {
        _msgCts?.Cancel();
        _msgCts = new CancellationTokenSource();
        var token = _msgCts.Token;
        _msgLabel.Text = text;
        _msgLabel.TextColor = ok ? _game.CurrentTheme.AccentColor : Colors.OrangeRed;
        _msgBorder.IsVisible = true;
        _msgBorder.Opacity = 1;
        try
        {
            await Task.Delay(3000, token);
            await _msgBorder.FadeTo(0, (uint)400);
            if (!token.IsCancellationRequested) _msgBorder.IsVisible = false;
        }
        catch (OperationCanceledException) { }
    }

    private void OnCraftClicked(object? sender, EventArgs e)
    {
        var craftPage = new CraftPage(_game);
        craftPage.Disappearing += OnCraftPageDisappearing;
        Navigation.PushAsync(craftPage);
    }

    private void OnCraftPageDisappearing(object? sender, EventArgs e)
    {
        if (sender is CraftPage cp)
            cp.Disappearing -= OnCraftPageDisappearing;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            RenderRoom();
            RefreshInventory();
        });
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        string? a = await DisplayActionSheet("Seaded", "Tagasi", null, "Tume", "Seepia", "Mets");
        var t = a switch { "Tume" => Theme.Dark, "Seepia" => Theme.Sepia, "Mets" => Theme.Forest, _ => (Theme?)null };
        if (t != null) { ApplyTheme(t); RenderRoom(); RefreshInventory(); }
    }
}
