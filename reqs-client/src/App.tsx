import "normalize.css";
import "@blueprintjs/core/lib/css/blueprint.css";
import "@blueprintjs/icons/lib/css/blueprint-icons.css";
import './App.css'
import { Button, Collapse, Drawer, Icon, InputGroup, Position, Pre, ProgressBar, Spinner } from "@blueprintjs/core";
import axios from "axios";
import { ChangeEvent, KeyboardEvent, useState } from "react";
import { AppToaster, IDiagramItem, IReqsDiagramResponse } from "./Models";
import { v4 as guid } from "uuid";

const ERROR_MESSAGE: string = "Ой, якась халепа! (=";

export function App() {
    const [text, setText] = useState("");
    const [diagrams, setDiagrams] = useState<Array<IDiagramItem>>([]);
    const [selectedDiagram, setSelectedDiagram] = useState<IDiagramItem | null>(null);
    const [isProcessing, setIsProcessing] = useState(false);
    const [isFocused, setIsFocused] = useState(false);
    const [drawer, setDrawer] = useState({
        title: "Digram Uml",
        autoFocus: true,
        canEscapeKeyClose: true,
        canOutsideClickClose: true,
        enforceFocus: true,
        hasBackdrop: true,
        isOpen: false,
        position: Position.RIGHT,
        size: undefined,
        usePortal: true
    });

    const process = () => {
        if (isProcessing) {
            return;
        }
        setIsProcessing(true);
        axios.get(`http://localhost:5175/reqs/diagram/${text}`)
            .then(res => {
                const response = res.data as IReqsDiagramResponse;
                if (response == null || response.remoteUrl == null) {
                    showWarning();
                    return;
                }
                setDiagrams([{
                    id: guid(),
                    text: text,
                    uml: response.uml,
                    url: response.remoteUrl
                }, ...diagrams]);
            }).catch((error) => {
                showWarning();
                console.error(error);
            })
            .finally(() => setIsProcessing(false));
    };

    function onDiagramClick(d: IDiagramItem): void {
        setDiagrams(diagrams.map((i) => {
            if (d.id == i.id) {
                return { ...i, isActive: !i.isActive };
            }

            return { ...i };
         }
        ));
    }

    function onUmlShow(diagram: IDiagramItem | null): void {
        setSelectedDiagram(diagram);
        setDrawer({ ...drawer, isOpen: !drawer.isOpen, title: diagram?.text ?? '' });
    }

    function loadedUmlImage(d: IDiagramItem): void {
        setDiagrams(diagrams.map((i) => {
            if (d.id == i.id) {
                return { ...i, isLoaded: true };
            }

            return { ...i };
        }
        ));
    }

    function setValue(v: ChangeEvent<HTMLInputElement>): void {
        setText(v.target?.value);
    }

    function keyDownHandler(k: KeyboardEvent<HTMLInputElement>): void {
        if (k.key == 'Enter') {
            process();
        }
    }

    return (
        <>
            <h2 className="header">
                <InputGroup
                    disabled={isProcessing}
                    className="reqs-input"
                    value={text}
                    leftIcon="diagram-tree"
                    maxLength={120}
                    placeholder={isFocused ? "" : "Введіть вашу вимогу..."}
                    onKeyDown={keyDownHandler}
                    onChange={setValue}
                    onFocus={() => setIsFocused(!isFocused)}
                    onBlur={() => setIsFocused(!isFocused)}
                />
                <Button className="go-btn" disabled={isProcessing} intent={isProcessing ? "none" : "primary"} onClick={() => process()}>
                    {isProcessing ? <Spinner intent="none" size={18.5} /> : "Go"}
                </Button>
            </h2>
            <ul className="diagrams">
                {diagrams.map((d: IDiagramItem) => {
                    return <li key={d.id} className="diagram-item">
                        <div className={"name" + (d.isActive ? ' active' : '')} onClick={() => onDiagramClick(d)}><Icon icon={d.isActive ? "caret-down" : "caret-right"} />{d.text}</div>
                        <Collapse className="vizual" isOpen={d.isActive}>
                            {!d.isLoaded && <ProgressBar intent="none" />}
                            <img src={d.url} alt="svg uml diagram" onLoad={() => loadedUmlImage(d)} />
                            <Button minimal icon="code" text={"Ісходний код UML"} fill={true} onClick={() => onUmlShow(d)} />
                        </Collapse>
                    </li>
                    })
                }
            </ul>
            <Drawer {...drawer}
                icon="code"
                onClose={() => onUmlShow(null)}
                size={"default"}>
                <Pre className="code-block">
                    {selectedDiagram?.uml}
                </Pre>
            </Drawer>
        </>
    );
}

const showWarning = async () => {
    (await AppToaster).show({ message: ERROR_MESSAGE, intent: "danger" });
};

export default App