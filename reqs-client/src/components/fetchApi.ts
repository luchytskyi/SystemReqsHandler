import { AppToaster, IDataSet, IReqsDiagramResponse } from "../Models.ts";
import axios from "axios";

const ERROR_MESSAGE: string = "Ой, якась халепа! (=";

export const showWarning = async () => {
    (await AppToaster).show({ message: ERROR_MESSAGE, intent: "warning" });
};


export async function getDiagram(text: string): Promise<IReqsDiagramResponse> {
    return await getData(`reqs/diagram/${ text }`);
}

export async function getConfig(): Promise<Array<IDataSet>> {
    return await getData("reqs/config");
}

async function getData<T>(url: string): Promise<T> {
    try {
        let response = await axios.get(`http://localhost:5175/${ url }`,
            { withCredentials: true })
        if (response.status == 200) {
            return response.data as T;
        }
        return Promise.reject(response);
        
    } catch (d) {
        await showWarning();
        return Promise.reject(d);
    }
}