using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Praktos3
{
    public partial class MainWindow : Window
    {
        bool loop = false;
        List<string> ListeningHistory = new List<string>();
        List<string> files = new List<string>();
        List<string> unshuffledList = new List<string>();
        Random random = new Random();
        string PlayingNow;
        int IndexForNextPlay;
        MediaPlayer mediaplayer = new MediaPlayer();
        private TimeSpan slidervalue;
        public string PauseOrPlay = "Pause";
        public MainWindow()
        {
            InitializeComponent();
            mediaplayer.MediaEnded += AutoPlayNextSong;
            mediaplayer.MediaOpened += MediaPlayer_MediaOpened;
        }
        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            if (mediaplayer.NaturalDuration.HasTimeSpan)
            {
                Thread thread = new Thread(_ =>
                {
                    while (true)
                    {
                        if (PauseOrPlay == "Pause")
                        {
                            Dispatcher.Invoke(() =>
                            {
                                slidervalue = mediaplayer.Position;
                                SongControl.Value = slidervalue.Ticks;
                            });
                        }
                        Thread.Sleep(100);
                    }
                });
                thread.Start();
            }
        }

        public void Play()
        {
            mediaplayer.Open(new Uri(files[IndexForNextPlay]));
            mediaplayer.Play();
            PlayingNow = files[IndexForNextPlay];
            SongName.Content = System.IO.Path.GetFileName(files[IndexForNextPlay]);
            ListeningHistory.Add(ListSongs.Items[IndexForNextPlay].ToString());
            SetPositionSlider();
        }

        private void AutoPlayNextSong(object sender, EventArgs e) //автопроигрывание
        {
            SongControl.Value = 0;

            if (loop == false)
            {
                foreach (string item in files)
                {
                    if (item.Contains(PlayingNow))
                    {
                        if (files.IndexOf(item) + 1 == files.Count)
                        {
                            IndexForNextPlay = 0;
                            Play();
                            break;
                        }
                        else
                        {
                            IndexForNextPlay = files.IndexOf(PlayingNow) + 1;
                            Play();
                            break;
                        }
                    }
                }
            }
            else
            {
                Play();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) //кнопка старт-стоп
        {
            var button = (sender as Button)?.Content as PackIcon;
            if (button != null)
            {
                if (button.Kind == PackIconKind.Play)
                {
                    button.Kind = PackIconKind.Pause;
                    PauseOrPlay = "Pause";
                }
                else
                {
                    button.Kind = PackIconKind.Play;
                    PauseOrPlay = "Play";
                }
            }
            if (PauseOrPlay == "Play")
            {
                SongControl.Value = slidervalue.Ticks;
                mediaplayer.Stop();
            }
            else
            {
                mediaplayer.Position = new TimeSpan(Convert.ToInt64(SongControl.Value));
                mediaplayer.Play();
            }
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e) //кнопка повтора
        {
            var button = (sender as Button)?.Content as PackIcon;
            if (button != null)
            {
                if (button.Kind == PackIconKind.RepeatOne)
                {
                    button.Kind = PackIconKind.RepeatOff;
                    loop = false;
                }
                else
                {
                    loop = true;
                    button.Kind = PackIconKind.RepeatOne;
                }
            }
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e) //кнопка перемешки
        {
            var packIcon = (sender as Button)?.Content as PackIcon;
            if (packIcon != null)
            {
                if (packIcon.Kind == PackIconKind.Shuffle)
                {
                    packIcon.Kind = PackIconKind.ShuffleDisabled;
                    files = unshuffledList;
                }
                else
                {
                    packIcon.Kind = PackIconKind.Shuffle;
                    List<string> shuffledFiles = new List<string>(files.Count);
                    foreach (var file in files)
                    {
                        shuffledFiles.Add(file);
                    }
                    for (int i = 0; i < shuffledFiles.Count; i++)
                    {
                        int j = random.Next(shuffledFiles.Count);
                        var temp = shuffledFiles[i];
                        shuffledFiles[i] = shuffledFiles[j];
                        shuffledFiles[j] = temp;
                    }
                    files = shuffledFiles;
                }
            }
        }


        private void OpenFolderWithSongs_Click(object sender, RoutedEventArgs e) //выбор папки с песнями
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                files = Directory.GetFiles(dialog.FileName, "*.mp3").ToList();
                unshuffledList = files;
                if(files.Count > 0)
                {
                    GridVisibility.Visibility = Visibility.Visible;
                    List<string> fileNames = new List<string>();
                    foreach (string file in files)
                    {
                        fileNames.Add(System.IO.Path.GetFileName(file));
                    }
                    LabelUKnopki.Content = $"Текущая папка с музыкой: {System.IO.Path.GetFileName(dialog.FileName)}";
                    ListSongs.ItemsSource = fileNames;
                    mediaplayer.Open(new Uri(files[0]));
                    mediaplayer.Play();
                    PlayPauseIcon.Kind = PackIconKind.Pause;
                    SongName.Content = System.IO.Path.GetFileName(files[0]);
                    PlayingNow = files[0];
                    ListeningHistory.Add(fileNames[0]);
                    SetPositionSlider();
                }
                else
                {
                    MessageBox.Show("В этой папке нету песен!");
                }
            }
        }

        private void ListSongs_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
            string selected = ListSongs.SelectedItem.ToString();
            SongName.Content = selected;
            foreach (string item in files)
            {
                if (item.Contains(selected))
                {
                    PlayPauseIcon.Kind = PackIconKind.Pause;
                    mediaplayer.Open(new Uri(item));
                    mediaplayer.Play();
                    PlayingNow = item;
                    ListeningHistory.Add(selected);
                    SetPositionSlider();
                }
            }
        }

        private void SkipNext_Click(object sender, RoutedEventArgs e) //след песня
        {
            foreach (string item in files)
            {
                if(item.Contains(PlayingNow))
                {
                    Repeat.Kind = PackIconKind.RepeatOff;
                    loop = false;
                    PlayPauseIcon.Kind = PackIconKind.Pause;
                    if (files.IndexOf(item)+1 >= files.Count)
                    {
                        IndexForNextPlay = 0;
                        Play();
                        break;
                    }
                    else
                    {
                        IndexForNextPlay = files.IndexOf(PlayingNow) + 1;
                        Play();
                        break;
                    }
                }
            }
        }
        private void SkipPrevious_Click(object sender, RoutedEventArgs e) //пред песня
        {
            foreach (string item in files)
            {
                if (item.Contains(PlayingNow))
                {
                    Repeat.Kind = PackIconKind.RepeatOff;
                    loop = false;
                    PlayPauseIcon.Kind = PackIconKind.Pause;
                    if (files.IndexOf(item) - 1 <= 0)
                    {
                        IndexForNextPlay = 0;
                        Play();
                        break;
                    }
                    else
                    {
                        IndexForNextPlay = files.IndexOf(PlayingNow) - 1;
                        Play();
                        break;
                    }
                }
            }
        }
        private void VolumeControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaplayer.Volume = VolumeControl.Value;
            if (VolumeControl.Value >= 0.33)
            {
                Volume.Kind = PackIconKind.VolumeMedium;
            }
            if (VolumeControl.Value >= 0.66)
            {
                Volume.Kind = PackIconKind.VolumeHigh;
            }
            if (VolumeControl.Value <= 0.33)
            {
                Volume.Kind = PackIconKind.VolumeLow;
            }
            if (VolumeControl.Value == 0)
            {
                Volume.Kind |= PackIconKind.VolumeOff;
            }
        }
        private void ListeningHistoryButton_Click(object sender, RoutedEventArgs e) //окно с историем прослушивания
        {
            ListeningHistory listeningHistoryWindow = new ListeningHistory(ListeningHistory);
            listeningHistoryWindow.ItemSelected += ListeningHistoryWindow_ItemSelected;
            listeningHistoryWindow.Show();
        }
        private void ListeningHistoryWindow_ItemSelected(object sender, string selectedItem)
        {
            SongName.Content = selectedItem;
            foreach (string item in files)
            {
                if (item.Contains(selectedItem))
                {
                    PlayPauseIcon.Kind = PackIconKind.Pause;
                    mediaplayer.Open(new Uri(item));
                    mediaplayer.Play();
                    PlayingNow = item;
                    ListeningHistory.Add(selectedItem);
                    SetPositionSlider();
                }
            }
        }
        private void SetPositionSlider()
        {
            while (true)
            {
                if (mediaplayer.NaturalDuration.HasTimeSpan)
                {
                    PauseOrPlay = "Pause";
                    long ticks = mediaplayer.NaturalDuration.TimeSpan.Ticks;
                    SongControl.Maximum = ticks;
                    double seconds = TimeSpan.FromTicks(ticks).TotalSeconds;
                    string formattedTime = TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
                    MaxValue.Content = formattedTime;
                    double seconds1 = TimeSpan.FromTicks(mediaplayer.Position.Ticks).TotalSeconds;
                    string formattedTime1 = TimeSpan.FromSeconds(seconds1).ToString(@"mm\:ss");
                    StartTimeCode.Content = formattedTime1;
                    break;
                }
            }
        }
        private void SongControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaplayer.NaturalDuration.HasTimeSpan)
            {
                mediaplayer.Position = new TimeSpan(Convert.ToInt64(SongControl.Value));
                double seconds = TimeSpan.FromTicks(mediaplayer.Position.Ticks).TotalSeconds;
                string formattedTime = TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
                StartTimeCode.Content = formattedTime;
            }
            else
            {
                slidervalue = new TimeSpan(Convert.ToInt64(SongControl.Value));
            }
        }
    }
}