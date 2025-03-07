﻿using System.Drawing;
using System.Runtime.InteropServices;

namespace ScottPlot;

// TODO: obsolete methods in this class that create paints. Pass paints in to minimize allocations at render time.

/// <summary>
/// Common operations using the default rendering system.
/// </summary>
public static class Drawing
{
    public static PixelSize MeasureString(string text, SKPaint paint)
    {
        var strings = text.Split('\n');
        if (strings.Length > 1)
        {
            return strings
                .Select(s => MeasureString(s, paint))
                .Aggregate((a, b) => new PixelSize(Math.Max(a.Width, b.Width), a.Height + b.Height));
        }

        SKRect bounds = new();
        paint.MeasureText(text, ref bounds);

        float width = bounds.Width;
        float height = bounds.Height;
        return new PixelSize(width, height);
    }

    public static PixelSize MeasureLargestString(string[] strings, SKPaint paint)
    {
        float maxWidth = 0;
        float maxHeight = 0;

        for (int i = 0; i < strings.Length; i++)
        {
            PixelSize tickSize = MeasureString(strings[i], paint);
            maxWidth = Math.Max(maxWidth, tickSize.Width);
            maxHeight = Math.Max(maxHeight, tickSize.Height);
        }

        return new PixelSize(maxWidth, maxHeight);
    }

    public static void DrawLine(SKCanvas canvas, SKPaint paint, PixelLine pixelLine)
    {
        DrawLine(canvas, paint, pixelLine.Pixel1, pixelLine.Pixel2);
    }

    public static void DrawLine(SKCanvas canvas, SKPaint paint, Pixel pt1, Pixel pt2)
    {
        if (paint.StrokeWidth == 0)
            return;

        canvas.DrawLine(pt1.ToSKPoint(), pt2.ToSKPoint(), paint);
    }

    public static void DrawLine(SKCanvas canvas, SKPaint paint, PixelLine pxLine, LineStyle lineStyle)
    {
        DrawLine(canvas, paint, pxLine.Pixel1, pxLine.Pixel2, lineStyle);
    }

    public static void DrawLine(SKCanvas canvas, SKPaint paint, Pixel pt1, Pixel pt2, LineStyle lineStyle)
    {
        if (lineStyle.Width == 0 || lineStyle.IsVisible == false) // TODO: move this check in the LineStyle class
            return;

        lineStyle.ApplyToPaint(paint);
        if (paint.StrokeWidth == 0)
            return;
        canvas.DrawLine(pt1.ToSKPoint(), pt2.ToSKPoint(), paint);
    }

    public static void DrawLine(SKCanvas canvas, SKPaint paint, Pixel pt1, Pixel pt2, Color color, float width = 1, bool antiAlias = true, LinePattern pattern = LinePattern.Solid)
    {
        if (width == 0)
            return;

        paint.Color = color.ToSKColor();
        paint.IsStroke = true;
        paint.IsAntialias = antiAlias;
        paint.StrokeWidth = width;
        paint.PathEffect = pattern.GetPathEffect();
        if (paint.StrokeWidth == 0)
            return;
        canvas.DrawLine(pt1.ToSKPoint(), pt2.ToSKPoint(), paint);
    }

    public static void DrawLines(SKCanvas canvas, Pixel[] starts, Pixel[] ends, Color color, float width = 1, bool antiAlias = true, LinePattern pattern = LinePattern.Solid)
    {
        if (width == 0)
            return;

        if (starts.Length != ends.Length)
            throw new ArgumentException($"{nameof(starts)} and {nameof(ends)} must have same length");

        using SKPaint paint = new()
        {
            Color = color.ToSKColor(),
            IsStroke = true,
            IsAntialias = antiAlias,
            StrokeWidth = width,
            PathEffect = pattern.GetPathEffect(),
        };

        using SKPath path = new();

        for (int i = 0; i < starts.Length; i++)
        {
            path.MoveTo(starts[i].X, starts[i].Y);
            path.LineTo(ends[i].X, ends[i].Y);
        }

        canvas.DrawPath(path, paint);
    }

    public static void DrawLines(SKCanvas canvas, SKPaint paint, IEnumerable<Pixel> pixels, Color color, float width = 1, bool antiAlias = true, LinePattern pattern = LinePattern.Solid)
    {
        LineStyle ls = new()
        {
            Color = color,
            AntiAlias = antiAlias,
            Width = width,
            Pattern = pattern,
        };

        DrawLines(canvas, paint, pixels, ls);
    }

    public static void DrawLines(SKCanvas canvas, SKPaint paint, IEnumerable<Pixel> pixels, LineStyle lineStyle)
    {
        if (lineStyle.Width == 0 || lineStyle.IsVisible == false || pixels.Count() < 2)
            return;

        lineStyle.ApplyToPaint(paint);

        using SKPath path = new();

        path.MoveTo(pixels.First().ToSKPoint());

        foreach (var pixel in pixels.Skip(1))
        {
            path.LineTo(pixel.ToSKPoint());
        }

        canvas.DrawPath(path, paint);
    }

    public static void Fillectangle(SKCanvas canvas, PixelRect rect, SKPaint paint)
    {
        canvas.DrawRect(rect.ToSKRect(), paint);
    }

