using DetektivGame.Models;
using Microsoft.Maui.Controls.Shapes;

namespace DetektivGame.Views;

public class CraftPage : ContentPage
{
    private readonly Game _game;
    private Item? _slot1, _slot2;
    private bool _isNavigatingBack = false;

    private Image _s1Img = null!, _s2Img = null!;
    private Label _s1Name = null!, _s2Name = null!;
    private Border _s1Border = null!, _s2Border = null!;
    private Button _craftBtn = null!;
    private Border _resultBorder = null!;
    private Image _resultImg = null!;
    private Label _resultName = null!;
    private Label _failLabel = null!;
    private FlexLayout _invLayout = null!;

    public CraftPage(Game game)
    {
        NavigationPage.SetHasNavigationBar(this, false);
        _game = game;
        BuildUI();
        RefreshInventory();
    }

    private void BuildUI()
    {
        BackgroundColor = _game.CurrentTheme.BackgroundColor;

        // Header
        var header = new Grid { BackgroundColor = _game.CurrentTheme.CardColor, Padding = new Thickness(16, 14) };
        header.Add(new Label
        {
            Text = "Craft",
            TextColor = _game.CurrentTheme.AccentColor,
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center
        });

        // Slot 1
        _s1Img = new Image { Aspect = Aspect.AspectFit, WidthRequest = 70, HeightRequest = 70, BackgroundColor = Colors.Transparent };
        _s1Name = new Label { Text = "Tühi", FontSize = 12, TextColor = Color.FromArgb("#8899AA"), HorizontalOptions = LayoutOptions.Center };
        _s1Border = MakeSlot(new VerticalStackLayout
        {
            Spacing = 4,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { _s1Img, _s1Name }
        });
        _s1Border.GestureRecognizers.Add(MakeClearTap(true));
        AddDropTarget(_s1Border, true);

        // Slot 2
        _s2Img = new Image { Aspect = Aspect.AspectFit, WidthRequest = 70, HeightRequest = 70, BackgroundColor = Colors.Transparent };
        _s2Name = new Label { Text = "Tühi", FontSize = 12, TextColor = Color.FromArgb("#8899AA"), HorizontalOptions = LayoutOptions.Center };
        _s2Border = MakeSlot(new VerticalStackLayout
        {
            Spacing = 4,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { _s2Img, _s2Name }
        });
        _s2Border.GestureRecognizers.Add(MakeClearTap(false));
        AddDropTarget(_s2Border, false);

        var plus = new Label
        {
            Text = "+",
            FontSize = 44,
            TextColor = _game.CurrentTheme.AccentColor,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        var slotsRow = new Grid { ColumnSpacing = 10, HorizontalOptions = LayoutOptions.Center };
        slotsRow.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        slotsRow.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        slotsRow.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        slotsRow.Add(_s1Border, 0, 0);
        slotsRow.Add(plus, 1, 0);
        slotsRow.Add(_s2Border, 2, 0);

        // Craft button
        _craftBtn = new Button
        {
            Text = "Kombineeri!",
            BackgroundColor = _game.CurrentTheme.AccentColor,
            TextColor = Color.FromArgb("#0D1117"),
            FontSize = 17,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 24,
            HeightRequest = 54,
            IsEnabled = false
        };
        _craftBtn.Clicked += OnCraftClicked;

        // Fail + result
        _failLabel = new Label
        {
            Text = "",
            TextColor = Colors.OrangeRed,
            FontSize = 13,
            HorizontalOptions = LayoutOptions.Center,
            IsVisible = false
        };
        _resultImg = new Image { Aspect = Aspect.AspectFit, HeightRequest = 80, WidthRequest = 80, BackgroundColor = Colors.Transparent };
        _resultName = new Label { TextColor = Colors.LightGreen, FontSize = 16, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center };
        _resultBorder = new Border
        {
            IsVisible = false,
            BackgroundColor = Color.FromArgb("#112211"),
            Stroke = new SolidColorBrush(Colors.LightGreen),
            StrokeThickness = 1.5,
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            Padding = new Thickness(20),
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label { Text = "Valmis!", TextColor = Colors.LightGreen, FontSize = 15, HorizontalOptions = LayoutOptions.Center },
                    _resultImg, _resultName
                }
            }
        };

