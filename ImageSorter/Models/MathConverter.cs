

using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace ImageSorter.Models.MathConverters;

public static class MathConverter
{
    // How to use
    // Add this to the top:
    // xmlns:math="using:ImageSorter.Models"

    // Use StringFormat to bind to value, then converter to bind to the StringFormat value

    // For one control:
    //<TextBlock Height="20"
    //           Tag="{ Binding $self.Height, StringFormat='5 + {0}'}"
    //           Text="{ Binding $self.Tag, Converter={x:Static math:MathConverters.SimpleAddConverter}}}"/>

    // For multiple controls:
    //<TextBlock Name="MyControl"
    //		     Height="20"
    //		     Text="{ Binding $self.Height, StringFormat='5 + {0}'}"/>
    //<TextBlock Text="{ Binding #MyControl.Text, Converter={x:Static math:MathConverters.SimpleAddConverter}}}"/>

    public static FuncValueConverter<string, double> SimpleAddConverter { get; } =
        new FuncValueConverter<string, double>(exp =>
        {
            var variables = exp.Split('-');
            double output = double.Parse(variables[0]) + double.Parse(variables[1]);
            return output;
        });

    public static FuncValueConverter<string, double> SimpleSubtractConverter { get; } =
        new FuncValueConverter<string, double>(exp =>
        {
            var variables = exp.Split('*');
            double output = double.Parse(variables[0]) - double.Parse(variables[1]);
            return output;
        });

    public static FuncValueConverter<string, double> SimpleMultiplyConverter { get; } =
        new FuncValueConverter<string, double>(exp =>
        {
            var variables = exp.Split('*');
            double output = double.Parse(variables[0]) * double.Parse(variables[1]);
            return output;
        });

    public static FuncValueConverter<string, double> SimpleDivisionConverter { get; } =
        new FuncValueConverter<string, double>(exp =>
        {
            var variables = exp.Split('/');
            double output = double.Parse(variables[0]) / double.Parse(variables[1]);
            return output;
        });

    public static FuncValueConverter<double, double> Smaller80Percent { get; } =
    new FuncValueConverter<double, double>(exp =>
    {
        return exp * 0.8;
    });

    public static FuncValueConverter<double, double> Smaller60Percent { get; } =
    new FuncValueConverter<double, double>(exp =>
    {
        return exp * 0.6;
    });

}