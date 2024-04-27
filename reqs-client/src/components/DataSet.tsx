import { ItemRendererProps } from "@blueprintjs/select/src/common/itemRenderer.ts";
import { CookiesProvider, useCookies } from "react-cookie";
import { Select } from "@blueprintjs/select";
import { Button } from "@blueprintjs/core";
import { IDataSet } from "../Models.ts";
import { useEffect } from "react";

export default function DataSet({ dataSetItems, itemRenderer, onItemSelect, dataSet }: {
    dataSetItems: IDataSet[],
    itemRenderer: (d: IDataSet, itemProps: ItemRendererProps) => JSX.Element,
    onItemSelect: (d: IDataSet) => void,
    dataSet: IDataSet | null
}) {

    const [cookie, setCookie] = useCookies(['lang']);

    useEffect(() => {
        if (dataSet == null && cookie.lang) {
            onItemSelect(dataSetItems.find(d => d.lang === cookie.lang)!);
        }
    }, []);

    const selectItemHandler = (dataSet: IDataSet) => {
        onItemSelect(dataSet);
        setCookie('lang', dataSet.lang, { path: '/' });
    };

    return <CookiesProvider>
        <div className={ "data-set" }>
            <Select<IDataSet>
                popoverProps={ { minimal: true } }
                filterable={ false }
                items={ dataSetItems }
                itemRenderer={ itemRenderer }
                onItemSelect={ selectItemHandler }>
                <Button text={ dataSet?.schema } rightIcon="double-caret-vertical" />
            </Select>
        </div>
    </CookiesProvider>
}