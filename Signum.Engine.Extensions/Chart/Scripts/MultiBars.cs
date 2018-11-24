
using Signum.Logic.Chart;
using Signum.Entities.Chart;
using Signum.Engine.Chart;
using System.Collections.Generic;

namespace Signum.Logic.Chart.Scripts 
{
    public class MultiBarsChartScript : ChartScript                
    {
        public MultiBarsChartScript() : base(D3ChartScript.MultiBars)
        {
            this.Icon = ChartScriptLogic.LoadIcon("multibars.png");
            this.Columns = new List<ChartScriptColumn>
            {
                new ChartScriptColumn("Vertical Axis", ChartColumnType.Groupable),
                new ChartScriptColumn("Sub-bars", ChartColumnType.Groupable) { IsOptional = true },
                new ChartScriptColumn("Width", ChartColumnType.Positionable) ,
                new ChartScriptColumn("Width 2", ChartColumnType.Positionable) { IsOptional = true },
                new ChartScriptColumn("Width 3", ChartColumnType.Positionable) { IsOptional = true },
                new ChartScriptColumn("Width 4", ChartColumnType.Positionable) { IsOptional = true },
                new ChartScriptColumn("Width 5", ChartColumnType.Positionable) { IsOptional = true }
            };
            this.Parameters = new List<ChartScriptParameter>
            {
                new ChartScriptParameter("LabelMargin", ChartParameterType.Number) {  ValueDefinition = new NumberInterval { DefaultValue = 140m } },
                new ChartScriptParameter("Scale", ChartParameterType.Enum) { ColumnIndex = 2,  ValueDefinition = EnumValueList.Parse("ZeroMax (M)|MinMax|Log (M)") },
                new ChartScriptParameter("NumberOpacity", ChartParameterType.Number) {  ValueDefinition = new NumberInterval { DefaultValue = 0.8m } },
                new ChartScriptParameter("NumberColor", ChartParameterType.String) {  ValueDefinition = new StringValue("#fff") },
                new ChartScriptParameter("ColorScheme", ChartParameterType.Enum) {  ValueDefinition = EnumValueList.Parse("category10|accent|dark2|paired|pastel1|pastel2|set1|set2|set3|BrBG[K]|PRGn[K]|PiYG[K]|PuOr[K]|RdBu[K]|RdGy[K]|RdYlBu[K]|RdYlGn[K]|Spectral[K]|Blues[K]|Greys[K]|Oranges[K]|Purples[K]|Reds[K]|BuGn[K]|BuPu[K]|OrRd[K]|PuBuGn[K]|PuBu[K]|PuRd[K]|RdPu[K]|YlGnBu[K]|YlGn[K]|YlOrBr[K]|YlOrRd[K]") },
                new ChartScriptParameter("ColorSchemeSteps", ChartParameterType.Enum) {  ValueDefinition = EnumValueList.Parse("3|4|5|6|7|8|9|10|11") }
            };
        }      
    }                
}