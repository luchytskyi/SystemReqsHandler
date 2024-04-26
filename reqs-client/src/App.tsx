import './App.css'
import { Button, Drawer, MenuItem, Overlay2, Position, Pre, Spinner, Tab, Tabs } from "@blueprintjs/core";
import { ChangeEvent, KeyboardEvent, useEffect, useState } from "react";
import { IDataSet, IDiagramItem } from "./Models";
import { v4 as guid } from "uuid";
import { DiagramItem } from "./components/DiagramItem";
import { ItemRendererProps } from "@blueprintjs/select/src/common/itemRenderer.ts";
import { DataSet } from "./components/DataSet.tsx";
import { Search } from "./components/Search.tsx";
import { getConfig, getDiagram } from "./components/fetchApi.ts";

export function App() {
    const [text, setText] = useState("");
    const [diagrams, setDiagrams] = useState<Array<IDiagramItem>>([]);
    const [selectedDiagram, setSelectedDiagram] = useState<IDiagramItem | null>(null);
    const [isProcessing, setIsProcessing] = useState(false);
    const [isFocused, setIsFocused] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [dataSet, setDataSet] = useState<IDataSet | null>(null);
    const [dataSetItems, setDataSetItems] = useState<IDataSet[]>([]);
    const [drawer, setDrawer] = useState({
        title: "Diagram Uml",
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

    useEffect(() => {
        setIsLoading(true);
        getConfig()
            .then((data: IDataSet[]) => {
                setDataSetItems(data);
            })
            .finally(() => setIsLoading(false));
    }, []);

    const process = async () => {
        if (isProcessing) {
            return;
        }
        setIsProcessing(true);
        const diagram = await getDiagram(text).finally(() => setIsProcessing(false));
        setDiagrams([{
            id: guid(),
            text: text,
            uml: diagram.uml,
            url: diagram.remoteUrl,
            tokens: diagram.tokens,
            isSrcLoaded: false,
            dataSetDto: diagram.dataSetDto
        }, ...diagrams]);
    };

    const onDiagramClick = (d: IDiagramItem): void => {
        setDiagrams(diagrams.map((i) => {
                if (d.id == i.id) {
                    return { ...i, isActive: !i.isActive };
                }

                return { ...i };
            }
        ));
    }

    const onUmlShow = (diagram: IDiagramItem | null): void => {
        setSelectedDiagram(diagram);
        setDrawer({ ...drawer, isOpen: !drawer.isOpen, title: diagram?.text ?? '' });
    }

    const loadedUmlImage = (d: IDiagramItem): void => {
        setDiagrams(diagrams.map((i) => d.id == i.id ? { ...i, isSrcLoaded: true } : { ...i }));
    }

    const setValue = (event: ChangeEvent<HTMLInputElement>): void => {
        setText(event.target?.value);
    }

    const keyDownHandler = (k: KeyboardEvent<HTMLInputElement>): void => {
        if (k.key == 'Enter') {
            process();
        }
    }

    const removeItemHandler = (item: IDiagramItem): void => {
        setDiagrams([...diagrams.filter(i => i.id !== item.id)]);
    }

    const renderDataSet = (d: IDataSet, itemProps: ItemRendererProps) => {
        return <MenuItem text={ d.schema } key={ d.lang } selected={ dataSet?.lang == d.lang }
                         onClick={ itemProps.handleClick } />
    }

    return (
        <>
            { dataSetItems.length > 0 &&
                <DataSet dataSetItems={ dataSetItems } itemRenderer={ renderDataSet }
                         onItemSelect={ (d: IDataSet) => setDataSet(d) } dataSet={ dataSet } />
            }
            <Search disabled={ isProcessing } value={ text } focused={ isFocused } onKeyDown={ keyDownHandler }
                    onChange={ setValue } onFocus={ () => setIsFocused(!isFocused) } onClick={ () => process() } />
            <div className="content">
                <div className="clear-all">
                    { diagrams.length > 0 &&
                        <Button onClick={ () => setDiagrams([]) } minimal small text={ "Видалити усі" } /> }
                </div>
                <ul className="diagrams">
                    { diagrams.map((d: IDiagramItem) => {
                        return <DiagramItem key={ d.id }
                                            item={ d }
                                            onDiagramClick={ onDiagramClick }
                                            onUmlShow={ onUmlShow }
                                            loadedUmlImage={ loadedUmlImage }
                                            onItemRemove={ removeItemHandler } />
                    })
                    }
                </ul>
            </div>
            <Drawer className={ "source-code" } { ...drawer }
                    icon="code"
                    onClose={ () => onUmlShow(null) }
                    size={ "default" }>
                <Tabs className={ "tabs-source-code" } id={ "code" }>
                    <Tab title={ "UML" } id={ "uml" } panel={ <Pre className="code-block" children={ selectedDiagram?.uml }/> } />
                    <Tab title={ "TOKENS" } id={ "tkn" } panel={ <Pre className="code-block" children={ selectedDiagram?.tokens }/> } />
                    <Tab title={ "DATA" } id={ "data" } panel={ <Pre dangerouslySetInnerHTML={{ __html: syntaxHighlight(selectedDiagram?.dataSetDto)}}/>} />
                </Tabs>
            </Drawer>
            <Overlay2 isOpen={ isLoading }>
                <div className={ "page-spinner" }>
                    <Spinner intent={ "primary" } size={ 50 }></Spinner>
                </div>
            </Overlay2>
        </>
    );
}


export default App

function syntaxHighlight(json: any | null) {
    if (json == null) {
        return undefined;
    }
    if (typeof json != 'string') {
        json = JSON.stringify(json, undefined, 2);
    }
    json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match: string) {
        var cls = 'number';
        if (/^"/.test(match)) {
            if (/:$/.test(match)) {
                cls = 'key';
            } else {
                cls = 'string';
            }
        } else if (/true|false/.test(match)) {
            cls = 'boolean';
        } else if (/null/.test(match)) {
            cls = 'null';
        }
        return '<span class="' + cls + '">' + match + '</span>';
    });
}