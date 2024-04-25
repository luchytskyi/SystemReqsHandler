import { Button } from "@blueprintjs/core";
import { IDiagramItem } from "../Models";
import { useControls } from "react-zoom-pan-pinch";

export const Controls = ({ diagram, onUmlShow }: { diagram: IDiagramItem; onUmlShow: (d: IDiagramItem) => void; }) => {
    const { zoomIn, zoomOut, resetTransform } = useControls();

    return (
        <div className="tools">
            <Button minimal icon="plus" onClick={() => zoomIn()}/>
            <Button minimal icon="minus" onClick={() => zoomOut()} />
            <Button minimal icon="reset" onClick={() => resetTransform()} />
            <Button minimal icon="code" onClick={() => onUmlShow(diagram)} />
        </div>
    );
};
