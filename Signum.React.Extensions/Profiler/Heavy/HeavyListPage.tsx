﻿import * as React from 'react'
import { Link } from 'react-router'
import * as numbro from 'numbro'
import * as moment from 'moment'
import * as Navigator from '../../../../Framework/Signum.React/Scripts/Navigator'
import * as Finder from '../../../../Framework/Signum.React/Scripts/Finder'
import EntityLink from '../../../../Framework/Signum.React/Scripts/SearchControl/EntityLink'
import {CountSearchControl, SearchControl } from '../../../../Framework/Signum.React/Scripts/Search'
import { QueryDescription, SubTokensOptions } from '../../../../Framework/Signum.React/Scripts/FindOptions'
import { getQueryNiceName, PropertyRoute, getTypeInfos } from '../../../../Framework/Signum.React/Scripts/Reflection'
import { ModifiableEntity, EntityControlMessage, Entity, parseLite, getToString, JavascriptMessage } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { API, HeavyProfilerEntry} from '../ProfilerClient'

require("./Profiler.css");

interface HeavyListProps extends ReactRouter.RouteComponentProps<{}, {}> {

}

export default class HeavyList extends React.Component<HeavyListProps, { enabled?: boolean; entries?: HeavyProfilerEntry[], fileToUpload?: File, fileVer?: number }> {

    constructor(props: HeavyListProps) {
        super(props);
        this.state = { fileVer: 0 };
    }

    componentWillMount() {

        this.loadIsEnabled().done()

        this.loadEntries().done();
    }

    loadEntries() {
        return API.Heavy.entries()
            .then(entries => this.setState({ entries }));
    }

    handleClear = (e: React.MouseEvent) => {
        API.Heavy.clear()
            .then(() => this.loadEntries())
            .done();
    }

    handleUpdate = (e: React.MouseEvent) => {
        this.loadEntries().done();
        this.loadIsEnabled().done();
    }

    loadIsEnabled() {
        return API.Heavy.isEnabled()
            .then(enabled => this.setState({ enabled }))
    }

    handleSetEnabled(value: boolean) {
        API.Heavy.setEnabled(value)
            .then(() => this.loadIsEnabled())
            .then(() => this.loadEntries())
            .done();
    }


    handleDownload = () => {
        API.Heavy.download(null);
    }

    handleInputChange = (e: React.FormEvent) => {
        let f = (e.currentTarget as HTMLInputElement).files[0];
        this.setState({ fileToUpload: f });
    }

    handleUpload = () => {
        let fileReader = new FileReader();
        fileReader.onerror = e => { setTimeout(() => { throw (e as any).error; }, 0); };
        fileReader.onload = e => {
            let content = ((e.target as any).result as string).after("base64,");
            let fileName = this.state.fileToUpload.name;

            API.Heavy.upload({ fileName, content })
                .then(() => this.setState({ fileToUpload: null, fileVer: this.state.fileVer + 1 }))
                .then(() => this.loadEntries())
                .done();
        };
        fileReader.readAsDataURL(this.state.fileToUpload);
    }

    render() {
        document.title = "Heavy Profiler";

        if (this.state.entries == null)
            return <h3>Heavy Profiler (loading...) </h3>;

        return (
            <div>
                <h2>Heavy Profiler</h2>
                <br />
                <div className="btn-toolbar" style={{ float: "right" }}>
                    <input key={this.state.fileVer} type="file" className="form-control" onChange={this.handleInputChange} style={{ display: "inline", float: "left", width: "inherit" }} />
                    <button onClick={this.handleUpload} className="btn btn-info" disabled={!this.state.fileToUpload}><span className="glyphicon glyphicon-cloud-upload" aria-hidden="true"></span> Upload</button>
                </div>
                <div className="btn-toolbar">
                    { !this.state.enabled ? <button onClick={() => this.handleSetEnabled(true) } className="btn btn-default primary">Enable</button> :
                        <button onClick={() => this.handleSetEnabled(false) } className="btn btn-default" style={{ color: "red" }}>Disable</button>
                    }
                    <button onClick={this.handleUpdate} className="btn btn-default">Update</button>
                    <button onClick={this.handleClear} className="btn btn-default">Clear</button>
                    <button onClick={this.handleDownload} className="btn btn-info"><span className="glyphicon glyphicon-cloud-download" aria-hidden="true"></span> Download</button>
                </div>
                <br/>
                <p className="help-block">Upload previous runs to compare performance.</p>
                <p className="help-block">Enable the profiler with the debugger with <code>HeavyProfiler.Enabled = true</code> and save the results with <code>HeavyProfiler.ExportXml().Save("profile.xml") </code>.</p>

                <br />
                <h3>Entries</h3>
                <div className="sf-profiler-chart" ref={d => this.chartContainer = d}>
                </div>
            </div>
        );
    }

