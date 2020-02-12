export class DrawerCloseContext<T=any> {
    Action: "Create" | "Update" | "UpdatePartial";
    Payload: T;
}


export class DrawerOpenContext<T> {
    Action: "Create" | "Edit";
    Payload: T;
}


export type DrawerOpenAction = "Create" | "Edit";
export type DrawerCloseAction = "Create" | "Update" | "UpdatePartial";
