import { OverlayToaster, Position } from "@blueprintjs/core";

export interface IDiagramItem {
    id: string;
    url: string;
    uml: string;
    text: string;
    isActive?: boolean;
    isLoaded?: boolean;
}

export interface IReqsDiagramResponse {
    remoteUrl: string;
    uml: string;
}

export const AppToaster = OverlayToaster.createAsync({
    className: "recipe-toaster",
    position: Position.TOP
});