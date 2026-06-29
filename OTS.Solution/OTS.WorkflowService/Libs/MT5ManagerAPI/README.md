# MT5 Manager SDK files

`OTS.WorkflowService` references the `MetaQuotes.MT5ManagerAPI64-net2.0` NuGet package, so the normal deployment path is to restore/build the project and let NuGet copy the SDK files to the application output folder.

If a deployment needs to override the NuGet-provided files, copy the official MetaTrader 5 Manager SDK runtime files into this folder and set `Mt5:SdkDirectory` to `Libs/MT5ManagerAPI`:

- `MetaQuotes.MT5CommonAPI(64).dll`
- `MetaQuotes.MT5ManagerAPI(64).dll`
- `MT5APIManager(64).dll`

The workflow service probes the application output directory first, then the optional `Mt5:SdkDirectory`, and finally the local NuGet package cache.
