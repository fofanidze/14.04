namespace WinFormsTasksStandalone;

public partial class Form1 : Form
{
    private readonly Random _random = new();
    private readonly Button[] _gameButtons = new Button[9];
    private readonly char[] _board = new char[9];

    private Button _runawayButton = null!;
    private Label _gameStatusLabel = null!;
    private TextBox _editorTextBox = null!;

    private bool _isXTurn = true;
    private string? _openedFilePath;

    public Form1()
    {
        InitializeComponent();
        BuildUi();
        ResetGame();
    }

    private void BuildUi()
    {
        Text = "WinForms Tasks - One Form";
        ClientSize = new Size(980, 650);
        StartPosition = FormStartPosition.CenterScreen;

        _runawayButton = new Button
        {
            Text = "Натисни мене :)",
            Size = new Size(140, 42),
            Location = new Point(30, 20)
        };
        _runawayButton.MouseEnter += RunawayButton_MouseEnter;
        Controls.Add(_runawayButton);

        Label gameTitle = new()
        {
            Text = "Хрестики-нулики",
            AutoSize = true,
            Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
            Location = new Point(30, 95)
        };
        Controls.Add(gameTitle);

        Panel gamePanel = new()
        {
            Location = new Point(30, 130),
            Size = new Size(310, 310)
        };
        Controls.Add(gamePanel);

        for (int i = 0; i < 9; i++)
        {
            Button cellButton = new()
            {
                Font = new Font(Font.FontFamily, 20, FontStyle.Bold),
                Size = new Size(100, 100),
                Location = new Point((i % 3) * 103, (i / 3) * 103),
                Tag = i
            };
            cellButton.Click += GameCell_Click;
            _gameButtons[i] = cellButton;
            gamePanel.Controls.Add(cellButton);
        }

        _gameStatusLabel = new Label
        {
            Text = "Хід: X",
            AutoSize = true,
            Location = new Point(30, 455)
        };
        Controls.Add(_gameStatusLabel);

        Button resetGameButton = new()
        {
            Text = "Нова гра",
            Size = new Size(120, 35),
            Location = new Point(30, 485)
        };
        resetGameButton.Click += (_, _) => ResetGame();
        Controls.Add(resetGameButton);

        Label fileTitle = new()
        {
            Text = "Редактор текстового файлу",
            AutoSize = true,
            Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
            Location = new Point(390, 20)
        };
        Controls.Add(fileTitle);

        Button openFileButton = new()
        {
            Text = "Відкрити файл",
            Size = new Size(120, 35),
            Location = new Point(390, 55)
        };
        openFileButton.Click += OpenFileButton_Click;
        Controls.Add(openFileButton);

        Button saveFileButton = new()
        {
            Text = "Зберегти",
            Size = new Size(120, 35),
            Location = new Point(520, 55)
        };
        saveFileButton.Click += SaveFileButton_Click;
        Controls.Add(saveFileButton);

        _editorTextBox = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Location = new Point(390, 100),
            Size = new Size(560, 500),
            Font = new Font("Consolas", 10),
            WordWrap = true
        };
        Controls.Add(_editorTextBox);
    }

    private void RunawayButton_MouseEnter(object? sender, EventArgs e)
    {
        int maxX = Math.Max(10, ClientSize.Width - _runawayButton.Width - 10);
        int maxY = Math.Max(10, ClientSize.Height - _runawayButton.Height - 10);

        _runawayButton.Location = new Point(
            _random.Next(10, maxX),
            _random.Next(10, maxY));
    }

    private void GameCell_Click(object? sender, EventArgs e)
    {
        if (sender is not Button clicked || clicked.Tag is not int index)
        {
            return;
        }

        if (_board[index] != '\0')
        {
            return;
        }

        char mark = _isXTurn ? 'X' : 'O';
        _board[index] = mark;
        clicked.Text = mark.ToString();

        if (HasWinner(mark))
        {
            _gameStatusLabel.Text = $"Переміг: {mark}";
            SetGameEnabled(false);
            return;
        }

        if (_board.All(cell => cell != '\0'))
        {
            _gameStatusLabel.Text = "Нічия";
            return;
        }

        _isXTurn = !_isXTurn;
        _gameStatusLabel.Text = $"Хід: {(_isXTurn ? 'X' : 'O')}";
    }

    private bool HasWinner(char mark)
    {
        int[][] lines =
        {
            new[] { 0, 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7, 8 },
            new[] { 0, 3, 6 }, new[] { 1, 4, 7 }, new[] { 2, 5, 8 },
            new[] { 0, 4, 8 }, new[] { 2, 4, 6 }
        };

        return lines.Any(line => line.All(index => _board[index] == mark));
    }

    private void ResetGame()
    {
        Array.Fill(_board, '\0');
        foreach (Button button in _gameButtons)
        {
            button.Text = string.Empty;
            button.Enabled = true;
        }

        _isXTurn = true;
        _gameStatusLabel.Text = "Хід: X";
    }

    private void SetGameEnabled(bool enabled)
    {
        foreach (Button button in _gameButtons)
        {
            button.Enabled = enabled;
        }
    }

    private void OpenFileButton_Click(object? sender, EventArgs e)
    {
        using OpenFileDialog openDialog = new()
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            Title = "Оберіть текстовий файл"
        };

        if (openDialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        _openedFilePath = openDialog.FileName;
        _editorTextBox.Text = File.ReadAllText(_openedFilePath);
        Text = $"WinForms Tasks - {_openedFilePath}";
    }

    private void SaveFileButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_openedFilePath))
        {
            using SaveFileDialog saveDialog = new()
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Збережіть файл"
            };

            if (saveDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            _openedFilePath = saveDialog.FileName;
        }

        File.WriteAllText(_openedFilePath, _editorTextBox.Text);
        MessageBox.Show("Файл успішно збережено.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
