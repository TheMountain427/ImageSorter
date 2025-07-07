using System.Reactive;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using ImageSorter.Models;
using ReactiveUI;

namespace ImageSorter.Models.Generators;

public class ImageOverviewContainerGenerator
{
    public static IEnumerable<StackPanel> CreateImageRows(
    IEnumerable<ImageDetails> images,
    int itemsPerLine,
    IObservable<bool> bulkSelectionActivated,
    Action<ImageDetails> onImageClick,
    ReactiveCommand<(Button, ImageDetails, (int, int)), Unit> onBulkImageClick)
    {
        var imageList = images as IList<ImageDetails> ?? images.ToList();
        for (int i = 0; i < imageList.Count; i += itemsPerLine)
        {
            var rowPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 0
            };

            foreach (var img in imageList.Skip(i).Take(itemsPerLine))
            {
                var border = new Border
                {
                    BorderBrush = new SolidColorBrush(Color.Parse("#FF232323")),
                    BorderThickness = new(2),
                    Width = 200,
                    Margin = new(5),
                    DataContext = img
                };

                var animation = new Animation
                {
                    Duration = TimeSpan.FromSeconds(2),
                    Children =
                    {
                        new KeyFrame
                        {
                            Cue = new Cue(0d),
                            Setters = { new Setter(Border.OpacityProperty, 0.0) }
                        },
                        new KeyFrame
                        {
                            Cue = new Cue(1d),
                            Setters = { new Setter(Border.OpacityProperty, 1.0) }
                        }
                    }
                };

                var style = new Style(x => x.OfType<Border>())
                {
                    Animations = { animation }
                };
                border.Styles.Add(style);

                var grid = new Grid
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                // Button inside border
                var imageButton = new Button
                {
                    Background = Brushes.Transparent,
                    CommandParameter = img.FileName,
                    Content = new StackPanel
                    {
                        Children = {
                    new StackPanel
                    {
                        Children =
                        {
                            new TextBlock
                            {
                                Text = img.FilteredValue,
                                FontSize = 11,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                                TextWrapping = TextWrapping.NoWrap
                            },
                            new Image
                            {
                                Source = img.ThumbnailBitmap,
                                Margin = new (5),
                                IsVisible = img.ThumbnailBitmap != null
                            },
                            new TextBlock
                            {
                                Text = img.FileName,
                                FontSize = 11,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                                TextWrapping = TextWrapping.NoWrap
                            }
                        }
                    } }
                    }
                };

                grid.Children.Add(imageButton);

                border.Child = grid;

                var bulkSelectButton = new Button
                {
                    Background = Brushes.Transparent,
                    IsVisible = false,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                bulkSelectionActivated.Subscribe(isBulk =>
                {
                    // UI updates must be dispatched to the UI thread
                    Dispatcher.UIThread.Post(() =>
                    {
                        bulkSelectButton.IsVisible = isBulk;
                        imageButton.IsHitTestVisible = !isBulk;

                        if (!isBulk)
                        {
                            bulkSelectButton.Tag = "0";
                        }
                    });
                });

                grid.Children.Add(bulkSelectButton);

                bulkSelectButton.Click += (s, e) =>
                {
                    if (bulkSelectButton.IsVisible)
                    {
                        bulkSelectButton.Tag = bulkSelectButton.Tag?.ToString() == "1" ? "0" : "1";
                    }
                };

                bulkSelectButton.PropertyChanged += (s, e) =>
                {
                    if (e.Property == Button.TagProperty)
                    {
                        if (bulkSelectButton.Tag?.ToString() == "1")
                        {
                            Dispatcher.UIThread.Post(() =>
                            {
                                bulkSelectButton.BorderBrush = new SolidColorBrush(Colors.LimeGreen); // Selected color
                            });
                        }
                        else
                        {
                            Dispatcher.UIThread.Post(() =>
                            {
                                bulkSelectButton.BorderBrush = new SolidColorBrush(Color.Parse("#FF232323")); // Default color
                            });
                        }
                    }
                };

                imageButton.Click += (_, __) => onImageClick(img);
                bulkSelectButton.Click += (_, __) => onBulkImageClick.Execute((bulkSelectButton, img, ((int)rowPanel.Tag!, (int)grid.Tag))).Subscribe();

                rowPanel.Children.Add(border);
            }

            yield return rowPanel;
        }
    }
}