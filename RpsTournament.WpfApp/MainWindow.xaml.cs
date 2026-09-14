using System;
using System.Linq;
using System.Windows;
using RpsTournament.Core;
using Res = RpsTournament.WpfApp.Properties.Resource1;

namespace RpsTournament.WpfApp
{
    public partial class MainWindow : Window
    {
        private const int MaxRounds = 5;
        private GameRound[] rounds = new GameRound[MaxRounds];
        private int currentRoundCount = 0;

        private int wins = 0;
        private int losses = 0;
        private int draws = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            PlayerMoveComboBox.ItemsSource = Enum.GetValues<Move>();
            PlayerMoveComboBox.SelectedIndex = -1;
            UpdateUi();
        }

        private void PlayRoundButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "";

            string playerName = PlayerNameTextBox.Text.Trim();
            if (playerName.Length < 2 || playerName.Length > 30)
            {
                StatusTextBlock.Text = Res.InvalidNameError;
                return;
            }

            if (PlayerMoveComboBox.SelectedItem == null)
            {
                StatusTextBlock.Text = Res.NoMoveSelectedError;
                return;
            }

            Move playerMove = (Move)PlayerMoveComboBox.SelectedItem;
            Move computerMove = GameLogic.GetComputerMove();
            RoundResult result = GameLogic.GetResult(playerMove, computerMove);

            GameRound round = new GameRound
            {
                Number = currentRoundCount + 1,
                PlayerMove = playerMove,
                ComputerMove = computerMove,
                Result = result
            };

            rounds[currentRoundCount] = round;
            currentRoundCount++;

            if (result == RoundResult.Win) wins++;
            else if (result == RoundResult.Loss) losses++;
            else draws++;

            UpdateUi();

            if (currentRoundCount >= MaxRounds)
            {
                PlayRoundButton.IsEnabled = false;
                ShowTournamentResult();
            }
        }

        private void UpdateUi()
        {
            RoundsDataGrid.ItemsSource = rounds.Take(currentRoundCount).ToList();
            ScoreTextBlock.Text = $"Skoor: Võite: {wins} | Kaotusi: {losses} | Viike: {draws}";
        }

        private void ShowTournamentResult()
        {
            string message;
            if (wins > losses)
                message = Res.PlayerWinsTournament;
            else if (losses > wins)
                message = Res.ComputerWinsTournament;
            else
                message = Res.TournamentDraw;

            MessageBox.Show(message, Res.TournamentOver, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NewTournamentButton_Click(object sender, RoutedEventArgs e)
        {
            rounds = new GameRound[MaxRounds];
            currentRoundCount = 0;
            wins = 0;
            losses = 0;
            draws = 0;

            PlayRoundButton.IsEnabled = true;
            StatusTextBlock.Text = "";
            PlayerMoveComboBox.SelectedIndex = -1;

            UpdateUi();
        }
    }
}