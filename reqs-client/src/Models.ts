import { OverlayToaster, Position } from "@blueprintjs/core";

export type Dictionary<K extends string, T> = {
    [key in K]?: T;
}

export interface IDataSet {
    lang: string;
    schema: string;
}

export interface IDiagramItem {
    id: string;
    url: string;
    uml: string;
    text: string;
    dataSetDto: any;
    tokens: string;
    isActive?: boolean;
    isSrcLoaded: boolean;
}

export interface IReqsDiagramResponse {
    remoteUrl: string;
    uml: string;
    tokens: string;
    dataSetDto: any;
}

export const AppToaster = OverlayToaster.createAsync({
    className: "recipe-toaster",
    position: Position.TOP
});