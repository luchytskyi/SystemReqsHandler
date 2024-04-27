import './App.css'
import { Button, Icon, MenuItem, Overlay2, Spinner } from "@blueprintjs/core";
import { ChangeEvent, KeyboardEvent, useEffect, useState } from "react";
import { IDataSet, IDiagramItem } from "./Models";
import { v4 as guid } from "uuid";
import { ItemRendererProps } from "@blueprintjs/select/src/common/itemRenderer.ts";
import { getConfig, getDiagram } from "./components/fetchApi.ts";
import DiagramItem from "./components/DiagramItem";
import DataSet from "./components/DataSet.tsx";
import Search from "./components/Search.tsx";
import SourceCode from './components/SourceCode.tsx';

export function App() {
    const [text, setText] = useState("");
    const [diagrams, setDiagrams] = useState<Array<IDiagramItem>>([]);
    const [selectedDiagram, setSelectedDiagram] = useState<IDiagramItem | null>(null);
    const [isProcessing, setIsProcessing] = useState(false);
    const [isFocused, setIsFocused] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [dataSet, setDataSet] = useState<IDataSet | null>(null);
    const [dataSetItems, setDataSetItems] = useState<IDataSet[]>([]);
    const [isSourceCodeOpen, setIsSourceCodeOpe] = useState(false);

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
        setDiagrams(diagrams.map((i) => d.id == i.id ? { ...i, isActive: !i.isActive } : { ...i }));
    }

    const onSorceCodeShow = (diagram: IDiagramItem | null): void => {
        setSelectedDiagram(diagram);
        setIsSourceCodeOpe(!isSourceCodeOpen);
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
        return <MenuItem
            key={ d.lang }
            onClick={ itemProps.handleClick }
            text={ <div className={"dataSet-menu-item"}>
                     <span>{ d.schema }</span>
                     { d.lang == dataSet?.lang ? <Icon icon={ "tick" } /> : undefined }
                    </div> 
                }
        />
    };

    return (
        <>
            { dataSetItems.length > 1 &&
                <DataSet
                    dataSetItems={ dataSetItems }
                    itemRenderer={ renderDataSet }
                    onItemSelect={ setDataSet }
                    dataSet={ dataSet } />
            }
            <Search
                disabled={ isProcessing }
                value={ text }
                focused={ isFocused }
                onKeyDown={ keyDownHandler }
                onChange={ setValue }
                onFocus={ () => setIsFocused(!isFocused) }
                onClick={ () => process() } />
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
                                            onUmlShow={ onSorceCodeShow }
                                            loadedUmlImage={ loadedUmlImage }
                                            onItemRemove={ removeItemHandler } />
                    })
                    }
                </ul>
            </div>
            { selectedDiagram &&
                <SourceCode diagram={ selectedDiagram } isOpen={ isSourceCodeOpen }
                            onClose={ () => onSorceCodeShow(null) } />
            }
            <Overlay2 isOpen={ isLoading }>
                <div className={ "page-spinner" }>
                    <Spinner intent={ "primary" } size={ 50 }></Spinner>
                </div>
            </Overlay2>
        </>
    );
}

export default App