using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Save_the_world
{
    public sealed partial class MainWindow : Window
    {

        Random random = new Random();
  
        DispatcherTimer enemyTimer = new DispatcherTimer();
        DispatcherTimer targetTimer = new DispatcherTimer();
        private bool _humanCaptured = false;

        public MainWindow()
        {
            this.InitializeComponent();

            DataContext = new MainWindowViewModel();

            enemyTimer.Tick += enemyTimer_Tick;
            enemyTimer.Interval = TimeSpan.FromSeconds(2);

            targetTimer.Tick += targetTimer_Tick;
            targetTimer.Interval = TimeSpan.FromSeconds(.1);
        }

       private int StartGame()
       {
            if (gameOverText.Visibility == Visibility.Visible)
            {
                gameOverText.Visibility = Visibility.Hidden;
            }

            if (WinText.Visibility == Visibility.Visible)
            {
                WinText.Visibility = Visibility.Hidden;
            }

            human.IsHitTestVisible = true;
           _humanCaptured = false;
           progressBar.Value = 0;
           startButton.Visibility = Visibility.Collapsed;
           playArea.Children.Clear();
           playArea.Children.Add(target);
           playArea.Children.Add(human);
           enemyTimer.Start();
           targetTimer.Start();
       }


        void targetTimer_Tick(object sender, object e)
        {
            progressBar.Value += 1;
            if (progressBar.Value >= progressBar.Maximum)
                EndTheGame();
        }

        private void EndTheGame()
        {
            if (gameOverText.Visibility != Visibility.Visible)
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                _humanCaptured = false;
                startButton.Visibility = Visibility.Visible;
                gameOverText.Visibility = Visibility.Visible;
            }
        }

        private void WinTheGame()
        {
            if (WinText.Visibility != Visibility.Visible)
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                _humanCaptured = false;
                startButton.Visibility = Visibility.Visible;
                WinText.Visibility = Visibility.Visible;
            }
        }

        private static void RestartGame(Exception e)
        {
            //Wenn Panda bei Target ist, Spiel neu starten.
        }

        void enemyTimer_Tick(object sender, object e)
        {
            AddEnemy();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void AddEnemy()
        {
            ContentControl enemy = new ContentControl {Template = Resources["EnemyTemplate"] as ControlTemplate};
            AnimateEnemy(enemy, 0, playArea.ActualWidth - 100, "(Canvas.Left)");
            AnimateEnemy(enemy, random.Next((int)playArea.ActualHeight - 100),
                random.Next((int)playArea.ActualHeight - 100), "(Canvas.Top)");
            playArea.Children.Add(enemy);
            enemy.MouseMove += Enemy_MouseMove;
        }

        private void Enemy_MouseMove(object sender, MouseEventArgs e)
        {
            if (_humanCaptured)
                EndTheGame();
        }

        private void AnimateEnemy(ContentControl enemy, double from, double to, string propertyToAnimate)
        {

            var duration = new Duration(TimeSpan.FromSeconds(random.Next(4, 6)));

            var storyboard = new Storyboard
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            var animation = new DoubleAnimation(from, to, duration);

            Storyboard.SetTarget(animation, enemy);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyToAnimate));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void human_PointerPressed(object sender, MouseButtonEventArgs e)
        {
            if (enemyTimer.IsEnabled)
            {
                _humanCaptured = true;
                human.IsHitTestVisible = false;
            }
        }

        private void playArea_MouseEntered(object sender, MouseEventArgs e)
        {
            if (targetTimer.IsEnabled && _humanCaptured)
            {
                //progressBar.Value = 0;
                Canvas.SetLeft(human, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(human, random.Next(100, (int)playArea.ActualHeight - 100));
                _humanCaptured = false;
                human.IsHitTestVisible = true;
            }
        }

        private void playArea_PointerMoved(object sender, MouseEventArgs e)
        {
            if (_humanCaptured)
            {
                Point pointerPostition = e.GetPosition(null);
                Point relativePosition = grid.TransformToVisual(playArea).Transform(pointerPostition);
                if ((Math.Abs(relativePosition.X - Canvas.GetLeft(human)) > human.ActualWidth * 3)
                    || (Math.Abs(relativePosition.Y - Canvas.GetTop(human)) > human.ActualHeight * 3))
                {
                    _humanCaptured = false;
                    human.IsHitTestVisible = true;
                }
                else
                {
                    Canvas.SetLeft(human, relativePosition.X - human.ActualWidth / 2);
                    Canvas.SetTop(human, relativePosition.Y - human.ActualHeight / 2);
                }
            }
        }

        private void playArea_PointerExited(object sender, MouseEventArgs e)
        {
            if (_humanCaptured)
                EndTheGame();
        }

        private void Target_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (_humanCaptured)
                WinTheGame();
                RestartGame();
        }
    }
}