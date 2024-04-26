import { Button, Icon, InputGroup, Spinner } from "@blueprintjs/core";

export function Search(props: {
    disabled: boolean,
    value: string,
    focused: boolean,
    onKeyDown: (k: React.KeyboardEvent<HTMLInputElement>) => void,
    onChange: (event: React.ChangeEvent<HTMLInputElement>) => void,
    onFocus: () => void,
    onClick: () => Promise<void>
}) {
    return <div className="search-control">
        <InputGroup
            disabled={ props.disabled }
            className="reqs-input"
            value={ props.value }
            leftIcon="diagram-tree"
            maxLength={ 70 }
            placeholder={ props.focused ? "" : "Введіть вашу вимогу..." }
            onKeyDown={ props.onKeyDown }
            onChange={ props.onChange }
            onFocus={ props.onFocus }
            onBlur={ props.onFocus }
        />
        <Button className="go-btn" disabled={ props.disabled } intent={ props.disabled ? "none" : "primary" }
                onClick={ props.onClick }>
            { props.disabled ? <Spinner intent="none" size={ 18.5 } /> : <Icon icon={"search"}/> }
        </Button>
    </div>;
}