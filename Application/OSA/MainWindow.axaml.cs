using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using OSA.Services;

namespace OSA
{
    public partial class MainWindow : Window
    {
        private readonly IWaveformService waveformService;
        public MainWindow()
        {
            waveformService = Splat.Locator.Current.GetRequiredService<IWaveformService>();
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if(e.Key == Key.M)
            {
                waveformService.ToggleMarkers();
                
            }
            if(e.Key == Key.Left)
            {
                waveformService.Marker1 -= 1;
            }
            if(e.Key == Key.Right)
            {
                waveformService.Marker1 += 1;
            }
            if (e.Key == Key.Down)
            {
                waveformService.Marker2 -= 1;
            }
            if (e.Key == Key.Up)
            {
                waveformService.Marker2 += 1;
            }
            Invalidate(VisualChildren);
            e.Handled = true;
        }
        private void Invalidate(IAvaloniaList<IVisual> visual)
        {
            foreach (var item in visual)
            {
                item.InvalidateVisual();
                Invalidate(item.VisualChildren);
            }
        }
        private void Invalidate(IAvaloniaReadOnlyList<IVisual> visual)
        {
            foreach (var item in visual)
            {
                item.InvalidateVisual();
                Invalidate(item.VisualChildren);
            }
        }
    }
}
