# YouScan Dashboard

A dashboard of chart and text widgets. Tabular files (`.xlsx`, `.xls`, `.csv`) are imported and each
table becomes a widget; the chart type is derived from the shape of the data rather than chosen by hand.

**Live:** _(deployment URL)_

Built with ASP.NET Core 8 + PostgreSQL on the back end, React 19 + TypeScript + Vite on the front end.

## Running it

### With Docker

From the solution root:

```bash
docker compose up --build
```

This starts PostgreSQL and the application, applies the EF Core migration and seeds the two sample
files from `YouScanDashboard.Server/SeedData`. The app is then on <http://localhost:8080> — one
origin for both the API and the SPA, so there is no CORS setup and no separate front-end host.

`docker compose down -v` also drops the database volume, which is the way to verify a clean first start.

### Without Docker

You need .NET 8 SDK, Node 22 and a PostgreSQL instance. Put your connection string in
`YouScanDashboard.Server/appsettings.Development.json` under `ConnectionStrings:Dashboard`, then
press F5 in Visual Studio — the server starts and the SPA proxy brings up the Vite dev server on
port 5173 alongside it.

## How it is put together

The back end is a small layered application, grouped by feature rather than by technical role:

| Folder | What lives there |
| --- | --- |
| `Domain` | `Widget` and `Dataset` entities, with the invariants enforced through factory methods |
| `Importing` | reading table files, seeding, upload handling |
| `Charts` | column-role resolution, chart-type selection, chart data building |
| `Widgets` | the service and the API contracts |
| `Data` | `DbContext` and EF Core configuration |

A dataset is stored as a single `jsonb` column: the imported table keeps its own shape, and the chart
is built from it on read. That is deliberate — the alternative (a normalised cell table) would mean a
join per widget and a schema that has to change whenever a new column type appears, for data that is
never queried by value.

The front end mirrors that: `api/` for the HTTP client and types, `hooks/widgetQueries.ts` for all
React Query keys and mutations, and components grouped by what they render. Every widget loads its own
data behind its own `Suspense` + `ErrorBoundary`, which is what makes per-widget loading and error
states possible.

## How the chart type is chosen

This was the part that needed the most thought, so it is worth spelling out. The rules look at column
*roles*, never at column names — a file with a column called `Sales` is treated exactly like one called
`Продажі`.

First the columns are resolved: the label is the first `Date` column, or failing that the first `Text`
column, or failing that the first `Number` column. The remaining numeric columns are the values. When
the label is a date and there is also a text column, that text column names the series.

Then:

- a date label means a time series, so **line chart**;
- a numeric label (a `Year` or `Id` column with nothing textual next to it) is not a category, so **bar chart**;
- a negative value anywhere, or all values zero, means there is no whole to split into parts, so **bar chart**;
- otherwise one value column is a **pie chart**, and two or more are a **stacked bar chart**.

The last rule is the one worth arguing about: a pie or a stacked bar says "these are parts of a
total", and that statement is false for negative numbers. Recharts will happily draw it anyway, which
is how you get a pie chart with a slice larger than the circle. Falling back to bars keeps the picture
honest.

Rows with an empty label are skipped, and duplicate labels in a wide table are summed rather than
silently overwriting each other.

## API

| | |
| --- | --- |
| `GET /api/widgets` | the dashboard list: id and type only |
| `GET /api/widgets/{id}` | one widget with its text or its chart data |
| `POST /api/widgets` | creates a widget; chart types get randomly generated data |
| `PUT /api/widgets/{id}/content` | saves the text of a text widget |
| `DELETE /api/widgets/{id}` | deletes the widget and, if it owns one, its dataset |
| `POST /api/imports` | uploads a file; every table in it that can be charted becomes a widget |

Errors are returned as RFC 7807 `ProblemDetails`. Swagger is available in Development at `/swagger`.

The list endpoint returning only ids and types is what makes the per-widget loading states work, at the
cost of one request per widget. With a large dashboard that becomes a real problem — the browser only
opens six connections per host — and the fix would be a batch endpoint. At this size the trade-off is
worth it.

## Known limitations

These are conscious, not overlooked:

- **The dashboard is shared.** There are no users and no sessions, so everyone sees the same widgets.
  The task did not ask for authentication.
- **CSV parsing expects ISO dates and a dot as the decimal separator.** Excel files carry their types,
  so this only affects CSV. Supporting a decimal comma would make `1,234` ambiguous, and the safer
  reading of an ambiguous file is the one that does not silently change a number by a factor of a thousand.
- **The first text column becomes the label.** For a table with several text columns, that may not be
  the one a human would pick. Naming the label column in the UI would solve it, but that is a feature
  the task did not ask for.
- **No code splitting.** The bundle is 1.19 MB (367 kB gzipped), almost entirely antd and Recharts.
  Lazy-loading the charts would cut the first load, but everything on this page is visible immediately,
  so it would buy little.
- **Migrations run at startup.** Convenient for a single instance and wrong for several — with more
  than one replica this belongs in a separate step.
