﻿namespace ScottPlotCookbook.Recipes.PlotTypes;

public class Finance : ICategory
{
    public string Chapter => "Plot Types";
    public string CategoryName => "Financial Plot";
    public string CategoryDescription => "Finance plots display price data binned into time ranges";

    public class Candlestick : RecipeBase
    {
        public override string Name => "Candlestick Chart";
        public override string Description => "Candlestick charts use symbols to display price data. " +
            "The rectangle indicates open and close prices, and the center line indicates minimum and " +
            "maximum price for the given time period. Color indicates whether the price increased or decreased " +
            "between open and close.";

        [Test]
        public override void Execute()
        {
            var prices = Generate.RandomOHLCs(30);
            myPlot.Add.Candlestick(prices);
            myPlot.Axes.DateTimeTicks(Edge.Bottom);
        }
    }

    public class OhlcChart : RecipeBase
    {
        public override string Name => "OHLC Chart";
        public override string Description => "OHLC charts use symbols to display price data " +
            "(open, high, low, and close) for specific time ranges.";

        [Test]
        public override void Execute()
        {
            var prices = Generate.RandomOHLCs(30);
            myPlot.Add.OHLC(prices);
            myPlot.Axes.DateTimeTicks(Edge.Bottom);
        }
    }

    public class FinanceRightAxis : RecipeBase
    {
        public override string Name => "Price on Right";
        public override string Description => "Finance charts can be created " +
            "which display price information on the right axis.";

        [Test]
        public override void Execute()
        {
            // add candlesticks to the plot
            var prices = Generate.RandomOHLCs(30);
            var candles = myPlot.Add.Candlestick(prices);

            // configure the candlesticks to use the plot's right axis
            candles.Axes.YAxis = myPlot.Axes.Right;
            candles.Axes.YAxis.Label.Text = "Price";

            // style the bottom axis to display date
            myPlot.Axes.DateTimeTicks(Edge.Bottom);
        }
    }

    public class FinanceSma : RecipeBase
    {
        public override string Name => "Simple Moving Average";
        public override string Description => "Tools exist for creating simple moving average (SMA) " +
            "curves and displaying them next to finanial data.";

        [Test]
        public override void Execute()
        {
            // generate and plot time series price data
            var prices = Generate.RandomOHLCs(75);
            myPlot.Add.Candlestick(prices);
            myPlot.Axes.DateTimeTicks(Edge.Bottom);

            // calculate SMA and display it as a scatter plot
            int[] windowSizes = { 3, 8, 20 };
            foreach (int windowSize in windowSizes)
            {
                ScottPlot.Finance.SimpleMovingAverage sma = new(prices, windowSize);
                var sp = myPlot.Add.Scatter(sma.Dates, sma.Means);
                sp.Label = $"SMA {windowSize}";
                sp.MarkerSize = 0;
                sp.LineWidth = 3;
                sp.Color = Colors.Navy.WithAlpha(1 - windowSize / 30.0);
            }

            myPlot.ShowLegend();
        }
    }

    public class FinanceBollinger : RecipeBase
    {
        public override string Name => "Bollinger Bands";
        public override string Description => "Tools exist for creating Bollinger Bands which " +
            "display weighted moving mean and variance for time series financial data.";

        [Test]
        public override void Execute()
        {
            // generate and plot time series price data
            var prices = Generate.RandomOHLCs(100);
            myPlot.Add.Candlestick(prices);
            myPlot.Axes.DateTimeTicks(Edge.Bottom);

            // calculate Bollinger Bands
            ScottPlot.Finance.BollingerBands bb = new(prices, 20);

            // display center line (mean) as a solid line
            var sp1 = myPlot.Add.Scatter(bb.Dates, bb.Means);
            sp1.MarkerSize = 0;
            sp1.Color = Colors.Navy;

            // display upper bands (positive variance) as a dashed line
            var sp2 = myPlot.Add.Scatter(bb.Dates, bb.UpperValues);
            sp2.MarkerSize = 0;
            sp2.Color = Colors.Navy;
            sp2.LineStyle.Pattern = LinePattern.Dotted;

            // display lower bands (positive variance) as a dashed line
            var sp3 = myPlot.Add.Scatter(bb.Dates, bb.LowerValues);
            sp3.MarkerSize = 0;
            sp3.Color = Colors.Navy;
            sp3.LineStyle.Pattern = LinePattern.Dotted;
        }
    }

    public class FinancialPlotWithoutGaps : RecipeBase
    {
        public override string Name => "Financial Plot Without Gaps";
        public override string Description => "When the DateTimes stored in OHLC objects " +
            "are used to determine the horizontal position of candlesticks, periods without data " +
            "like weekends and holidays appear as gaps in the plot. Enabling sequential mode causes " +
            "the plot to ignore the OHLC DateTimes and display candles at integer positions starting " +
            "from zero. Since this is not a true DateTime axis, users enabling this mode must customize " +
            "the tick labels themselves.";

        [Test]
        public override void Execute()
        {
            // create a candlestick plot
            var prices = Generate.RandomOHLCs(31);
            var candlePlot = myPlot.Add.Candlestick(prices);

            // enable sequential mode to place candles at X = 0, 1, 2, ...
            candlePlot.Sequential = true;

            // determine a few candles to display ticks for
            int tickCount = 5;
            int tickDelta = prices.Count / tickCount;
            DateTime[] tickDates = prices
                .Where((x, i) => i % tickDelta == 0)
                .Select(x => x.DateTime)
                .ToArray();

            // use a manual tick generator for the horizontal axis
            double[] tickPositions = Generate.Consecutive(tickDates.Length, tickDelta);
            string[] tickLabels = tickDates.Select(x => x.ToString("MM/dd")).ToArray();
            ScottPlot.TickGenerators.NumericManual tickGen = new(tickPositions, tickLabels);
            myPlot.Axes.Bottom.TickGenerator = tickGen;
        }
    }
}