        var craftCard = MakeCard(new VerticalStackLayout
        {
            Spacing = 16,
            Children =
            {
                new Label { Text = "Tüki esemele slotti  •  tüki slotile = eemalda", TextColor = Color.FromArgb("#667788"), FontSize = 12, HorizontalOptions = LayoutOptions.Center },
                slotsRow,
                _craftBtn,
                _failLabel,
                _resultBorder
            }
        });

        // Inventory
        _invLayout = new FlexLayout
        {
            Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
            JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Start,
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center
        };
        var invCard = MakeCard(new VerticalStackLayout
        {
            Spacing = 12,
            Children =
            {
                new Label { Text = "Inventar", TextColor = _game.CurrentTheme.AccentColor, FontSize = 16, FontAttributes = FontAttributes.Bold },
                _invLayout
            }
        });

        // Back
        var backBtn = new Button
        {
            Text = "←  Tagasi mängu",
            BackgroundColor = _game.CurrentTheme.InventoryColor,
            TextColor = _game.CurrentTheme.AccentColor,
            FontSize = 16,
            HeightRequest = 54
        };
        backBtn.Clicked += async (s, e) =>
        {
            if (_isNavigatingBack) return;
            _isNavigatingBack = true;
            await Navigation.PopAsync();
        };

        var root = new Grid();
        root.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        root.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        root.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        root.Add(header, 0, 0);
        root.Add(new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(16),
                Spacing = 14,
                Children = { craftCard, invCard }
            }
        }, 0, 1);
        root.Add(backBtn, 0, 2);
        Content = root;
    }

    // ── Slot helpers ───────────────────────────────────────────────
    private TapGestureRecognizer MakeClearTap(bool isSlot1)
    {
        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) =>
        {
            if (isSlot1) ClearSlot1(); else ClearSlot2();
            RefreshInventory(); UpdateBtn();
        };
        return tap;
    }

    private void AddDropTarget(Border slot, bool isSlot1)
    {
        var drop = new DropGestureRecognizer { AllowDrop = true };
        drop.DragOver += (s, e) => e.AcceptedOperation = DataPackageOperation.Copy;
        drop.Drop += (s, e) =>
        {
            if (e.Data.Properties.TryGetValue("item_id", out var obj) && obj is string id)
            {
                var item = _game.Player.GetItem(id);
                if (item == null) return;
                if (isSlot1) FillSlot1(item); else FillSlot2(item);
                RefreshInventory(); UpdateBtn();
            }
        };
        slot.GestureRecognizers.Add(drop);
    }

    private void FillSlot1(Item i) { _slot1 = i; _s1Img.Source = i.ImageSource; _s1Name.Text = i.Name; _s1Border.BackgroundColor = Color.FromArgb("#2A200A"); _s1Border.Stroke = new SolidColorBrush(Color.FromArgb("#C9A84C")); }
    private void FillSlot2(Item i) { _slot2 = i; _s2Img.Source = i.ImageSource; _s2Name.Text = i.Name; _s2Border.BackgroundColor = Color.FromArgb("#2A200A"); _s2Border.Stroke = new SolidColorBrush(Color.FromArgb("#C9A84C")); }
    private void ClearSlot1() { _slot1 = null; _s1Img.Source = null; _s1Name.Text = "Tühi"; _s1Border.BackgroundColor = Color.FromArgb("#0D1520"); _s1Border.Stroke = new SolidColorBrush(Color.FromArgb("#3A5080")); }
    private void ClearSlot2() { _slot2 = null; _s2Img.Source = null; _s2Name.Text = "Tühi"; _s2Border.BackgroundColor = Color.FromArgb("#0D1520"); _s2Border.Stroke = new SolidColorBrush(Color.FromArgb("#3A5080")); }
    private void UpdateBtn() => _craftBtn.IsEnabled = _slot1 != null && _slot2 != null;

    // ── Inventory ──────────────────────────────────────────────────
    private void RefreshInventory()
    {
        _invLayout.Children.Clear();
        if (!_game.Player.Inventory.Any())
        {
            _invLayout.Add(new Label { Text = "Inventar on tühi.", TextColor = Color.FromArgb("#667788"), FontSize = 13 });
            return;
        }

        foreach (var item in _game.Player.Inventory)
        {
            bool inSlot = (_slot1 == item || _slot2 == item);

            var img = new Image { Source = item.ImageSource, Aspect = Aspect.AspectFit, HeightRequest = 60, WidthRequest = 60, BackgroundColor = Colors.Transparent };
            var name = new Label { Text = item.Name, FontSize = 10, HorizontalOptions = LayoutOptions.Center, TextColor = inSlot ? Color.FromArgb("#FFE599") : Color.FromArgb("#C8DEFF"), MaxLines = 1, LineBreakMode = LineBreakMode.TailTruncation };

            var border = new Border
            {
                WidthRequest = 88,
                HeightRequest = 100,
                Margin = new Thickness(4),
                BackgroundColor = inSlot ? Color.FromArgb("#251A04") : Color.FromArgb("#0E1B2E"),
                Stroke = new SolidColorBrush(inSlot ? Color.FromArgb("#C9A84C") : Color.FromArgb("#2D5099")),
                StrokeThickness = inSlot ? 2 : 1.5,
                StrokeShape = new RoundRectangle { CornerRadius = 16 },
                Padding = new Thickness(6, 8, 6, 6),
                Content = new VerticalStackLayout { Spacing = 3, Children = { img, name } }
            };

            var capturedItem = item;
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                if (_slot1 == capturedItem) ClearSlot1();
                else if (_slot2 == capturedItem) ClearSlot2();
                else if (_slot1 == null) FillSlot1(capturedItem);
                else if (_slot2 == null) FillSlot2(capturedItem);
                else FillSlot1(capturedItem);
                RefreshInventory(); UpdateBtn();
            };
            border.GestureRecognizers.Add(tap);

            var drag = new DragGestureRecognizer { CanDrag = true };
            drag.DragStarting += (s, e) => e.Data.Properties["item_id"] = capturedItem.Id;
            border.GestureRecognizers.Add(drag);

            _invLayout.Add(border);
        }
    }

    // ── Craft ──────────────────────────────────────────────────────
    private async void OnCraftClicked(object? sender, EventArgs e)
    {
        if (_slot1 == null || _slot2 == null || _isNavigatingBack) return;

        await _craftBtn.ScaleTo(0.92, (uint)80);
        await _craftBtn.ScaleTo(1.0, (uint)80);

        var result = _game.TryCraft(_slot1.Id, _slot2.Id);

        if (result.Success && result.CraftedItem != null)
        {
            // Clear slots only on success
            ClearSlot1(); ClearSlot2(); UpdateBtn();
            _failLabel.IsVisible = false;
            _resultImg.Source = result.CraftedItem.ImageSource;
            _resultName.Text = result.CraftedItem.Name;
            _resultBorder.IsVisible = true;
            await _resultBorder.ScaleTo(1.08, (uint)180);
            await _resultBorder.ScaleTo(1.0, (uint)180);
            RefreshInventory();

            // no alert here — result border already shows success; chest awaits in room 1
        }
        else
        {
            // FAIL — slots keep items, show error, shake
            _resultBorder.IsVisible = false;
            _failLabel.Text = result.FailReason;
            _failLabel.IsVisible = true;
            uint d = 50;
            await _craftBtn.TranslateTo(-10, 0, d);
            await _craftBtn.TranslateTo(10, 0, d);
            await _craftBtn.TranslateTo(-5, 0, d);
            await _craftBtn.TranslateTo(0, 0, d);
            // Items stay in slots — no clear
            await Task.Delay(3000);
            if (_failLabel.IsVisible) _failLabel.IsVisible = false;
        }
    }

    // ── Helpers ────────────────────────────────────────────────────
    private Border MakeCard(View content) => new()
    {
        BackgroundColor = Color.FromArgb("#0F1318"),
        Stroke = new SolidColorBrush(Color.FromArgb("#21262D")),
        StrokeThickness = 1,
        StrokeShape = new RoundRectangle { CornerRadius = 18 },
        Padding = new Thickness(18),
        Content = content
    };

    private Border MakeSlot(View content) => new()
    {
        WidthRequest = 120,
        HeightRequest = 110,
        BackgroundColor = Color.FromArgb("#0D1520"),
        Stroke = new SolidColorBrush(Color.FromArgb("#3A5080")),
        StrokeThickness = 1.5,
        StrokeShape = new RoundRectangle { CornerRadius = 16 },
        Padding = new Thickness(8),
        HorizontalOptions = LayoutOptions.Center,
        Content = content
    };
}