    componentDidUpdate() {
        this.mountChart();
    }

    chartContainer: HTMLDivElement;

    mountChart() {

        if (this.chartContainer == null)
            return;

        let data = this.state.entries;

        let fontSize = 12;
        let fontPadding = 4;
        let characterWidth = 7;
        let labelWidth = 60 * characterWidth; //Max characters: 100
        let rightMargin = 10 * characterWidth; //Aproximate elapsed time length: 10

        let width = this.chartContainer.getBoundingClientRect().width;
        let height = (fontSize + (2 * fontPadding)) * (data.length);
        this.chartContainer.style.height = height + "px";

        let minStart = data.map(a => a.BeforeStart).min();
        let maxEnd = data.map(a => a.End).max();

        let x = d3.scale.linear()
            .domain([minStart, maxEnd])
            .range([labelWidth + 3, width - rightMargin]);

        let y = d3.scale.linear()
            .domain([0, data.length])
            .range([0, height - 1]);

        let entryHeight = y(1);

        d3.select(this.chartContainer).selectAll("svg").remove();

        let chart = d3.select(this.chartContainer)
            .append('svg:svg').attr('width', width).attr('height', height);

        let groups = chart.selectAll("g.entry").data(data).enter()
            .append('svg:g').attr('class', 'entry')
            .attr('data-full-index', function (v) { return v.FullIndex; });

        groups.append('svg:rect').attr('class', 'left-background')
            .attr('x', 0)
            .attr('y', function (v, i) { return y(i); })
            .attr('width', labelWidth)
            .attr('height', entryHeight)
            .attr('fill', '#ddd')
            .attr('stroke', '#fff');

        let labelsLeft = groups.append('svg:text').attr('class', 'label label-left')
            .attr('dy', function (v, i) { return y(i); })
            .attr('y', fontPadding + fontSize)
            .attr('fill', '#000')
            .text(function (v) { return v.Role + " " + v.AdditionalData; });

        groups.append('svg:rect').attr('class', 'right-background')
            .attr('x', labelWidth)
            .attr('y', function (v, i) { return y(i); })
            .attr('width', width - labelWidth)
            .attr('height', entryHeight)
            .attr('fill', '#fff')
            .attr('stroke', '#ddd');

        let rectangles = groups.append('svg:rect').attr('class', 'shape')
            .attr('x', function (v) { return x(v.Start); })
            .attr('y', function (v, i) { return y(i); })
            .attr('width', function (v) { return x(v.End) - x(v.Start); })
            .attr('height', entryHeight)
            .attr('fill', function (v) { return v.Color; });

        let labelsRight = groups.append('svg:text').attr('class', 'label label-right')
            .attr('dx', function (v) { return x(v.End) + 3; })
            .attr('dy', function (v, i) { return y(i); })
            .attr('y', fontPadding + fontSize)
            .attr('fill', '#000')
            .text(function (v) { return v.Elapsed; });

        groups.append('svg:title').text(function (v) { return v.Elapsed + " - " + v.AdditionalData; });
        //labelsLeft.append('svg:title').text(function (v) { return v.Elapsed + " - " + v.AdditionalData; });
        //labelsRight.append('svg:title').text(function (v) { return v.Elapsed + " - " + v.AdditionalData; });

        groups.on("click", e => {
            let url = Navigator.currentHistory.createHref("~/profiler/heavy/entry/" + e.FullIndex);

            if (d3.event.ctrlKey) {
                window.open(url);
            } else {
                Navigator.currentHistory.push(url);
                window.location.href = url;
            }
        });
    }
}



