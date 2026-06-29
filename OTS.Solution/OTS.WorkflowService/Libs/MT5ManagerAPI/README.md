# MT5 Manager SDK files

Copy the official MetaTrader 5 Manager SDK runtime files into this folder before running `OTS.WorkflowService`:

- `MetaQuotes.MT5CommonAPI(64).dll`
- `MetaQuotes.MT5ManagerAPI(64).dll`
- `MT5APIManager(64).dll`

The workflow service loads the SDK dynamically from the `Mt5:SdkDirectory` setting so the proprietary DLLs do not need to be committed to source control.
