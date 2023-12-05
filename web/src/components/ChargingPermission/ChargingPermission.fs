module ChargingPermission

open Feliz

type ChargingPermission =
    | Allow
    | Disallow

type ErrorInfo =
    | Unauthorized
    | InternalServerError

type ReceiveValue<'T> =
    | Available of 'T
    | Unavailable
    | ReceiveFailed of ErrorInfo

type SendValue<'T> =
    | Uninitialized
    | Initial of 'T
    | Sending of 'T
    | Sent of 'T
    | SendFailed of 'T * ErrorInfo


module RestApi =

    open Browser

    let getChargingPermission onReceive =
        window.setTimeout (
            fun () ->
                onReceive (
                    Available Disallow
                // Unavailable
                // (ReceiveFailed Unauthorized)
                )
            , 2000
        )
        |> ignore

    let putChargingPermission (value: ChargingPermission) (onSent: SendValue<ChargingPermission> -> unit) =
        window.setTimeout (
            fun () -> onSent (Sent value)
            // (SendFailed(value, Unauthorized))
            , 2000
        )
        |> ignore


type ChargingPermissionComponent =

    [<ReactComponent>]
    static member RadioInput
        (
            radioInputValue: ChargingPermission,
            receiveChargingPermission: ReceiveValue<ChargingPermission>,
            sendChargingPermission: SendValue<ChargingPermission>,
            onChange: ChargingPermission -> unit
        ) =
        let inputValueStr = $"%A{radioInputValue}"

        Html.div [
            prop.classes [ "flex"; "gap-2" ]
            prop.children [
                Html.input [
                    prop.type'.radio
                    prop.name "charging-permission"
                    prop.value inputValueStr

                    prop.isChecked (
                        match sendChargingPermission, receiveChargingPermission with
                        | (Initial permission | Sent permission), _ when permission = radioInputValue -> true
                        | (Sending _ | SendFailed _), Available permission when permission = radioInputValue -> true
                        | _ -> false
                    )

                    prop.disabled (
                        match sendChargingPermission with
                        | Initial _
                        | Sent _
                        | SendFailed _ -> false
                        | Uninitialized
                        | Sending _ -> true
                    )

                    prop.onChange (fun (selectedValue: string) ->
                        if selectedValue = $"%A{Allow}" then
                            onChange Allow
                        elif selectedValue = $"%A{Disallow}" then
                            onChange Disallow
                        else
                            ())
                ]
                Html.label [ prop.for' inputValueStr; prop.text inputValueStr ]
            ]
        ]

    [<ReactComponent>]
    static member ChargingPermissionComponent() =
        let receiveChargingPermission, setReceiveChargingPermission =
            React.useState Unavailable

        let sendChargingPermission, setSendChargingPermission = React.useState Uninitialized

        React.useEffectOnce (fun () ->
            RestApi.getChargingPermission (fun receiveChargingPermission ->
                setReceiveChargingPermission receiveChargingPermission

                match receiveChargingPermission with
                | Available chargingPermission -> setSendChargingPermission (Initial chargingPermission)
                | Unavailable
                | ReceiveFailed _ -> ()))

        Html.div [
            prop.children [
                Html.div [ prop.classes [ "text-xl"; "my-4" ]; prop.text "Charging Permission" ]
                ChargingPermissionComponent.RadioInput(
                    radioInputValue = Allow,
                    receiveChargingPermission = receiveChargingPermission,
                    sendChargingPermission = sendChargingPermission,
                    onChange =
                        (fun value ->
                            setSendChargingPermission (Sending value)
                            RestApi.putChargingPermission value setSendChargingPermission)
                )
                ChargingPermissionComponent.RadioInput(
                    radioInputValue = Disallow,
                    receiveChargingPermission = receiveChargingPermission,
                    sendChargingPermission = sendChargingPermission,
                    onChange =
                        (fun value ->
                            setSendChargingPermission (Sending value)
                            RestApi.putChargingPermission value setSendChargingPermission)
                )
                match sendChargingPermission with
                | SendFailed(_, errorInfo) -> Html.div [ prop.text $"Error: %A{errorInfo}" ]
                | _ -> ()
            ]
        ]

let view () =
    ChargingPermissionComponent.ChargingPermissionComponent()
