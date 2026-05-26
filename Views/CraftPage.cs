using DetektivGame.Models;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

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

    private HorizontalStackLayout _invLayout = null!;

    private AbsoluteLayout _rootAbsolute = null!;
    private Border? _ghostView = null;
    private Item? _draggedItem = null;
    private bool _isDragging = false;
    private Point _ghostStartPos = Point.Zero;

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

        var header = new Grid
        {
            BackgroundColor = _game.CurrentTheme.CardColor,
            Padding = new Thickness(16, 14)
        };
        header.Add(new Label
        {
            Text = "Craft",
            TextColor = _game.CurrentTheme.AccentColor,
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center
        });

        _s1Img = MakeSlotImage();
        _s1Name = MakeSlotLabel("Tühi");
        _s1Border = MakeSlot(new VerticalStackLayout
        {
            Spacing = 4,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { _s1Img, _s1Name }
        });
        var t1 = new TapGestureRecognizer();
        t1.Tapped += (_, _) => { ClearSlot1(); RefreshInventory(); UpdateBtn(); };
        _s1Border.GestureRecognizers.Add(t1);

        _s2Img = MakeSlotImage();
        _s2Name = MakeSlotLabel("Tühi");
        _s2Border = MakeSlot(new VerticalStackLayout
        {
            Spacing = 4,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { _s2Img, _s2Name }
        });
        var t2 = new TapGestureRecognizer();
        t2.Tapped += (_, _) => { ClearSlot2(); RefreshInventory(); UpdateBtn(); };
        _s2Border.GestureRecognizers.Add(t2);

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

        _failLabel = new Label
        {
            TextColor = Colors.OrangeRed,
            FontSize = 13,
            HorizontalOptions = LayoutOptions.Center,
            IsVisible = false
        };
        _resultImg = new Image { Aspect = Aspect.AspectFit, HeightRequest = 80, WidthRequest = 80 };
        _resultName = new Label
        {
            TextColor = Colors.LightGreen,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center
        };
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
                    _resultImg,
                    _resultName
                }
            }
        };

        var craftCard = MakeCard(new VerticalStackLayout
        {
            Spacing = 16,
            Children =
            {
                new Label
                {
                    Text = "Lohista ese slotti  •  tüki slotile = eemalda",
                    TextColor = Color.FromArgb("#667788"),
                    FontSize = 12,
                    HorizontalOptions = LayoutOptions.Center
                },
                slotsRow,
                _craftBtn,
                _failLabel,
                _resultBorder
            }
        });

        _invLayout = new HorizontalStackLayout
        {
            Spacing = 10,
            HorizontalOptions = LayoutOptions.Center
        };

        var invCard = MakeCard(new VerticalStackLayout
        {
            Spacing = 12,
            Children =
            {
                new Label
                {
                    Text = "Inventar",
                    TextColor = _game.CurrentTheme.AccentColor,
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold
                },
                _invLayout
            }
        });

        var backBtn = new Button
        {
            Text = "←  Tagasi mängu",
            BackgroundColor = _game.CurrentTheme.InventoryColor,
            TextColor = _game.CurrentTheme.AccentColor,
            FontSize = 16,
            HeightRequest = 54
        };
        backBtn.Clicked += async (_, _) =>
        {
            if (_isNavigatingBack) return;
            _isNavigatingBack = true;
            await Navigation.PopAsync();
        };

        var contentStack = new VerticalStackLayout
        {
            Padding = new Thickness(16),
            Spacing = 14,
            Children = { craftCard, invCard }
        };

        var pageGrid = new Grid();
        pageGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        pageGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        pageGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        pageGrid.Add(header, 0, 0);
        pageGrid.Add(contentStack, 0, 1);
        pageGrid.Add(backBtn, 0, 2);

        _rootAbsolute = new AbsoluteLayout();
        AbsoluteLayout.SetLayoutBounds(pageGrid, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(pageGrid, AbsoluteLayoutFlags.SizeProportional);
        _rootAbsolute.Add(pageGrid);

        Content = _rootAbsolute;
    }

    private void RefreshInventory()
    {
        _invLayout.Children.Clear();

        var available = _game.Player.Inventory
            .Where(i => i != _slot1 && i != _slot2)
            .ToList();

        if (!available.Any())
        {
        
            return;
        }

        foreach (var item in available)
        {
            var capturedItem = item;

            var img = new Image
            {
                Source = item.ImageSource,
                Aspect = Aspect.AspectFit,
                HeightRequest = 60,
                WidthRequest = 60,
                BackgroundColor = Colors.Transparent 
            };

            var nameLabel = new Label
            {
                Text = item.Name,
                FontSize = 10,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.FromArgb("#C8DEFF"),
                MaxLines = 1,
                LineBreakMode = LineBreakMode.TailTruncation
            };

            var border = new Border
            {
                WidthRequest = 88,
                HeightRequest = 100,
                BackgroundColor = Color.FromArgb("#0E1B2E"),
                Stroke = new SolidColorBrush(Color.FromArgb("#2D5099")),
                StrokeThickness = 1.5,
                StrokeShape = new RoundRectangle { CornerRadius = 16 },
                Padding = new Thickness(6, 8, 6, 6),
                Content = new VerticalStackLayout
                {
                    Spacing = 3,
                    Children = { img, nameLabel }
                }
            };

            AttachDragGesture(border, capturedItem);

            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) =>
            {
                if (_slot1 == null) FillSlot1(capturedItem);
                else if (_slot2 == null) FillSlot2(capturedItem);
                else FillSlot1(capturedItem); 
                RefreshInventory();
                UpdateBtn();
            };
            border.GestureRecognizers.Add(tap);

            _invLayout.Add(border);
        }
    }

    private void AttachDragGesture(Border cardBorder, Item item)
    {
        var pan = new PanGestureRecognizer();

        pan.PanUpdated += async (_, e) =>
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    {
                        if (_isDragging) break;
                        _isDragging = true;

                        _draggedItem = item;
                        _ = cardBorder.FadeTo(0.3, 100);
                        _ghostStartPos = GetAbsolutePosition(cardBorder);

                        _ghostView = new Border
                        {
                            WidthRequest = 88,
                            HeightRequest = 100,
                            BackgroundColor = Color.FromArgb("#2A200A"),
                            Stroke = new SolidColorBrush(Color.FromArgb("#C9A84C")),
                            StrokeThickness = 2,
                            StrokeShape = new RoundRectangle { CornerRadius = 16 },
                            Padding = new Thickness(6, 8, 6, 6),
                            Opacity = 0.9,
                            InputTransparent = true,
                            Content = new VerticalStackLayout
                            {
                                Spacing = 3,
                                Children =
                            {
                                new Image
                                {
                                    Source        = item.ImageSource,
                                    Aspect        = Aspect.AspectFit,
                                    HeightRequest = 60,
                                    WidthRequest  = 60
                                },
                                new Label
                                {
                                    Text              = item.Name,
                                    FontSize          = 10,
                                    HorizontalOptions = LayoutOptions.Center,
                                    TextColor         = Color.FromArgb("#FFE599"),
                                    MaxLines          = 1,
                                    LineBreakMode     = LineBreakMode.TailTruncation
                                }
                            }
                            }
                        };

                        AbsoluteLayout.SetLayoutBounds(
                            _ghostView,
                            new Rect(_ghostStartPos.X, _ghostStartPos.Y, 88, 100));
                        AbsoluteLayout.SetLayoutFlags(_ghostView, AbsoluteLayoutFlags.None);
                        _rootAbsolute.Add(_ghostView);

                        HighlightSlots(true);
                        break;
                    }

                case GestureStatus.Running:
                    {
                        if (_ghostView == null || !_isDragging) break;
                        _ghostView.TranslationX = e.TotalX;
                        _ghostView.TranslationY = e.TotalY;
                        UpdateSlotGlow();
                        break;
                    }

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    {
                        if (!_isDragging) break;
                        _isDragging = false;

                        _ = cardBorder.FadeTo(1.0, 100);
                        HighlightSlots(false);

                        int target = HitTestSlots();
                        if (target == 1 && _draggedItem != null) { FillSlot1(_draggedItem); RefreshInventory(); UpdateBtn(); }
                        if (target == 2 && _draggedItem != null) { FillSlot2(_draggedItem); RefreshInventory(); UpdateBtn(); }

                        if (_ghostView != null)
                        {
                            var g = _ghostView;
                            _ghostView = null;
                            await g.FadeTo(0, 100);
                            _rootAbsolute.Remove(g);
                        }

                        _draggedItem = null;
                        break;
                    }
            }
        };

        cardBorder.GestureRecognizers.Add(pan);
    }

    private Point GetAbsolutePosition(View view)
    {
        double x = 0, y = 0;
        Element? current = view;
        while (current != null && current != _rootAbsolute)
        {
            if (current is VisualElement ve)
            {
                x += ve.Bounds.X;
                y += ve.Bounds.Y;
            }
            current = current.Parent;
        }
        return new Point(x, y);
    }

    private int HitTestSlots()
    {
        if (_ghostView == null) return 0;

        double cx = _ghostStartPos.X + _ghostView.TranslationX + 44;
        double cy = _ghostStartPos.Y + _ghostView.TranslationY + 50;

        var p1 = GetAbsolutePosition(_s1Border);
        var p2 = GetAbsolutePosition(_s2Border);

        if (new Rect(p1.X, p1.Y, _s1Border.Width, _s1Border.Height).Contains(cx, cy) && _slot1 == null)
            return 1;
        if (new Rect(p2.X, p2.Y, _s2Border.Width, _s2Border.Height).Contains(cx, cy) && _slot2 == null)
            return 2;

        return 0;
    }

    private void UpdateSlotGlow()
    {
        int over = HitTestSlots();
        _s1Border.BackgroundColor = (over == 1 && _slot1 == null)
            ? Color.FromArgb("#1A301A")
            : (_slot1 == null ? Color.FromArgb("#0D1520") : Color.FromArgb("#2A200A"));
        _s2Border.BackgroundColor = (over == 2 && _slot2 == null)
            ? Color.FromArgb("#1A301A")
            : (_slot2 == null ? Color.FromArgb("#0D1520") : Color.FromArgb("#2A200A"));
    }

    private void HighlightSlots(bool on)
    {
        if (on)
        {
            if (_slot1 == null) { _s1Border.Stroke = new SolidColorBrush(Color.FromArgb("#44CC66")); _s1Border.StrokeThickness = 2; }
            if (_slot2 == null) { _s2Border.Stroke = new SolidColorBrush(Color.FromArgb("#44CC66")); _s2Border.StrokeThickness = 2; }
        }
        else
        {
            _s1Border.Stroke = new SolidColorBrush(_slot1 == null ? Color.FromArgb("#3A5080") : Color.FromArgb("#C9A84C"));
            _s1Border.StrokeThickness = _slot1 == null ? 1.5 : 2;
            _s2Border.Stroke = new SolidColorBrush(_slot2 == null ? Color.FromArgb("#3A5080") : Color.FromArgb("#C9A84C"));
            _s2Border.StrokeThickness = _slot2 == null ? 1.5 : 2;
        }
    }

    private void FillSlot1(Item i)
    {
        if (_slot2 == i) ClearSlot2();

        _slot1 = i;
        _s1Img.Source = i.ImageSource;
        _s1Name.Text = i.Name;
        _s1Border.BackgroundColor = Color.FromArgb("#2A200A");
        _s1Border.Stroke = new SolidColorBrush(Color.FromArgb("#C9A84C"));
        _s1Border.StrokeThickness = 2;
    }

    private void FillSlot2(Item i)
    {
        if (_slot1 == i) ClearSlot1();

        _slot2 = i;
        _s2Img.Source = i.ImageSource;
        _s2Name.Text = i.Name;
        _s2Border.BackgroundColor = Color.FromArgb("#2A200A");
        _s2Border.Stroke = new SolidColorBrush(Color.FromArgb("#C9A84C"));
        _s2Border.StrokeThickness = 2;
    }

    private void ClearSlot1()
    {
        _slot1 = null;
        _s1Img.Source = null;
        _s1Name.Text = "Tühi";
        _s1Border.BackgroundColor = Color.FromArgb("#0D1520");
        _s1Border.Stroke = new SolidColorBrush(Color.FromArgb("#3A5080"));
        _s1Border.StrokeThickness = 1.5;
    }

    private void ClearSlot2()
    {
        _slot2 = null;
        _s2Img.Source = null;
        _s2Name.Text = "Tühi";
        _s2Border.BackgroundColor = Color.FromArgb("#0D1520");
        _s2Border.Stroke = new SolidColorBrush(Color.FromArgb("#3A5080"));
        _s2Border.StrokeThickness = 1.5;
    }

    private void UpdateBtn() => _craftBtn.IsEnabled = _slot1 != null && _slot2 != null;

    private async void OnCraftClicked(object? sender, EventArgs e)
    {
        if (_slot1 == null || _slot2 == null || _isNavigatingBack) return;

        await _craftBtn.ScaleTo(0.92, 80);
        await _craftBtn.ScaleTo(1.0, 80);

        var result = _game.TryCraft(_slot1.Id, _slot2.Id);

        if (result.Success && result.CraftedItem != null)
        {
            ClearSlot1(); ClearSlot2(); UpdateBtn();
            _failLabel.IsVisible = false;
            _resultImg.Source = result.CraftedItem.ImageSource;
            _resultName.Text = result.CraftedItem.Name;
            _resultBorder.IsVisible = true;
            await _resultBorder.ScaleTo(1.08, 180);
            await _resultBorder.ScaleTo(1.0, 180);
            RefreshInventory();
        }
        else
        {
            _resultBorder.IsVisible = false;
            _failLabel.Text = result.FailReason;
            _failLabel.IsVisible = true;
            await _craftBtn.TranslateTo(-10, 0, 50);
            await _craftBtn.TranslateTo(10, 0, 50);
            await _craftBtn.TranslateTo(-5, 0, 50);
            await _craftBtn.TranslateTo(0, 0, 50);
            await Task.Delay(3000);
            if (_failLabel.IsVisible) _failLabel.IsVisible = false;
        }
    }

    private static Image MakeSlotImage() => new()
    {
        Aspect = Aspect.AspectFit,
        WidthRequest = 70,
        HeightRequest = 70,
        BackgroundColor = Colors.Transparent
    };

    private static Label MakeSlotLabel(string text) => new()
    {
        Text = text,
        FontSize = 12,
        TextColor = Color.FromArgb("#8899AA"),
        HorizontalOptions = LayoutOptions.Center
    };

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