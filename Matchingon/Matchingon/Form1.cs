using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Matchingon
{
    public partial class Form1 : Form
    {
        private const int BOARD_SIZE = 8;
        private const int TOTAL_PAIRS = 32;

        private GameButton[,] gameButtons;
        private List<int> shuffledImageIndices;

        private GameButton firstSelected = null;
        private GameButton secondSelected = null;
        private bool isProcessing = false;

        private int matchedPairs = 0;
        private int attempts = 0;

        private Stopwatch gameStopwatch;

        public Form1()
        {
            InitializeComponent();
            gameStopwatch = new Stopwatch();

            // Build the dynamic UI elements
            GenerateBordersAndBoard();

            // Setup game variables
            InitializeGame();

            // Start the timers
            timerUpdater.Start();
            gameStopwatch.Start();
        }

        private void GenerateBordersAndBoard()
        {
            string[] letters = { "A", "B", "C", "D", "E", "F", "G", "H" };
            string[] numbers = { "8", "7", "6", "5", "4", "3", "2", "1" };
            gameButtons = new GameButton[BOARD_SIZE, BOARD_SIZE];

            tableLayoutPanel1.SuspendLayout();

            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    // Corners
                    if ((row == 0 || row == 9) && (col == 0 || col == 9))
                    {
                        tableLayoutPanel1.Controls.Add(CreateBorderLabel("•", true), col, row);
                    }
                    // Top & Bottom Letters
                    else if ((row == 0 || row == 9) && (col > 0 && col < 9))
                    {
                        tableLayoutPanel1.Controls.Add(CreateBorderLabel(letters[col - 1], false), col, row);
                    }
                    // Left & Right Numbers
                    else if ((col == 0 || col == 9) && (row > 0 && row < 9))
                    {
                        tableLayoutPanel1.Controls.Add(CreateBorderLabel(numbers[row - 1], false), col, row);
                    }
                    // Inner Game Board
                    else
                    {
                        int gameRow = row - 1;
                        int gameCol = col - 1;
                        GameButton btn = new GameButton(gameRow, gameCol)
                        {
                            Dock = DockStyle.Fill,
                            FlatStyle = FlatStyle.Flat,
                            Cursor = Cursors.Hand,
                            BackColor = Color.FromArgb(52, 73, 94),
                            BackgroundImageLayout = ImageLayout.Zoom
                        };
                        btn.FlatAppearance.BorderSize = 1;
                        btn.FlatAppearance.BorderColor = Color.FromArgb(41, 128, 185);
                        btn.Click += GameButton_Click;

                        gameButtons[gameRow, gameCol] = btn;
                        tableLayoutPanel1.Controls.Add(btn, col, row);
                    }
                }
            }

            tableLayoutPanel1.ResumeLayout();
        }

        private Label CreateBorderLabel(string text, bool isCorner)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = isCorner ? Color.FromArgb(60, 63, 65) : Color.FromArgb(52, 73, 94),
                ForeColor = isCorner ? Color.FromArgb(200, 200, 200) : Color.White,
                Font = isCorner ? new Font("Segoe UI", 14F, FontStyle.Bold) : new Font("Segoe UI Semibold", 13F, FontStyle.Bold)
            };
        }

        private void TimerUpdater_Tick(object sender, EventArgs e)
        {
            if (gameStopwatch.IsRunning)
            {
                TimeSpan elapsed = gameStopwatch.Elapsed;
                lblTimer.Text = $"⏱️ {elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
            }
        }

        private void InitializeGame()
        {
            ResetGameStats();
            ShuffleImages();
            AssignCardsToBoard();
        }

        private void ShuffleImages()
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < TOTAL_PAIRS; i++)
            {
                indices.Add(i);
                indices.Add(i);
            }

            Random rnd = new Random();
            int n = indices.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                int temp = indices[k];
                indices[k] = indices[n];
                indices[n] = temp;
            }

            shuffledImageIndices = indices;
        }

        private void AssignCardsToBoard()
        {
            int index = 0;
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    GameButton btn = gameButtons[row, col];
                    btn.CardIndex = shuffledImageIndices[index];
                    btn.FrontImage = LoadImageFromResource(btn.CardIndex);

                    // Reset Button State
                    btn.IsFlipped = false;
                    btn.IsMatched = false;
                    btn.BackgroundImage = null;
                    btn.BackColor = Color.FromArgb(52, 73, 94);
                    btn.Text = "";

                    index++;
                }
            }
        }

        private Image LoadImageFromResource(int imageIndex)
        {
            try
            {
                string resourceName = $"card{imageIndex + 1}";
                object resource = Properties.Resources.ResourceManager.GetObject(resourceName);
                return resource as Image;
            }
            catch
            {
                return null;
            }
        }

        private void ResetGameStats()
        {
            matchedPairs = 0;
            attempts = 0;
            firstSelected = null;
            secondSelected = null;
            isProcessing = false;
            UpdateScoreDisplay();
        }

        private async void GameButton_Click(object sender, EventArgs e)
        {
            if (isProcessing) return;

            GameButton clickedBtn = sender as GameButton;

            if (clickedBtn == null || clickedBtn.IsFlipped || clickedBtn.IsMatched) return;

            FlipCard(clickedBtn);

            if (firstSelected == null)
            {
                firstSelected = clickedBtn;
            }
            else if (secondSelected == null && clickedBtn != firstSelected)
            {
                secondSelected = clickedBtn;
                attempts++;
                UpdateScoreDisplay();

                isProcessing = true;
                await CheckForMatchAsync();
            }
        }

        private void FlipCard(GameButton btn)
        {
            btn.IsFlipped = !btn.IsFlipped;

            if (btn.IsFlipped)
            {
                btn.BackColor = Color.White;
                if (btn.FrontImage != null)
                {
                    btn.BackgroundImage = btn.FrontImage;
                }
                else
                {
                    // Fallback if image resource is missing
                    btn.Text = (btn.CardIndex + 1).ToString();
                    btn.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
                    btn.ForeColor = Color.Black;
                }
            }
            else
            {
                btn.BackgroundImage = null;
                btn.Text = "";
                btn.BackColor = Color.FromArgb(52, 73, 94);
            }
        }

        private async Task CheckForMatchAsync()
        {
            await Task.Delay(500);

            if (firstSelected.CardIndex == secondSelected.CardIndex)
            {
                firstSelected.IsMatched = true;
                secondSelected.IsMatched = true;

                firstSelected.BackColor = Color.FromArgb(39, 174, 96);
                secondSelected.BackColor = Color.FromArgb(39, 174, 96);

                matchedPairs++;
                UpdateScoreDisplay();

                if (matchedPairs == TOTAL_PAIRS)
                {
                    ShowVictoryMessage();
                }
            }
            else
            {
                await Task.Delay(400);

                FlipCard(firstSelected);
                FlipCard(secondSelected);
            }

            firstSelected = null;
            secondSelected = null;
            isProcessing = false;
        }

        private void BtnRestart_Click(object sender, EventArgs e)
        {
            gameStopwatch.Restart();
            InitializeGame();
        }

        private void UpdateScoreDisplay()
        {
            lblScore.Text = $"⭐ Cütlər: {matchedPairs} / {TOTAL_PAIRS}";
            lblAttempts.Text = $"🎯 Cəhdlər: {attempts}";
        }

        private void ShowVictoryMessage()
        {
            gameStopwatch.Stop();

            TimeSpan elapsedTime = gameStopwatch.Elapsed;
            string timeString = $"{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}";
            double efficiency = (double)(TOTAL_PAIRS * 100) / Math.Max(attempts, 1);

            DialogResult result = MessageBox.Show(
                $"🎉 QAZANDINIZ!\n\n⏱️ Vaxt: {timeString}\n💫 Cəhdlər: {attempts}\n📊 Effektivlik: {efficiency:F0}%",
                "QƏLƏBƏ!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            if (result == DialogResult.OK)
            {
                this.Close();
            }
        }
    }
}