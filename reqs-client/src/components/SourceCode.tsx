import { Drawer, DrawerProps, Pre, Tab, Tabs } from "@blueprintjs/core";
import { IDiagramItem } from "../Models";

interface ISourceCodeProps extends Omit<DrawerProps, "icon"> {
    diagram: IDiagramItem;
}

export default function SourceCode(props: ISourceCodeProps) {
    const state = {
        ...props,
        className: "source-code",
        title: props.diagram.text,
        autoFocus: true,
        canOutsideClickClose: true,
        canEscapeKeyClose: true
    };

    return <Drawer {...state} icon="code">
            <Tabs className={"tabs-source-code"} id={"code"}>
                <Tab title={"UML"} id={"uml"} panel={<Pre className="code-block" children={state.diagram.uml} />} />
                <Tab title={"TOKENS"} id={"tkn"} panel={<Pre className="code-block" children={state.diagram.tokens} />} />
                <Tab title={"DATA"} id={"data"} panel={<Pre dangerouslySetInnerHTML={{ __html: syntaxHighlight(state.diagram.dataSetDto) }} />} />
            </Tabs>
        </Drawer>
}


function syntaxHighlight(json: any) {
    if (json == null) {
        return undefined;
    }
    if (typeof json != 'string') {
        json = JSON.stringify(json, undefined, 2);
    }
    json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match: string) {
        let cls = 'number';
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