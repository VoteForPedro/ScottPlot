﻿namespace ScottPlotCookbook.Recipes.PlotTypes;

public class Bar : ICategory
{
    public string Chapter => "Plot Types";
    public string CategoryName => "Bar Plot";
    public string CategoryDescription => "Bar plots represent values as horizontal or vertical rectangles";

    public class Quickstart : RecipeBase
    {
        public override string Name => "Bar Plot Quickstart";
        public override string Description => "Bar plots can be added from a series of values.";

        [Test]
        public override void Execute()
        {
            // add bars
            double[] values = { 5, 10, 7, 13 };
            myPlot.Add.Bars(values);

            // tell the plot to autoscale with no padding beneath the bars
            myPlot.Axes.Margins(bottom: 0);
        }
    }

    public class BarPosition : RecipeBase
    {
        public override string Name => "Bar Positioning";
        public override string Description => "The exact position and size of each bar may be customized.";

        [Test]
        public override void Execute()
        {
            List<ScottPlot.Bar> bars = new()
            {
                new() { Position = 1, Value = 5, ValueBase = 3, },
                new() { Position = 2, Value = 7, ValueBase = 0, },
                new() { Position = 4, Value = 3, ValueBase = 2, },
            };

            myPlot.Add.Bars(bars);
        }
    }

    public class BarWithError : RecipeBase
    {
        public override string Name => "Bars with Error";
        public override string Description => "Bars can have errorbars.";

        [Test]
        public override void Execute()
        {
            List<ScottPlot.Bar> bars = new()
            {
                new() { Position = 1, Value = 5, Error = 1, },
                new() { Position = 2, Value = 7, Error = 2, },
                new() { Position = 3, Value = 6, Error = 1, },
                new() { Position = 4, Value = 8, Error = 2, },
            };

            myPlot.Add.Bars(bars);

            // tell the plot to autoscale with no padding beneath the bars
            myPlot.Axes.Margins(bottom: 0);
        }
    }

    public class BarTickLabels : RecipeBase
    {
        public override string Name => "Bars with Labeled Ticks";
        public override string Description => "Bars can be labeled by manually specifying axis tick mark positions and labels.";

        [Test]
        public override void Execute()
        {
            myPlot.Add.Bar(position: 1, value: 5, error: 1);
            myPlot.Add.Bar(position: 2, value: 7, error: 2);
            myPlot.Add.Bar(position: 3, value: 6, error: 1);
            myPlot.Add.Bar(position: 4, value: 8, error: 2);

            Tick[] ticks =
            {
                new(1, "Apple"),
                new(2, "Orange"),
                new(3, "Pear"),
                new(4, "Banana"),
            };

            myPlot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
            myPlot.Axes.Bottom.MajorTickStyle.Length = 0;
            myPlot.HideGrid();

            // tell the plot to autoscale with no padding beneath the bars
            myPlot.Axes.Margins(bottom: 0);
        }
    }

    public class BarStackVertically : RecipeBase
    {
        public override string Name => "Stacked Bar Plot";
        public override string Description => "Bars can be positioned on top of each other.";

        [Test]
        public override void Execute()
        {
            Color[] colors = {
                myPlot.Palette.GetColor(0),
                myPlot.Palette.GetColor(1),
                myPlot.Palette.GetColor(2),
            };

            ScottPlot.Bar[] bars =
            {
                // first set of stacked bars
                new() { Position = 1, ValueBase = 0, Value = 2, FillColor = colors[0] },
                new() { Position = 1, ValueBase = 2, Value = 5, FillColor = colors[1] },
                new() { Position = 1, ValueBase = 5, Value = 10, FillColor = colors[2] },

                // second set of stacked bars
                new() { Position = 2, ValueBase = 0, Value = 4, FillColor = colors[0] },
                new() { Position = 2, ValueBase = 4, Value = 7, FillColor = colors[1] },
                new() { Position = 2, ValueBase = 7, Value = 10, FillColor = colors[2] },
            };

            myPlot.Add.Bars(bars);

            Tick[] ticks =
            {
                new(1, "Spring"),
                new(2, "Summer"),
            };

            myPlot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
            myPlot.Axes.Bottom.MajorTickStyle.Length = 0;
            myPlot.HideGrid();

            // tell the plot to autoscale with no padding beneath the bars
            myPlot.Axes.Margins(bottom: 0);
        }
    }

    public class GroupedBarPlot : RecipeBase
    {
        public override string Name => "Grouped Bar Plot";
        public override string Description => "Bars can be grouped by position and color.";

        [Test]
        public override void Execute()
        {
            Color[] colors = {
                myPlot.Palette.GetColor(0),
                myPlot.Palette.GetColor(1),
                myPlot.Palette.GetColor(2),
            };

            ScottPlot.Bar[] bars =
            {
                // first group
                new() { Position = 1, Value = 2, FillColor = colors[0], Error = 1 },
                new() { Position = 2, Value = 5, FillColor = colors[1], Error = 2 },
                new() { Position = 3, Value = 7, FillColor = colors[2], Error = 1 },

                // second group
                new() { Position = 5, Value = 4, FillColor = colors[0], Error = 2 },
                new() { Position = 6, Value = 7, FillColor = colors[1], Error = 1 },
                new() { Position = 7, Value = 13, FillColor = colors[2], Error = 3 },
                
                // third group
                new() { Position = 9, Value = 5, FillColor = colors[0], Error = 1 },
                new() { Position = 10, Value = 6, FillColor = colors[1], Error = 3 },
                new() { Position = 11, Value = 11, FillColor = colors[2], Error = 2 },
            };

            myPlot.Add.Bars(bars);

            // build the legend manually
            myPlot.Legend.IsVisible = true;
            myPlot.Legend.Location = Alignment.UpperLeft;
            myPlot.Legend.ManualItems.Add(new LegendItem() { Label = "Monday", FillColor = colors[0] });
            myPlot.Legend.ManualItems.Add(new LegendItem() { Label = "Tuesday", FillColor = colors[1] });
            myPlot.Legend.ManualItems.Add(new LegendItem() { Label = "Wednesday", FillColor = colors[2] });

            // show group labels on the bottom axis
            Tick[] ticks =
            {
                new(2, "Group 1"),
                new(6, "Group 2"),
                new(10, "Group 3"),
            };
            myPlot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
            myPlot.Axes.Bottom.MajorTickStyle.Length = 0;
            myPlot.HideGrid();

            // tell the plot to autoscale with no padding beneath the bars
            myPlot.Axes.Margins(bottom: 0);
        }
    }

    public class HorizontalBar : RecipeBase
    {
        public override string Name => "Horizontal Bar Plot";
        public override string Description => "Bar plots can be displayed horizontally.";

        [Test]
        public override void Execute()
        {
            ScottPlot.Bar[] bars =
            {
                new() { Position = 1, Value = 5, Error = 1, },
                new() { Position = 2, Value = 7, Error = 2, },
                new() { Position = 3, Value = 6, Error = 1, },
                new() { Position = 4, Value = 8, Error = 2, },
            };

            var barPlot = myPlot.Add.Bars(bars);
            barPlot.Horizontal = true;

            myPlot.Axes.Margins(left: 0);
        }
    }
}
