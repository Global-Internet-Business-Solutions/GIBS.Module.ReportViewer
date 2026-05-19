# GIBS.Module.ReportViewer

A configurable **Oqtane** module for running SQL queries and rendering the results in a page module.

## Overview

`GIBS.Module.ReportViewer` lets site administrators define a query in module settings and display the output as either:

- a table/grid view, or
- a tokenized template view for custom markup.

The module supports local (tenant) database queries and remote SQL Server queries.

## Features

- Local or remote data source selection
- Free-form SQL query execution
- Table rendering with dynamic columns
- Template rendering using row tokens:
  - `[ColumnName]`
  - `{{ColumnName}}`
- Optional template header and footer
- In-memory output caching (configurable per module)
- Manual cache bypass/refresh action for editors
- Standard Oqtane module permissions for view/edit access

## Requirements

- Oqtane Framework (module package type targets Oqtane Framework `10.1.2`)
- .NET `10.0`
- SQL Server provider for remote connections (`Microsoft.Data.SqlClient`)

## Installation

### Option 1: Install package in Oqtane

1. Build and package the module.
2. Upload/install the generated `.nupkg` in Oqtane.
3. Add **ReportViewer** to a page.

### Option 2: Local development workflow

This repository includes helper scripts in `Package/`:

- `debug.cmd <TargetFramework> <ProjectName>`
  - Copies module assemblies and static assets to the local Oqtane host output.
- `release.cmd <TargetFramework> <ProjectName>`
  - Packs the NuGet module and copies it to the Oqtane `Packages` folder.

Example values:

- `TargetFramework`: `net10.0`
- `ProjectName`: `GIBS.Module.ReportViewer`

## Module Settings

Configure the module in **Settings**:

- **Data Source**: `Local` or `Remote`
- **Connection String**: required when Data Source is `Remote`
- **SQL Query**: required
- **Cache Minutes**: `0` to `1440` (`0` disables cache)
- **View Mode**: `Table` or `Template`
- **Template Header** (template mode)
- **Template** (template mode)
- **Template Footer** (template mode)

## Usage

1. Add the module to a page.
2. Open module settings.
3. Configure data source and SQL query.
4. Choose `Table` or `Template` output.
5. Save settings and view rendered results.
6. Use **Refresh** (edit permission) to bypass cache and rerun query.

## Security Notes

- Query execution is controlled by Oqtane module permissions.
- This module executes configured SQL directly.
- Restrict edit access to trusted administrators only.
- Use least-privilege SQL credentials, especially for remote connections.

## Project Structure

- `Client/` - Blazor UI components and client service
- `Server/` - API controller, business logic, EF context/repository, static assets
- `Shared/` - shared models and service contract
- `Package/` - NuGet packaging scripts and nuspec

## Version

Current module version: **1.0.0**

## License

MIT (per module package metadata)
