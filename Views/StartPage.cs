using DetektivGame.Models;
using Microsoft.Maui.Controls.Shapes;

namespace DetektivGame.Views;

public class StartPage : ContentPage
{
    private Theme _theme = Theme.Dark;
    private Entry _nameEntry = null!;
    private Button _darkBtn = null!, _sepiaBtn = null!, _forestBtn = null!;
    private Button _resultsBtn = null!;
    private Border _resultsCard = null!;
    private Grid _resultsGrid = null!;

    public StartPage()
    {
        NavigationPage.SetHasNavigationBar(this, false);
        BuildUI();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateResults();
    }

    private void UpdateResults()
    {
        string raw = Preferences.Get("game_results", "");
        _resultsGrid.Children.Clear();
        _resultsGrid.RowDefinitions.Clear();

        if (raw.Length == 0)
        {
            _resultsBtn.IsVisible = false;
            _resultsCard.IsVisible = false;
            return;
        }

        var entries = raw.Split(',')
            .Where(x => x.Contains('|'))
            .Select(x => x.Split('|'))
            .Where(p => p.Length == 2)
            .Take(5)
            .ToList();

        if (entries.Count == 0)
        {
            _resultsBtn.IsVisible = false;
            _resultsCard.IsVisible = false;
            return;
        }

        _resultsBtn.IsVisible = true;

        for (int i = 0; i < entries.Count; i++)
        {
            _resultsGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            string name = entries[i][0];
            int sec = int.TryParse(entries[i][1], out int s) ? s : 0;
            string time = $"{sec / 60:00}:{sec % 60:00}";

            _resultsGrid.Add(new Label { Text = $"{i + 1}.", TextColor = Color.FromArgb("#667788"), FontSize = 12, VerticalOptions = LayoutOptions.Center }, 0, i);
            _resultsGrid.Add(new Label { Text = name, TextColor = Color.FromArgb("#C8DEFF"), FontSize = 13, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 1, i);
            _resultsGrid.Add(new Label { Text = time, TextColor = Color.FromArgb("#C9A84C"), FontSize = 13, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center }, 2, i);
        }
    }

    private void BuildUI()
    {
        // ── Title card ─────────────────────────────────────────────
        var titleCard = new Border
        {
            BackgroundColor = Color.FromArgb("#120D26"),
            Stroke = new SolidColorBrush(Color.FromArgb("#C9A84C")),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 24 },
            Padding = new Thickness(24, 14),
            HorizontalOptions = LayoutOptions.Fill,
            Content = new VerticalStackLayout
            {
                Spacing = 6,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label
                    {
                        Text = "DETEKTIIV",
                        FontSize = 30,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#C9A84C"),
                        CharacterSpacing = 3,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new BoxView { HeightRequest = 1, Color = Color.FromArgb("#C9A84C"), Opacity = 0.4, HorizontalOptions = LayoutOptions.Fill },
                    new Label
                    {
                        Text = "Saladuse Tuba",
                        FontSize = 13,
                        TextColor = Color.FromArgb("#9B7E38"),
                        CharacterSpacing = 3,
                        HorizontalOptions = LayoutOptions.Center
                    }
                }
            }
        };

        var subtitle = new Label
        {
            Text = "Uuri. Kogu. Lahenda.",
            FontSize = 12,
            TextColor = Color.FromArgb("#5C6E80"),
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 6, 0, 0)
        };

        // ── Input card ─────────────────────────────────────────────
        _nameEntry = new Entry
        {
            Text = "Detektiiv",
            TextColor = Colors.White,
            BackgroundColor = Colors.Transparent,
            FontSize = 17,
            Placeholder = "Sisesta nimi...",
            PlaceholderColor = Color.FromArgb("#3D4E60")
        };

        var nameBox = new Border
        {
            BackgroundColor = Color.FromArgb("#0C1220"),
            Stroke = new SolidColorBrush(Color.FromArgb("#1E2D45")),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            Padding = new Thickness(14, 4),
            Content = _nameEntry
        };

        var startBtn = new Button
        {
            Text = "Alusta mängu",
            BackgroundColor = Color.FromArgb("#C9A84C"),
            TextColor = Color.FromArgb("#07090F"),
            FontSize = 17,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 28,
            HeightRequest = 54,
            Margin = new Thickness(0, 10, 0, 0)
        };
        startBtn.Clicked += OnStartClicked;

