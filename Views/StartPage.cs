using DetektivGame.Models;
using Microsoft.Maui.Controls.Shapes;

namespace DetektivGame.Views;

public class StartPage : ContentPage
{
    private Theme _theme = Theme.Dark;
    private Entry _nameEntry = null!;
    private Button _darkBtn = null!, _sepiaBtn = null!, _forestBtn = null!;

    public StartPage()
    {
        Shell.SetNavBarIsVisible(this, false);
        BackgroundColor = Color.FromArgb("#07090F");
        BuildUI();
    }

    private void BuildUI()
    {
        // ── Layered dark background ────────────────────────────────
        var bgGrid = new Grid();
        bgGrid.Add(new BoxView { Color = Color.FromArgb("#07090F") });
        bgGrid.Add(new BoxView { Color = Color.FromArgb("#0E0520"), Opacity = 0.65 });
        bgGrid.Add(new BoxView { Color = Color.FromArgb("#1A0A40"), Opacity = 0.25 });

        // ── Header section ─────────────────────────────────────────
        var eyeLabel = new Label
        {
            Text = "🔍",
            FontSize = 22,
            TextColor = Color.FromArgb("#C9A84C"),
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 6)
        };

        var detective = new Label
        {
            Text = "🕵️",
            FontSize = 100,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10)
        };

        // Title plate
        var titleInner = new VerticalStackLayout
        {
            Spacing = 4,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Label
                {
                    Text = "D E T E K T I I V",
                    FontSize = 30,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#C9A84C"),
                    CharacterSpacing = 6,
                    HorizontalOptions = LayoutOptions.Center
                },
                new BoxView
                {
                    HeightRequest = 1,
                    Color = Color.FromArgb("#C9A84C"),
                    Opacity = 0.5,
                    HorizontalOptions = LayoutOptions.Fill
                },
                new Label
                {
                    Text = "Saladuse Tuba",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#9B7E38"),
                    CharacterSpacing = 3,
                    HorizontalOptions = LayoutOptions.Center
                }
            }
        };

        var titleCard = new Border
        {
            BackgroundColor = Color.FromArgb("#120D26"),
            Stroke = new SolidColorBrush(Color.FromArgb("#C9A84C")),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 28 },
            Padding = new Thickness(36, 18),
            HorizontalOptions = LayoutOptions.Center,
            Content = titleInner
        };

        var subtitle = new Label
        {
            Text = "✦  Uuri. Kogu. Lahenda.  ✦",
            FontSize = 13,
            TextColor = Color.FromArgb("#5C6E80"),
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 12, 0, 0)
        };

        // ── Input card ─────────────────────────────────────────────
        var nameLabel = new Label
        {
            Text = "Sinu nimi",
            TextColor = Color.FromArgb("#C9A84C"),
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(2, 0, 0, 6)
        };

        _nameEntry = new Entry
        {
            Text = "Detektiiv",
            TextColor = Colors.White,
            BackgroundColor = Colors.Transparent,
            FontSize = 18,
            Placeholder = "Sisesta nimi...",
            PlaceholderColor = Color.FromArgb("#3D4E60")
        };

        var nameBox = new Border
        {
            BackgroundColor = Color.FromArgb("#0C1220"),
            Stroke = new SolidColorBrush(Color.FromArgb("#1E2D45")),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 18 },
            Padding = new Thickness(14, 4),
            Content = _nameEntry
        };

        var startBtn = new Button
        {
            Text = "▶  Alusta mängu",
            BackgroundColor = Color.FromArgb("#C9A84C"),
            TextColor = Color.FromArgb("#07090F"),
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 32,
            HeightRequest = 60,
            Margin = new Thickness(0, 12, 0, 0)
        };
        startBtn.Clicked += OnStartClicked;

        var themeLabel = new Label
        {
            Text = "Vali teema",
            TextColor = Color.FromArgb("#3D4E60"),
            FontSize = 11,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 18, 0, 8)
        };

        _darkBtn  = MakeThemeBtn("🌑  Tume",  "#0E1220", "#C9A84C");
        _sepiaBtn = MakeThemeBtn("📜  Seepia","#1E1008", "#D4A853");
        _forestBtn= MakeThemeBtn("🌿  Mets",  "#081408", "#66BB6A");

        _darkBtn.Clicked  += (s, e) => SelectTheme(Theme.Dark,   _darkBtn);
        _sepiaBtn.Clicked += (s, e) => SelectTheme(Theme.Sepia,  _sepiaBtn);
        _forestBtn.Clicked+= (s, e) => SelectTheme(Theme.Forest, _forestBtn);
        SelectTheme(Theme.Dark, _darkBtn);

        var themeRow = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 8,
            Children = { _darkBtn, _sepiaBtn, _forestBtn }
        };

        var inputCard = new Border
        {
            BackgroundColor = Color.FromArgb("#0A1020"),
            Stroke = new SolidColorBrush(Color.FromArgb("#1A2740")),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 28 },
            Padding = new Thickness(26, 22),
            Content = new VerticalStackLayout
            {
                Spacing = 4,
                Children = { nameLabel, nameBox, startBtn, themeLabel, themeRow }
            }
        };

        // ── Bottom hints ───────────────────────────────────────────
        var hintsRow = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 20,
            Children =
            {
                MakeHint("🗝️", "Võti"),
                MakeHint("⛏️", "Kirves"),
                MakeHint("🏺", "Kuvšin"),
            }
        };

        var bottomLine = new Label
        {
            Text = "Leia aare. Lahenda mõistatus.",
            TextColor = Color.FromArgb("#283545"),
            FontSize = 11,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 8, 0, 0)
        };

        // ── Root ───────────────────────────────────────────────────
        var content = new VerticalStackLayout
        {
            Padding = new Thickness(28, 56, 28, 40),
            Spacing = 0,
            Children =
            {
                eyeLabel,
                detective,
                titleCard,
                subtitle,
                new BoxView { HeightRequest = 28, Color = Colors.Transparent },
                inputCard,
                new BoxView { HeightRequest = 28, Color = Colors.Transparent },
                hintsRow,
                bottomLine
            }
        };

        var scroll = new ScrollView { Content = content };

        var rootGrid = new Grid();
        rootGrid.Add(bgGrid);
        rootGrid.Add(scroll);
        Content = rootGrid;
    }

    private Button MakeThemeBtn(string text, string bg, string fg) => new()
    {
        Text = text,
        BackgroundColor = Color.FromArgb(bg),
        TextColor = Color.FromArgb(fg),
        FontSize = 12,
        CornerRadius = 22,
        Padding = new Thickness(14, 9),
        BorderWidth = 0
    };

    private View MakeHint(string emoji, string label) => new VerticalStackLayout
    {
        Spacing = 4,
        HorizontalOptions = LayoutOptions.Center,
        Children =
        {
            new Label { Text = emoji, FontSize = 26, HorizontalOptions = LayoutOptions.Center },
            new Label { Text = label, FontSize = 10, TextColor = Color.FromArgb("#2A3B4C"), HorizontalOptions = LayoutOptions.Center }
        }
    };

    private void SelectTheme(Theme theme, Button active)
    {
        _theme = theme;
        BackgroundColor = theme.BackgroundColor;
        foreach (var b in new[] { _darkBtn, _sepiaBtn, _forestBtn })
        {
            b.BorderColor = Colors.Transparent;
            b.BorderWidth = 0;
        }
        active.BorderColor = Color.FromArgb("#C9A84C");
        active.BorderWidth = 2;
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        string name = _nameEntry.Text?.Trim() ?? "Detektiiv";
        if (string.IsNullOrEmpty(name)) name = "Detektiiv";
        await Navigation.PushAsync(new GamePage(name, _theme));
    }
}
