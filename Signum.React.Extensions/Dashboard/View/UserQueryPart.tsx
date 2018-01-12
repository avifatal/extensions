﻿
import * as React from 'react'
import { FormGroup, FormControlReadonly, ValueLine, ValueLineType, EntityLine, EntityCombo, EntityList, EntityRepeater, RenderEntity } from '../../../../Framework/Signum.React/Scripts/Lines'
import * as Finder from '../../../../Framework/Signum.React/Scripts/Finder'
import { QueryDescription, SubTokensOptions, FindOptions } from '../../../../Framework/Signum.React/Scripts/FindOptions'
import { getQueryNiceName, PropertyRoute, getTypeInfos } from '../../../../Framework/Signum.React/Scripts/Reflection'
import { ModifiableEntity, EntityControlMessage, Entity, parseLite, getToString, Lite, is, JavascriptMessage } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import * as Navigator from '../../../../Framework/Signum.React/Scripts/Navigator'
import * as Constructor from '../../../../Framework/Signum.React/Scripts/Constructor'
import { SearchControl, ValueSearchControl } from '../../../../Framework/Signum.React/Scripts/Search'
import { TypeContext, FormGroupStyle, mlistItemContext } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import QueryTokenEntityBuilder from '../../UserAssets/Templates/QueryTokenEntityBuilder'
import * as UserQueryClient from '../../UserQueries/UserQueryClient'
import { UserQueryPartEntity, PanelPartEmbedded, PanelStyle } from '../Signum.Entities.Dashboard'
import { classes } from '../../../../Framework/Signum.React/Scripts/Globals';


export interface UserQueryPartProps {
    partEmbedded: PanelPartEmbedded;
    part: UserQueryPartEntity;
    entity?: Lite<Entity>;
}

export default class UserQueryPart extends React.Component<UserQueryPartProps, { fo?: FindOptions }> {

    constructor(props: any) {
        super(props);
        this.state = { fo: undefined };
    }
    
    componentWillMount() {
        this.loadFindOptions(this.props);
    }

    componentWillReceiveProps(newProps: UserQueryPartProps) {

        if (is(this.props.part.userQuery, newProps.part.userQuery) &&
            is(this.props.entity, newProps.entity))
            return;

        this.loadFindOptions(newProps);
    }

    loadFindOptions(props: UserQueryPartProps) {

        UserQueryClient.Converter.toFindOptions(props.part.userQuery!, props.entity)
            .then(fo => this.setState({ fo: fo }))
            .done();
    }

    render() {

        if (!this.state.fo)
            return <span>{JavascriptMessage.loading.niceToString()}</span>;

        if (this.props.part.renderMode == "BigValue") {
            return <BigValueSearchCounter
                findOptions={this.state.fo}
                text={this.props.partEmbedded.title || undefined}
                style={this.props.partEmbedded.style!}
                iconName={this.props.partEmbedded.iconName || undefined}
                iconColor={this.props.partEmbedded.iconColor || undefined}
            />;
        }

        return (
            <SearchControl
                findOptions={this.state.fo}
                showHeader={false}
                showFooter={false}
                allowSelection={this.props.part.renderMode == "SearchControl"} />
        );
    }
}


interface BigValueBadgeProps {
    findOptions: FindOptions;
    text?: string;
    style: PanelStyle;
    iconName?: string;
    iconColor?: string;
}

export class BigValueSearchCounter extends React.Component<BigValueBadgeProps> {

    vsc: ValueSearchControl;
    render() {
        
        return (
            <div className={"panel panel-" + this.props.style.toLowerCase()}>
                <div className="panel-heading" onClick={this.vsc && this.vsc.handleClick} style={{ cursor: "pointer" }}>
                    <div className="row">
                        <div className="col-xs-3">
                            <i className={classes(this.props.iconName, "fa-5x")} style={{ color: this.props.iconColor }}></i>
                        </div>
                        <div className="col-xs-9 flip text-right">
                            <div className="huge">
                                <ValueSearchControl
                                    ref={vsc => {
                                        if (this.vsc == null && vsc) {
                                            this.vsc = vsc;
                                            this.forceUpdate();
                                        }
                                    }}
                                    findOptions={this.props.findOptions} isLink={true} isBadge={false} />
                            </div>
                            <div className="large">{this.props.text || getQueryNiceName(this.props.findOptions.queryName)}</div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}





