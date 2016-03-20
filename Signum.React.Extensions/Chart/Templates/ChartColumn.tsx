﻿
import * as React from 'react'
import { DropdownButton, MenuItem, } from 'react-bootstrap'
import { Dic, classes } from '../../../../Framework/Signum.React/Scripts/Globals'
import * as Finder from '../../../../Framework/Signum.React/Scripts/Finder'
import { Lite, toLite } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { ResultTable, FindOptions, FilterOption, QueryDescription, SubTokensOptions, QueryToken } from '../../../../Framework/Signum.React/Scripts/FindOptions'
import { TypeContext, FormGroupSize, FormGroupStyle, StyleOptions, StyleContext } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import { SearchMessage, JavascriptMessage, parseLite, is } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { getTypeInfos, TypeInfo } from '../../../../Framework/Signum.React/Scripts/Reflection'
import * as Navigator from '../../../../Framework/Signum.React/Scripts/Navigator'
import { ValueLine, FormGroup } from '../../../../Framework/Signum.React/Scripts/Lines'
import { ChartColumnEntity, ChartScriptColumnEntity, IChartBase, GroupByChart, ChartMessage, ChartColorEntity } from '../Signum.Entities.Chart'
import * as ChartClient from '../ChartClient'
import QueryTokenEntityBuilder from '../../UserAssets/Templates/QueryTokenEntityBuilder'

export interface ChartColumnProps {
    ctx: TypeContext<ChartColumnEntity>;
    scriptColumn: ChartScriptColumnEntity;
    chartBase: IChartBase;
    queryKey: string;
    onToggleInfo: () => void;
    onInvalidate: () => void;
}


export class ChartColumn extends React.Component<ChartColumnProps, { }> {

    constructor(props) {
        super(props);
    }

    handleExpanded = () => {
        this.props.onToggleInfo();
    }

    handleGroupChecked = (e: React.FormEvent) => {

        this.props.chartBase.groupResults = (e.currentTarget as HTMLInputElement).checked;
        if (!this.props.chartBase.groupResults)
            ChartClient.removeAggregates(this.props.chartBase);

        this.props.onInvalidate();
    }

    render() {

        var sc = this.props.scriptColumn;
        var cb = this.props.chartBase;

        var groupVisible = this.props.chartBase.chartScript.groupBy != "Never" && sc.isGroupKey;
        
        return (
            <tr className="sf-chart-token">
                <th>{ sc.displayName + (sc.isOptional ? "?" : "") }</th>
                <td style={{ textAlign: "center" }}>
                    {groupVisible && <input type="checkbox" checked={cb.groupResults} className="sf-chart-group-trigger" disabled={cb.chartScript.groupBy == "Always"} onChange={this.handleGroupChecked}/>}
                </td>
                <td>
                    <div className={classes("sf-query-token", this.props.ctx.formGroupSizeCss)}>
                        <QueryTokenEntityBuilder
                            ctx={this.props.ctx.subCtx(a => a.token, { formGroupStyle: FormGroupStyle.None }) }
                            queryKey={this.props.queryKey}
                            subTokenOptions={SubTokensOptions.CanElement | (cb.groupResults && !sc.isGroupKey ? SubTokensOptions.CanAggregate : 0) } />
                    </div>
                    <a className="sf-chart-token-config-trigger" onClick={this.handleExpanded}>{ ChartMessage.Chart_ToggleInfo.niceToString() } </a>
                </td>
            </tr>
        );
    }
}


export interface ChartColumnInfoProps {
    ctx: TypeContext<ChartColumnEntity>;
    onRedraw: () => void;
    colorPalettes: string[];
}

export class ChartColumnInfo extends React.Component<ChartColumnInfoProps, void> {

    getColorPalettes() {
        var token = this.props.ctx.value.token;

        const t = token && token.token.type;

        if (t == null || Navigator.isReadOnly(ChartColorEntity))
            return [];

        if (!t.isLite && !t.isEnum)
            return [];

        return getTypeInfos(t);
    }

    render() {

        var ctx = this.props.ctx.subCtx({ formGroupSize: FormGroupSize.Small, formGroupStyle: FormGroupStyle.Basic });



        return (
            <tr className= "sf-chart-token-config">
                <td></td>
                <td></td>
                <td colSpan={1}>
                    <div className="form-vertical">
                        <div className="row">
                            <div className="col-sm-4">
                                <ValueLine ctx={ctx.subCtx(a => a.displayName) } onChange={this.props.onRedraw} />
                            </div>
                            { this.getColorPalettes().map((t, i) =>
                                <div className="col-sm-4" key={i}>
                                    <ChartLink ctx={this.props.ctx} type={t}  currentPalettes={this.props.colorPalettes}/>
                                </div>) }
                        </div>
                    </div>
                </td>
            </tr>
        );
    }


}


export interface ChartLinkProps {
    type: TypeInfo;
    currentPalettes: string[];
    ctx: StyleContext;
}

export var ChartLink = (props: ChartLinkProps) =>
    <FormGroup ctx={props.ctx as any}
        title={ChartMessage.ColorsFor0.niceToString(props.type.niceName) }>
        <a href={"/chartColors/" + props.type.name} className="form-control">
            {props.currentPalettes.contains(props.type.name) ? ChartMessage.ViewPalette.niceToString() : ChartMessage.CreatePalette.niceToString()  }
        </a>
    </FormGroup>;