        var themeLabel = new Label
        {
            Text = "Vali teema",
            TextColor = Color.FromArgb("#3D4E60"),
            FontSize = 11,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 12, 0, 4)
        };

        _darkBtn   = MakeThemeBtn("Tume",   "#0E1220", "#C9A84C");
        _sepiaBtn  = MakeThemeBtn("Seepia", "#1E1008", "#D4A853");
        _forestBtn = MakeThemeBtn("Mets",   "#081408", "#66BB6A");

        _darkBtn.Clicked   += (s, e) => SelectTheme(Theme.Dark,   _darkBtn);
        _sepiaBtn.Clicked  += (s, e) => SelectTheme(Theme.Sepia,  _sepiaBtn);
        _forestBtn.Clicked += (s, e) => SelectTheme(Theme.Forest, _forestBtn);
        SelectTheme(Theme.Dark, _darkBtn);

        var inputCard = new Border
        {
            BackgroundColor = Color.FromArgb("#0A1020"),
            Stroke = new SolidColorBrush(Color.FromArgb("#1A2740")),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 24 },
            Padding = new Thickness(22, 16),
            Content = new VerticalStackLayout
            {
                Spacing = 4,
                Children =
                {
                    new Label { Text = "Sinu nimi", TextColor = Color.FromArgb("#C9A84C"), FontSize = 11, FontAttributes = FontAttributes.Bold, Margin = new Thickness(2, 0, 0, 4) },
                    nameBox,
                    startBtn,
                    themeLabel,
                    new HorizontalStackLayout { HorizontalOptions = LayoutOptions.Center, Spacing = 8, Children = { _darkBtn, _sepiaBtn, _forestBtn } }
                }
            }
        };

        // ── Results (bottom) ───────────────────────────────────────
        _resultsGrid = new Grid { ColumnSpacing = 10, RowSpacing = 6 };
        _resultsGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(20)));
        _resultsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        _resultsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

        _resultsCard = new Border
        {
            BackgroundColor = Color.FromArgb("#080E1A"),
            Stroke = new SolidColorBrush(Color.FromArgb("#1A2740")),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            Padding = new Thickness(16, 12),
            Margin = new Thickness(0, 0, 0, 6),
            IsVisible = false,
            Content = _resultsGrid
        };

        _resultsBtn = new Button
        {
            Text = "Viimased tulemused",
            BackgroundColor = Color.FromArgb("#0E1220"),
            TextColor = Color.FromArgb("#C9A84C"),
            FontSize = 13,
            CornerRadius = 0,
            HeightRequest = 46,
            BorderWidth = 0,
            IsVisible = false
        };
        _resultsBtn.Clicked += (s, e) =>
            _resultsCard.IsVisible = !_resultsCard.IsVisible;

        // ── Root: center content + bottom results ──────────────────
        var centerContent = new VerticalStackLayout
        {
            Spacing = 0,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(24, 0),
            Children =
            {
                titleCard,
                subtitle,
                new BoxView { HeightRequest = 14, Color = Colors.Transparent },
                inputCard
            }
        };

        var bottomSection = new VerticalStackLayout
        {
            Padding = new Thickness(16, 0, 16, 0),
            Spacing = 0,
            Children = { _resultsCard, _resultsBtn }
        };

        var root = new Grid();
        root.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        root.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        root.Add(centerContent, 0, 0);
        root.Add(bottomSection, 0, 1);
        Content = root;
    }

    private Button MakeThemeBtn(string text, string bg, string fg) => new()
    {
        Text = text,
        BackgroundColor = Color.FromArgb(bg),
        TextColor = Color.FromArgb(fg),
        FontSize = 12,
        CornerRadius = 20,
        Padding = new Thickness(14, 8),
        BorderWidth = 0
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

        await DisplayAlert(
            "Saladuslik kutsumine",
            $"Detektiiv {name},\n\n" +
            "Said anonüümse kõne. Hääletu hääl ütles:\n\n" +
            "\"Linnas on kolme toaga korter. Keegi pole seal aastaid käinud. " +
            "Kuuldused räägivad peidetud aardest — kuid seif on lukus.\"\n\n" +
            "Sinu ülesanne: uuri tube, kogu esemeid, kombineeri tööriistu ja leia viis seifi avamiseks.\n\n" +
            "Kell tikib. Edu, detektiiv.",
            "Alusta uurimist");

        await Navigation.PushAsync(new GamePage(name, _theme));
    }
}
