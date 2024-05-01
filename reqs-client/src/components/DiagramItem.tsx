import { Callout, Collapse, Icon, ProgressBar } from "@blueprintjs/core";
import { IDiagramItem } from "../Models";
import { TransformComponent, TransformWrapper } from "react-zoom-pan-pinch";
import { ControlButtons } from "./ControlButtons.tsx";

export default function DiagramItem({ item, onDiagramClick, onUmlShow, loadedUmlImage, onItemRemove }: {
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
                <Icon className={ "remove" } onClick={ () => onItemRemove(item) } icon={ "remove" } />
                <div className={ "name" + (item.isActive ? ' active' : '') } onClick={ () => onDiagramClick(item) }>
                    <Icon icon={ item.isActive ? "caret-down" : "caret-right" } />
                    { name }
                </div>
            </div>
            <Collapse isOpen={ item.isActive }>
                { !item.isSrcLoaded && <ProgressBar intent="none" /> }
                <TransformWrapper centerOnInit initialScale={ 0.8 }>
                    <div className={ "schema" }>
                        <ControlButtons isLoaded={item.isSrcLoaded} diagram={ item } onUmlShow={ () => onUmlShow(item) } />
                        <TransformComponent>
                            {item.uml.length > 0 && <img src={ item.url } alt="svg uml diagram" onLoad={ () => loadedUmlImage(item) } />}

                            {item.uml.length == 0 && 
                                <Callout intent={"warning"} icon={"warning-sign"} title={"Щось пішло не так..."}>
                                    Завітайтей до <Icon icon={"code"}/> для детального дослідження проблеми.
                                </Callout>
                            }
                        </TransformComponent>
                        <ControlButtons isLoaded={item.isSrcLoaded} diagram={ item } onUmlShow={ () => onUmlShow(item) } />
                    </div>
                </TransformWrapper>
            </Collapse>
        </li>
    );
}
