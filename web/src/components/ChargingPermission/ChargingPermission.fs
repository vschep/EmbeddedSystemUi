module ChargingPermission

open Feliz

type ChargingPermission =
    [<ReactComponent>]
    static member ChargingPermissionComponent () =
        Html.div [
            prop.text "ChargingPermissionComponent"
        ]

let view () = ChargingPermission.ChargingPermissionComponent()
