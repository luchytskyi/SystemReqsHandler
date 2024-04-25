import { Collapse, Icon, ProgressBar } from "@blueprintjs/core";
import { IDiagramItem } from "../Models";
import { TransformComponent, TransformWrapper } from "react-zoom-pan-pinch";
import { Controls } from "./Controls";

export function DiagramItem({ item, onDiagramClick, onUmlShow, loadedUmlImage }: { item: IDiagramItem; onDiagramClick: (d: IDiagramItem) => void; onUmlShow: (diagram: IDiagramItem | null) => void; loadedUmlImage: (d: IDiagramItem) => void; }) {
    return (
        <li className="diagram-item">
            <div className={"name" + (item.isActive ? ' active' : '')} onClick={() => onDiagramClick(item)}>
                <Icon icon={item.isActive ? "caret-down" : "caret-right"} />
                {item.text}
            </div>
            <Collapse className="vizual" isOpen={item.isActive}>
                {!item.isLoaded && <ProgressBar intent="none" />}
                <TransformWrapper centerOnInit initialScale={0.8}>
                    <>
                        <Controls diagram={item} onUmlShow={() => onUmlShow(item)} />
                        <TransformComponent>
                            <img src={item.url} alt="svg uml diagram" onLoad={() => loadedUmlImage(item)} />
                        </TransformComponent>
                    </>
                </TransformWrapper>
            </Collapse>
        </li>
    );
}
