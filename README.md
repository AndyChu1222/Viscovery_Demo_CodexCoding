# Viscovery Demo POS

Viscovery_Demo_CodexCoding is a sample WPF application that demonstrates how to integrate VisAgent’s recognition engine into a point-of-sale workflow.

## Features

- **Domain model** – DTOs for VisAgent recognition responses and simplified order/product representations.
- **API client** – Calls VisAgent endpoints for health checks, configuration, recognition and page switching.
- **POS callback server** – Lightweight HTTP listener that receives checkout results.
- **Recognition comparison** – Merges recognized items with expected order and marks each with a recognition status.
- **MVVM UI** – WPF application with commands to initialize services, load demo QR orders, trigger VisAgent and reset.
- **Logging** – Appends recognition results to a CSV log file.

## Project Structure

| Folder | Description |
| ------ | ----------- |
| `ViscoveryDemoPOS.Domain` | DTOs such as `UnifiedRecognitionResponse`, `ProductItem` and `QrOrder`. |
| `ViscoveryDemoPOS.Services` | `VisAgentApiClient` for REST calls and `PosCallbackServer` for POS webhooks. |
| `ViscoveryDemoPOS.BLL` | `RecognitionComparer` that merges and marks items. |
| `ViscoveryDemoPOS.DAL` | `IRecognitionLogRepository` and file-based implementation. |
| `ViscoveryDemoPOS.ViewModels` | `MainViewModel` coordinating API calls, callbacks and UI state. |
| `ViscoveryDemoPOS.Presentation` | WPF views, converters and application entry point. |

## Prerequisites

- Windows with .NET Framework 4.6.1
- Visual Studio 2019 or later
- VisAgent server available at `http://127.0.0.1:1688`
- POS callback endpoint (default `http://127.0.0.1:8080/visagent/`)

## Build and Run

1. Open `ViscoveryDemoPOS.sln` in Visual Studio.
2. Restore NuGet packages and build the solution.
3. Set `ViscoveryDemoPOS.Presentation` as startup project and run.

## Usage

1. Launch the app; click **Start Scanning** to enter the main page.
2. Click **初始化** to start the callback server, check VisAgent health and send configuration.
3. Click **取得 QRCode** to load a sample order.
4. Click **啟用 Viscovery** to invoke unified recognition on the current frame.
5. When the POS posts checkout data to `/checkout`, the grid updates and a banner displays the result.
6. Click **重置** to clear the session and return to the home view.

## Logging

Recognition results are written to `recognition_log.csv` with timestamp, order ID, product details and status.

