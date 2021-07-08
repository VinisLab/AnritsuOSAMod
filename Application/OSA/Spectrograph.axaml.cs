using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using OSA.Services;
using System;
using System.Collections.Generic;

namespace OSA
{
    public partial class Spectrograph : UserControl
    {
        private int GridHeightDivisions = 6;
        private int GridWidthDivisions = 8;

        
        private Dictionary<int, double> Graph;
        private readonly IWaveformService waveformService;
        public Spectrograph()
        {
            waveformService = Splat.Locator.Current.GetRequiredService<IWaveformService>();
            Graph = waveformService.Wave;
           
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        public override void Render(DrawingContext context)
        {
            if(Graph is null)
            {
                return;
            }

            var heightDiv = Bounds.Height / GridHeightDivisions;
            var widthDiv = Bounds.Width / GridWidthDivisions;

            context.FillRectangle(Brushes.Transparent, new Rect(0,0,Bounds.Width,Bounds.Height));

            for (double h = heightDiv; h < Bounds.Height; h += heightDiv)
            {
                context.DrawLine(new Pen(Brushes.Gray, 1), new Point(0, h), new Point(Bounds.Width, h));
            }
            for (double w = heightDiv; w < Bounds.Width; w += widthDiv)
            {
                context.DrawLine(new Pen(Brushes.Gray, 1), new Point(w, 0), new Point(w, Bounds.Height));
            }

            
            var div = (waveformService.MaxSetWavelength - waveformService.MinSetWavelength)/ Bounds.Width;
            double wl = waveformService.MinSetWavelength;
            double peakW = 0;
            var peakH = 0;
            var mk1w = -1.0;
            var mk1wli = 0;
            var mk2w = -1.0;
            var mk2wli = 0;
            for (double w = 0; w < Bounds.Width; w++)
            {
                int wli = (int)Math.Round(wl);
                if (Graph.ContainsKey(wli))
                {
                    context.DrawLine(new Pen(WaveLengthToRGB(wli), 1), new Point(w, Bounds.Height - Graph[wli]), new Point(w, Bounds.Height));
                    context.DrawLine(new Pen(Brushes.White, 1), new Point(w, Bounds.Height - Graph[wli]), new Point(w, (Bounds.Height - Graph[wli])-2));
                    if (wl <= waveformService.Peak +1 && wl >= waveformService.Peak -1)
                    {
                        peakW = w;
                        peakH = wli;
                    }
                    if (waveformService.Marker1 == wli)
                    {
                        mk1w = w;
                        mk1wli = wli;                     
                    }
                    if (waveformService.Marker2 == wli)
                    {
                        mk2w = w;
                        mk2wli = wli;
                    }
                }

                wl += div;
            }
            if(mk1w >= 0)
            {
                context.DrawLine(new Pen(Brushes.LightSeaGreen, 1), new Point(mk1w, 0), new Point(mk1w, Bounds.Height));
                context.DrawLine(new Pen(Brushes.LightSeaGreen, 1), new Point(0, Bounds.Height - Graph[mk1wli]), new Point(Bounds.Width, Bounds.Height - Graph[mk1wli]));
                context.DrawText(Brushes.LightSeaGreen, new Point(mk1w +5, 5), new FormattedText(mk1wli.ToString(), Typeface.Default, 12, TextAlignment.Left, TextWrapping.NoWrap, new Size(50, 20)));
                context.DrawText(Brushes.LightSeaGreen, new Point(5, Bounds.Height - Graph[mk1wli]), new FormattedText(Graph[mk1wli].ToString("#,###") , Typeface.Default, 12, TextAlignment.Left, TextWrapping.NoWrap, new Size(120, 20)));
            }
            if (mk2w >= 0)
            {
                context.DrawLine(new Pen(Brushes.GreenYellow, 1), new Point(mk2w, 0), new Point(mk2w, Bounds.Height));
                context.DrawLine(new Pen(Brushes.GreenYellow, 1), new Point(0, Bounds.Height - Graph[mk2wli]), new Point(Bounds.Width, Bounds.Height - Graph[mk2wli]));
                context.DrawText(Brushes.GreenYellow, new Point(mk2w + 5, 5), new FormattedText(mk2wli.ToString(), Typeface.Default, 12, TextAlignment.Left, TextWrapping.NoWrap, new Size(50, 20)));
                context.DrawText(Brushes.GreenYellow, new Point(5, Bounds.Height - Graph[mk2wli]), new FormattedText(Graph[mk2wli].ToString("#,###"), Typeface.Default, 12, TextAlignment.Left, TextWrapping.NoWrap, new Size(120, 20)));
            }
            if (peakH != 0 && peakW != 0)
            {
                context.DrawLine(new Pen(Brushes.Yellow, 2), new Point(peakW, (Bounds.Height - Graph[peakH]) - 10), new Point(peakW, (Bounds.Height - Graph[peakH]) + 10));
                context.DrawLine(new Pen(Brushes.Yellow, 2), new Point(peakW - 10, Bounds.Height - Graph[peakH]), new Point(peakW + 10, Bounds.Height - Graph[peakH]));
            }
            if(mk1w >= 0 && mk2w >= 0)
            {
               // context.DrawText(Brushes.Red, new Point(100, 100), new FormattedText("Text", Typeface.Default, 12, TextAlignment.Center, TextWrapping.NoWrap,new Size(60,20)));
            }

            
            //context.DrawRectangle(new Pen(Brushes.Gray, 1), new Rect(0, 0, Bounds.Width, Bounds.Height));
            base.Render(context);
        }
        static private double Gamma = 0.80;
        static private double IntensityMax = 255;
        public static Brush WaveLengthToRGB(double Wavelength)
        {
            double factor;
            double Red, Green, Blue;

            if ((Wavelength >= 380) && (Wavelength < 440))
            {
                Red = -(Wavelength - 440) / (440 - 380);
                Green = 0.0;
                Blue = 1.0;
            }
            else if ((Wavelength >= 440) && (Wavelength < 490))
            {
                Red = 0.0;
                Green = (Wavelength - 440) / (490 - 440);
                Blue = 1.0;
            }
            else if ((Wavelength >= 490) && (Wavelength < 510))
            {
                Red = 0.0;
                Green = 1.0;
                Blue = -(Wavelength - 510) / (510 - 490);
            }
            else if ((Wavelength >= 510) && (Wavelength < 580))
            {
                Red = (Wavelength - 510) / (580 - 510);
                Green = 1.0;
                Blue = 0.0;
            }
            else if ((Wavelength >= 580) && (Wavelength < 645))
            {
                Red = 1.0;
                Green = -(Wavelength - 645) / (645 - 580);
                Blue = 0.0;
            }
            else if ((Wavelength >= 645) && (Wavelength < 781))
            {
                Red = 1.0;
                Green = 0.0;
                Blue = 0.0;
            }
            else if (Wavelength >= 781)
            {
                Red = 1.0;
                Green = 0.0;
                Blue = 0.0;
            }
            else
            {
                Red = 1.0;
                Green = 0.0;
                Blue = 1.0;
            }

            // Let the intensity fall off near the vision limits

            if ((Wavelength >= 380) && (Wavelength < 420))
            {
                factor = 0.3 + 0.7 * (Wavelength - 380) / (420 - 380);
            }
            else if ((Wavelength >= 420) && (Wavelength < 701))
            {
                factor = 1.0;
            }
            else if ((Wavelength >= 701) && (Wavelength < 781))
            {
                factor = 0.3 + 0.7 * (780 - Wavelength) / (780 - 700);
            }
            else
            {
                factor = 0.15;
            }


            int[] rgb = new int[3];

            // Don't want 0^x = 1 for x <> 0
            rgb[0] = Red == 0.0 ? 0 : (int)Math.Round(IntensityMax * Math.Pow(Red * factor, Gamma));
            rgb[1] = Green == 0.0 ? 0 : (int)Math.Round(IntensityMax * Math.Pow(Green * factor, Gamma));
            rgb[2] = Blue == 0.0 ? 0 : (int)Math.Round(IntensityMax * Math.Pow(Blue * factor, Gamma));

            return new SolidColorBrush(new Color(255, (byte)rgb[0], (byte)rgb[1], (byte)rgb[2]));
        }
    }
}
