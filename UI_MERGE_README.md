# P1 Inventory SaaS — UI Merge

The StockPilot UI has been merged into the existing architecture rather than replacing the solution structure.

## Architecture preserved

- `P1(Inventory SaaS)` — .NET MAUI Blazor Hybrid host
- `P1(Inventory SaaS).Shared` — reusable Razor UI, models, and UI service contracts
- `P1(Inventory SaaS).Web` — ASP.NET Core web host using the same shared UI

The new UI is located primarily in the Shared project so both MAUI and Web render the same components.

## Temporary data layer

`MockInventoryUiService` supplies demo data for the UI. In the backend/API phase, replace its registration with a typed HTTP API client while keeping `IInventoryUiService` as the UI boundary if desired.

## Main routes

- `/` Dashboard
- `/products`
- `/products/{id}`
- `/inventory`
- `/stock-movements`
- `/transfers`
- `/purchase-orders`
- `/warehouses`
- `/users`
- `/settings`
- `/scan`

## Visual Studio

Open `P1(Inventory SaaS).slnx`. Set either `P1(Inventory SaaS)` for MAUI or `P1(Inventory SaaS).Web` for the web host as startup project.
