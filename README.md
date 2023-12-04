# Sample project for a blog article about an Embedded System UI

Originally written for the [F# Advent Calendar 2023](https://sergeytihon.com/2023/10/28/f-advent-calendar-in-english-2023/).

The blog article can be found at [medium.com](https://medium.com/@viktorschepik).

The project is based on the awesome [FableStarter](https://github.com/rastreus/FableStarter) template.

## Install pre-requisites

You'll need to install the following pre-requisites in order to use the Fable Starter template:

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node](https://nodejs.org/en/download/)
- [Yarn](https://classic.yarnpkg.com/lang/en/)

## Running the project

Run the following commands from the root of the project directory:

0. `cd <your project name>`

1. `dotnet tool restore`

2. `dotnet paket install`

3. `yarn install`

4. `dotnet build shared/src/Shared.fsproj`

5. `dotnet build api/src/Api./fsproj`

6. `dotnet run --project api/src/Api.fsproj`

7. `yarn dev:fable`

8. Open a brower to `http://localhost:5173`
