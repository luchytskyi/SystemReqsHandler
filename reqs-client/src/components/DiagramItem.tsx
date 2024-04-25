import { Collapse, Icon, ProgressBar } from "@blueprintjs/core";
import { IDiagramItem } from "../Models";
import { TransformComponent, TransformWrapper } from "react-zoom-pan-pinch";
import { Controls } from "./Controls";

export function DiagramItem({ item, onDiagramClick, onUmlShow, loadedUmlImage, onItemRemove }: {
    item: IDiagramItem;
    onDiagramClick: (d: IDiagramItem) => void;
    onUmlShow: (diagram: IDiagramItem | null) => void;
    loadedUmlImage: (d: IDiagramItem) => void;
    onItemRemove: (d: IDiagramItem) => void;
}) {
    const name = `${item.text.charAt(0).toUpperCase()}${item.text.slice(1)}`;

    return (
        <li className="diagram-item">
            <div className={ "header" }>
                <Icon className={ "remove" } onClick={ () => onItemRemove(item) } icon={ "trash" } />
                <div className={ "name" + (item.isActive ? ' active' : '') } onClick={ () => onDiagramClick(item) }>
                    <Icon icon={ item.isActive ? "caret-down" : "caret-right" } />
                    { name }
                </div>
            </div>
            <Collapse isOpen={ item.isActive }>
                { !item.isLoaded && <ProgressBar intent="none" /> }
                <TransformWrapper centerOnInit initialScale={ 0.8 }>
                    <div className={ "schema" }>
                        <Controls diagram={ item } onUmlShow={ () => onUmlShow(item) } />
                        <TransformComponent>
                            <img src={ item.url } alt="svg uml diagram" onLoad={ () => loadedUmlImage(item) } />
                        </TransformComponent>
                        <Controls diagram={ item } onUmlShow={ () => onUmlShow(item) } />
                    </div>
                </TransformWrapper>
            </Collapse>
        </li>
    );
}