    public static void Fillectangle(SKCanvas canvas, PixelRect rect, Color color)
    {
        using SKPaint paint = new()
        {
            Color = color.ToSKColor(),
            IsStroke = false,
            IsAntialias = true,
        };

        canvas.DrawRect(rect.ToSKRect(), paint);
    }

    public static void DrawRectangle(SKCanvas canvas, PixelRect rect, SKPaint paint)
    {
        canvas.DrawRect(rect.ToSKRect(), paint);
    }

    public static void DrawRectangle(SKCanvas canvas, PixelRect rect, Color color, float lineWidth = 1)
    {
        using SKPaint paint = new()
        {
            Color = color.ToSKColor(),
            IsStroke = true,
            StrokeWidth = lineWidth,
            IsAntialias = true,
        };

        DrawRectangle(canvas, rect, paint);
    }

    public static void DrawDebugRectangle(SKCanvas canvas, PixelRect rect, Pixel point, Color color, float lineWidth = 1)
    {
        using SKPaint paint = new()
        {
            Color = color.ToSKColor(),
            IsStroke = true,
            StrokeWidth = lineWidth,
            IsAntialias = true,
        };

        canvas.DrawRect(rect.ToSKRect(), paint);
        canvas.DrawLine(rect.BottomLeft.ToSKPoint(), rect.TopRight.ToSKPoint(), paint);
        canvas.DrawLine(rect.TopLeft.ToSKPoint(), rect.BottomRight.ToSKPoint(), paint);

        canvas.DrawCircle(point.ToSKPoint(), 5, paint);

        paint.IsStroke = false;
        paint.Color = paint.Color.WithAlpha(20);
        canvas.DrawRect(rect.ToSKRect(), paint);
    }

    public static void DrawCircle(SKCanvas canvas, Pixel center, Color color, float radius = 5, bool fill = true)
    {
        using SKPaint paint = new()
        {
            Color = color.ToSKColor(),
            IsStroke = !fill,
            IsAntialias = true,
        };

        canvas.DrawCircle(center.ToSKPoint(), radius, paint);
    }

    public static void DrawMarker(SKCanvas canvas, SKPaint paint, Pixel pixel, MarkerStyle style)
    {
        if (!style.IsVisible)
            return;

        IMarker renderer = style.Shape.GetRenderer();

        renderer.Render(canvas, paint, pixel, style.Size, style.Fill, style.Outline);
    }

    public static void DrawMarkers(SKCanvas canvas, SKPaint paint, IEnumerable<Pixel> pixels, MarkerStyle style)
    {
        if (!style.IsVisible)
            return;

        IMarker renderer = style.Shape.GetRenderer();

        foreach (Pixel pixel in pixels)
        {
            renderer.Render(canvas, paint, pixel, style.Size, style.Fill, style.Outline);
        }
    }

    public static SKBitmap BitmapFromArgbs(uint[] argbs, int width, int height)
    {
        GCHandle handle = GCHandle.Alloc(argbs, GCHandleType.Pinned);

        var imageInfo = new SKImageInfo(width, height);
        var bmp = new SKBitmap(imageInfo);
        bmp.InstallPixels(
            info: imageInfo,
            pixels: handle.AddrOfPinnedObject(),
            rowBytes: imageInfo.RowBytes,
            releaseProc: (IntPtr _, object _) => handle.Free());

        return bmp;
    }

    public static SKColorFilter GetMaskColorFilter(Color foreground, Color? background = null)
    {
        // This function and the math is explained here: https://bclehmann.github.io/2022/11/06/UnmaskingWithSKColorFilter.html

        background ??= Colors.Black;

        float redDifference = foreground.Red - background.Value.Red;
        float greenDifference = foreground.Green - background.Value.Green;
        float blueDifference = foreground.Blue - background.Value.Blue;
        float alphaDifference = foreground.Alpha - background.Value.Alpha;

        // See https://learn.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/effects/color-filters
        // for an explanation of this matrix
        // 
        // Essentially, this matrix maps all gray colours to a line from `background.Value` to `foreground`.
        // Black and white are at the extremes on this line, 
        // so they get mapped to `background.Value` and `foreground` respectively
        var mat = new float[] {
                redDifference / 255, 0, 0, 0, background.Value.Red / 255.0f,
                0, greenDifference / 255, 0, 0, background.Value.Green / 255.0f,
                0, 0, blueDifference / 255, 0, background.Value.Blue / 255.0f,
                alphaDifference / 255, 0, 0, 0, background.Value.Alpha / 255.0f,
            };

        var filter = SKColorFilter.CreateColorMatrix(mat);
        return filter;
    }

    public static SKSurface CreateSurface(int width, int height)
    {
        SKImageInfo imageInfo = new(
            width: width,
            height: height,
            colorType: SKColorType.Rgba8888,
            alphaType: SKAlphaType.Premul);

        return SKSurface.Create(imageInfo);
    }

    public static void SavePng(SKSurface surface, string filename)
    {
        using SKImage skimg = surface.Snapshot();
        Image img = new(skimg);
        img.SavePng(filename);
    }
}
