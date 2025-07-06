using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using ImageSorter.Models;

namespace ImageSorter.Models.Generators;

public static class ImageOverviewContainerGenerator
{
    public static IEnumerable<StackPanel> CreateImageRows(
    IEnumerable<ImageDetails> images,
    int itemsPerLine,
    Action<ImageDetails>? onImageClick = null)
    {
        var imageList = images as IList<ImageDetails> ?? images.ToList();
        for (int i = 0; i < imageList.Count; i += itemsPerLine)
        {
            var rowPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 0 };

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

                // Animation style (same as your XAML)
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

                // Button inside border
                var button = new Button
                {
                    Background = Brushes.Transparent,
                    CommandParameter = img.FileName,
                    Content = new StackPanel
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
                    }
                };
                if (onImageClick != null)
                {
                    button.Click += (_, __) => onImageClick(img);
                }
                border.Child = button;
                rowPanel.Children.Add(border);
            }
            yield return rowPanel;
        }
    }
}